package com.rockvr.test.view;

import android.content.Context;
import android.opengl.GLSurfaceView;

public class TestGLSurfaceView extends GLSurfaceView {
    public TestGLSurfaceView(Context context) {
        super(context);
        setEGLContextClientVersion(2);
    }
}
