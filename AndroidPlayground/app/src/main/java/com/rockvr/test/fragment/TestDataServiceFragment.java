package com.rockvr.test.fragment;

import android.Manifest;
import android.content.Context;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;

import androidx.activity.result.ActivityResultCallback;
import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.core.content.ContextCompat;
import com.rockvr.test.R;
import com.rockvr.test.dataservice.TestDataServiceApi;

import java.util.List;
import java.util.Map;

public class TestDataServiceFragment extends AbstractFragment {
    private ActivityResultLauncher<String[]> mPermissionResult;
    private TestDataServiceApi mDataService;

    public TestDataServiceFragment() {
        super();
        mTitle = "Data Service";
    }

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_dataservice, container, false);

        mPermissionResult = registerForActivityResult(
                new ActivityResultContracts.RequestMultiplePermissions(),
                new ActivityResultCallback<Map<String, Boolean>>() {
                    @Override
                    public void onActivityResult(Map<String, Boolean> result) {
                        mDataService.refresh();
                    }
                }
        );

        Button refreshVideoButton = view.findViewById(R.id.refreshVideoButton);
        refreshVideoButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                onClickRefreshVideo(view);
            }
        });

        Button getVideosButton = view.findViewById(R.id.getVideosButton);
        getVideosButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                onClickGetVideos(view);
            }
        });
        return view;
    }

    @Override
    public void onStart() {
        super.onStart();

        mDataService = new TestDataServiceApi(getContext());
    }

    public void onClickRefreshVideo(View view) {
        Log.d("[MOON][TEST]", "OnClickRefreshVideo");
        if(checkSelfPermissions(getContext(),
                Manifest.permission.READ_EXTERNAL_STORAGE,
                Manifest.permission.WRITE_EXTERNAL_STORAGE)) {
            mDataService.refresh();
        } else {
            mPermissionResult.launch(new String[] {Manifest.permission.READ_EXTERNAL_STORAGE, Manifest.permission.WRITE_EXTERNAL_STORAGE});
        }
    }

    public void onClickGetVideos(View view) {
        Log.d("[MOON][TEST]", "OnClickRefreshVideo");
        if(checkSelfPermissions(getContext(),
                Manifest.permission.READ_EXTERNAL_STORAGE,
                Manifest.permission.WRITE_EXTERNAL_STORAGE)) {
            mDataService.getRecentVideos();
        } else {
            mPermissionResult.launch(new String[] {Manifest.permission.READ_EXTERNAL_STORAGE, Manifest.permission.WRITE_EXTERNAL_STORAGE});
        }
    }

    private boolean checkSelfPermissions(Context context, String ... permissions) {
        if (context != null && permissions != null) {
            for (String permission : permissions) {
                if (ContextCompat.checkSelfPermission(context, permission) != PackageManager.PERMISSION_GRANTED) {
                    return false;
                }
            }
        }
        return true;
    }
}
