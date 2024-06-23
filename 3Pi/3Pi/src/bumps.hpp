#include "includes.hpp"

void iInit()
{
    pinMode(EMIT, INPUT);
    pinMode(IR1, INPUT);
    pinMode(IR2, INPUT);
}

void iChargeIRSensor()
{
    pinMode(IR1, OUTPUT);
    digitalWrite(IR1, HIGH);
    pinMode(IR2, OUTPUT);
    digitalWrite(IR2, HIGH);
    delayMicroseconds(10);
    pinMode(IR1, INPUT);
    pinMode(IR2, INPUT);
}

int iRValue[2] = {0, 0};
void iUpdateIRs()
{
    pinMode(EMIT, OUTPUT);
    digitalWrite(EMIT, HIGH);
    iChargeIRSensor();
    unsigned long startTime = micros();
    unsigned long ir1t = 0, ir2t = 0;
    while (digitalRead(IR1) || digitalRead(IR2))
    {
        if (micros() - startTime > IRTIMEOUT)
            break;
        if (digitalRead(IR1))
            ir1t = micros();
        if (digitalRead(IR2))
            ir2t = micros();
    }
    pinMode(EMIT, INPUT);
    digitalWrite(EMIT, LOW);
    iRValue[0] = ir1t - startTime;
    iRValue[1] = ir2t - startTime;
    //  Serial.print(ir1t-startTime);
    //  Serial.print(" ");
    //  Serial.print(ir2t-startTime);
    //  Serial.print("\n");
}
