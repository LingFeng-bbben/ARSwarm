#include "includes.h"
#include "net_handle.hpp"

TaskHandle_t Task1;
TaskHandle_t Task2;

extern esp_websocket_client_handle_t wsclient;
extern String macid;

void Task1code(void *parameter)
{
  WSConnect();
  delay(1000);
  SendTextToWS(wsclient, "IDREQ " + macid);
  vTaskDelete(Task1);
}

//TODO: 接收下位机信息，序列化消息放入队列

void Task2code(void *parameter)
{
  //TODO: 处理任务发送队列
  //Test code for the latency
  while (1)
  {
    if (M5.Touch.ispressed())
    {
      SendTextToWS(wsclient, "TOUCH " + macid);
      Serial.print("touch");
    }
    delay(20);
  }
  vTaskDelete(Task2);
}