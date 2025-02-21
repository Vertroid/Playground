package com.vertex.plugin.player.mpv;

import android.content.ContentResolver;
import android.content.Context;
import android.content.res.AssetFileDescriptor;
import android.graphics.SurfaceTexture;
import android.net.Uri;
import android.util.Log;
import android.view.Surface;
import android.view.SurfaceHolder;

import androidx.annotation.NonNull;


import java.io.FileDescriptor;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;

import is.xyz.mpv.MPVLib;
import com.vertex.plugin.player.AbstractMediaPlayer;
import com.vertex.plugin.player.ITrackInfo;
import com.vertex.plugin.player.MediaInfo;
import com.vertex.plugin.player.strategy.IPlayerStrategy;

public class MPVMediaPlayer extends AbstractMediaPlayer implements MPVLib.EventObserver {
    private final String TAG = "MPV";
    private SurfaceTexture mSurfaceTexture;
    private Surface mSurface;
    private long timePos = 0;
    private long duration = 0;
    private long width = 0;
    private long height = 0;
    private IPlayerStrategy currentStrategy = null;
    private IPlayerStrategy baseStrategy;
    private final List<IPlayerStrategy> strategies = new ArrayList<>();

    public MPVMediaPlayer(Context context) {
        MPVLib.create(context);
        init();
    }

    public void init() {
        MPVLib.setOptionString("priority", "high");
        MPVLib.setOptionString("osc", "no");

        MPVLib.setOptionString("profile", "fast");

        MPVLib.setOptionString("force-window", "no");
        MPVLib.setOptionString("vo", "gpu");
        MPVLib.setOptionString("gpu-context", "android");
        MPVLib.setOptionString("opengl-es", "yes");
        MPVLib.setOptionString("hwdec", "auto");
        MPVLib.setOptionString("hwdec-codecs", "h264,hevc,mpeg4,mpeg2video,vp8,vp9,av1");
        MPVLib.setOptionString("ao", "audiotrack,opensles");

        // vo
//        MPVLib.setOptionString("video-latency-hacks", "yes");
//        MPVLib.setOptionString("vf", "gradfun=radius=12");
        MPVLib.setOptionString("video-output-level", "full");
        MPVLib.setOptionString("fbo-format", "rgba8");

        // vd
        MPVLib.setOptionString("vd-lavc-dr", "yes");
        MPVLib.setOptionString("vd-lavc-fast", "yes");
        MPVLib.setOptionString("vd-lavc-skiploopfilter", "nonkey");

        // demuxer
        MPVLib.setOptionString("demuxer-max-bytes", "" + 64 * 1024 * 1024);
        MPVLib.setOptionString("demuxer-max-back-bytes", "" + 64 * 1024 * 1024);

        MPVLib.setOptionString("override-display-fps", String.valueOf(60));

        MPVLib.init();

        // Observe
        observeProperties();
    }

    private void observeProperties() {
        MPVLib.observeProperty("pause", MPVLib.mpvFormat.MPV_FORMAT_FLAG);
        MPVLib.observeProperty("time-pos", MPVLib.mpvFormat.MPV_FORMAT_INT64);
        MPVLib.observeProperty("duration", MPVLib.mpvFormat.MPV_FORMAT_INT64);
        MPVLib.observeProperty("hwdec-current", MPVLib.mpvFormat.MPV_FORMAT_NONE);
        MPVLib.observeProperty("width", MPVLib.mpvFormat.MPV_FORMAT_INT64);
        MPVLib.observeProperty("height", MPVLib.mpvFormat.MPV_FORMAT_INT64);

        MPVLib.addObserver(this);
    }

    @Override
    public void setDisplay(SurfaceHolder sh) {

    }

    @Override
    public void setDataSource(Context context, Uri uri) throws IOException, IllegalArgumentException, SecurityException, IllegalStateException {
        final String scheme = uri.getScheme();
        if (ContentResolver.SCHEME_FILE.equals(scheme)) {
            // File
            currentStrategy = baseStrategy;
            setDataSource(uri.getPath());
        } else if (ContentResolver.SCHEME_CONTENT.equals(scheme)) {
            AssetFileDescriptor fd = null;
            try {
                ContentResolver resolver = context.getContentResolver();
                fd = resolver.openAssetFileDescriptor(uri, "r");
                if (fd == null) {
                    return;
                }
                // Note: using getDeclaredLength so that our behavior is the same
                // as previous versions when the content provider is returning
                // a full file.
                currentStrategy = baseStrategy;
                if (fd.getDeclaredLength() < 0) {
                    setDataSource(fd.getFileDescriptor());
                } else {
                    //setDataSource(fd.getFileDescriptor(), fd.getStartOffset(), fd.getDeclaredLength());
                }
            } catch (SecurityException ignored) {
            } catch (IOException ignored) {
            } finally {
                if (fd != null) {
                    fd.close();
                }
            }
        } else {
            // URL
            Log.d(TAG, "Couldn't open file on client side, trying server side");
            // setDataSource(uri.toString(), headers);
//            for (int i = 0; i < strategies.size(); i++) {
//                if(strategies.get(i).checkUrlValidation(uri.toString()))
//                    currentStrategy = strategies.get(i);
//            }
//            if(currentStrategy == null)
//                currentStrategy = baseStrategy;

//            if(uri.toString().contains("smb://")) {
//                Thread gfgThread = new Thread(new Runnable() {
//                    @Override
//                    public void run() {
//                        try {
//                            String username = "Michael";
//                            String password = "qwe123@ewq";
//                            SingletonContext baseContext = SingletonContext.getInstance();
//                            Credentials credentials = new NtlmPasswordAuthenticator("smb://192.168.10.247/", username, password);
//                            CIFSContext testCtx = baseContext.withCredentials(credentials);
//                            SmbFile file = new SmbFile(uri.toString(), testCtx);
//
//                            mCurrentMedia = new Media(mLibVLC, uri);
//                            mCurrentMedia.setEventListener(mediaEventListener);
//                            player.setMedia(mCurrentMedia);
//                        } catch (Exception e) {
//                            e.printStackTrace();
//                        }
//                    }
//                });
//
//                gfgThread.start();
//
//
//            } else {
//            }

            setDataSource(uri.toString());
        }
    }

    @Override
    public void setDataSource(Context context, Uri uri, Map<String, String> headers) throws IOException, IllegalArgumentException, SecurityException, IllegalStateException {
        setDataSource(uri.getPath());
    }

    @Override
    public void setDataSource(FileDescriptor fd) throws IOException, IllegalArgumentException, IllegalStateException {

    }

    @Override
    public void setDataSource(String path) throws IOException, IllegalArgumentException, SecurityException, IllegalStateException {
        MPVLib.command(new String[] { "loadfile", path });
    }

    @Override
    public String getDataSource() {
        return null;
    }

    @Override
    public void prepareAsync() throws IllegalStateException {

    }

    @Override
    public void start() throws IllegalStateException {
        if(MPVLib.getPropertyBoolean("pause"))
            MPVLib.command(new String[] {"cycle", "pause"});
    }

    @Override
    public void stop() throws IllegalStateException {
        MPVLib.command(new String[] {"stop"});
    }

    @Override
    public void pause() throws IllegalStateException {
        if(!MPVLib.getPropertyBoolean("pause"))
            MPVLib.command(new String[] {"cycle", "pause"});
    }

    @Override
    public void setScreenOnWhilePlaying(boolean screenOn) {

    }

    @Override
    public int getVideoWidth() {
        return (int)width;
    }

    @Override
    public int getVideoHeight() {
        return (int)height;
    }

    @Override
    public boolean isPlaying() {
        boolean pause = MPVLib.getPropertyBoolean("pause");
        return !pause;
    }

    @Override
    public void seekTo(long msec) throws IllegalStateException {
        long sec = msec / 1000;
        MPVLib.command(new String[] { "seek", "" + sec, "absolute" });
    }

    @Override
    public long getCurrentPosition() {
        return timePos * 1000;
    }

    @Override
    public long getDuration() {
        return duration * 1000;
    }

    @Override
    public void release() {
        MPVLib.setPropertyString("vo", "null");
        MPVLib.setOptionString("force-window", "no");

        MPVLib.removeObserver(this);
        MPVLib.detachSurface();
        MPVLib.destroy();
    }

    @Override
    public void reset() {

    }

    @Override
    public void setSpeed(float speed) {
        MPVLib.setPropertyDouble("speed", (double)speed);
    }

    @Override
    public float getSpeed() {
        float speed = MPVLib.getPropertyDouble("speed").floatValue();
        return speed;
    }

    @Override
    public void setVolume(float leftVolume, float rightVolume) {

    }

    @Override
    public int getAudioSessionId() {
        return 0;
    }

    @Override
    public MediaInfo getMediaInfo() {
        return null;
    }

    @Override
    public void setLogEnabled(boolean enable) {

    }

    @Override
    public boolean isPlayable() {
        return true;
    }

    @Override
    public void setAudioStreamType(int streamtype) {

    }

    @Override
    public void setKeepInBackground(boolean keepInBackground) {

    }

    @Override
    public int getVideoSarNum() {
        return 1;
    }

    @Override
    public int getVideoSarDen() {
        return 1;
    }

    @Override
    public void setWakeMode(Context context, int mode) {

    }

    @Override
    public void setLooping(boolean looping) {

    }

    @Override
    public boolean isLooping() {
        return false;
    }

    @Override
    public ITrackInfo[] getTrackInfo() {
        return new ITrackInfo[0];
    }

    @Override
    public void setSurface(Surface surface) {
        // Surface
    }

    @Override
    public void setSurfaceTexture(SurfaceTexture surfaceTexture) {
        mSurfaceTexture = surfaceTexture;
        mSurface = new Surface(mSurfaceTexture);
        MPVLib.attachSurface(mSurface);
        MPVLib.setOptionString("force-window", "yes");
    }

    @Override
    public void setWindowSize(int width, int height) {
        mSurfaceTexture.setDefaultBufferSize(width, height);
        MPVLib.setPropertyString("android-surface-size", String.format("%sx%s", width, height));
    }

    @Override
    public void setAudioTrack(int i) {

    }

    @Override
    public void setSubtitleTrack(int i) {

    }

    // Events
    @Override
    public void eventProperty(@NonNull String property) {

    }

    @Override
    public void eventProperty(@NonNull String property, long value) {
        switch(property) {
            case "time-pos":
                timePos = value;
                break;
            case "duration":
                duration = value;
                break;
            case "width":
                width = value;
                break;
            case "height":
                height = value;
                break;
        }
    }

    @Override
    public void eventProperty(@NonNull String property, boolean value) {

    }

    @Override
    public void eventProperty(@NonNull String property, @NonNull String value) {

    }

    @Override
    public void event(int eventId) {
        switch(eventId) {
            case MPVLib.mpvEventId.MPV_EVENT_START_FILE:
                notifyOnPrepared();
                getVideoWidth();
                getVideoHeight();
                getDuration();
                break;
            case MPVLib.mpvEventId.MPV_EVENT_END_FILE:
                notifyOnCompletion();
                break;
        }
    }
}
