#include <ESP8266WiFi.h>
#include <ESP8266WiFiMulti.h>
#include <ESP8266HTTPClient.h>
#include <SPI.h>
#include "MFRC522.h"
#include "WifiSecrets.h"

/* wiring the MFRC522 to ESP8266 (ESP-12)
RST     = GPIO5
SDA(SS) = GPIO4 
MOSI    = GPIO13
MISO    = GPIO12
SCK     = GPIO14
GND     = GND
3.3V    = 3.3V
*/

//Use GPIO Pin numbers
#define RST_PIN	5 
#define SS_PIN	15  

#define WIFI_CONNECTION_RETRY_TIMEOUT_MS 10000
#define WIFI_CONNECTION_RETRY_INTERNVAL_MS 500

const char *ssid = WIFI_SSID;
const char *pass = WIFI_PASSWORD;
String host = "https://attire-tracker-kxhmjx7pyq-uw.a.run.app";

MFRC522 mfrc522(SS_PIN, RST_PIN);
WiFiClientSecure wifiClient;

void setup() {
  Serial.begin(115200);    // Initialize serial communications
  delay(250);
  Serial.println(F("Booting...."));
  
  SPI.begin();	         // Init SPI bus
  mfrc522.PCD_Init();    // Init MFRC522
  
  WiFi.begin(ssid, pass);
  
  int retries = 0;
  int maxNumRetries = WIFI_CONNECTION_RETRY_TIMEOUT_MS / WIFI_CONNECTION_RETRY_INTERNVAL_MS;
  while ((WiFi.status() != WL_CONNECTED) && (retries < maxNumRetries)) {
    retries++;
    delay(WIFI_CONNECTION_RETRY_INTERNVAL_MS);
    Serial.print(".");
  }

  if (WiFi.status() == WL_CONNECTED) {
    Serial.println(F("WiFi connected"));
  }
  else {
    Serial.println("Failed to connect to Wifi");
  }

  Serial.println(F("Attire Tracker Embedded Launching"));
  Serial.println(F("======================================================")); 
  Serial.println(F("Scan for Card and print UID:"));
}

bool cardIsPresent = false;
void loop() { 
  bool updatedCardIsPresent = getCardIsPresentDebounced();
  if (cardIsPresent != updatedCardIsPresent)
  {
    cardIsPresent = updatedCardIsPresent;

    if(cardIsPresent)
    {
        Serial.print(F("Card UID:"));
        String rfidUid = byteArrayToString(mfrc522.uid.uidByte, mfrc522.uid.size);
        Serial.println(rfidUid);
        toggleActivityRequest(rfidUid);
    }
    else
    {
      Serial.println("Card removed");
    }
  }
  delay(50);
}

void toggleActivityRequest(String rfidUid)
{
  wifiClient.setInsecure();
  wifiClient.connect(host, 443);
  
  if(!wifiClient.connected())
    Serial.println("Wifi client failed to connect to host");

  if(wifiClient.connected()) {
    HTTPClient http; 
    http.begin(wifiClient, host + "/attirePieces/" + rfidUid + "/activity");
    
    int httpCode = http.POST("");
    String response = http.getString();
    Serial.print("Response: ");
    Serial.println(response);    

    if (httpCode <= 0) {
      Serial.println("HTTP Code Not > 0");    
    }
    http.end();
    wifiClient.stop();
  }
  else {
    Serial.println("Wifi client not connected");
  }
}

bool getCardIsPresentRaw()
{
  return mfrc522.PICC_IsNewCardPresent() && mfrc522.PICC_ReadCardSerial();
}

bool getCardIsPresentDebounced()
{
  //The direct checks for card is present oscilate, probably because they are doing initialization checks
  //rather than simply maintaining the "connection". Ignoring IsNewCardPresent's result seems to have no
  //effect, so rather than combine other lower level functions, this simple hack consistently returns the
  //correct state at the cost of making every check twice. For this applicaiton, the latency increase is
  //trivial. 
  for(int i = 0; i < 2; i++)
  {
    if(getCardIsPresentRaw()) return true;
  }
  return false;
}

String byteArrayToString(byte *buffer, byte bufferSize) {
  String result = "";
  for (byte i = 0; i < bufferSize; i++) {
    result += buffer[i] < 0x10 ? "0" : "";
    result += String(buffer[i], HEX);
  }
  result.toLowerCase();
  return result;
}

// Helper routine to dump a byte array as hex values to Serial
void dump_byte_array(byte *buffer, byte bufferSize) {
  for (byte i = 0; i < bufferSize; i++) {
    Serial.print(" ");
    Serial.print(buffer[i] < 0x10 ? "0" : "");
    Serial.print(buffer[i], HEX);
  }
}