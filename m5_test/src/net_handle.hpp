#include "includes.h"


esp_websocket_client_handle_t wsclient;
String macid;

void WifiConnect()
{
    M5.Lcd.print("Connecting Wifi");
    WiFi.setSleep(WIFI_PS_NONE);
    WiFi.begin(WIFI_SSID, WIFI_PASS);
    for (int i = 0; i < 5; i++)
    {
        M5.Lcd.print(".");
        delay(500);
        if (WiFi.isConnected())
            break;
    }
    if (WiFi.isConnected())
    {
        M5.Lcd.print("OK,");
        M5.Lcd.print("IP addr:");
        M5.Lcd.println(WiFi.localIP().toString());
        M5.Lcd.print("MAC addr:");
        macid = WiFi.macAddress();
        M5.Lcd.println(macid);
    }
    else
    {
        M5.Lcd.setTextColor(RED);
        M5.Lcd.print("Failed");
        delay(5000);
        esp_restart();
    }
}

void websocket_event_handler(void *handler_args, esp_event_base_t base, int32_t event_id, void *event_data)
{
    esp_websocket_event_data_t *data = (esp_websocket_event_data_t *)event_data;
    String str = "";
    switch (event_id)
    {
    case WEBSOCKET_EVENT_CONNECTED:
        Serial.println("WEBSOCKET_EVENT_CONNECTED");
        break;
    case WEBSOCKET_EVENT_DISCONNECTED:
        //Serial.println("WEBSOCKET_EVENT_DISCONNECTED");
        M5.Lcd.setTextColor(RED);
        M5.Lcd.print("WS Disconnected...Reboot");
        delay(5000);
        esp_restart();
        break;
    case WEBSOCKET_EVENT_DATA:
        //Serial.println("WEBSOCKET_EVENT_DATA, OPCODE:" + data->op_code);
        if(data->data_len==0) return;
        if (data->op_code == 0x2) { // Opcode 0x2 indicates binary data
            Serial.printf("Received binary data", data->data_ptr, data->data_len);
        } else if (data->op_code == 0x08 && data->data_len == 2) {
            Serial.printf("Received closed message with code=%d", 256 * data->data_ptr[0] + data->data_ptr[1]);
        } else {
            //Serial.printf("Received=%.*s\n\n", data->data_len, (char *)data->data_ptr);
            str = String(data->data_ptr,data->data_len);
            ReceiveTextWS(str);
        }
        break;
    case WEBSOCKET_EVENT_ERROR:
        //Serial.println("WEBSOCKET_EVENT_ERROR");
        M5.Lcd.setTextColor(RED);
        M5.Lcd.print("WS ERROR...Reboot");
        delay(5000);
        esp_restart();
        break;
    }
}

esp_websocket_client_handle_t WSConnect()
{
    esp_websocket_client_config_t websocket_cfg = {};
    websocket_cfg.uri = WEBSOCKET_URI;
    wsclient = esp_websocket_client_init(&websocket_cfg);
    M5.Lcd.print("Connecting WebSocket");
    esp_websocket_client_start(wsclient);
    for (int i = 0; i < 5; i++)
    {
        M5.Lcd.print(".");
        delay(500);
        if (esp_websocket_client_is_connected(wsclient))
            break;
    }
    if (esp_websocket_client_is_connected(wsclient))
    {
        esp_websocket_register_events(wsclient, WEBSOCKET_EVENT_ANY, websocket_event_handler, (void *)wsclient);
        M5.Lcd.println("OK");
        return wsclient;
    }
    else
    {
        M5.Lcd.setTextColor(RED);
        M5.Lcd.println("Failed");
        delay(5000);
        esp_restart();
    }
}


