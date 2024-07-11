#include "includes.hpp"

void setup()
{
  Wire.setClock(400000);
  // 3Pi as master, just in case we need some "npc" robot without m5
  Wire.begin();
  Serial.begin(9600);
  iInit();
  eInit();
  mInit();
  tInit();
  i2cInit();
}

const int threshold = 2500;
int loopi = 0;
int turn = 0;
void loop()
{
  // getSensors();

  iUpdateIRs();

  char msg[32];

  // read virtual sensor
  i2cSend("MTREQ");
  Wire.requestFrom(M5_I2C_ADDR, 32, true);
  memset(msg, 0, 32);
  for (int i = 0; i < 32; i++)
  {
    char buf = Wire.read();
    if (buf == -1)
      break;
    msg[i] = buf;
  }
  const char sep[2] = ",";
  int distance = atoi(strtok(msg, sep));
  int angle = atoi(strtok(NULL, sep));
  // Serial.println(distance);
  // Serial.println(angle);

  if (iRValue[1] > threshold ||
      iRValue[2] > threshold ||
      iRValue[3] > threshold ||
      iRValue[0] > threshold ||
      iRValue[4] > threshold ||
      bPValue[0] > 1000 ||
      bPValue[1] > 1000)
  {
    sprintf(msg, "Bump!Edge!");
    i2cSend(msg);
    pSetSpeed(-10, -10);
    delay(500);
    pSetSpeed(-10, 10);
    delay(random(300, 1000));
    pSetSpeed(10, 10);
  }

  else
  {
    if (distance < 30)
    {
      sprintf(msg, "SeekBall");
      i2cSend(msg);
      // chase the ball
      if (angle > 30)
      {
        pSetSpeed(5, -5);
      }
      else if (angle > 10)
      {
        pSetSpeed(10, 5);
      }
      else if (angle < -30)
      {
        pSetSpeed(-5, 5);
      }
      else if (angle < -10)
      {
        pSetSpeed(5, 10);
      }
      else
      {
        pSetSpeed(10, 10);
      }
    }
    else
    {
      sprintf(msg, "Walk..");
      i2cSend(msg);
      // random walk
      if (loopi > 200)
      {
        turn = random(-5, 5);
        loopi = 0;
      }
      pSetSpeed(8 + turn, 8 - turn);
      loopi++;
    }
  }

  // sprintf(msg,"Hello mate");
  // //read ir
  // setIRMessage(msg,32);
  // for(int i=0;i<4;i++){
  //   getIRMessage(i);
  // }
  // read vitural sensor

  // memset(msg, 0, 32);
  // sprintf(msg, "BPSEN %d,%d", iRValue[0], iRValue[1]);
  // i2cSend(msg);
  // sprintf(msg, "ECSEN %d,%d", eCount1, eCount2);
  // i2cSend(msg);
  // delay(200);
}
