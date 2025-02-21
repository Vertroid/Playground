package com.vertex.plugin.player.strategy;

public interface IPlayerStrategy {
    boolean checkUrlValidation(String url);
    int getWidth();
    int getHeight();
    void reset();
}
