#include "includes.h"

extern QueueHandle_t ws_send_que_handle;
bool isInitMsgRecv = false;

void onI2cRecv(int length)
{
    Serial.println(xPortGetCoreID());
    isInitMsgRecv = true;
    //Serial.println(length);
    byte buf[length];
    Wire.readBytes(buf, length);
    char str[128];
    sprintf(str,"%.*s", length, buf);
    if(ws_send_que_handle ==0)return;
    xQueueSend(ws_send_que_handle,(void*)str,0);
    //SendTextToWS(wsclient,String(str,strleng));
    //Serial.printf("%.*s\n", length, buf);
}

void I2CConnect()
{
    M5.Lcd.print("3Pi I2C Connect");
    Wire.begin(5);
    Wire.onReceive(onI2cRecv);
    for (int i = 0; i < 5; i++)
    {
        M5.Lcd.print(".");
        delay(500);
        if (isInitMsgRecv)
            break;
    }
    if (isInitMsgRecv)
    {
        M5.Lcd.println("OK");
    }
    else
    {
        M5.Lcd.setTextColor(RED);
        M5.Lcd.print("Failed");
        delay(5000);
        esp_restart();
    }
}