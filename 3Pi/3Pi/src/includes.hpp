#ifndef HEADERSDEF
#define HEADERSDEF

#define REV HIGH
#define FWD LOW
#define M1D 15
#define M2D 16
#define M1S 9
#define M2S 10
#define E1AXB 7
#define E1B 23
#define E2AXB 8
#define E2B 33
#define EMIT 11
#define IR1 12
#define IR2 A0
#define IR3 A2
#define IR4 A3
#define IR5 A4
#define BP1 4
#define BP2 5
#define IRTIMEOUT 1000 //micro seconds
#define BUZZ 6

#define M5_I2C_ADDR  5
#define IRCOMM_I2C_ADDR  8

#include <Arduino.h>
#include <Wire.h>

#include "i2c_handle.hpp"
#include "bumps.hpp"
#include "encoders.hpp"
#include "ircomm.hpp"
#include "motor.hpp"
#include "pid.h"

#endif