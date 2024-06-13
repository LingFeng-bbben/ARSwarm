#include "includes.h"

extern esp_websocket_client_handle_t wsclient;
extern String macid;

bool SendTextToWS(esp_websocket_client_handle_t client, String data)
{
    if (esp_websocket_client_is_connected(client))
    {
        Serial.println("Sending: " + data);
        const char *buf = data.c_str();
        esp_websocket_client_send_text(client, buf, strlen(buf), portMAX_DELAY);
        return true;
    }
    else
    {
        return false;
    }
}

void ReceiveTextWS(String message)
{
    String command = message.substring(0, 5);
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
}