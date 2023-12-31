package com.vertex.plugin.player;

public interface IMediaFormat {
    // Common keys
    String KEY_MIME = "mime";

    // Video Keys
    String KEY_WIDTH = "width";
    String KEY_HEIGHT = "height";

    String getString(String name);

    int getInteger(String name);
}
