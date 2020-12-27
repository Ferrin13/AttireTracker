#include "RfidService.h"

static String byteArrayToString(byte *buffer, byte bufferSize);

RfidService::RfidService(byte selectPin, byte resetPin) {
  mfrc522 = MFRC522(selectPin, resetPin);
}

void RfidService::init() {
  SPI.begin();
  mfrc522.PCD_Init();
}

void RfidService::listenForAndHandleRfidChange()
{
  bool updatedCardIsPresent = getCardIsPresentDebounced();
  if (cardIsPresent != updatedCardIsPresent)
  {
    cardIsPresent = updatedCardIsPresent;
    onRfidStatusChange(cardIsPresent);
  }
}

void RfidService::registerOnCardDetected(std::function<void(String)> onCardDetected)
{
  // char buffer[50];
  // sprintf(buffer, "Test function initial address is %p", reinterpret_cast<void*&>(onCardDetected));
  // Serial.println(buffer);
  this->onCardDetected = onCardDetected;
}

void RfidService::registerOnCardRemoved(std::function<void()> onCardRemoved)
{
  this->onCardRemoved = onCardRemoved;
}

void RfidService::onRfidStatusChange(bool newStatus)
{
  if(cardIsPresent)
    onCardDetected(byteArrayToString(mfrc522.uid.uidByte, mfrc522.uid.size));
  else
    onCardRemoved();
}

bool RfidService::getCardIsPresentDebounced()
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

bool RfidService::getCardIsPresentRaw()
{
  return mfrc522.PICC_IsNewCardPresent() && mfrc522.PICC_ReadCardSerial();
}

static String byteArrayToString(byte *buffer, byte bufferSize) {
  String result = "";
  for (byte i = 0; i < bufferSize; i++) {
    result += buffer[i] < 0x10 ? "0" : "";
    result += String(buffer[i], HEX);
  }
  result.toLowerCase();
  return result;
}