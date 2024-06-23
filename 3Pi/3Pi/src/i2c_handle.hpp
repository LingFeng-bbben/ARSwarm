#include "includes.hpp"

//send all message serialized, so it is readable and easy to change.
int i2cSend(const char *message)
{
    Wire.beginTransmission(5);
    Wire.write(message);
    return Wire.endTransmission();
}

void i2cInit()
{
    int ret = -1;
    while (ret != 0)
    {
        //pin the m5 to wait for it
        ret = i2cSend("Hi");
        delay(1000);
    }
    Serial.println("I2C OK");
}