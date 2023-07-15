using MoonXR.Video.Players.EasyMovieTexture;
using UnityEngine;

namespace MoonXR.Video
{
    public enum DecodeApi
    {
        Auto = 0,
        DirectShow = 1,
        MediaFoundation = 2,
    }
    public interface IVideoPlayer
    {
        // Media Control

        void Open(string path, bool autoPlay = true);

        void Play();

        void Pause();

        void Stop();

        void Close();

        void SeekTo(float timeMs);

        float GetCurrentTimeMs();

        void SetVolume(float volume); // Range from 0 ~ 1

        float GetVolume();

        void SetSpeed(float speed);

        float GetSpeed();

        // Media Info

        float GetDurationMs();

        int GetVideoWidth();

        int GetVideoHeight();

        // Media Renderer

        void SetMeshRenderer(MeshRenderer renderer);

        // Media Texture

        Texture GetVideoTexture();

        // Audio Track

        int GetCurrentAudioTrack();

        void SetStereoMode(Material mat, StereoPacking packing, bool debugTint);

        void SetDecodeApi(DecodeApi decodeApi);
    }
}