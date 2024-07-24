#include "includes.h"

extern esp_websocket_client_handle_t wsclient;
extern String macid;

extern unsigned long sendTime;

bool SendTextToWS(esp_websocket_client_handle_t client, String data)
{
    if (esp_websocket_client_is_connected(client))
    {
        Serial.println("Sending: " + data);
        const char *buf = data.c_str();
        esp_websocket_client_send_text(client, buf, strlen(buf), pdMS_TO_TICKS(1000));
        return true;
    }
    else
    {
        return false;
    }
}

char wsRcvBuf[32];
int tagId = -1;

void ReceiveTextWS(String message)
{
    String command = message.substring(0, 5);
    if(command == "PINGR"){
        
        M5.Lcd.println(millis()-sendTime);
    }
    if (command == "APTAG")
    {
        M5.Lcd.clear(BLACK);
        String tagbin = message.substring(6, 22);
        for (byte i = 0; i < 4; i++)
        {
            for (byte j = 0; j < 4; j++)
            {
                bool thisbit = tagbin[i + j * 4] == '1' ? true : false;
                if (thisbit)
                {
                    M5.Lcd.fillRect(i * 60 + 40, j * 60, 60, 60, WHITE);
                }
            }
        }
    }
    if (command == "TAGNM"){
        int id = message.substring(6, 22).toInt();
        M5.Lcd.println(id);
        tagId = id;
    }
    if(command == "MTSEN"){
        memset(wsRcvBuf,0,32);
        String digits = message.substring(6);
        
        sprintf(wsRcvBuf,"%s",digits.c_str());
        //Serial.println(wsRcvBuf);
    }
}