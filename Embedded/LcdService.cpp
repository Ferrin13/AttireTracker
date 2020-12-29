#include "LcdService.h"

String createEmptyString(int countChars);

LcdService::LcdService() {}

void LcdService::init() {
  lcd.init();
  lcd.backlight();
}

void LcdService::clear() {
  lcd.clear();
}

void LcdService::clearRows(int startIndex, int endIndex) {
  String blankString = createEmptyString(20);
  for(int i = startIndex; i <= endIndex; i++) {
    lcd.setCursor(0, i);
    lcd.print(blankString);
  }
}

void LcdService::displayCenteredString(String str, int rowIndex) {
  int totalOffset = 20 - str.length();
  String leftPaddingString = createEmptyString(totalOffset / 2);
  String rightPaddingString = createEmptyString((totalOffset / 2) + (totalOffset % 2));
  lcd.setCursor(0, rowIndex);
  lcd.print(leftPaddingString + str + rightPaddingString); 
}

String createEmptyString(int countChars) {
  String result = "";
  for(int i = 0; i < countChars; i++) {
    result += ' ';
  }
  return result;
  // char buffer[countChars + 1];
  // for(int i = 0; i < countChars; i++) {
  //   buffer[i] = ' ';
  // }
  // buffer[countChars] = '\0';
  // return buffer;
}