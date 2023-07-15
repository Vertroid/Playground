package org.videolan;

import android.content.ContentResolver;
import android.content.Context;
import android.content.res.AssetFileDescriptor;
import android.graphics.SurfaceTexture;
import android.media.RingtoneManager;
import android.net.Uri;
import android.os.Build;
import android.os.ParcelFileDescriptor;
import android.provider.Settings;
import android.text.TextUtils;
import android.util.Log;
import android.view.Surface;
import android.view.SurfaceHolder;

import com.vertex.plugin.player.AbstractMediaPlayer;
import com.vertex.plugin.player.ITrackInfo;
import com.vertex.plugin.player.MediaInfo;

import org.videolan.libvlc.LibVLC;
import org.videolan.libvlc.Media;
import org.videolan.libvlc.MediaPlayer;
import org.videolan.libvlc.interfaces.IMedia;

import java.io.FileDescriptor;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Map;

public class VLCMediaPlayer extends AbstractMediaPlayer implements MediaPlayer.EventListener {
    private final String TAG = "VLC";

    private LibVLC mLibVLC = null;
    private MediaPlayer player = null;
    private SurfaceTexture mSurfaceTexture = null;
    private Surface mSurface = null;
    private SurfaceHolder mSurfaceHolder = null;
    private Media mCurrentMedia = null;
    private final int TRACK_DISABLE = -1;

    public VLCMediaPlayer(Context context) {
        final ArrayList<String> args = new ArrayList<>();
        args.add("-vvv");

        mLibVLC = new LibVLC(context, args);
        player = new MediaPlayer(mLibVLC);
        player.setEventListener(this);
    }

    public void init() {

    }

    @Override
    public void setDisplay(SurfaceHolder sh) {

    }

    @Override
    public void setDataSource(Context context, Uri uri) throws IOException, IllegalArgumentException, SecurityException, IllegalStateException {
        setDataSource(context, uri, null);
    }

    @Override
    public void setDataSource(Context context, Uri uri, Map<String, String> headers) throws IOException, IllegalArgumentException, SecurityException, IllegalStateException {
        final String scheme = uri.getScheme();
        if (ContentResolver.SCHEME_FILE.equals(scheme)) {
            // File
            setDataSource(uri.getPath());
        } else if (ContentResolver.SCHEME_CONTENT.equals(scheme)
                && Settings.AUTHORITY.equals(uri.getAuthority())) {
            // ContentResolver
            // Redirect ringtones to go directly to underlying provider
            uri = RingtoneManager.getActualDefaultRingtoneUri(context,
                    RingtoneManager.getDefaultType(uri));
            if (uri == null) {
                throw new FileNotFoundException("Failed to resolve default ringtone");
            }

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
                if (fd.getDeclaredLength() < 0) {
                    setDataSource(fd.getFileDescriptor());
                } else {
                    setDataSource(fd.getFileDescriptor(), fd.getStartOffset(), fd.getDeclaredLength());
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
            mCurrentMedia = new Media(mLibVLC, uri);
            player.setMedia(mCurrentMedia);
        }

    }

    private void setDataSource(FileDescriptor fd, long offset, long length)
            throws IOException, IllegalArgumentException, IllegalStateException {
        // FIXME: handle offset, length
        setDataSource(fd);
    }

    @Override
    public void setDataSource(FileDescriptor fd) throws IOException, IllegalArgumentException, IllegalStateException {
        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.HONEYCOMB_MR1) {
            try {
                mCurrentMedia = new Media(mLibVLC, fd);
                player.setMedia(mCurrentMedia);
            } catch (Exception e) {
                e.printStackTrace();
            }
        } else {
            ParcelFileDescriptor pfd = ParcelFileDescriptor.dup(fd);
            try {
                mCurrentMedia = new Media(mLibVLC, pfd.getFileDescriptor());
                player.setMedia(mCurrentMedia);
            } finally {
                pfd.close();
            }
        }
    }

    public void setDataSource(String path, Map<String, String> headers)
            throws IOException, IllegalArgumentException, SecurityException, IllegalStateException
    {
        if (headers != null && !headers.isEmpty()) {
            StringBuilder sb = new StringBuilder();
            for(Map.Entry<String, String> entry: headers.entrySet()) {
                sb.append(entry.getKey());
                sb.append(":");
                String value = entry.getValue();
                if (!TextUtils.isEmpty(value))
                    sb.append(entry.getValue());
                sb.append("\r\n");

                //setOption(OPT_CATEGORY_FORMAT, "headers", sb.toString());
                //setOption(IjkMediaPlayer.OPT_CATEGORY_FORMAT, "protocol_whitelist", "async,cache,crypto,file,http,https,ijkhttphook,ijkinject,ijklivehook,ijklongurl,ijksegment,ijktcphook,pipe,rtp,tcp,tls,udp,ijkurlhook,data");
            }
        }
        setDataSource(path);
    }

    @Override
    public void setDataSource(String path) throws IOException, IllegalArgumentException, SecurityException, IllegalStateException {
        final Media media = new Media(mLibVLC, path);
        player.setMedia(media);
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
        player.play();
    }

    @Override
    public void stop() throws IllegalStateException {
        player.stop();
    }

    @Override
    public void pause() throws IllegalStateException {
        player.pause();
    }

    @Override
    public void setScreenOnWhilePlaying(boolean screenOn) {

    }

    @Override
    public int getVideoWidth() {
        int width = 0;
        if(player.hasMedia()) {
            IMedia.VideoTrack t = player.getCurrentVideoTrack();
            if(t != null)
                width = t.width;
        }
        return width;
    }

    @Override
    public int getVideoHeight() {
        int height = 0;
        if(player.hasMedia()) {
            IMedia.VideoTrack t = player.getCurrentVideoTrack();
            if(t != null)
                height = t.height;
        }
        return height;
    }

    @Override
    public boolean isPlaying() {
        return player.isPlaying();
    }

    @Override
    public void seekTo(long msec) throws IllegalStateException {
        if(player.isSeekable()) {
            player.setTime(msec, true);
        } else {
            Log.w(TAG, "Current media is not seekable!");
        }
    }

    @Override
    public long getCurrentPosition() {
        if(player.hasMedia()) {
            return player.getTime();
        }
        return 0;
    }

    @Override
    public long getDuration() {
        if(player.hasMedia()) {
            return player.getLength();
        }
        return 0;
    }

    @Override
    public void release() {
        player.release();
        mLibVLC.release();
    }

    @Override
    public void reset() {

    }

    @Override
    public void setVolume(float leftVolume, float rightVolume) {
        //player.setVolume()
    }

    @Override
    public void setSpeed(float speed) {
        player.setRate(speed);
    }

    @Override
    public float getSpeed() {
        return player.getRate();
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
        // do nothing
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
        return 0;
    }

    @Override
    public int getVideoSarDen() {
        return 0;
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
        ArrayList<ITrackInfo> list = new ArrayList<>();
        if(player.hasMedia()) {
            // Video
            MediaPlayer.TrackDescription[] vts = player.getVideoTracks();
            if(vts != null) {
                for(int i = 0; i < vts.length; i++) {
                    VLCTrackInfo item = new VLCTrackInfo();
                    if(vts[i].id == -1)
                        continue;
                    item.setTrackType(ITrackInfo.MEDIA_TRACK_TYPE_VIDEO);
                    item.setId(vts[i].id);
                    item.setTitle(vts[i].name);
                    list.add(item);
                }
            }

            // Audio
            MediaPlayer.TrackDescription[] ats = player.getAudioTracks();
            if(ats != null) {
                for(int i = 0; i < ats.length; i++) {
                    VLCTrackInfo item = new VLCTrackInfo();
                    if(ats[i].id == -1)
                        continue;
                    item.setTrackType(ITrackInfo.MEDIA_TRACK_TYPE_AUDIO);
                    item.setId(ats[i].id);
                    item.setTitle(ats[i].name);
                    list.add(item);
                }
            }

            // Subtitle
            MediaPlayer.TrackDescription[] sts = player.getSpuTracks();
            if(sts != null) {
                for(int i = 0;i < sts.length; i++) {
                    VLCTrackInfo item = new VLCTrackInfo();
                    if(sts[i].id == -1)
                        continue;
                    item.setTrackType(ITrackInfo.MEDIA_TRACK_TYPE_SUBTITLE);
                    item.setId(sts[i].id);
                    item.setTitle(sts[i].name);
                    list.add(item);
                }
            }
        }
        return list.toArray(new ITrackInfo[list.size()]);
    }

    @Override
    public void setSurface(Surface surface) {
        // do nothing
    }

    @Override
    public void setSurfaceTexture(SurfaceTexture surfaceTexture) {
        mSurfaceTexture = surfaceTexture;
        player.getVLCVout().setVideoSurface(surfaceTexture);
        player.getVLCVout().attachViews();
        player.setVideoTrackEnabled(true);
    }

    @Override
    public void setWindowSize(int width, int height) {
        mSurfaceTexture.setDefaultBufferSize(width, height);
        player.getVLCVout().setWindowSize(width, height);
    }

    @Override
    public void setAudioTrack(int i) {
        player.setAudioTrack(i);
    }

    @Override
    public void setSubtitleTrack(int i) {
        player.setSpuTrack(i);
    }

    @Override
    public void onEvent(MediaPlayer.Event event) {
        switch(event.type) {
            case MediaPlayer.Event.MediaChanged:
                Log.d(TAG, "MediaChanged");
                notifyOnPrepared();
                getTrackInfo();
                break;
            case MediaPlayer.Event.Opening:
                Log.d(TAG, "Opening");
                break;
            case MediaPlayer.Event.Buffering:
                Log.d(TAG, "Buffering");
                break;
            case MediaPlayer.Event.Playing:
                Log.d(TAG, "Playing");
                break;
            case MediaPlayer.Event.Stopped:
                Log.d(TAG, "Stopped");
                break;
            case MediaPlayer.Event.EndReached:
                Log.d(TAG, "EndReached");
                notifyOnCompletion();
                break;
            case MediaPlayer.Event.EncounteredError:
                Log.d(TAG, "EncounteredError");
                break;
        }
    }
}
