using System.Collections.Generic;
using System.IO;
using EasyMovieTexture.Scripts;
using MoonXR.Video.Events;
using UnityEngine;
using static MoonXR.Video.Players.EasyMovieTexture.RendererHelper;

namespace MoonXR.Video.Players.EasyMovieTexture
{
    /// <summary>
    /// Wrapper implementation for EasyMovieTexture MediaPlayerCtrl.
    /// </summary>
    public class EasyMovieTexturePlayer :
        MediaPlayerCtrl, IVideoPlayer
    {
        // The public video player instance.
        private VideoPlayer mPlayerInstance;
        private EasyMovieTextureApplyToMesh mApplyToMesh;
        private EasyMovieTextureUpdateStereoMaterial mUpdateStereoMaterial;

        protected void Awake()
        {
            base.Awake();

            mApplyToMesh = gameObject.AddComponent<EasyMovieTextureApplyToMesh>();
            mApplyToMesh.Player = this;

            mUpdateStereoMaterial = gameObject.AddComponent<EasyMovieTextureUpdateStereoMaterial>();
            mUpdateStereoMaterial._camera = Camera.main;

            mPlayerInstance = GetComponent<VideoPlayer>();
        }

        protected void OnEnable()
        {
            OnReady += HandleVideoReady;
            OnEnd += HandleVideoEnd;
            OnVideoError += HandleVideoError;
            OnVideoFirstFrameReady += HandleFirstFrameReady;
        }

        protected void OnDisable()
        {
            OnReady -= HandleVideoReady;
            OnEnd -= HandleVideoEnd;
            OnVideoError -= HandleVideoError;
            OnVideoFirstFrameReady -= HandleFirstFrameReady;
        }

        public void Open(string path, bool autoPlay = true)
        {
            AutoPlay = autoPlay;
            base.Load(path);
        }

        public void Save()
        {
            if(VideoTexture != null)
            {
                VideoTexture.Apply();
                var tex = VideoTexture.EncodeToPNG();
                FileStream fs = File.Create("/sdcard/Pictures/test.png");
                fs.Write(tex, 0, tex.Length);
                fs.Close();
            }
        }

        public void Play()
        {
            base.Play();
        }

        public void Pause()
        {
            base.Pause();
        }

        public void Stop()
        {
            base.Stop();
        }

        public void Close()
        {
            base.UnLoad();
        }

        public void SeekTo(float timeMs)
        {
            base.SeekTo((int) timeMs);
        }

        public float GetCurrentTimeMs()
        {
            return base.GetCurrentPosition();
        }

        public void SetVolume(float volume)
        {
            base.SetVolume(volume);
        }

        public float GetVolume()
        {
            return base.GetVolume();
        }

        public void SetSpeed(float speed)
        {
            base.SetSpeed(speed);
        }

        public float GetSpeed()
        {
            return base.GetSpeed();
        }

        public float GetDurationMs()
        {
            return base.GetDuration();
        }

        public int GetVideoWidth()
        {
            return base.GetVideoWidth();
        }

        public int GetVideoHeight()
        {
            return base.GetVideoHeight();
        }

        Texture IVideoPlayer.GetVideoTexture()
        {
            return base.GetVideoTexture();
        }

        public void SetMeshRenderer(MeshRenderer renderer)
        {
            mApplyToMesh.MeshRenderer = renderer;
            mUpdateStereoMaterial._renderer = renderer;
        }

        public int GetCurrentAudioTrack()
        {
            return base.getAudioSelectedTrack();
        }

        private void HandleVideoReady()
        {
            mPlayerInstance.Event.Invoke(VideoPlayerEvents.EVENT_READY, VideoPlayerErrors.ERROR_NONE);
            var items = getVideoTrackInfo();
            if(items == null || items.Length == 0)
            {
                mPlayerInstance.Event.Invoke(VideoPlayerEvents.EVENT_FIRST_FRAME_READY, VideoPlayerErrors.ERROR_NONE);
                if (AutoPlay)
                {
                    Play();
                }
            }
        }

        private void HandleVideoEnd()
        {
            mPlayerInstance.Event.Invoke(VideoPlayerEvents.EVENT_ENDED, VideoPlayerErrors.ERROR_NONE);
        }

        private void HandleFirstFrameReady()
        {
            mPlayerInstance.Event.Invoke(VideoPlayerEvents.EVENT_FIRST_FRAME_READY, VideoPlayerErrors.ERROR_NONE);

            if (AutoPlay)
            {
                Play();
            }
        }

        private void HandleVideoError(MediaPlayerError err, MediaPlayerError errExtra)
        {
            var errorCode = VideoPlayerErrors.ERROR_NONE;
            switch (err)
            {
                case MediaPlayerError.MEDIA_ERROR_UNSUPPORTED:
                    errorCode = VideoPlayerErrors.ERROR_UNSUPPORTED;
                    break;
                case MediaPlayerError.MEDIA_ERROR_IO:
                    errorCode = VideoPlayerErrors.ERROR_IO;
                    break;
                case MediaPlayerError.MEDIA_ERROR_SERVER_DIED:
                    errorCode = VideoPlayerErrors.ERROR_SERVER_DIED;
                    break;
                case MediaPlayerError.MEDIA_ERROR_TIMED_OUT:
                    errorCode = VideoPlayerErrors.ERROR_TIMEOUT;
                    break;
                default:
                    errorCode = VideoPlayerErrors.ERROR_UNKNOWN;
                    break;
            }
            mPlayerInstance.Event.Invoke(VideoPlayerEvents.EVENT_ERROR, errorCode);
        }

        public void SetStereoMode(Material mat, StereoPacking packing, bool debugTint)
        {
            RendererHelper.SetupStereoMaterial(mat, packing, debugTint);
        }

        public void SetDecodeApi(DecodeApi decodeApi)
        {

        }
    }

}