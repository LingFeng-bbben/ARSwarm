#include"includes.hpp"

void mInit() {
  pinMode(M1D,OUTPUT);
  pinMode(M1S,OUTPUT);
  pinMode(M2D,OUTPUT);
  pinMode(M2S,OUTPUT);
}

void mSetSpeed(short l,short r){
  if(l>=0){
    digitalWrite(M2D,FWD);
  }else{
    digitalWrite(M2D,REV);
  }
  analogWrite(M2S,abs(l)); 
  if(r>=0){
    digitalWrite(M1D,FWD);
  }else{
    digitalWrite(M1D,REV);
  }
  analogWrite(M1S,abs(r));
}