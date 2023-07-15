package com.vertex.plugin.player;

import java.util.Locale;

public class TrackItem {
    public final int index;
    public final ITrackInfo trackInfo;
    public final String infoInline;

    public TrackItem(int index, ITrackInfo trackInfo) {
        this.index = index;
        this.trackInfo = trackInfo;
        infoInline = String.format(Locale.US, "# %d: %s", this.index, this.trackInfo.getInfoInline());
    }

    public String getInfoInline() {
        return infoInline;
    }
    public int getIndex() {
        return index;
    }
}
