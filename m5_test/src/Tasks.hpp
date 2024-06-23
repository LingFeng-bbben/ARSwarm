#include "includes.h"


TaskHandle_t Task1;
TaskHandle_t Task2;

extern esp_websocket_client_handle_t wsclient;
extern String macid;

QueueHandle_t ws_send_que_handle;

void Task1code(void *parameter)
{
  WSConnect();
  SendTextToWS(wsclient, "IDREQ " + macid);
  vTaskDelete(Task1);
}

//TODO: 接收下位机信息，序列化消息放入队列

void Task2code(void *parameter)
{
  char rxbuf[128];
  ws_send_que_handle = xQueueCreate(5,sizeof(rxbuf));
  //TODO: 处理任务发送队列
  //Test code for the latency
  while (1)
  {
    if(xQueueReceive(ws_send_que_handle,&(rxbuf),(TickType_t)5)){
      SendTextToWS(wsclient,String(rxbuf,sizeof(rxbuf)));
      vTaskDelay(pdMS_TO_TICKS(20));
    }
  }
  vTaskDelete(Task2);
}