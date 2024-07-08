#include "includes.hpp"

void setup()
{
  Wire.setClock(400000);
  //3Pi as master, just in case we need some "npc" robot without m5
  Wire.begin();
  Serial.begin(9600);
  iInit();
  eInit();
  mInit();
  tInit();
  i2cInit();
}

void loop()
{
  //getSensors();
  
  iUpdateIRs();
  char msg[32];
  sprintf(msg,"Hello mate");
  //read ir
  setIRMessage(msg,32);
  for(int i=0;i<4;i++){
    getIRMessage(i);
  }
  //read vitural sensor
  i2cSend("MTREQ");
  Wire.requestFrom(M5_I2C_ADDR,32,true);
  memset(msg,0,32);
  for(int i=0;i<32;i++){
    char buf = Wire.read();
    if(buf == -1) break;
    msg[i] = buf;
  }
  const char sep[2] = ",";
  int param1 = atoi(strtok(msg,sep));
  int param2 = atoi(strtok(NULL,sep));
  pSetSpeed(param1,param2);
  
  sprintf(msg,"BPSEN %d,%d",iRValue[0],iRValue[1]);
  i2cSend(msg);
  sprintf(msg,"ECSEN %d,%d",eCount1,eCount2);
  i2cSend(msg);
  delay(200);
}
