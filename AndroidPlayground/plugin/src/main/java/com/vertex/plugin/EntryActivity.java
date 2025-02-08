package com.vertex.plugin;

import android.content.Intent;
import android.os.Bundle;

import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.ActivityResultRegistry;
import androidx.activity.result.contract.ActivityResultContract;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.core.app.ActivityOptionsCompat;

import com.unity3d.player.UnityPlayerActivity;

public class EntryActivity extends UnityPlayerActivity {
    public static EntryActivity thisActivity;
    private ActivityResultLauncher<Intent> openDocumentTreeLauncher;
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        thisActivity = this;
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        
    }
}
