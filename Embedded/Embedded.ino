#include <Arduino.h>
#include "ActivityModeService.h"
#include "RfidService.h"
#include "HttpsService.h"
#include "WifiSecrets.h"
#include "JsonHelpers.h"

//Use GPIO Pin numbers
#define RST_PIN	16
#define SS_PIN	15  
#define UPDATE_MODE_PIN 0
#define SEND_REQUEST_TOGGLE_INPUT_PIN 9
#define SEND_REQUEST_TOGGLE_OUTPUT_PIN 2 //Onboard LED

const char *wifiSsid = WIFI_SSID;
const char *wifiPassword = WIFI_PASSWORD;
String host = "https://attire-tracker-kxhmjx7pyq-uw.a.run.app";

ActivityModeService modeService(UPDATE_MODE_PIN);
RfidService rfidService(SS_PIN, RST_PIN);
HttpsService httpsService(wifiSsid, wifiPassword);

bool sendHttpRequests = false;
bool sendHttpRequestDebounce = false;

void setup() {
  pinMode(SEND_REQUEST_TOGGLE_INPUT_PIN, INPUT_PULLUP);
  pinMode(SEND_REQUEST_TOGGLE_OUTPUT_PIN, OUTPUT);

  Serial.begin(115200);
  delay(250);
  Serial.println("Booting....");
  
  rfidService.init();
  httpsService.init();

  rfidService.registerOnCardDetected(onCardDetected);
  rfidService.registerOnCardRemoved(onCardRemoved);

  Serial.println(F("Attire Tracker Embedded Launching"));
  Serial.println(F("======================================================")); 
  Serial.println(F("Scan for Card and print UID:"));
}

void loop() { 
  updateAndDisplaySendRequestToggle();
  modeService.update();
  rfidService.listenForAndHandleRfidChange();
  delay(50);
}

void updateAndDisplaySendRequestToggle()
{
  bool buttonIsPressed = !digitalRead(SEND_REQUEST_TOGGLE_INPUT_PIN); //Pin is pulled high
  if(buttonIsPressed && !sendHttpRequestDebounce) 
  {
    sendHttpRequestDebounce = true;
    sendHttpRequests = !sendHttpRequests;
    Serial.print("Send HTTP requests toggled to: ");
    Serial.println(sendHttpRequests);
  }
  else if (!buttonIsPressed) {
    sendHttpRequestDebounce = false;
  }

  digitalWrite(SEND_REQUEST_TOGGLE_OUTPUT_PIN, !sendHttpRequests); //Onboard LED is on when pin is low.
}

void onCardDetected(String uid) {
  Serial.println("Card UID: " + uid);

  if(sendHttpRequests)
    updateActivityHistoryRequest(uid);
}

void onCardRemoved() {
  Serial.println("Card removed");
}

void updateActivityHistoryRequest(String rfidUid)
{
  String path = getModePath(modeService.getMode(), rfidUid);
  String result = httpsService.post(host, path, "");
  String pieceName = getStringProperty(result, "pieceName");
  String newStatus = getStringProperty(result, "description");
  Serial.print(pieceName);
  Serial.print(" status updated to: ");
  Serial.println(newStatus);
}

static String getModePath(ActivityMode mode, String rfidUid) {
  String modeUrlFragment = mode == laundry ? "laundry" : "wardrobe";
  return "attirePieces/" + rfidUid + "/activity/" + modeUrlFragment;
}