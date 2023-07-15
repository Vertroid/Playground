package com.vertex.plugin.utils;

import android.opengl.GLES11Ext;
import android.opengl.GLES30;
import android.util.Log;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.FloatBuffer;
import java.nio.ShortBuffer;

public class GLESUtils {
    private static final String TAG = "[MOONVR]";

    // 根据类型编译着色器
    public static int compileShader(int type, String shaderCode) {
        final int shaderObjectId = GLES30.glCreateShader(type);
        if (shaderObjectId == 0) {
            Log.e(TAG, "glCreateShader failed");
            return 0;
        }
        Log.e(TAG, "glCreateShader success");
        GLES30.glShaderSource(shaderObjectId, shaderCode);
        GLES30.glCompileShader(shaderObjectId);
        final int[] compileStatus = new int[1];
        GLES30.glGetShaderiv(shaderObjectId, GLES30.GL_COMPILE_STATUS, compileStatus, 0);
        if (compileStatus[0] == 0) {
            Log.e(TAG, "glCompileShader failed");
            GLES30.glDeleteShader(shaderObjectId);
            return 0;
        }
        Log.e(TAG, "glCompileShader success");
        return shaderObjectId;
    }

    // 创建 OpenGL 程序和着色器链接
    public static int linkProgram(int vertexShaderId, int fragmentShaderId) {
        // 创建 OpenGL 程序 ID
        final int programObjectId = GLES30.glCreateProgram();
        if (programObjectId == 0) {
            return 0;
        }
        GLES30.glAttachShader(programObjectId, vertexShaderId);
        GLES30.glAttachShader(programObjectId, fragmentShaderId);
        GLES30.glLinkProgram(programObjectId);
        final int[] linkStatus = new int[1];
        GLES30.glGetProgramiv(programObjectId, GLES30.GL_LINK_STATUS, linkStatus, 0);
        if (linkStatus[0] == 0) {
            // 失败则删除 OpenGL 程序
            GLES30.glDeleteProgram(programObjectId);
            return 0;
        }
        return programObjectId;
    }

    // 链接了 OpenGL 程序后，就是验证 OpenGL 是否可用。
    public static boolean validateProgram(int programObjectId) {
        GLES30.glValidateProgram(programObjectId);
        final int[] validateStatus = new int[1];
        GLES30.glGetProgramiv(programObjectId, GLES30.GL_VALIDATE_STATUS, validateStatus, 0);
        return validateStatus[0] != 0;
    }

    // 创建 OpenGL 程序过程
    public static int buildProgram(String vertexShaderSource, String fragmentShaderSource) {
        int vertexShader = compileShader(GLES30.GL_VERTEX_SHADER, vertexShaderSource);
        int fragmentShader = compileShader(GLES30.GL_FRAGMENT_SHADER, fragmentShaderSource);
        int program = linkProgram(vertexShader, fragmentShader);
        boolean valid = validateProgram(program);
        if(!valid) {
            GLES30.glDeleteProgram(program);
            Log.d(TAG, "buildProgram: glLinkProgram err");
            return 0;
        }
        return program;
    }

    public static int createFBO() {
        int[] fbo = new int[1];
        GLES30.glGenFramebuffers(fbo.length, fbo, 0);
        return fbo[0];
    }

    public static int createVAO() {
        int[] vao = new int[1];
        GLES30.glGenVertexArrays(vao.length, vao, 0);
        return vao[0];
    }

    public static int createVBO() {
        int[] vbo = new int[1];
        GLES30.glGenBuffers(2, vbo, 0);
        return vbo[0];
    }

    public static int createOESTextureID() {
        int[] texture = new int[1];
        GLES30.glGenTextures(texture.length, texture, 0);
        GLES30.glBindTexture(GLES11Ext.GL_TEXTURE_EXTERNAL_OES, texture[0]);
        GLES30.glTexParameterf(GLES11Ext.GL_TEXTURE_EXTERNAL_OES, GLES30.GL_TEXTURE_MIN_FILTER, GLES30.GL_LINEAR_MIPMAP_LINEAR);
        GLES30.glTexParameterf(GLES11Ext.GL_TEXTURE_EXTERNAL_OES, GLES30.GL_TEXTURE_MAG_FILTER, GLES30.GL_LINEAR);
        GLES30.glTexParameteri(GLES11Ext.GL_TEXTURE_EXTERNAL_OES, GLES30.GL_TEXTURE_WRAP_S, GLES30.GL_CLAMP_TO_EDGE);
        GLES30.glTexParameteri(GLES11Ext.GL_TEXTURE_EXTERNAL_OES, GLES30.GL_TEXTURE_WRAP_T, GLES30.GL_CLAMP_TO_EDGE);
        GLES30.glGenerateMipmap(GLES11Ext.GL_TEXTURE_EXTERNAL_OES);
        return texture[0];
    }

    public static int create2DTextureId(int width, int height) {
        int[] textures = new int[1];
        GLES30.glGenTextures(textures.length, textures, 0);
        GLES30.glBindTexture(GLES30.GL_TEXTURE_2D, textures[0]);
        GLES30.glTexImage2D(GLES30.GL_TEXTURE_2D, 0, GLES30.GL_RGBA, width, height, 0,
                GLES30.GL_RGBA, GLES30.GL_UNSIGNED_BYTE, null);
        GLES30.glTexParameterf(GLES30.GL_TEXTURE_2D, GLES30.GL_TEXTURE_MIN_FILTER, GLES30.GL_LINEAR);
        GLES30.glTexParameterf(GLES30.GL_TEXTURE_2D, GLES30.GL_TEXTURE_MAG_FILTER, GLES30.GL_LINEAR);
        GLES30.glTexParameteri(GLES30.GL_TEXTURE_2D, GLES30.GL_TEXTURE_WRAP_S, GLES30.GL_CLAMP_TO_EDGE);
        GLES30.glTexParameteri(GLES30.GL_TEXTURE_2D, GLES30.GL_TEXTURE_WRAP_T, GLES30.GL_CLAMP_TO_EDGE);
        GLES30.glGenerateMipmap(GLES30.GL_TEXTURE_2D);
        return textures[0];
    }
}
