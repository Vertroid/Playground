package com.vertex.plugin.player;

import android.app.Activity;
import android.content.Context;
import android.content.res.AssetManager;
import android.graphics.SurfaceTexture;
import android.graphics.SurfaceTexture.OnFrameAvailableListener;
import android.media.AudioManager;
import android.media.MediaPlayer;
import android.net.Uri;
import android.opengl.GLES20;
import android.util.Log;
import android.view.Surface;
import android.view.SurfaceHolder;

import com.vertex.plugin.player.IMediaPlayer;
import com.vertex.plugin.utils.GLESUtils;

import org.videolan.VLCMediaPlayer;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;

public class MediaPlayerProxy implements OnFrameAvailableListener, IMediaPlayer.OnPreparedListener, IMediaPlayer.OnCompletionListener, IMediaPlayer.OnErrorListener, IMediaPlayer.OnBufferingUpdateListener, IMediaPlayer.OnInfoListener {

    // Context
    // open resource / open MediaPlayer
    private Activity mUnityActivity = null;

    // used to play video
    public IMediaPlayer mediaPlayer = null;

    // Unity Texture2D id
    private int mIUnityTextureID = -1;

    // NDK InitExtTexture
    private int mISurfaceTextureID = -1;

    // Captures frames from an image stream as an OpenGL ES texture.
    private SurfaceTexture mSurfaceTexture = null;

    // A Surface created from a SurfaceTexture can be used as an output destination for the
    // android.hardware.camera2, MediaCodec, MediaPlayer, and Allocation APIs
    private Surface mSurface = null;

    // current seek percent
    private int mCurrentSeekPercent = 0;

    // current seek position =  mMediaPlayer.getCurrentPosition();
    private int mICurrentSeekPosition = 0;

    // mINativeMgrID = InitNDK(obj);
    public int mINativeMgrID;

    // Call_Load -> NDK_SetFileName
    private String mStrFileName;

    private int mInfoCode;

    // error code
    private int mIErrorCode;

    // error code extr
    private int mIErrorCodeExtra;

    // ??
    private boolean mBRockchip = true;

    // is have extra oob file
    private boolean mBSplitOBB = false;

    // Application.dataPath.Contains(".obb"
    // extra oob file name
    private String mStrOBBName;

    // is update frame
    public boolean mBUpdateFrame = false;

    private Map<String, String> mHeaders;

    private Uri mUri;

    private MEDIAPLAYER_STATE m_iCurrentState = MEDIAPLAYER_STATE.NOT_READY;

    /**
     * int[] textures = new int[1];
     * GLES20.glGenTextures(1, textures, 0);
     */
    public void Destroy() {
        Log.d("[MOON][EMT]", "Destroy");
        if (mISurfaceTextureID != -1) {
            int[] textures = new int[1];
            textures[0] = mISurfaceTextureID;
            GLES20.glDeleteTextures(1, textures, 0);
            mISurfaceTextureID = -1;
        }

        SetManagerID(mINativeMgrID);
        QuitApplication();
    }

    // release EasyMovieTexture
    public void UnLoad() {
        Log.d("[MOON][EMT]", "UnLoad");
        if (mediaPlayer != null) {
            if (m_iCurrentState != MEDIAPLAYER_STATE.NOT_READY) {
                try {
                    mediaPlayer.stop();
                    mediaPlayer.release();
                } catch (SecurityException e) {
                    e.printStackTrace();
                } catch (IllegalStateException e) {
                    e.printStackTrace();
                }
                mediaPlayer = null;

            } else {
                try {
                    mediaPlayer.release();
                } catch (SecurityException e) {
                    e.printStackTrace();
                } catch (IllegalStateException e) {
                    e.printStackTrace();
                }
                mediaPlayer = null;
            }

            if (mSurface != null) {
                mSurface.release();
                mSurface = null;
            }

            if (mSurfaceTexture != null) {
                mSurfaceTexture.release();
                mSurfaceTexture = null;
            }

            if (mISurfaceTextureID != -1) {
                int[] textures = new int[1];
                textures[0] = mISurfaceTextureID;
                GLES20.glDeleteTextures(1, textures, 0);
                mISurfaceTextureID = -1;
            }
        }
    }

    // load EasyMovieTexture
    public boolean Load() throws SecurityException, IllegalStateException, IOException {
        return Load(mUnityActivity.getApplicationContext());
    }

    public boolean Load(Context context) throws SecurityException, IllegalStateException, IOException {
        Log.d("[MOON][EMT]", "Load");
        UnLoad();
        m_iCurrentState = MEDIAPLAYER_STATE.NOT_READY;
        if(mediaPlayer == null) {
            mediaPlayer = createPlayer(context);
            mediaPlayer.setAudioStreamType(AudioManager.STREAM_MUSIC);

            Log.d("[MOON][EMT]", "Load Listener");
            mediaPlayer.setOnPreparedListener(this);
            mediaPlayer.setOnCompletionListener(this);
            mediaPlayer.setOnErrorListener(this);
            mediaPlayer.setOnInfoListener(this);
        }
        mBUpdateFrame = false;

        if (mISurfaceTextureID == -1) {
            //mISurfaceTextureID = InitExtTexture();
            mISurfaceTextureID = GLESUtils.createOESTextureID();
        }

        Log.d("[MOON][EMT]", "Load m_iCurrentState=" + m_iCurrentState + " mBUpdateFrame=" + mBUpdateFrame + " mUri=" + mUri + " mISurfaceTextureID=" + mISurfaceTextureID);
        if(mSurfaceTexture == null || mSurface == null) {
            mSurfaceTexture = new SurfaceTexture(mISurfaceTextureID);
            mSurfaceTexture.setOnFrameAvailableListener(this);
            mSurface = new Surface(mSurfaceTexture);
            mediaPlayer.setSurfaceTexture(mSurfaceTexture);
        }

        Log.d("[MOON][EMT]", "Load prepareAsync before");
        mUri = Uri.parse(mStrFileName);
        mediaPlayer.setDataSource(mUnityActivity.getApplicationContext(), mUri);
        mediaPlayer.prepareAsync();
        Log.d("[MOON][EMT]", "Load prepareAsync after");

        return true;
    }

    private IMediaPlayer createPlayer(Context context) {
        IMediaPlayer player = new VLCMediaPlayer(context);
        return player;
    }

    public List<TrackItem> getTrackInfo() {
        if (mediaPlayer == null) {
            return new ArrayList<>();
        }

        ITrackInfo[] trackInfos = mediaPlayer.getTrackInfo();

        List<TrackItem> trackItems = new ArrayList<>();
        for (int i = 0; i < trackInfos.length; i++) {
            trackItems.add(new TrackItem(i, trackInfos[i]));
        }

        return trackItems;
    }

    public List<TrackItem> getVideoTrackInfo() {
        if (mediaPlayer == null) {
            return new ArrayList<>();
        }

        ITrackInfo[] trackInfos = mediaPlayer.getTrackInfo();

        List<TrackItem> trackItems = new ArrayList<>();
        for (int i = 0; i < trackInfos.length; i++) {
            ITrackInfo trackInfo = trackInfos[i];
            if (trackInfo.getTrackType() == ITrackInfo.MEDIA_TRACK_TYPE_VIDEO) {
                trackItems.add(new TrackItem(i, trackInfos[i]));
            }
        }
        return trackItems;
    }

    public List<TrackItem> getAudioTrackInfo() {
        if (mediaPlayer == null) {
            return new ArrayList<>();
        }

        ITrackInfo[] trackInfos = mediaPlayer.getTrackInfo();

        List<TrackItem> trackItems = new ArrayList<>();
        for (int i = 0; i < trackInfos.length; i++) {
            ITrackInfo trackInfo = trackInfos[i];
            if (trackInfo.getTrackType() == ITrackInfo.MEDIA_TRACK_TYPE_AUDIO) {
                trackItems.add(new TrackItem(i, trackInfos[i]));
            }
        }
        return trackItems;
    }

    public List<TrackItem> getSubtitleTrackInfo() {
        if (mediaPlayer == null) {
            return new ArrayList<>();
        }

        ITrackInfo[] trackInfos = mediaPlayer.getTrackInfo();

        List<TrackItem> trackItems = new ArrayList<>();
        for (int i = 0; i < trackInfos.length; i++) {
            ITrackInfo trackInfo = trackInfos[i];
            if (trackInfo.getTrackType() == ITrackInfo.MEDIA_TRACK_TYPE_SUBTITLE) {
                trackItems.add(new TrackItem(i, trackInfos[i]));
            }
        }
        return trackItems;
    }

    public MediaInfo getMediaInfo() {
        //return MediaPlayerCompat.getMediaInfo(mediaPlayer);
    }

    // is can update  frame
    public void onFrameAvailable(SurfaceTexture surface) {
        mBUpdateFrame = true;
    }

    // update video texture
    // http://blog.csdn.net/jinzhuojun/article/details/44062175
    public void UpdateVideoTexture() {

        Log.d("[MOON][EMT]", "UpdateVideoTexture mMediaPlayer=" + mediaPlayer + " mINativeMgrID=" + mINativeMgrID + " mBUpdateFrame=" + mBUpdateFrame);

        if (!mBUpdateFrame) {
            return;
        }

        if (mediaPlayer != null) {
            SetManagerID(mINativeMgrID);

            boolean[] abValue = new boolean[1];
            GLES20.glGetBooleanv(GLES20.GL_DEPTH_TEST, abValue, 0);
            GLES20.glDisable(GLES20.GL_DEPTH_TEST);

            mSurfaceTexture.updateTexImage();

            float[] mMat = new float[16];

            mSurfaceTexture.getTransformMatrix(mMat);

            RenderScene(mMat, mISurfaceTextureID, mIUnityTextureID);

            if (abValue[0]) {
                GLES20.glEnable(GLES20.GL_DEPTH_TEST);
            } else {

            }
            abValue = null;
        }
    }

    public void SetRockchip(boolean bValue) {
        Log.d("[MOON][EMT]", "SetRockchip bValue=" + bValue);
        mBRockchip = bValue;
    }

    public void SetLooping(boolean bLoop) {
        Log.d("[MOON][EMT]", "SetLooping bLoop=" + bLoop);
        if (mediaPlayer != null)
            mediaPlayer.setLooping(bLoop);
    }

    public void SetVolume(float fVolume) {
        Log.d("[MOON][EMT]", "SetVolume fVolume=" + fVolume);
        if (mediaPlayer != null) {
            mediaPlayer.setVolume(fVolume, fVolume);
        }
    }

    public void SetSpeed(float speed) {
        Log.d("[MOON][EMT]", "SetSpeed speed=" + speed);
        if(mediaPlayer != null) {
            mediaPlayer.setSpeed(speed);
        }
    }

    public float GetSpeed() {
        Log.d("[MOON][EMT]", "GetSpeed");
        if(mediaPlayer != null) {
            return mediaPlayer.getSpeed();
        }
        return 1.0f;
    }

    public void SetSeekPosition(int iSeek) {
        Log.d("[MOON][EMT]", "SetSeekPosition iSeek=" + iSeek);
        if (mediaPlayer != null) {
            mediaPlayer.seekTo(iSeek);
        }
    }

    public int GetSeekPosition() {
        if (mediaPlayer != null) {
            try {
                mICurrentSeekPosition = (int) mediaPlayer.getCurrentPosition();
                Log.d("[MOON][EMT]", "SetSeekPosition mICurrentSeekPosition=" + mICurrentSeekPosition);
            } catch (SecurityException e) {
                e.printStackTrace();
            } catch (IllegalStateException e) {
                e.printStackTrace();
            }
        }
        return mICurrentSeekPosition;
    }

    public int GetCurrentPosition() {
        if (mediaPlayer != null) {
            int currentPosition = (int) mediaPlayer.getCurrentPosition();
            Log.d("[MOON][EMT]", "GetCurrentPosition currentPosition=" + currentPosition);
            return currentPosition;
        }
        return 0;
    }

    public int GetCurrentSeekPercent() {
        return mCurrentSeekPercent;
    }

    public void Play(int iSeek) {
        if (mediaPlayer != null) {
            Log.d("[MOON][EMT]", "Play iSeek=" + iSeek + " m_iCurrentState=" + m_iCurrentState);
            if (m_iCurrentState == MEDIAPLAYER_STATE.READY || m_iCurrentState == MEDIAPLAYER_STATE.PAUSED || m_iCurrentState == MEDIAPLAYER_STATE.END) {
                mediaPlayer.start();
                m_iCurrentState = MEDIAPLAYER_STATE.PLAYING;
            }
        }
    }

    public void Reset() {
        if (mediaPlayer != null) {
            Log.d("[MOON][EMT]", "Reset m_iCurrentState=" + m_iCurrentState);
            if (m_iCurrentState == MEDIAPLAYER_STATE.PLAYING) {
                mediaPlayer.reset();
            }
        }
        m_iCurrentState = MEDIAPLAYER_STATE.NOT_READY;
    }

    public void Stop() {
        if (mediaPlayer != null) {
            Log.d("[MOON][EMT]", "Stop m_iCurrentState=" + m_iCurrentState);
            if (m_iCurrentState == MEDIAPLAYER_STATE.PLAYING) {
                mediaPlayer.stop();
            }
        }
        m_iCurrentState = MEDIAPLAYER_STATE.NOT_READY;
    }

    public void RePlay() {
        if (mediaPlayer != null) {
            Log.d("[MOON][EMT]", "RePlay m_iCurrentState=" + m_iCurrentState);
            if (m_iCurrentState == MEDIAPLAYER_STATE.PAUSED) {
                mediaPlayer.start();
                m_iCurrentState = MEDIAPLAYER_STATE.PLAYING;
            }
        }
    }

    public void Pause() {
        if (mediaPlayer != null) {
            Log.d("[MOON][EMT]", "Pause m_iCurrentState=" + m_iCurrentState);
            if (m_iCurrentState == MEDIAPLAYER_STATE.PLAYING) {
                mediaPlayer.pause();
                m_iCurrentState = MEDIAPLAYER_STATE.PAUSED;
            }
        }
    }

    public int GetVideoWidth() {
        if (mediaPlayer != null) {
            int videoWidth = mediaPlayer.getVideoWidth();
            Log.d("[MOON][EMT]", "Pause videoWidth=" + videoWidth);
            return videoWidth;
        }
        return 0;
    }

    public int GetVideoHeight() {
        if (mediaPlayer != null) {
            int videoHeight = mediaPlayer.getVideoHeight();
            Log.d("[MOON][EMT]", "Pause videoHeight=" + videoHeight);
            return videoHeight;
        }
        return 0;
    }

    public boolean IsUpdateFrame() {
        return mBUpdateFrame;
    }

    public void SetUnityTexture(int iTextureID) {
        Log.d("[MOON][EMT]", "SetUnityTexture iTextureID=" + iTextureID);
        mIUnityTextureID = iTextureID;
        SetManagerID(mINativeMgrID);
        SetUnityTextureID(mIUnityTextureID);
    }

    public void SetUnityTextureID(Object texturePtr) {
        Log.d("[MOON][EMT]", "SetUnityTextureID texturePtr=" + texturePtr);
    }

    public void SetSplitOBB(boolean bValue, String strOBBName) {
        Log.d("[MOON][EMT]", "SetSplitOBB bValue=" + bValue + " strOBBName=" + strOBBName);
        mBSplitOBB = bValue;
        mStrOBBName = strOBBName;
    }

    public int GetDuration() {
        if (mediaPlayer != null) {
            int duration = (int) mediaPlayer.getDuration();
            Log.d("[MOON][EMT]", "SetSplitOBB duration=" + duration);
            return duration;
        }
        return -1;
    }

    public int InitNative(MediaPlayerProxy obj) {
        IjkMediaPlayer.loadLibrariesOnce(null);
        IjkMediaPlayer.native_profileBegin("libijkplayer.so");
        mINativeMgrID = InitNDK(obj);
        mObjCtrl.add(this);
        Log.d("[MOON][EMT]", "InitNative obj=" + obj + " mINativeMgrID=" + mINativeMgrID);
        return mINativeMgrID;
    }

    public void SetUnityActivity(Activity unityActivity) {
        SetManagerID(mINativeMgrID);
        mUnityActivity = unityActivity;
        SetAssetManager(mUnityActivity.getAssets());
        Log.d("[MOON][EMT]", "SetUnityActivity unityActivity=" + unityActivity + " mINativeMgrID=" + mINativeMgrID);
    }

    public void NDK_SetFileName(String strFileName) {
        mStrFileName = strFileName;
        Log.d("[MOON][EMT]", "NDK_SetFileName mStrFileName=" + mStrFileName);
    }

    public void InitJniManager() {
        SetManagerID(mINativeMgrID);
        InitApplication();
        Log.d("[MOON][EMT]", "NDK_SetFileName mINativeMgrID=" + mINativeMgrID);
    }

    public int GetStatus() {
        int status = m_iCurrentState.GetValue();
        return status;
    }

    public void SetNotReady() {
        m_iCurrentState = MEDIAPLAYER_STATE.NOT_READY;
        Log.d("[MOON][EMT]", "SetNotReady m_iCurrentState=" + m_iCurrentState);
    }

    public void SetWindowSize() {
        Log.d("[MOON][EMT]", "SetWindowSize mINativeMgrID=" + mINativeMgrID);
        SetManagerID(mINativeMgrID);
        int width = GetVideoWidth();
        int height = GetVideoHeight();
        //SetWindowSize(width, height, mIUnityTextureID, mBRockchip);
        mediaPlayer.setWindowSize(width, height);
    }

    public int GetError() {
        Log.d("[MOON][EMT]", "GetError mIErrorCode=" + mIErrorCode);
        return mIErrorCode;
    }

    public int GetErrorExtra() {
        Log.d("[MOON][EMT]", "GetErrorExtra mIErrorCodeExtra=" + mIErrorCodeExtra);
        return mIErrorCodeExtra;
    }

    public int GetInfo() {
        Log.d("[MOON][EMT]", "GetInfo mInfoCode=" + mInfoCode);
        return mInfoCode;
    }

    @Override
    public boolean onError(IMediaPlayer arg0, int arg1, int arg2) {
        Log.d("[MOON][EMT]", "onError arg0=" + arg0 + " arg1=" + " arg2=" + arg2);
        if (arg0 == mediaPlayer) {
            String strError;
            switch (arg1) {
                case MediaPlayer.MEDIA_ERROR_NOT_VALID_FOR_PROGRESSIVE_PLAYBACK:
                    strError = "MEDIA_ERROR_NOT_VALID_FOR_PROGRESSIVE_PLAYBACK";
                    break;
                case MediaPlayer.MEDIA_ERROR_SERVER_DIED:
                    strError = "MEDIA_ERROR_SERVER_DIED";
                    break;
                case MediaPlayer.MEDIA_ERROR_UNKNOWN:
                    strError = "MEDIA_ERROR_UNKNOWN";
                    break;
                default:
                    strError = "Unknown error " + arg1;
            }
            mIErrorCode = arg1;
            mIErrorCodeExtra = arg2;
            m_iCurrentState = MEDIAPLAYER_STATE.ERROR;
            return true;
        }
        return false;
    }

    @Override
    public void onCompletion(IMediaPlayer iMediaPlayer) {
        if (iMediaPlayer == mediaPlayer) {
            m_iCurrentState = MEDIAPLAYER_STATE.END;
        }
        Log.d("[MOON][EMT]", "onCompletion iMediaPlayer=" + iMediaPlayer);
    }

    @Override
    public void onBufferingUpdate(IMediaPlayer arg0, int arg1) {
        if (arg0 == mediaPlayer)
            mCurrentSeekPercent = arg1;

        Log.d("[MOON][EMT]", "onBufferingUpdate IMediaPlayer=" + arg0 + " arg1=" + arg1);
    }

    @Override
    public void onPrepared(IMediaPlayer iMediaPlayer) {
        Log.d("[MOON][EMT]", "onPrepared iMediaPlayer=" + iMediaPlayer);

        if (iMediaPlayer == mediaPlayer) {
            m_iCurrentState = MEDIAPLAYER_STATE.READY;
            SetManagerID(mINativeMgrID);
            mCurrentSeekPercent = 0;
            mediaPlayer.setOnBufferingUpdateListener(this);
        }
    }

    @Override
    public boolean onInfo(IMediaPlayer iMediaPlayer, int i, int i1) {
        mInfoCode = i;
        Log.d("[MOON][EMT]", "onInfo iMediaPlayer=" + iMediaPlayer + " i=" + i + " i1=" + i1);
        return true;
    }

    public enum MEDIAPLAYER_STATE {
        NOT_READY(0),
        READY(1),
        END(2),
        PLAYING(3),
        PAUSED(4),
        STOPPED(5),
        ERROR(6);
        private int iValue;

        MEDIAPLAYER_STATE(int i) {
            iValue = i;
        }

        public int GetValue() {
            return iValue;
        }
    }
}
