using UnityEngine.Events;

namespace MoonXR.Video.Events
{
    // Events
    public enum VideoPlayerEvents
    {
        EVENT_READY = 1,
        EVENT_STARTED = 2,
        EVENT_FIRST_FRAME_READY = 3,
        EVENT_ENDED = 4,
        EVENT_ERROR = 5,
    };

    // Errors
    public enum VideoPlayerErrors
    {
        ERROR_NONE = 100,
        ERROR_LOAD_FAILED,
        ERROR_DECODE_FAILED,
        ERROR_UNAUTHORIZED_ACCESS_EXCEPTION,
        ERROR_UNSUPPORTED,
        ERROR_IO,
        ERROR_SERVER_DIED,
        ERROR_TIMEOUT,
        ERROR_UNKNOWN
    }

    /// <summary>
    /// 视频播放事件类型
    /// </summary>
    public class VideoPlayerEvent : UnityEvent<VideoPlayerEvents, VideoPlayerErrors>
    {
    }
}