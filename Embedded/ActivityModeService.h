#ifndef _ActivityModeService_H_
#define _ActivityModeService_H_
#include <Arduino.h>

enum ActivityMode {
  wardrobe = 1,
  laundry = 2
};

class ActivityModeService {
  byte modeUpdatePin;
  bool updateModeDebounce = false;
  ActivityMode mode = wardrobe;
  std::function<void(ActivityMode)> onModeUpdated;
  
public:
  ActivityModeService(byte updatePin);
  void update();
  ActivityMode getMode();
  void registerOnModeUpdate(std::function<void(ActivityMode)> onModeUpdated);
};

String modeToString(ActivityMode mode);
#endif