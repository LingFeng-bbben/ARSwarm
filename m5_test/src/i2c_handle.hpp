#include "includes.h"

extern QueueHandle_t ws_send_que_handle;
bool isInitMsgRecv = false;

bool isGetTagIdMod = false;

void onI2cRecv(int length)
{
    // Serial.println(xPortGetCoreID());
    isInitMsgRecv = true;
    // Serial.println(length);
    byte buf[length];
    Wire.readBytes(buf, length);
    char str[128];
    memset(str, 0, 128);
    sprintf(str, "%.*s", length, buf);

    if (strncmp(str, "GTTAG", 5) == 0)
    {
        isGetTagIdMod = true;
        return;
    }
    else
    {
        isGetTagIdMod = false;
    }

    // quit if it is null or full
    if (ws_send_que_handle == 0)
        return;
    if (xQueueIsQueueFullFromISR(ws_send_que_handle)){
        xQueueReset(ws_send_que_handle);
    }
       
    xQueueSend(ws_send_que_handle, (void *)str, 0);
    // SendTextToWS(wsclient,String(str,strleng));
    // Serial.printf("%.*s\n", length, buf);
}

extern char wsRcvBuf[32];
extern int tagId;

void onI2cReq()
{
    if (isGetTagIdMod)
    {
        char buf[5];
        memset(buf,0,5);
        itoa(tagId,buf,10);
        M5.Lcd.println(buf);
        Wire.write(buf);
    }
    else
    {
        Wire.write(wsRcvBuf);
    }
}

void I2CConnect()
{
    M5.Lcd.print("3Pi I2C Connect");
    Wire.begin(5);
    Wire.onReceive(onI2cRecv);
    Wire.onRequest(onI2cReq);
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