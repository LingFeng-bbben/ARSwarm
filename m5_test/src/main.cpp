#include "includes.h"
#include "Tasks.hpp"
#include "tsumu.hpp"

void setup()
{
  // put your setup code here, to run once:
  M5.begin(true, false, true, false);

  M5.Lcd.drawJpg(TSUMU,sizeof(TSUMU),0,0);
  M5.Lcd.setTextColor(GREEN);
  M5.Lcd.setTextSize(1);
  M5.Lcd.println(TSUMU_TITLE);
  M5.Lcd.print("Comp Time: ");
  M5.Lcd.print(__DATE__);
  M5.Lcd.print(" ");
  M5.Lcd.println(__TIME__);
  M5.Lcd.println("***Booting***\n");
  M5.Lcd.print("Battery voltage:");
  M5.Lcd.println(M5.Axp.GetBatVoltage());

  M5.Lcd.print("Connecting Wifi");
  WifiConnect();
  // This core for the WS
  xTaskCreatePinnedToCore(Task1code, "Task1", 10000, NULL, 1, &Task1, 1);
  // This core for the I2C/UART
  xTaskCreatePinnedToCore(Task2code, "Task2", 10000, NULL, 1, &Task2, 0);
}

void loop()
{
}
