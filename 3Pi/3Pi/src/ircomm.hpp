#include "includes.hpp"

#ifndef IRCOMM_DATA_H
#define IRCOMM_DATA_H

#pragma pack 1

// A general status structure to discover
// what mode the board is in, and the number
// of messages received correctly or with error
// The max possible size of an i2c struct is
// 32 bytes.
typedef struct i2c_status
{
    uint8_t mode;           // 1  bytes
    uint16_t fail_count[4]; // 8 bytes
    uint16_t pass_count[4]; // 8 bytes
} i2c_status_t;

// Small struct used to change mode.
// It's also 1 byte, so convenient to
// pass around 1 byte of data.
typedef struct i2c_mode
{
    uint8_t mode;
} i2c_mode_t;

// Struct to track the activity levels
// of the receivers
typedef struct i2c_activity
{
    float rx[4]; // 4x4 = 16bytes
} i2c_activity_t;

// Struct to report just the estimated
// direction of neighbours
typedef struct i2c_bearing
{
    float theta;
    float mag;
} i2c_bearing_t;

typedef struct i2c_sensors
{
    int16_t ldr[3];  // 6 bytes
    int16_t prox[2]; // 4 bytes
} i2c_sensors_t;

#define MODE_REPORT_STATUS 0
#define MODE_STOP_TX 1
#define MODE_REPORT_LDR0 2
#define MODE_REPORT_LDR1 3
#define MODE_REPORT_LDR2 4
#define MODE_STATUS_MSG0 5
#define MODE_STATUS_MSG0 6
#define MODE_STATUS_MSG1 7
#define MODE_STATUS_MSG2 8
#define MODE_STATUS_MSG3 9
#define MODE_REPORT_MSG0 10
#define MODE_REPORT_MSG1 11
#define MODE_REPORT_MSG2 12
#define MODE_REPORT_MSG3 13
#define MODE_REPORT_SENSORS 14
#define MODE_RESET_COUNTS 15
#define MODE_REPORT_ACTIVITY 16
#define MODE_REPORT_DIRECTION 17
#define MAX_MODE 18

#endif

// A data structure to commmunicate
// the mode.
// Keep consistent between devices.
i2c_mode_t ircomm_mode;

// A data structure to receive back
// the status of the board.
// Keep consistent between devices.
i2c_status_t ircomm_status;

// Use this function to set a message to transmit to
// other robots.
void setIRMessage(char *str_to_send, int len)
{

    // Message must be shorter than 32 bytes
    // and at least 1 byte.
    if (len <= 32 && len >= 1)
    {

        // The communication board will always default
        // to waiting to receive a message to transmit
        // so we don't need to change the mode.
        Wire.beginTransmission(IRCOMM_I2C_ADDR);
        Wire.write((byte *)str_to_send, len);
        Wire.endTransmission();

        char msg[42];
        memset(msg, 0, 42);
        sprintf(msg, "IRSED %s", str_to_send);
        i2cSend(msg);
    }
}

void getSensors()
{
    // Set mode to read fetch sensor data
    ircomm_mode.mode = MODE_REPORT_SENSORS;
    Wire.beginTransmission(IRCOMM_I2C_ADDR);
    Wire.write((byte *)&ircomm_mode, sizeof(ircomm_mode));
    Wire.endTransmission();

    // struct to store sensor data
    i2c_sensors_t sensors;

    // Read across bytes
    Wire.requestFrom(IRCOMM_I2C_ADDR, sizeof(sensors));
    Wire.readBytes((uint8_t *)&sensors, sizeof(sensors));

    Serial.println("Sensors:");
    Serial.print(" - LDR: ");
    for (int i = 0; i < 3; i++)
    {
        Serial.print(sensors.ldr[i]);
        Serial.print(",");
    }
    Serial.print("\n - Prox:");
    for (int i = 0; i < 2; i++)
    {
        Serial.print(sensors.prox[i]);
        Serial.print(",");
    }
    Serial.println();
}

// Get the latest message from the communication board
// from receiver "which_rx" (0,1,2,3).
// This is a little bit more complicated because we don't
// know how long a message will be. So we have to first
// ask how many bytes are available (present).
void getIRMessage(int which_rx)
{
    if (which_rx < 0 || which_rx >= 4)
    {
        // Invalid
        return;
    }

    // Select right mode to ask how many bytes are
    // available.
    if (which_rx < 0 || which_rx > 3)
    {
        // catch error first.
        return;
    }
    else if (which_rx == 0)
    {
        ircomm_mode.mode = MODE_STATUS_MSG0;
    }
    else if (which_rx == 1)
    {
        ircomm_mode.mode = MODE_STATUS_MSG1;
    }
    else if (which_rx == 2)
    {
        ircomm_mode.mode = MODE_STATUS_MSG2;
    }
    else if (which_rx == 3)
    {
        ircomm_mode.mode = MODE_STATUS_MSG3;
    }

    // Set mode to read back how many bytes are
    // available.
    Wire.beginTransmission(IRCOMM_I2C_ADDR);
    Wire.write((byte *)&ircomm_mode, sizeof(ircomm_mode));
    Wire.endTransmission();

    // We'll use this struct to check how many
    // bytes are available of a message.
    // 0 bytes means no message.
    i2c_mode msg_status;

    // Request the message size to be sent across into
    // msg_status
    Wire.requestFrom(IRCOMM_I2C_ADDR, sizeof(msg_status));
    Wire.readBytes((uint8_t *)&msg_status, sizeof(msg_status));

    // If data is available, we change mode to read it
    // across.
    if (msg_status.mode <= 0 || msg_status.mode > 32)
    {
        // catch error first
        // message size shouldn't be -ve, 0 or more than
        // 32 bytes.

        // Serial.println("nomsg,or msg>32");

        return;
    }
    else
    { // bytes available > 0

        // Format mode request for which receiver
        if (which_rx == 0)
        {
            ircomm_mode.mode = MODE_REPORT_MSG0;
        }
        else if (which_rx == 1)
        {
            ircomm_mode.mode = MODE_REPORT_MSG1;
        }
        else if (which_rx == 2)
        {
            ircomm_mode.mode = MODE_REPORT_MSG2;
        }
        else if (which_rx == 3)
        {
            ircomm_mode.mode = MODE_REPORT_MSG3;
        }
        else
        {
            // error should have been caught much earlier.
        }

        // Set mode to send across a full message
        Wire.beginTransmission(IRCOMM_I2C_ADDR);
        Wire.write((byte *)&ircomm_mode, sizeof(ircomm_mode));
        Wire.endTransmission();

        // char array to store message into
        char buf[32];
        memset(buf, 0, 32);
        int count = 0;
        Wire.requestFrom(IRCOMM_I2C_ADDR, msg_status.mode);
        while (Wire.available() && count < 32)
        {
            char c = Wire.read();
            buf[count] = c;
            count++;
        }

        // Mainly debugging here.
        // Need to decide what to do with the char array
        // once a message has been sent across.
        if (count > 0)
        {
            char msg[42];
            memset(msg, 0, 42);
            sprintf(msg, "IRRCV %d %s", which_rx, buf);
            Serial.println(msg);
            i2cSend(msg);
            // Serial.println( buf );
        }
    }
}