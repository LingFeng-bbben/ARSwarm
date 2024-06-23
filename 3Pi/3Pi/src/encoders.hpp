#include "includes.hpp"

bool lasta1 = 0;
bool lastb1 = 0;
bool lasta2 = 0;
bool lastb2 = 0;
int eCount1 = 0;
int eCount2 = 0;

void eTrigInt1(){
  bool b = digitalRead(E1B);
  bool a = b^digitalRead(E1AXB);
  if(!lasta1&&a&&!b){
    eCount1++;
    //Serial.print("1:");
    //Serial.println(eCount1);
  }
  if(!lastb1&&b&&!a){
    eCount1--;
    //Serial.print("1:");
    //Serial.println(eCount1);
  }
  lasta1 = a;
  lastb1 = b;
}

void eTrigInt2(){
  bool b = PINE&B00000100; //get PE2
  bool a = b^digitalRead(E2AXB);
  if(!lasta2&&a&&!b){
    eCount2++;
    //Serial.print("2:");
    //Serial.println(eCount2);
  }
  if(!lastb2&&b&&!a){
    eCount2--;
    //Serial.print("2:");
    //Serial.println(eCount2);
  }
  lasta2 = a;
  lastb2 = b;
}

ISR (PCINT0_vect) {
  //if(digitalRead(E2AXB)!=HIGH) return;
  eTrigInt2();
}

void eInit() {
  pinMode(E1AXB, INPUT);
  pinMode(E1B,INPUT);
  pinMode(E2AXB,INPUT);
  pinMode(E2B, INPUT);  
  attachInterrupt(digitalPinToInterrupt(E1AXB), eTrigInt1, HIGH);
  PCICR  |= B00000001; //enable PB pin change intrrupt
  PCMSK0 |= B00010000; //only accept from PB4
  //DDRE   |= B00000000; //PE2 mode=input

}