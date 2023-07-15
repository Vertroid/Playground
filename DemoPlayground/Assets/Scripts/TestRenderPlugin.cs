using System;
using UnityEngine;
using UnityEngine.UI;

public class TestRenderPlugin : MonoBehaviour
{
    [SerializeField] private RawImage image;
    private Texture2D imageTexture2D;
    private IntPtr nativeTexturePointer;
    private AndroidJavaObject androidAPI;

    private void Start()
    {
        imageTexture2D = new Texture2D(320, 176, TextureFormat.ARGB32, false)
        { filterMode = FilterMode.Point };
        imageTexture2D.Apply();
        image.texture = imageTexture2D;
        nativeTexturePointer = imageTexture2D.GetNativeTexturePtr();

    }

    private void Update()
    {
        if (androidAPI == null)
        {
            // it is important to call this in update method. Single Threaded Rendering will run in UnityMain Thread
            InitializeAndroidSurface(1280, 800);
        }
        else
        {
            androidAPI.Call("updateSurfaceTexture");
            GL.InvalidateState();
        }
    }

    public void InitializeAndroidSurface(int viewportWidth, int viewportHeight)
    {
        AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivityObject = playerClass.GetStatic<AndroidJavaObject>("currentActivity");

        androidAPI = new AndroidJavaObject("com.vertex.plugin.VideoPlugin");
        androidAPI.Call("start", currentActivityObject, nativeTexturePointer.ToInt32(), viewportWidth, viewportHeight);
    }

    public void OnPlay()
    {
        androidAPI.Call("play", "https://www.w3school.com.cn/example/html5/mov_bbb.mp4");
    }
}