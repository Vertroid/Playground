#include <jni.h>
#include <string>
#include <GLES3/gl3.h>
#include <GLES2/gl2ext.h>

extern "C"
JNIEXPORT jstring JNICALL
Java_com_vertex_plugin_jni_NativePlugin_hello(JNIEnv *env, jclass clazz) {
    std::string hello = "Hello from C++";
    return env->NewStringUTF(hello.c_str());
}