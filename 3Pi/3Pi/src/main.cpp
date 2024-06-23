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
  iUpdateIRs();
  char msg[25];
  sprintf(msg,"IRSEN %d,%d",iRValue[0],iRValue[1]);
  Serial.println(i2cSend(msg));
  sprintf(msg,"ECSEN %d,%d",eCount1,eCount2);
  Serial.println(i2cSend(msg));
  delay(200);
}
