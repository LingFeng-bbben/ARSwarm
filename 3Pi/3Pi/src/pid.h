// this #ifndef stops this file
// from being included mored than
// once by the compiler. 
#define PIDHZ 20
#include "includes.hpp"
#ifndef _PID_H
#define _PID_H

// Class to contain generic PID algorithm.
class PID {
  private:
    float err_sum = 0;
    float last_err = 0;
  public:
    float kp = 1, ki = 0, kd = 0;
    // Constructor, must exist.
    PID(float _kp,float _ki, float _kd) {
      kp = _kp;
      ki = _ki;
      kd = _kd;
    } 
    float update(float demand, float measure){
      float err = demand - measure;
      err_sum += err * 0.05f;
      float p = kp * err;
      float i = ki * err_sum;
      float d = kd * ( (err - last_err) / 0.05f);
      //Serial.println(d);
      //if (p>max)
      last_err = err;
      return p+i+d;
    }
};

void tInit(){
  cli();
  TCCR3A = 0;
  TCCR3B = 0;
  TCCR3B |= (1 << WGM32);
  TCCR3B |= (1 << CS32);
  OCR3A = 62500/PIDHZ; 
  TIMSK3 = TIMSK3 | (1 << OCIE3A);
  sei();
}

int tLastCount1 = 0;
int tMoSpeed1 = 0;
volatile short pTargetSpd1;
short mPWMPower1;

int tLastCount2 = 0;
int tMoSpeed2 = 0;
volatile short pTargetSpd2;
short mPWMPower2;

PID mPid1(1.0f,0.001f,0.001f);
PID mPid2(1.0f,0.001f,0.001f);

//motor control loop

volatile float x=0,y=0,r=0.017,l=0.088;
volatile float heading = 0;
ISR( TIMER3_COMPA_vect ) {
  //PID update
  tMoSpeed1 = eCount1 - tLastCount1;
  tMoSpeed2 = eCount2 - tLastCount2;
  tLastCount2 = eCount2;
  tLastCount1 = eCount1;
  
  if(pTargetSpd1 != 0)
    mPWMPower1 += mPid1.update(pTargetSpd1,tMoSpeed1);
  else
    mPWMPower1 = 0;
  if(mPWMPower1>255)mPWMPower1=255;
  if(mPWMPower1<-255)mPWMPower1=-255;


  if(pTargetSpd2 != 0)
    mPWMPower2 += mPid2.update(pTargetSpd2,tMoSpeed2);
  else
    mPWMPower2 = 0;
  if(mPWMPower2>255)mPWMPower2=255;
  if(mPWMPower2<-255)mPWMPower2=-255;
  mSetSpeed(mPWMPower2,mPWMPower1);
  //Kinematics
  float vl = tMoSpeed2 * 0.06981f * r; //0.
  float vr = tMoSpeed1 * 0.06981f * r;
  // cm per tick(0.1s)
  float rspeed = (vr-vl)/l;
  float yspeed = (vl+vr)/2;
  heading += rspeed;
  x += yspeed * cos(heading);
  y += yspeed * sin(heading);

  

//  Serial.print(x);
//  Serial.print(" ");
//  Serial.print(y);
//  Serial.print(" ");
//  Serial.println(heading);
  
  
}

void pSetSpeed(int l, int r){
  pTargetSpd1 = r/2;
  pTargetSpd2 = l/2;
}


#endif