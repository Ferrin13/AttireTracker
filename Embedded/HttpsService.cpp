#include <Arduino.h>
#include "HttpsService.h"

HttpsService::HttpsService(const char *wifiSsid, const char *wifiPassword) {
  this->wifiSsid = wifiSsid;
  this->wifiPassword = wifiPassword;
}

void HttpsService::init() {
  WiFi.begin(wifiSsid, wifiPassword);
   
  int retries = 0;
  int maxNumRetries = WIFI_CONNECTION_RETRY_TIMEOUT_MS / WIFI_CONNECTION_RETRY_INTERNVAL_MS;
  while ((WiFi.status() != WL_CONNECTED) && (retries < maxNumRetries)) {
    retries++;
    delay(WIFI_CONNECTION_RETRY_INTERNVAL_MS);
    Serial.print(".");
  }

  if (WiFi.status() == WL_CONNECTED) {
    Serial.println(F("WiFi initialized"));
  }
  else {
    Serial.println("Failed to initialize to Wifi");
  }
}

String HttpsService::post(String host, String path, String content) {
  wifiClient.setInsecure();
  wifiClient.connect(host, 443);
  
  if(!wifiClient.connected())
  {
    Serial.println("Https client failed to connect to host");
    return "";
  }

  HTTPClient http; 
  http.begin(wifiClient, host + "/" + path);
  
  int httpCode = http.POST(content);
  String response = http.getString();

  if (httpCode <= 0) {
    Serial.println("HTTP Code Not > 0");    
  }

  http.end();
  wifiClient.stop();

  return response;
}