package com.vertex.plugin.service;

import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.util.Log;

import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.ActivityResultRegistry;
import androidx.activity.result.contract.ActivityResultContract;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.core.app.ActivityOptionsCompat;
import androidx.documentfile.provider.DocumentFile;

public class UsbService extends BroadcastReceiver {
    private final String TAG = "UsbService";

    private Activity thisActivity;
    private ActivityResultLauncher<Intent> openDocumentTreeLauncher;
    private ActivityResultRegistry registry = new ActivityResultRegistry() {
        @Override
        public <I, O> void onLaunch(int requestCode, @NonNull ActivityResultContract<I, O> contract, I input, @Nullable ActivityOptionsCompat options) {

        }
    };
    public UsbService(Activity activity) {
        Log.i(TAG, "UsbService");

        thisActivity = activity;
        openDocumentTreeLauncher = registry.register(
                "unique_key",
                new ActivityResultContracts.StartActivityForResult(),
                result -> {
                    if (result.getResultCode() == Activity.RESULT_OK) {
                        Uri treeUri = result.getData().getData();
                        if (treeUri != null) {
                            thisActivity.getContentResolver().takePersistableUriPermission(
                                    treeUri,
                                    Intent.FLAG_GRANT_READ_URI_PERMISSION | Intent.FLAG_GRANT_WRITE_URI_PERMISSION
                            );

                            DocumentFile otgRoot = DocumentFile.fromTreeUri(thisActivity, treeUri);
                            if (otgRoot != null) {
                                // Do something
                            }
                        }
                    }
                }
        );
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

    public void selectOTGDocument() {
        Intent intent = new Intent(Intent.ACTION_OPEN_DOCUMENT_TREE);
        intent.addCategory(Intent.CATEGORY_DEFAULT);
        openDocumentTreeLauncher.launch(intent);
    }
}
