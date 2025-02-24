package com.vertex.plugin.jni;

public class NativePlugin {
    static {
        System.loadLibrary("nativeplugin");
    }

    public static native String hello();
}
