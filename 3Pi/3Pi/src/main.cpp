#include <Arduino.h>
#include <Wire.h>

int i2cSend(const char *message)
{
  Wire.beginTransmission(5);
  Wire.write(message);
  return Wire.endTransmission();
}

void setup()
{
  Wire.setClock(100000);
  Wire.begin();
  Serial.begin(9600);
  int ret = -1;
  while (ret != 0)
  {
    ret = i2cSend("Hi");
  }
  Serial.println("I2C OK");
}

void loop()
{
  Serial.println(i2cSend("Ping"));
  delay(1000);
}
