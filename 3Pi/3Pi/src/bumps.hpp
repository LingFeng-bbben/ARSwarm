#include "includes.hpp"
void iInit()
{
    pinMode(EMIT, INPUT);
    pinMode(IR1, INPUT);
    pinMode(IR2, INPUT);
    pinMode(IR3, INPUT);
    pinMode(IR4, INPUT);
    pinMode(IR5, INPUT);
    pinMode(BP1, INPUT);
    pinMode(BP2, INPUT);
}

void iChargeIRSensor()
{
    pinMode(IR1, OUTPUT);
    digitalWrite(IR1, HIGH);
    pinMode(IR2, OUTPUT);
    digitalWrite(IR2, HIGH);
    pinMode(IR3, OUTPUT);
    digitalWrite(IR3, HIGH);
    pinMode(IR4, OUTPUT);
    digitalWrite(IR4, HIGH);
    pinMode(IR5, OUTPUT);
    digitalWrite(IR5, HIGH);
    delayMicroseconds(10);
    pinMode(IR1, INPUT);
    pinMode(IR2, INPUT);
    pinMode(IR3, INPUT);
    pinMode(IR4, INPUT);
    pinMode(IR5, INPUT);
}

void iChargeBPSensor()
{
    pinMode(BP1, OUTPUT);
    digitalWrite(BP1, HIGH);
    pinMode(BP2, OUTPUT);
    digitalWrite(BP2, HIGH);
    delayMicroseconds(10);
    pinMode(BP1, INPUT);
    pinMode(BP2, INPUT);
}

int iRValue[5] = {0, 0, 0, 0, 0};
int bPValue[2] = {0,0};
void iUpdateIRs()
{
    pinMode(EMIT, OUTPUT);
    digitalWrite(EMIT, HIGH);
    iChargeIRSensor();
    unsigned long startTime = micros();
    unsigned long ir1t = 0, ir2t = 0, ir3t = 0, ir4t = 0, ir5t = 0;
    while (digitalRead(IR1) || digitalRead(IR2) || digitalRead(IR3) || digitalRead(IR4) || digitalRead(IR5))
    {
        // if(micros()-IRTIMEOUT>4000) break;
        if (digitalRead(IR1))
            ir1t = micros();
        if (digitalRead(IR2))
            ir2t = micros();
        if (digitalRead(IR3))
            ir3t = micros();
        if (digitalRead(IR4))
            ir4t = micros();
        if (digitalRead(IR5))
            ir5t = micros();
    }
    //pinMode(EMIT, INPUT);
    digitalWrite(EMIT, LOW);
    iRValue[0] = ir1t - startTime;
    iRValue[1] = ir2t - startTime;
    iRValue[2] = ir3t - startTime;
    iRValue[3] = ir4t - startTime;
    iRValue[4] = ir5t - startTime;
    iChargeBPSensor();
    startTime = micros();
    while (digitalRead(BP1) || digitalRead(BP2))
    {
        if (micros() - startTime > IRTIMEOUT)
            break;
        if (digitalRead(BP1))
            ir1t = micros();
        if (digitalRead(BP2))
            ir2t = micros();
    }
    pinMode(EMIT, INPUT);
    bPValue[0] = ir1t - startTime;
    bPValue[1] = ir2t - startTime;
    
    // Serial.print(ir1t - startTime);
    // Serial.print(" ");
    // Serial.print(ir2t - startTime);
    // Serial.print(" ");
    // Serial.print(ir3t - startTime);
    // Serial.print(" ");
    // Serial.print(ir4t - startTime);
    // Serial.print(" ");
    // Serial.print(ir5t - startTime);
    // Serial.print("\n");
}
