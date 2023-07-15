package com.rockvr.test.fragment;

import android.content.ContentResolver;
import android.content.ContentUris;
import android.content.Context;
import android.content.Intent;
import android.database.Cursor;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.Environment;
import android.provider.DocumentsContract;
import android.provider.MediaStore;
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
import androidx.core.graphics.PathUtils;

import com.rockvr.test.R;
import com.rockvr.test.player.TestVideoPlayer;
import com.rockvr.test.utility.FileUtils;

import java.io.File;

public class TestVideoApiFragment extends AbstractFragment {
    private TestVideoPlayer mPlayer;
    private ActivityResultLauncher<String[]> mFileDialogResult;
    private View mView;

    public TestVideoApiFragment() {
        super();
        mTitle = "Video API";
    }

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        mView = inflater.inflate(R.layout.fragment_video_api, container, false);

        Button startButton = mView.findViewById(R.id.startButton);
        startButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                onClickStartButton(view);
            }
        });

        Button stopButton = mView.findViewById(R.id.stopButton);
        stopButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                onClickStopButton(view);
            }
        });

        Button pauseButton = mView.findViewById((R.id.pauseButton));
        pauseButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                onClickPauseButton(view);
            }
        });

        Button selectVideoButton = mView.findViewById(R.id.selectVideoButton);
        selectVideoButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                onClickSelectVideoButton(view);
            }
        });

        Button seekButton = mView.findViewById(R.id.seekButton);
        seekButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                onClickSeekButton(view);
            }
        });

        Button audioTrackButton = mView.findViewById(R.id.audioTrackButton);
        audioTrackButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                onClickAudioTrackButton(view);
            }
        });

        mFileDialogResult = registerForActivityResult(
                new ActivityResultContracts.OpenDocument(),
                new ActivityResultCallback<Uri>() {
                    @Override
                    public void onActivityResult(Uri result) {
                        onSelectVideoCallback(result);
                    }
                }
        );

        return mView;
    }

    @Override
    public void onStart() {
        super.onStart();

        mPlayer = mView.findViewById(R.id.testVideoPlayer);
    }

    public void onClickStartButton(View view) {
        mPlayer.start();
    }

    public void onClickStopButton(View view) {
        mPlayer.stop();
    }

    public void onClickPauseButton(View view) {
        mPlayer.pause();
    }

    public void onClickSelectVideoButton(View view) {
        mFileDialogResult.launch(new String[] { "video/*" });
    }

    public void onClickSeekButton(View view) {
        mPlayer.seek(1);
    }

    public void onClickAudioTrackButton(View view) {
        mPlayer.applyAudioTrack();
    }

    public void onSelectVideoCallback(Uri uri) {
        File file = new File(uri.getPath());
        String path = FileUtils.getFilePathByUri(getContext(), uri);
        mPlayer.setVideoPath(path);
    }
}
