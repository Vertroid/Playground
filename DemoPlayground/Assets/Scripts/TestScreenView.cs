using MoonXR.Video;
using MoonXR.Video.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScreenView : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private VideoRenderer videoRenderer;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoRenderer = GetComponent<VideoRenderer>();
        videoPlayer.SetVideoRenderer(videoRenderer);

    }

    private void Start()
    {
    }

    private void OnEnable()
    {
        videoPlayer.AddEventListener(OnVideoPlayerEvent);
    }

    private void OnDisable()
    {
        videoPlayer.RemoveEventListener(OnVideoPlayerEvent);
    }

    private void OnVideoPlayerEvent(VideoPlayerEvents eventType, VideoPlayerErrors error)
    {
        // to do
    }

    public void TestOpen()
    {
        videoPlayer.Open("/sdcard/Movies/360.mkv");
    }

    public void TestOpen2()
    {
        videoPlayer.Open("/sdcard/Movies/test.flv");
    }

    public void TestPlay()
    {
        videoPlayer.Play();
    }

    public void TestSave()
    {
        videoPlayer.Save();
    }
}
