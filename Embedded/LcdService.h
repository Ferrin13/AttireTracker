#ifndef _LcdService_H_
#define _LcdService_H_
#include <Arduino.h>
#include <LiquidCrystal_I2C.h>

class LcdService {
  LiquidCrystal_I2C lcd = LiquidCrystal_I2C(0x27, 20, 4); // Change to (0x27,20,4) for 20x4 LCD.

public:
  LcdService();
  void init();
  void clear();
  void clearRows(int startIndex, int endIndex);
  void displayCenteredString(String str, int rowIndex);
};

#endif