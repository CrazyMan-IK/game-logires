package com.crazy_games.AndroidNative;

public interface CustomEventsHandler
{
    void VolumeChanged(double value);
    void BatteryLevelChanged(double value);
    void BatteryChargingStateChanged(boolean value);
}