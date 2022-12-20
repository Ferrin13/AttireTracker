#include <Arduino.h>
#include "ActivityModeService.h"
#include "RfidService.h"
#include "HttpsService.h"
#include "LcdService.h"
#include "WifiSecrets.h"
#include "JsonHelpers.h"

// SDA (0) -> D8 (0)
// SCK (1) -> D5 (3)
// MOSI (2) -> D7 (1)
// MISO (3) -> D6 (2)
// IRQ(4) -> Nothing
// GND (5) -> GND
// RST (6) -> D0
// 3.3v (7) -> 3.3V

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
LcdService lcdService;

bool sendHttpRequests = true;
bool sendHttpRequestDebounce = false;

void setup() {
  pinMode(SEND_REQUEST_TOGGLE_INPUT_PIN, INPUT_PULLUP);
  pinMode(SEND_REQUEST_TOGGLE_OUTPUT_PIN, OUTPUT);

  Serial.begin(115200);
  delay(250);
  Serial.println("Booting....");
  lcdService.init();
  lcdService.displayCenteredString("Booting", 1);
  
  rfidService.init();
  httpsService.init();

  rfidService.registerOnCardDetected(onCardDetected);
  rfidService.registerOnCardRemoved(onCardRemoved);
  modeService.registerOnModeUpdate(onModeUpdated);

  Serial.println(F("Attire Tracker Embedded Launching"));
  Serial.println(F("======================================================")); 
  Serial.println(F("Scan for Card and print UID:"));
  lcdService.displayCenteredString("Mode: " + modeToString(modeService.getMode()), 0);
  lcdService.displayCenteredString("", 1);
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
  Serial.println("Card detected:");
  Serial.println(uid);
  showCardDetected(uid);

  if(sendHttpRequests)
    updateActivityHistoryRequest(uid);
}

void onCardRemoved() {
  Serial.println("Card removed");
}

void onModeUpdated(ActivityMode mode) {
  Serial.print("Mode updated to: ");
  Serial.println(modeToString(mode));
  lcdService.displayCenteredString("Mode: " + modeToString(mode), 0);
}

void updateActivityHistoryRequest(String rfidUid)
{
  String path = getModePath(modeService.getMode(), rfidUid);
  String result = httpsService.post(host, path, "");

  if(result == "")
  {
    showPieceUpdate("Empty HTTP Response", "N/A");
    return;
  }

  String pieceName = getStringProperty(result, "pieceName");
  String newStatus = getStringProperty(result, "description");
  showPieceUpdate(pieceName, newStatus);
}

static String getModePath(ActivityMode mode, String rfidUid) {
  String modeUrlFragment = mode == laundry ? "laundry" : "wardrobe";
  return "attirePieces/" + rfidUid + "/activity/" + modeUrlFragment;
}

void showPieceUpdate(String pieceName, String newStatus) {
  lcdService.clearRows(1, 3);
  lcdService.displayCenteredString(pieceName, 1);
  lcdService.displayCenteredString("Updated:", 2);
  lcdService.displayCenteredString(newStatus, 3);
}

void showCardDetected(String uid) {
  lcdService.clearRows(1, 3);
  lcdService.displayCenteredString("Requesting Info...", 1);
  lcdService.displayCenteredString(uid, 2);
}

void showCardRemoved() {
  lcdService.clear();
  lcdService.displayCenteredString("Card Removed", 1);
}

void showModeUpdate(String newMode) {
  lcdService.displayCenteredString("Mode: " + newMode, 0);
}