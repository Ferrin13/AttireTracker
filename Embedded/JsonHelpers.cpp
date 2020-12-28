#include "JsonHelpers.h"

String getStringPropertyFromIndex(String json, int index);

// Returns the first property with the given name
String getStringProperty(String json, String propName)
{
  int i = 0;
  int matchIndex = 0;
  while(i < json.length()) {
    if(matchIndex == propName.length())
      return getStringPropertyFromIndex(json, i);

    if(json.charAt(i) == propName.charAt(matchIndex))
      matchIndex++;
    else
      matchIndex = 0;

    i++;
  }
  return "";
}

String getStringPropertyFromIndex(String json, int index) {
  int i = index;
  int countQuotes = 0;
  String result = "";
  while(countQuotes < 3) { // Quote to close property name, quote to start property, quote to end it.
    char c = json.charAt(i);
    if(c == '"')
      countQuotes++;
    else if(countQuotes == 2) // In the body of the property
      result += c;

    i++;
  }
  return result;
}