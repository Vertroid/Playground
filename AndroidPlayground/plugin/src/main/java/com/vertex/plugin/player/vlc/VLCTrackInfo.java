package com.vertex.plugin.player.vlc;

import com.vertex.plugin.player.IMediaFormat;
import com.vertex.plugin.player.ITrackInfo;

public class VLCTrackInfo implements ITrackInfo {
    private int mTrackType = MEDIA_TRACK_TYPE_UNKNOWN;
    private int mId = -1;
    private String mTitle = "";
    private String mLanguage = "";
    @Override
    public IMediaFormat getFormat() {
        return null;
    }

    @Override
    public String getLanguage() {
        if(mLanguage.isEmpty())
            return "und";
        return mLanguage;
    }

    @Override
    public int getTrackType() {
        return mTrackType;
    }

    @Override
    public String getInfoInline() {
        StringBuilder out = new StringBuilder(128);
        switch(mTrackType) {
            case MEDIA_TRACK_TYPE_AUDIO:
                out.append("AUDIO ");
                break;
            case MEDIA_TRACK_TYPE_VIDEO:
                out.append("VIDEO ");
                break;
            case MEDIA_TRACK_TYPE_SUBTITLE:
                out.append("SUBTITLE ");
                break;
        }
        out.append(mId);
        out.append(" ");
        out.append(mTitle);
        return out.toString();
    }

    public void setTrackType(int trackType) {
        mTrackType = trackType;
    }

    public void setId(int id) {
        mId = id;
    }

    public void setTitle(String title) {
        mTitle = title;
    }

    public void setLanguage(String lang) {
        mLanguage = lang;
    }
}
