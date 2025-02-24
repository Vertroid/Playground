package com.vertex.plugin.opengl;

import android.content.Context;
import android.graphics.SurfaceTexture;
import android.net.Uri;
import android.opengl.GLES11Ext;
import android.opengl.GLES20;
import android.opengl.GLES30;
import android.opengl.GLSurfaceView;
import android.util.Log;
import android.view.Surface;

import com.vertex.plugin.player.IMediaPlayer;
import com.vertex.plugin.utils.GLESUtils;

import com.vertex.plugin.player.vlc.VLCMediaPlayer;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.FloatBuffer;

import javax.microedition.khronos.egl.EGLConfig;
import javax.microedition.khronos.opengles.GL10;

public class GLRenderer implements GLSurfaceView.Renderer, SurfaceTexture.OnFrameAvailableListener {
    private final String VSH_CODE =
            "uniform mat4 uSTMatrix;\n"+
                    "attribute vec4 aPosition;\n"+
                    "attribute vec4 aTexCoord;\n"+
                    "varying vec2 vTexCoord;\n"+
                    "void main(){\n"+
                    "   vTexCoord = aTexCoord.xy;\n"+
                    "   gl_Position = aPosition;\n"+
                    "}";

    private  final String FSH_CODE =
            "#extension GL_OES_EGL_image_external : require\n"+
                    "precision mediump float;\n"+
                    "varying vec2 vTexCoord;\n"+
                    //"uniform mat4 uColorMatrix;\n"+
                    "uniform samplerExternalOES sTexture;\n"+
                    "void main() {\n"+
                    //" gl_FragColor = uColorMatrix*texture2D(sTexture, vTexCoord).rgba;\n"+
                    "   gl_FragColor = texture2D(sTexture, vTexCoord).rgba;\n"+
                    "}";
    private final String VSH_2D_CODE =
            "attribute vec4 aPosition; " +
                    "attribute vec2 aTexCoord; " +
                    "varying vec2 vTexCoord; " +
                    "void main() { " +
                    "    vTexCoord = aTexCoord; " +
                    "    gl_Position = aPosition; " +
                    "}";

    private final String FSH_2D_CODE =
            "precision mediump float;\n" +
                    "varying vec2 vTexCoord;\n" +
                    "uniform sampler2D sTexture;\n" +
                    "void main() {\n" +
                    "   gl_FragColor = texture2D(sTexture, vTexCoord);\n" +
                    //"    gl_FragColor = vec4(color, color, color, 1);\n" +
                    "}";


    //顶点着色器坐标，z为0
    private final float[] vertices = {
            -1.0f, -1.0f, 0.0f,
            1.0f, -1.0f, 0.0f,
            -1.0f, 1.0f, 0.0f,
            1.0f, 1.0f, 0.0f,
    };
    //纹理坐标，texture坐标ST，需要根据图像进行转换
    private final float[] fboTexCoords = {
            0.0f, 0.0f,
            1.0f, 0.0f,
            0.0f, 1.0f,
            1.0f, 1.0f
    };

    private final float[] displayTexCoords = {
            0.0f, 1.0f,
            1.0f, 1.0f,
            0.0f, 0.0f,
            1.0f, 0.0f
    };

    private int mOESTextureId;
    private int mFrameTextureId;
    private int mFBOId;
    private int mOESProgramId;
    private int mFrameProgramId;
    private FloatBuffer vertexBuffer;
    private int mVertexBufferId;
    private FloatBuffer displayVertexBuffer;
    private int mDisplayBufferId;
    private FloatBuffer fboVertexBuffer;
    private int mFboVertexBufferId;
    private int mSTMatrixHandle;
    private int aPositionOES;
    private int aTexCoordOES;
    private int sTextureOES;
    private int aPosition2D;
    private int aTexCoord2D;
    private int sTexture2D;
    private float[] mSTMatrix = new float[16];
    private SurfaceTexture mSurfaceTexture;
    private VLCMediaPlayer mediaPlayer;
    private Context mContext;
    private int mWidth;
    private int mHeight;
    private boolean mUpdateFrame = false;

    public GLRenderer(Context context) {
        super();
        this.mContext = context;
    }

    public IMediaPlayer getMediaPlayer() {
        return mediaPlayer;
    }

    @Override
    public void onSurfaceCreated(GL10 gl10, EGLConfig eglConfig) {
        initGLProgram();
        Surface surface = createSurface();
        mediaPlayer = new VLCMediaPlayer(mContext);
        mediaPlayer.setSurfaceTexture(mSurfaceTexture);
        mSurfaceTexture.setOnFrameAvailableListener(this);
        try {
            mediaPlayer.setDataSource(mContext, Uri.parse("file:///sdcard/Movies/360.mkv"));
            mediaPlayer.prepareAsync();
            mediaPlayer.start();
        } catch(Exception e) {
            e.printStackTrace();
        }
    }

    @Override
    public void onSurfaceChanged(GL10 gl, int width, int height) {
        mWidth = width;
        mHeight = height;

        mSurfaceTexture.setDefaultBufferSize(mWidth, mHeight);
        mediaPlayer.setWindowSize(mWidth, mHeight);

        GLES30.glBindTexture(GLES30.GL_TEXTURE_2D, mFrameTextureId);
        GLES30.glTexImage2D(GLES30.GL_TEXTURE_2D, 0, GLES30.GL_RGBA, mWidth, mHeight, 0, GLES30.GL_RGBA, GLES30.GL_UNSIGNED_BYTE, null);
        GLES30.glTexParameteri(GLES30.GL_TEXTURE_2D, GLES20.GL_TEXTURE_MIN_FILTER, GLES30.GL_LINEAR);
        GLES30.glTexParameteri(GLES30.GL_TEXTURE_2D, GLES20.GL_TEXTURE_MAG_FILTER, GLES30.GL_LINEAR);
        GLES30.glBindTexture(GLES30.GL_TEXTURE_2D, 0);
    }

    @Override
    public void onDrawFrame(GL10 gl) {
        synchronized (this) {
            if(mUpdateFrame) {
                mSurfaceTexture.updateTexImage();
                mSurfaceTexture.getTransformMatrix(mSTMatrix);
                mUpdateFrame = false;
            }

            GLES30.glViewport(0, 0, mWidth, mHeight);

            GLES30.glClearColor(0, 0, 0, 1);
            GLES30.glClear(GLES30.GL_DEPTH_BUFFER_BIT | GLES30.GL_COLOR_BUFFER_BIT);

            // Draw to OES Texture
            GLES30.glUseProgram(mOESProgramId);
//            GLES30.glBindFramebuffer(GLES30.GL_FRAMEBUFFER, mFBOId);
//            GLES30.glFramebufferTexture2D(GLES30.GL_FRAMEBUFFER, GLES30.GL_COLOR_ATTACHMENT0, GLES30.GL_TEXTURE_2D, mFrameTextureId, 0);
            GLES30.glUniformMatrix4fv(mSTMatrixHandle, 1, false, mSTMatrix, 0);
            GLES30.glEnableVertexAttribArray(aPositionOES);
            GLES30.glEnableVertexAttribArray(aTexCoordOES);
            GLES30.glBindBuffer(GLES30.GL_ARRAY_BUFFER, mVertexBufferId);
            GLES30.glVertexAttribPointer(aPositionOES, 3, GLES30.GL_FLOAT, false, 0, 0);
            GLES30.glBindBuffer(GLES30.GL_ARRAY_BUFFER, mFboVertexBufferId);
            GLES30.glVertexAttribPointer(aTexCoordOES, 2, GLES30.GL_FLOAT, false, 0, 0);
            GLES30.glActiveTexture(GLES30.GL_TEXTURE0);
            GLES30.glBindTexture(GLES11Ext.GL_TEXTURE_EXTERNAL_OES, mOESTextureId);
            GLES30.glUniform1i(sTextureOES, 0);
            GLES30.glDrawArrays(GLES30.GL_TRIANGLE_STRIP, 0, 4);
            GLES30.glBindTexture(GLES11Ext.GL_TEXTURE_EXTERNAL_OES, 0);
//            GLES30.glBindFramebuffer(GLES30.GL_FRAMEBUFFER, 0);
            GLES30.glDisableVertexAttribArray(aPositionOES);
            GLES30.glDisableVertexAttribArray(aTexCoordOES);

            // Draw to Texture 2D
            GLES30.glUseProgram(mFrameProgramId);
            GLES30.glBindBuffer(GLES30.GL_ARRAY_BUFFER, mVertexBufferId);
            GLES30.glVertexAttribPointer(aPosition2D, 3, GLES30.GL_FLOAT, false, 0, 0);
            GLES30.glBindBuffer(GLES30.GL_ARRAY_BUFFER, mDisplayBufferId);
            GLES30.glVertexAttribPointer(aTexCoord2D, 2, GLES30.GL_FLOAT, false, 0, 0);
            GLES30.glActiveTexture(GLES30.GL_TEXTURE0);
            GLES30.glBindTexture(GLES30.GL_TEXTURE_2D, mFrameTextureId);
            GLES30.glUniform1i(sTexture2D, 0);
            GLES30.glDrawArrays(GLES30.GL_TRIANGLE_STRIP, 0, 4);
            GLES30.glBindTexture(GLES30.GL_TEXTURE_2D, 0);
            GLES30.glDisableVertexAttribArray(aPosition2D);
            GLES30.glDisableVertexAttribArray(aTexCoord2D);
        }
    }

    private void initGLProgram() {
        // Shader programs
        mOESProgramId = GLESUtils.buildProgram(VSH_CODE, FSH_CODE);
        mFrameProgramId = GLESUtils.buildProgram(VSH_2D_CODE, FSH_2D_CODE);

        // Shader property location
        mSTMatrixHandle = GLES30.glGetUniformLocation(mOESProgramId, "uSTMatrix");
        aPositionOES = GLES30.glGetAttribLocation(mOESProgramId, "aPosition");
        aTexCoordOES = GLES30.glGetAttribLocation(mOESProgramId,"aTexCoord");
        sTextureOES = GLES30.glGetAttribLocation(mOESProgramId, "sTexture");
        aPosition2D = GLES30.glGetAttribLocation(mFrameProgramId, "aPosition");
        aTexCoord2D = GLES30.glGetAttribLocation(mFrameProgramId,"aTexCoord");
        sTexture2D = GLES30.glGetAttribLocation(mFrameProgramId, "sTexture");

        // Buffers
        int[] vbos = new int[3];
        GLES30.glGenBuffers(3, vbos, 0);
        vertexBuffer = ByteBuffer.allocateDirect(vertices.length * 4)
                .order(ByteOrder.nativeOrder())
                .asFloatBuffer()
                .put(vertices);
        vertexBuffer.position(0);
        mVertexBufferId = vbos[0];
        GLES30.glBindBuffer(GLES30.GL_ARRAY_BUFFER, mVertexBufferId);
        GLES30.glBufferData(GLES30.GL_ARRAY_BUFFER, vertices.length * 4, vertexBuffer, GLES30.GL_STATIC_DRAW);

        displayVertexBuffer = ByteBuffer.allocateDirect(displayTexCoords.length * 4)
                .order(ByteOrder.nativeOrder())
                .asFloatBuffer()
                .put(displayTexCoords);
        displayVertexBuffer.position(0);
        mDisplayBufferId = vbos[1];
        GLES30.glBindBuffer(GLES30.GL_ARRAY_BUFFER, mDisplayBufferId);
        GLES30.glBufferData(GLES30.GL_ARRAY_BUFFER, displayTexCoords.length * 4, displayVertexBuffer, GLES30.GL_STATIC_DRAW);

        fboVertexBuffer = ByteBuffer.allocateDirect(fboTexCoords.length * 4)
                .order(ByteOrder.nativeOrder())
                .asFloatBuffer()
                .put(fboTexCoords);
        fboVertexBuffer.position(0);
        mFboVertexBufferId = vbos[2];
        GLES30.glBindBuffer(GLES30.GL_ARRAY_BUFFER, mFboVertexBufferId);
        GLES30.glBufferData(GLES30.GL_ARRAY_BUFFER, fboTexCoords.length * 4, fboVertexBuffer, GLES30.GL_STATIC_DRAW);

        GLES30.glBindBuffer(GLES30.GL_ARRAY_BUFFER, 0);
    }

    private Surface createSurface() {
        int[] textures = new int[2];
        // GL_TEXTURE_EXTERNAL_OES
        GLES30.glGenTextures(1, textures, 0);
        mOESTextureId = textures[0];
        GLES30.glBindTexture(GLES11Ext.GL_TEXTURE_EXTERNAL_OES, mOESTextureId);
        GLES30.glTexParameterf(GLES11Ext.GL_TEXTURE_EXTERNAL_OES, GLES30.GL_TEXTURE_MIN_FILTER, GLES30.GL_NEAREST);
        GLES30.glTexParameterf(GLES11Ext.GL_TEXTURE_EXTERNAL_OES, GLES30.GL_TEXTURE_MAG_FILTER, GLES30.GL_LINEAR);
        GLES30.glTexParameteri(GLES11Ext.GL_TEXTURE_EXTERNAL_OES, GLES30.GL_TEXTURE_WRAP_S, GLES30.GL_CLAMP_TO_EDGE);
        GLES30.glTexParameteri(GLES11Ext.GL_TEXTURE_EXTERNAL_OES, GLES30.GL_TEXTURE_WRAP_T, GLES30.GL_CLAMP_TO_EDGE);

        // GL_TEXTURE_2D
        GLES30.glGenTextures(1, textures, 1);
        mFrameTextureId = textures[1];
        GLES30.glBindTexture(GLES30.GL_TEXTURE_2D, mFrameTextureId);

        // FBO
        int[] fbos = new int[1];
        GLES30.glGenFramebuffers(1, fbos, 0);
        mFBOId = fbos[0];
        int status = GLES30.glCheckFramebufferStatus(GLES30.GL_FRAMEBUFFER);
        if(status != GLES30.GL_FRAMEBUFFER_COMPLETE) {
            Log.d("FBO", "FBO not ready! " + status);
        }

        mSurfaceTexture = new SurfaceTexture(mOESTextureId);
        Surface surface = new Surface(mSurfaceTexture);
        return surface;
    }

    @Override
    public void onFrameAvailable(SurfaceTexture surfaceTexture) {
        mUpdateFrame = true;
    }
}
