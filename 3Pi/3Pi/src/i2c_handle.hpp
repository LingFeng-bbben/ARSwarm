#include "includes.hpp"

// send all message serialized, so it is readable and easy to change.
int i2cSend(const char *message)
{
    Wire.beginTransmission(M5_I2C_ADDR);
    Wire.write(message);
    return Wire.endTransmission();
}

void i2cInit()
{
    int ret = -1;
    while (ret != 0)
    {
        // pin the m5 to wait for it
        ret = i2cSend("Hi");
        delay(1000);
    }
    Serial.println("I2C OK");
}

int i2cGetId()
{
    if (i2cSend("GTTAG") == 0)
    {
        int count = Wire.requestFrom(M5_I2C_ADDR, 5, true);
        char msg[count];
        memset(msg,0,5);
        for (int i = 0; i < count; i++)
        {
            char buf = Wire.read();
            if (buf == -1)
                break;
            msg[i] = buf;
        }
        return atoi(msg);
    }
    else
    {
        return -1;
    }
}