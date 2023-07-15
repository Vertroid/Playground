package com.vertex.plugin.player;

public class MediaInfo {
    private String audioDecoder;
    private String videoDecoder;
    private boolean isMediaCodec;

    public String getAudioDecoder() {
        return audioDecoder;
    }
    public void setAudioDecoder(String audioDecoder) {
        this.audioDecoder = audioDecoder;
    }
    public String getVideoDecoder() {
        return videoDecoder;
    }
    public void setVideoDecoder(String videoDecoder) {
        this.videoDecoder = videoDecoder;
    }
    public boolean isMediaCodec() {
        return isMediaCodec;
    }
    public void setMediaCodec(boolean mediaCodec) {
        isMediaCodec = mediaCodec;
    }
}
