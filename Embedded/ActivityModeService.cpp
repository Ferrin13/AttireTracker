#include <Arduino.h>
#include "ActivityModeService.h"

static ActivityMode getNextMode(ActivityMode currentMode);

ActivityModeService::ActivityModeService(byte updatePin)
{
  modeUpdatePin = updatePin;
  pinMode(modeUpdatePin, INPUT_PULLUP);
}

void ActivityModeService::update()
{ 
  bool buttonIsPressed = !digitalRead(modeUpdatePin); //Pin is pulled high
  if(buttonIsPressed && !updateModeDebounce) 
  {
    updateModeDebounce = true;
    mode = getNextMode(mode);
    Serial.println("Mode set to: " + modeToString(mode));
  }
  else if (!buttonIsPressed) {
    updateModeDebounce = false;
  }
}

ActivityMode ActivityModeService::getMode()
{ 
  return mode;
}

String modeToString(ActivityMode mode)
{
  switch (mode){
    case wardrobe: return "Wardrobe";
    case laundry: return "Laundry";
  }
}

static ActivityMode getNextMode(ActivityMode currentMode)
{
  return currentMode == wardrobe ? laundry : wardrobe;
}