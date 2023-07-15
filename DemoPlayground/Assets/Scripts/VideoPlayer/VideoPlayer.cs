using UnityEngine;
using MoonXR.Video.Events;
using MoonXR.Video.Players.EasyMovieTexture;

namespace MoonXR.Video
{
    public class VideoPlayer : MonoBehaviour
    {
        public VideoPlayerEvent Event => playerEvent ?? (playerEvent = new VideoPlayerEvent());
        private IVideoPlayer actualPlayer;
        private VideoPlayerEvent playerEvent;
        private VideoRenderer videoRenderer;
        public delegate void OnEventChanged(VideoPlayerEvents eventType, VideoPlayerErrors errorCode);
        public event OnEventChanged OnPlayerEventChanged;

        public enum VideoPluginOptions
        {
            AVPRO,
            IJK,
            VLC
        }

        private VideoPluginOptions usedPlugin;

        public VideoPluginOptions UsedPlugin => usedPlugin;
        public IVideoPlayer ActualPlayer => actualPlayer;

        private void Awake()
        {
            // 2.2.2: 保持原状，AVPro无法在Quest端播放MoonLink视频
            usedPlugin = VideoPluginOptions.IJK;
            Init();
            Event.AddListener(OnVideoPlayerEvent);
        }

        private void Init()
        {
            switch (usedPlugin)
            {
                case VideoPluginOptions.IJK:
                    actualPlayer = gameObject.AddComponent<EasyMovieTexturePlayer>();
                    break;
            }
        }

        public void SetMeshRenderer(MeshRenderer renderer)
        {
            actualPlayer?.SetMeshRenderer(renderer);
        }

        public void SetRenderMode(VideoRenderer.RenderMode mode)
        {
            if (videoRenderer != null)
            {
                videoRenderer.SetRenderMode(mode);
            }
        }

        public void SetStereoMode(VideoRenderer.StereoMode mode)
        {
            if (videoRenderer != null)
            {
                videoRenderer.SetStereoMode(mode);
            }
        }

        // Range from -0.3 ~ 0.3
        public void SetBrightness(float brightness)
        {
            if (videoRenderer != null)
            {
                videoRenderer.SetRenderBrightness(brightness);
            }
        }

        public void SetFlipX(bool flip)
        {
            if (videoRenderer != null)
            {
                videoRenderer.SetRenderFlipX(flip);
            }
        }

        public void Open(string url, bool autoPlay = true)
        {
            actualPlayer?.Open(url, autoPlay);
        }

        public void Close()
        {
            actualPlayer?.Close();
        }

        public void Save()
        {
            if(actualPlayer != null && actualPlayer is EasyMovieTexturePlayer)
                ((EasyMovieTexturePlayer)actualPlayer).Save();
        }

        public void Play()
        {
            actualPlayer?.Play();
        }

        public void Pause()
        {
            actualPlayer?.Pause();
        }

        public void SeekTo(float timeMs)
        {
            actualPlayer?.SeekTo(timeMs);
        }

        public float GetDurationMs()
        {
            if (actualPlayer != null)
            {
                var duration = actualPlayer.GetDurationMs();
                return duration;
            }
            return 0f;
        }

        public float GetCurrentTimeMs()
        {
            return actualPlayer?.GetCurrentTimeMs() ?? 0f;
        }

        public int GetVideoWidth()
        {
            return actualPlayer?.GetVideoWidth() ?? 0;
        }

        public int GetVideoHeight()
        {
            return actualPlayer?.GetVideoHeight() ?? 0;
        }

        // Range from 0 ~ 1
        public void SetVolume(float volume)
        {
            actualPlayer?.SetVolume(volume);
        }

        public float GetVolume()
        {
            return actualPlayer?.GetVolume() ?? 0f;
        }

        public void SetSpeed(float speed)
        {
            actualPlayer?.SetSpeed(speed);
        }

        public float GetSpeed()
        {
            return actualPlayer != null? actualPlayer.GetSpeed() : 1.0f;
        }

        private void OnVideoPlayerEvent(VideoPlayerEvents evt, VideoPlayerErrors err)
        {
            switch (evt)
            {
                case VideoPlayerEvents.EVENT_READY:
                case VideoPlayerEvents.EVENT_FIRST_FRAME_READY:
                case VideoPlayerEvents.EVENT_STARTED:
                    if (videoRenderer != null)
                    {
                        videoRenderer.SetVideoSize(GetVideoWidth(), GetVideoHeight());
                        videoRenderer.AdjustScreenSize();
                    }
                    break;
                // 2.2.2: 播放结束时强制唤醒UI，避免无法被更新
                case VideoPlayerEvents.EVENT_ENDED:
                    break;
            }

            OnPlayerEventChanged?.Invoke(evt, err);
        }

        public void AddEventListener(OnEventChanged onPlayerEventChanged)
        {
            OnPlayerEventChanged += onPlayerEventChanged;
        }

        public void RemoveEventListener(OnEventChanged onPlayerEventChanged)
        {
            OnPlayerEventChanged -= onPlayerEventChanged;
        }

        public void SetVideoRenderer(VideoRenderer instance)
        {
            videoRenderer = instance;
            if (videoRenderer != null)
            {
                videoRenderer.SetVideoPlayer(this);
            }
        }
        public void SetDecodeApi(DecodeApi decodeApi)
        {
            actualPlayer.SetDecodeApi(decodeApi);
        }
    }
}