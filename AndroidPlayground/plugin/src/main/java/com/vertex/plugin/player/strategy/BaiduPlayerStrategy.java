package com.vertex.plugin.player.strategy;

import com.vertex.plugin.player.AbstractMediaPlayer;
import com.vertex.plugin.player.IMediaPlayer;

import org.videolan.libvlc.MediaPlayer;
import org.videolan.libvlc.interfaces.IMedia;
import java.util.regex.Pattern;


public class BaiduPlayerStrategy extends CommonPlayerStrategy{
    private long fetchCounter = 0;
    private boolean fetchComplete = false;
    private final String REGEX = ".*d\\.pcs\\.baidu\\.com.*";
    private final int DEFAULT_WIDTH = 3840;
    private final int DEFAULT_HEIGHT = 2160;
    private final long METADATA_FETCH_LIMIT = 20;

    public BaiduPlayerStrategy(AbstractMediaPlayer wrapper, MediaPlayer player) {
        super(wrapper, player);
    }

    @Override
    public boolean checkUrlValidation(String url) {
        if(Pattern.matches(REGEX, url))
            return true;
        return false;
    }

    @Override
    public int getWidth() {
        updateVideoWidthAndHeight();
        return width;
    }

    @Override
    public int getHeight() {
        updateVideoWidthAndHeight();
        return height;
    }

    @Override
    public void reset() {
        super.reset();
        fetchComplete = false;
        fetchCounter = 0;
    }

    @Override
    protected void updateVideoWidthAndHeight() {
        int w = DEFAULT_WIDTH;
        int h = DEFAULT_HEIGHT;
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
        if(w == 0 && h == 0) {
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
        }
    }
}
