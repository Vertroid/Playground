package com.vertex.plugin.jni;

public class NativePlugin {
    static {
        System.loadLibrary("native-plugin");
    }

    public static native String hello();
}
