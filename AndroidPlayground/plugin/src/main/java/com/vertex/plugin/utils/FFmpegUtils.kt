package com.vertex.plugin.utils

class FFmpegUtils {
    companion object {
        private val jniLibs = arrayOf (
            "crypto",
            "ssl",
            "swscale",
            "avutil",
            "avcodec",
            "avformat",
            "nativeplugin"
        )
        init {
            jniLibs.forEach {
                System.loadLibrary(it);
            }
        }

        fun testCheckProtocol() {
            checkProtocol()
        }

        @JvmStatic
        external fun checkProtocol()
    }

}