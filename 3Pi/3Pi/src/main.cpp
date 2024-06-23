#include "includes.hpp"

void setup()
{
  Wire.setClock(400000);
  //3Pi as master, just in case we need some "npc" robot without m5
  Wire.begin();
  Serial.begin(9600);
  iInit();
  eInit();
  i2cInit();
}

void loop()
{
  //getSensors();
  
  iUpdateIRs();
  char msg[32];
  sprintf(msg,"Hello mate");
  setIRMessage(msg,32);
  for(int i=0;i<4;i++){
    getIRMessage(i);
  }
  sprintf(msg,"BPSEN %d,%d",iRValue[0],iRValue[1]);
  i2cSend(msg);
  sprintf(msg,"ECSEN %d,%d",eCount1,eCount2);
  i2cSend(msg);
  delay(200);
}
