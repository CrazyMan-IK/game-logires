package com.crazy_games.AndroidNative;

import android.util.Log;
import android.os.Handler;
import android.os.Environment;
import android.os.BatteryManager;
import android.widget.Toast;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.ContextWrapper;
import android.media.AudioManager;
import android.database.ContentObserver;

public class AndroidNative
{
    private Context context;
    private ContextWrapper cWrapper;
    private CustomEventsHandler handler;
    private Handler unityMainThreadHandler;

    public AndroidNative(Context context, CustomEventsHandler handler)
    {
        this.context = context;
        this.cWrapper = new ContextWrapper(context);
        this.handler = handler;
        this.unityMainThreadHandler = new Handler();
        Log.d("Logires", "Library object created");

        OnSoundChanged();
        OnBatteryChanged();
    }

    private Intent GetBatteryStatusIntent()
    {
        IntentFilter ifilter = new IntentFilter(Intent.ACTION_BATTERY_CHANGED);
        return context.registerReceiver(null, ifilter);
    }
    
    private void runOnUnityThread(Runnable runnable)
    {
        if(unityMainThreadHandler != null && runnable != null) 
        {
            unityMainThreadHandler.post(runnable);
        }
    }

    private void OnSoundChanged()
    {
        Log.d("Logires", "Audio handler attaching");

        ContentObserver observer = new ContentObserver(new Handler())
        {
            @Override
            public void onChange(boolean selfChange)
            {
                handler.VolumeChanged(GetSoundVolume());
            }
        };

        context.getContentResolver().registerContentObserver(android.provider.Settings.System.CONTENT_URI, true, observer);

        Log.d("Logires", "Audio handler attached");

        handler.VolumeChanged(GetSoundVolume());
    }

    private void OnBatteryChanged()
    {
        BroadcastReceiver receiver = new BroadcastReceiver() {
            @Override
            public void onReceive(Context context, Intent intent)
            {
                double level = GetBatteryLevel(intent);
                boolean isCharging = IsBatteryCharging(intent);

                Log.d("Logires", "Battery level changed: " + level + "\nBattery charging state: " + isCharging);
                handler.BatteryLevelChanged(level);
                handler.BatteryChargingStateChanged(isCharging);
            };
        };
        IntentFilter ifilter = new IntentFilter(Intent.ACTION_BATTERY_CHANGED);
        Intent batteryStatus = context.registerReceiver(receiver, ifilter);
        
        handler.BatteryLevelChanged(GetBatteryLevel(batteryStatus));
    }

    public double GetSoundVolume()
    {
        AudioManager audio = (AudioManager)context.getSystemService(Context.AUDIO_SERVICE);

        double current = audio.getStreamVolume(AudioManager.STREAM_MUSIC);
        int max = audio.getStreamMaxVolume(AudioManager.STREAM_MUSIC);

        return (current / max) * 100;
    }

    public double GetBatteryLevel(Intent custom)
    {
        Intent batteryStatus = custom != null ? custom : GetBatteryStatusIntent();

        double level = batteryStatus.getIntExtra(BatteryManager.EXTRA_LEVEL, -1);
        int scale = batteryStatus.getIntExtra(BatteryManager.EXTRA_SCALE, -1);
        
        return level * 100 / scale;
    }

    public boolean IsBatteryCharging(Intent custom)
    {
        Intent batteryStatus = custom != null ? custom : GetBatteryStatusIntent();

        int status = batteryStatus.getIntExtra(BatteryManager.EXTRA_STATUS, -1);

        return  status == BatteryManager.BATTERY_STATUS_CHARGING ||
                status == BatteryManager.BATTERY_STATUS_FULL;
    }

    public void ShowToast(String message, int duration)
    {
        Toast toast = Toast.makeText(context, message, duration);

        toast.show();
    }

    public String GetDataDir()
    {
        return cWrapper.getDataDir().getAbsolutePath();
    }
}