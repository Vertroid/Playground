package com.rockvr.test.player;

import android.app.Activity;
import android.content.Context;
import android.opengl.GLSurfaceView;
import android.util.AttributeSet;
import android.util.Log;
import android.view.Gravity;
import android.view.SurfaceHolder;
import android.view.SurfaceView;
import android.widget.FrameLayout;

import androidx.annotation.AttrRes;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;

import com.rockvr.test.view.TestGLSurfaceView;
import com.vertex.plugin.opengl.GLRenderer;

import java.io.IOException;

public class TestVideoPlayer extends FrameLayout {
    private GLSurfaceView mGLSurfaceView;
    private GLRenderer mRenderer;
    private Context mContext;

    public TestVideoPlayer(@NonNull Context context) {
        super(context);
        init(context);
    }

    public TestVideoPlayer(@NonNull Context context, @Nullable AttributeSet attrs) {
        super(context, attrs);
        init(context);
    }

    public TestVideoPlayer(@NonNull Context context, @Nullable AttributeSet attrs, @AttrRes int defStyleAttr) {
        super(context, attrs, defStyleAttr);
        init(context);
    }

    private void init(Context context) {
        mContext = context;
        createSurfaceView();
        setFocusable(true);
    }

    public void setVideoPath(String path) {
        try {
            mRenderer.getMediaPlayer().setDataSource(path);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void start() {
        mRenderer.getMediaPlayer().start();
    }

    public void stop() {
        mRenderer.getMediaPlayer().stop();
    }

    public void pause() {
        mRenderer.getMediaPlayer().pause();
    }

    public void seek(int second) {
        // todo
    }

    public void setSpeed(float s) {
        // todo
    }

    public void applyAudioTrack() {
        // todo
    }

    private void createSurfaceView() {
        //生成一个新的surface view
        mGLSurfaceView = new TestGLSurfaceView(mContext);
        mRenderer = new GLRenderer(mContext);
        mGLSurfaceView.setRenderer(mRenderer);
        LayoutParams layoutParams = new LayoutParams(LayoutParams.MATCH_PARENT
                , LayoutParams.MATCH_PARENT, Gravity.CENTER);
        mGLSurfaceView.setLayoutParams(layoutParams);
        this.addView(mGLSurfaceView);
    }
}

