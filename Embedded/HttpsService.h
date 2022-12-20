#ifndef _HttpsService_H_
#define _HttpsService_H_
#include <Arduino.h>
#include <ESP8266WiFi.h>
#include <ESP8266WiFiMulti.h>
#include <ESP8266HTTPClient.h>

#define WIFI_CONNECTION_RETRY_TIMEOUT_MS 30000
#define WIFI_CONNECTION_RETRY_INTERNVAL_MS 500

class HttpsService {
  const char* wifiSsid;
  const char* wifiPassword;
  WiFiClientSecure wifiClient;

public:
  HttpsService(const char *wifiSsid, const char *wifiPassword);
  void init();
  String get(String url);
  String post(String host, String path, String content = "");
};

#endif