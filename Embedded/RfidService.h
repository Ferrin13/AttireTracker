#ifndef _RfidService_H_
#define _RfidService_H_
#include <Arduino.h>
#include "MFRC522.h"
#include "SPI.h"

class RfidService {
  MFRC522 mfrc522;
  byte cardIsPresent;
  std::function<void(String)> onCardDetected;
  std::function<void()> onCardRemoved;

  bool getCardIsPresentDebounced();
  bool getCardIsPresentRaw();
  void onRfidStatusChange(bool newStatus);

public:
  RfidService(byte selectPin, byte resetPin);
  void listenForAndHandleRfidChange();
  void registerOnCardDetected(std::function<void(String)> onCardDetected);
  void registerOnCardRemoved(std::function<void()> onCardRemobed);
  void init();
};
#endif