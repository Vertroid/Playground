package com.vertex.plugin.service;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.util.Log;

public class UsbService extends BroadcastReceiver {
    private final String TAG = "UsbService";

    public UsbService() {
        Log.i(TAG, "UsbService");
    }

    @Override
    public void onReceive(Context cotext, Intent intent) {
        String action = intent.getAction();
        Log.e(TAG, "Broadcast Received Action: " + action);
        switch (action) {
            case Intent.ACTION_MEDIA_MOUNTED:
            case Intent.ACTION_MEDIA_UNMOUNTED:
            case Intent.ACTION_MEDIA_EJECT:
        }
    }
}
