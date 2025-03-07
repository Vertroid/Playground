//
// Created by cxy92 on 2025/2/19.
//

extern "C" {
#include <libavformat/avformat.h>
}
#include <dlfcn.h>
#include <jni.h>
#include "LogUtils.h"
#include "FFmpegUtils.h"

extern "C"
JNIEXPORT void JNICALL
Java_com_vertex_plugin_utils_FFmpegUtils_checkProtocol(JNIEnv *env, jclass clazz) {
    LOGE("Calling checkProtocol");

    LOGE("libavcodec version: %u\n", avcodec_version());

    avformat_network_init();

    void *opaque = NULL;
    const char* protocol = NULL;
    while ((protocol = avio_enum_protocols(&opaque, 0))) {
        LOGE("Protocol %s\n", protocol)
    }

    avformat_network_deinit();
}
