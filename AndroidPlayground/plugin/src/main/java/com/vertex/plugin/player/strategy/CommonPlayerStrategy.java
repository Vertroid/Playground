package com.vertex.plugin.player.strategy;

import com.vertex.plugin.player.AbstractMediaPlayer;
import com.vertex.plugin.player.IMediaPlayer;

import org.videolan.libvlc.MediaPlayer;
import org.videolan.libvlc.interfaces.IMedia;


public class CommonPlayerStrategy implements IPlayerStrategy{
    protected MediaPlayer player;
    protected AbstractMediaPlayer wrapper;
    protected int width = 0;
    protected int height = 0;

    private long fetchCounter = 0;
    private boolean fetchComplete = false;
    private final int DEFAULT_WIDTH = 3840;
    private final int DEFAULT_HEIGHT = 2160;
    private final long METADATA_FETCH_LIMIT = 20;
    public CommonPlayerStrategy(AbstractMediaPlayer wrapper, MediaPlayer player) {
        this.wrapper = wrapper;
        this.player = player;
    }

    public boolean checkUrlValidation(String url) {
        return true;
    }

    public int getWidth() {
        updateVideoWidthAndHeight();
        return width;
    }

    public int getHeight() {
        updateVideoWidthAndHeight();
        return height;
    }

    public void reset() {
        width = 0;
        height = 0;
        fetchComplete = false;
        fetchCounter = 0;
    }

    protected void updateVideoWidthAndHeight() {
        int w = 0;
        int h = 0;
        IMedia.VideoTrack t = player.getCurrentVideoTrack();
        if(t != null) {
            w = t.width;
            h = t.height;
        } else {
            int count = player.getMedia().getTrackCount();
            for(int i = 0; i < count; i++) {
                IMedia.Track item = player.getMedia().getTrack(i);
                if(item.type == IMedia.Track.Type.Video) {
                    try {
                        IMedia.VideoTrack vt = (IMedia.VideoTrack)item;
                        w = vt.width;
                        h = vt.height;
                        break;
                    } catch (Exception e) {
                        continue;
                    }
                }
            }
        }
        fetchCounter++;
        this.width = w;
        this.height = h;
        /*if(w == 0 && h == 0) {
            if(fetchCounter >= METADATA_FETCH_LIMIT && !fetchComplete){
                fetchComplete = true;
                this.width = DEFAULT_WIDTH;
                this.height = DEFAULT_HEIGHT;
                wrapper.notifyOnError(IMediaPlayer.MEDIA_ERROR_FETCH_METADATA_FAILED, 0);
            }
        } else {
            fetchComplete = true;
            this.width = w;
            this.height = h;
        }*/
    }
}
