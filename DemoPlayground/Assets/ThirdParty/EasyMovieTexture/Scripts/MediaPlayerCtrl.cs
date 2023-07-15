using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

using UnityEngine;

// ReSharper disable All

namespace EasyMovieTexture.Scripts {

    public class MediaPlayerCtrl : MonoBehaviour {

        public string FileName;
        public bool FullScreen; //Please use only in FullScreen prefab.
        public bool SupportRockchip = true; //Using a device support Rochchip or Low-end devices
        public bool Init;
        public MediaScale ScaleValue;
        public GameObject[] ObjResize;
        public bool Loop;
        public bool AutoPlay = true;
        public Texture2D VideoTexture;

        private bool First;
        private Texture2D mVideoTextureDummy;
        private MediaPlayerState mCurrentState;
        private int mCurrentSeekPosition;
        private float mVolume = 1.0f;
        private float mPlaybackSpeed = 1.0f;
        private int mAndroidMgrID;
        private bool mIsFirstFrameReady;
        private bool mStop;
        private bool mCheckFBO;
        private bool mPause;

        public delegate void VideoEnd();
        public delegate void VideoReady();
        public delegate void VideoError(MediaPlayerError errorCode, MediaPlayerError errorCodeExtra);
        public delegate void VideoFirstFrameReady();
        public VideoReady OnReady;
        public VideoEnd OnEnd;
        public VideoError OnVideoError;
        public VideoFirstFrameReady OnVideoFirstFrameReady;

        [DllImport("BlueDoveMediaRender")]
        public static extern void InitNDK();

        [DllImport("BlueDoveMediaRender")]
        private static extern IntPtr EasyMovieTextureRender();

        public enum MediaPlayerInfo {
            MEDIA_INFO_UNKNOWN = 1,
            MEDIA_INFO_STARTED_AS_NEXT = 2,
            MEDIA_INFO_VIDEO_RENDERING_START = 3,
            MEDIA_INFO_VIDEO_TRACK_LAGGING = 700,
            MEDIA_INFO_BUFFERING_START = 701,
            MEDIA_INFO_BUFFERING_END = 702,
            MEDIA_INFO_NETWORK_BANDWIDTH = 703,
            MEDIA_INFO_BAD_INTERLEAVING = 800,
            MEDIA_INFO_NOT_SEEKABLE = 801,
            MEDIA_INFO_METADATA_UPDATE = 802,
            MEDIA_INFO_TIMED_TEXT_ERROR = 900,
            MEDIA_INFO_UNSUPPORTED_SUBTITLE = 901,
            MEDIA_INFO_SUBTITLE_TIMED_OUT = 902,
            MEDIA_INFO_VIDEO_ROTATION_CHANGED = 10001,
            MEDIA_INFO_AUDIO_RENDERING_START = 10002
        }

        public enum MediaPlayerError {

            MEDIA_ERROR_NOT_VALID_FOR_PROGRESSIVE_PLAYBACK = 200,

            MEDIA_ERROR_IO = -1004,

            MEDIA_ERROR_MALFORMED = -1007,

            MEDIA_ERROR_TIMED_OUT = -110,

            MEDIA_ERROR_UNSUPPORTED = -1010,

            MEDIA_ERROR_SERVER_DIED = 100,

            MEDIA_ERROR_UNKNOWN = 1

        }

        public enum MediaPlayerState {

            NOT_READY = 0,

            READY = 1,

            END = 2,

            PLAYING = 3,

            PAUSED = 4,

            STOPPED = 5,

            ERROR = 6

        }

        public enum MediaScale {

            SCALE_X_TO_Y = 0,

            SCALE_X_TO_Z = 1,

            SCALE_Y_TO_X = 2,

            SCALE_Y_TO_Z = 3,

            SCALE_Z_TO_X = 4,

            SCALE_Z_TO_Y = 5,

            SCALE_X_TO_Y_2 = 6,

        }

        protected void Awake() {
            if (SystemInfo.deviceModel.Contains("rockchip")) {
                SupportRockchip = true;
            }
            else {
                SupportRockchip = false;
            }
        }

        // Use this for initialization
        void Start() {
            if (SystemInfo.graphicsMultiThreaded == true)
                InitNDK();

            mAndroidMgrID = Call_InitNDK();
            Call_SetUnityActivity();

            if (Application.dataPath.Contains(".obb")) {
                Call_SetSplitOBB(true, Application.dataPath);
            }
            else {
                Call_SetSplitOBB(false, null);
            }

            Init = true;
        }

        void OnApplicationQuit() {
            if (Directory.Exists(Application.persistentDataPath + "/Data"))
                Directory.Delete(Application.persistentDataPath + "/Data", true);
        }

        void OnDisable() {
            if (GetCurrentState() == MediaPlayerState.PLAYING) {
                Pause();
            }
        }

        void OnEnable() {
            if (GetCurrentState() == MediaPlayerState.PAUSED) {
                Play();
            }
        }

        void Update() {
            if (string.IsNullOrEmpty(FileName)) {
                return;
            }

            if (First == false) {
                string strName = FileName.Trim();

                if (SupportRockchip) {
                    Call_SetRockchip(SupportRockchip);

                    if (strName.Contains("://")) {
                        Call_Load(strName, 0);
                    }
                    else {
                        //Call_Load(strName,0);GetObject
                        StartCoroutine(CopyStreamingAssetVideoAndLoad(strName));
                    }
                }
                else {
                    if (strName.Contains("://"))
                        Call_Load(strName, 0);
                    else
                        Call_Load("file://" + strName, 0);
                }

                Call_SetLooping(Loop);
                First = true;
            }

            if (mCurrentState == MediaPlayerState.PLAYING || mCurrentState == MediaPlayerState.PAUSED ||
                mCurrentState == MediaPlayerState.END) {
                if (mCheckFBO == false) {
                    if (Call_GetVideoWidth() <= 0 || Call_GetVideoHeight() <= 0) {
                        CheckStatusChange();
                        return;
                    }

                    Resize();

                    if (VideoTexture != null) {
                        //Destroy(m_VideoTexture);
                        if (mVideoTextureDummy != null) {
                            Destroy(mVideoTextureDummy);
                            mVideoTextureDummy = null;
                        }

                        mVideoTextureDummy = VideoTexture;
                        VideoTexture = null;
                    }

                    if (SupportRockchip) {
                        VideoTexture = new Texture2D(Call_GetVideoWidth(), Call_GetVideoHeight(),
                            TextureFormat.RGB565, false);
                    }
                    else {
                        VideoTexture = new Texture2D(Call_GetVideoWidth(), Call_GetVideoHeight(),
                            TextureFormat.ARGB32, false);
                    }

                    VideoTexture.filterMode = FilterMode.Bilinear;
                    VideoTexture.wrapMode = TextureWrapMode.Clamp;

                    Call_SetUnityTexture((int) VideoTexture.GetNativeTexturePtr());

                    Call_SetWindowSize();
                    mCheckFBO = true;
                }

                Call_UpdateVideoTexture();
                mCurrentSeekPosition = Call_GetSeekPosition();
            }

            CheckStatusChange();
        }

        private void CheckStatusChange()
        {
            if (mCurrentState != Call_GetStatus())
            {
                mCurrentState = Call_GetStatus();

                if (mCurrentState == MediaPlayerState.READY)
                {
                    if (OnReady != null)
                        OnReady();

                    if (AutoPlay)
                        Call_Play(0);

                    SetVolume(mVolume);
                }
                else if (mCurrentState == MediaPlayerState.END)
                {
                    if (OnEnd != null)
                        OnEnd();

                    if (Loop)
                    {
                        Call_Play(0);
                    }
                }
                else if (mCurrentState == MediaPlayerState.ERROR)
                {
                    OnError((MediaPlayerError)Call_GetError(), (MediaPlayerError)Call_GetErrorExtra());
                }
            }
        }

        public void Resize() {
            if (mCurrentState != MediaPlayerState.PLAYING)
                return;

            if (Call_GetVideoWidth() <= 0 || Call_GetVideoHeight() <= 0) {
                return;
            }

            if (ObjResize != null) {
                int iScreenWidth = Screen.width;
                int iScreenHeight = Screen.height;

                float fRatioScreen = iScreenHeight / (float) iScreenWidth;
                int iWidth = Call_GetVideoWidth();
                int iHeight = Call_GetVideoHeight();

                float fRatio = iHeight / (float) iWidth;
                float fRatioResult = fRatioScreen / fRatio;

                for (int i = 0; i < ObjResize.Length; i++) {
                    if (ObjResize[i] == null)
                        continue;

                    if (FullScreen) {
                        ObjResize[i].transform.localScale =
                            new Vector3(20.0f / fRatioScreen, 20.0f / fRatioScreen, 1.0f);

                        if (fRatio < 1.0f) {
                            if (fRatioScreen < 1.0f) {
                                if (fRatio > fRatioScreen) {
                                    ObjResize[i].transform.localScale *= fRatioResult;
                                }
                            }

                            ScaleValue = MediaScale.SCALE_X_TO_Y;
                        }
                        else {
                            if (fRatioScreen > 1.0f) {
                                if (fRatio >= fRatioScreen) {
                                    ObjResize[i].transform.localScale *= fRatioResult;
                                }
                            }

                            ScaleValue = MediaScale.SCALE_X_TO_Y;
                        }
                    }

                    if (ScaleValue == MediaScale.SCALE_X_TO_Y) {
                        ObjResize[i].transform.localScale
                            = new Vector3(ObjResize[i].transform.localScale.x
                                , ObjResize[i].transform.localScale.x * fRatio
                                , ObjResize[i].transform.localScale.z);
                    }
                    else if (ScaleValue == MediaScale.SCALE_X_TO_Y_2) {
                        ObjResize[i].transform.localScale
                            = new Vector3(ObjResize[i].transform.localScale.x
                                , ObjResize[i].transform.localScale.x * fRatio / 2.0f
                                , ObjResize[i].transform.localScale.z);
                    }
                    else if (ScaleValue == MediaScale.SCALE_X_TO_Z) {
                        ObjResize[i].transform.localScale
                            = new Vector3(ObjResize[i].transform.localScale.x
                                , ObjResize[i].transform.localScale.y
                                , ObjResize[i].transform.localScale.x * fRatio);
                    }
                    else if (ScaleValue == MediaScale.SCALE_Y_TO_X) {
                        ObjResize[i].transform.localScale
                            = new Vector3(ObjResize[i].transform.localScale.y / fRatio
                                , ObjResize[i].transform.localScale.y
                                , ObjResize[i].transform.localScale.z);
                    }
                    else if (ScaleValue == MediaScale.SCALE_Y_TO_Z) {
                        ObjResize[i].transform.localScale
                            = new Vector3(ObjResize[i].transform.localScale.x
                                , ObjResize[i].transform.localScale.y
                                , ObjResize[i].transform.localScale.y / fRatio);
                    }
                    else if (ScaleValue == MediaScale.SCALE_Z_TO_X) {
                        ObjResize[i].transform.localScale
                            = new Vector3(ObjResize[i].transform.localScale.z * fRatio
                                , ObjResize[i].transform.localScale.y
                                , ObjResize[i].transform.localScale.z);
                    }
                    else if (ScaleValue == MediaScale.SCALE_Z_TO_Y) {
                        ObjResize[i].transform.localScale
                            = new Vector3(ObjResize[i].transform.localScale.x
                                , ObjResize[i].transform.localScale.z * fRatio
                                , ObjResize[i].transform.localScale.z);
                    }
                    else {
                        ObjResize[i].transform.localScale
                            = new Vector3(ObjResize[i].transform.localScale.x, ObjResize[i].transform.localScale.y,
                                ObjResize[i].transform.localScale.z);
                    }
                }
            }
        }

        /// The error code is the following sites related documents.
        /// http://developer.android.com/reference/android/media/MediaPlayer.OnErrorListener.html 
        void OnError(MediaPlayerError iCode, MediaPlayerError iCodeExtra) {
            string strError;

            switch (iCode) {
                case MediaPlayerError.MEDIA_ERROR_NOT_VALID_FOR_PROGRESSIVE_PLAYBACK:
                    strError = "MEDIA_ERROR_NOT_VALID_FOR_PROGRESSIVE_PLAYBACK";

                    break;
                case MediaPlayerError.MEDIA_ERROR_SERVER_DIED:
                    strError = "MEDIA_ERROR_SERVER_DIED";

                    break;
                case MediaPlayerError.MEDIA_ERROR_UNKNOWN:
                    strError = "MEDIA_ERROR_UNKNOWN";
                    break;
                default:
                    strError = "Unknown error " + iCode;
                    break;
            }

            strError += " ";

            switch (iCodeExtra) {
                case MediaPlayerError.MEDIA_ERROR_IO:
                    strError += "MEDIA_ERROR_IO";

                    break;
                case MediaPlayerError.MEDIA_ERROR_MALFORMED:
                    strError += "MEDIA_ERROR_MALFORMED";

                    break;
                case MediaPlayerError.MEDIA_ERROR_TIMED_OUT:
                    strError += "MEDIA_ERROR_TIMED_OUT";

                    break;
                case MediaPlayerError.MEDIA_ERROR_UNSUPPORTED:
                    strError += "MEDIA_ERROR_UNSUPPORTED";

                    break;
                default:
                    strError = "Unknown error " + iCode;

                    break;
            }

            Debug.LogError(strError);

            if (OnVideoError != null) {
                OnVideoError(iCode, iCodeExtra);
            }
        }

        void OnDestroy() {
            Call_UnLoad();

            if (mVideoTextureDummy != null) {
                Destroy(mVideoTextureDummy);
                mVideoTextureDummy = null;
            }

            if (VideoTexture != null)
                Destroy(VideoTexture);

            Call_Destroy();
        }

        void OnApplicationPause(bool bPause) {
            if (bPause) {
                if (mCurrentState == MediaPlayerState.PAUSED) {
                    mPause = true;
                }

                Call_Pause();
            }
            else {
                Call_RePlay();

                if (mPause) {
                    Call_Pause();
                    mPause = false;
                }
            }
        }

        public MediaPlayerState GetCurrentState() {
            return mCurrentState;
        }

        public Texture2D GetVideoTexture() {
            return VideoTexture;
        }

        public void Play() {
            if (mStop) {
                Call_Play(0);
                mStop = false;
            }

            if (mCurrentState == MediaPlayerState.PAUSED) {
                Call_RePlay();
            }
            else if (mCurrentState == MediaPlayerState.READY || mCurrentState == MediaPlayerState.STOPPED ||
                     mCurrentState == MediaPlayerState.END) {
                Call_Play(0);
            }
        }

        public void Stop() {
            if (mCurrentState == MediaPlayerState.PLAYING)
                Call_Pause();

            mStop = true;
            mCurrentState = MediaPlayerState.STOPPED;
            mCurrentSeekPosition = 0;
        }

        public void Pause() {
            if (mCurrentState == MediaPlayerState.PLAYING)
                Call_Pause();

            mCurrentState = MediaPlayerState.PAUSED;
        }

        public void Load(string strFileName) {
            if (GetCurrentState() != MediaPlayerState.NOT_READY)
                UnLoad();

            mIsFirstFrameReady = false;
            First = false;
            mCheckFBO = false;
            FileName = strFileName;

            if (Init == false)
                return;

            mCurrentState = MediaPlayerState.NOT_READY;
        }

        public void SetVolume(float fVolume) {
            if (mCurrentState == MediaPlayerState.PLAYING || mCurrentState == MediaPlayerState.PAUSED ||
                mCurrentState == MediaPlayerState.END || mCurrentState == MediaPlayerState.READY ||
                mCurrentState == MediaPlayerState.STOPPED) {
                mVolume = fVolume;
                Call_SetVolume(fVolume);
            }
        }

        public float GetVolume() {
            return mVolume;
        }

        public void SetSpeed(float speed)
        {
            if (mCurrentState == MediaPlayerState.PLAYING || mCurrentState == MediaPlayerState.PAUSED ||
                mCurrentState == MediaPlayerState.END || mCurrentState == MediaPlayerState.READY ||
                mCurrentState == MediaPlayerState.STOPPED)
            {
                mPlaybackSpeed = speed;
                Call_SetSpeed(speed);
            }
        }

        public float GetSpeed()
        {
            return mPlaybackSpeed;
        }

        //return milisecond  
        public int GetSeekPosition() {
            if (mCurrentState == MediaPlayerState.PLAYING || mCurrentState == MediaPlayerState.PAUSED ||
                mCurrentState == MediaPlayerState.END)
                return mCurrentSeekPosition;
            else
                return 0;
        }

        public void SeekTo(int iSeek) {
            if (mCurrentState == MediaPlayerState.PLAYING || mCurrentState == MediaPlayerState.READY ||
                mCurrentState == MediaPlayerState.PAUSED || mCurrentState == MediaPlayerState.END ||
                mCurrentState == MediaPlayerState.STOPPED)
                Call_SetSeekPosition(iSeek);
        }

        public int GetCurrentPosition() {
            if (mCurrentState == MediaPlayerState.PLAYING || mCurrentState == MediaPlayerState.READY ||
                mCurrentState == MediaPlayerState.PAUSED || mCurrentState == MediaPlayerState.END ||
                mCurrentState == MediaPlayerState.STOPPED)
                return Call_GetCurrentPosition();

            return 0;
        }

        //Gets the duration of the file.
        //Returns
        //the duration in milliseconds, if no duration is available (for example, if streaming live content), -1 is returned.
        public int GetDuration() {
            if (mCurrentState == MediaPlayerState.PLAYING || mCurrentState == MediaPlayerState.PAUSED ||
                mCurrentState == MediaPlayerState.END || mCurrentState == MediaPlayerState.READY ||
                mCurrentState == MediaPlayerState.STOPPED)
                return Call_GetDuration();
            else
                return 0;
        }

        public float GetSeekBarValue() {
            if (mCurrentState == MediaPlayerState.PLAYING || mCurrentState == MediaPlayerState.PAUSED ||
                mCurrentState == MediaPlayerState.END || mCurrentState == MediaPlayerState.READY ||
                mCurrentState == MediaPlayerState.STOPPED) {
                if (GetDuration() == 0) {
                    return 0;
                }

                return GetSeekPosition() / (float) GetDuration();
            }
            else
                return 0;
        }

        public void SetSeekBarValue(float fValue) {
            switch (mCurrentState) {
                case MediaPlayerState.PLAYING:
                case MediaPlayerState.PAUSED:
                case MediaPlayerState.END:
                case MediaPlayerState.READY:
                case MediaPlayerState.STOPPED: {
                    if (GetDuration() == 0) {
                        return;
                    }

                    SeekTo((int) (GetDuration() * fValue));

                    break;
                }
                default:
                    return;
            }
        }

        //Get update status in buffering a media stream received through progressive HTTP download. 
        //The received buffering percentage indicates how much of the content has been buffered or played. 
        //For example a buffering update of 80 percent when half the content has already been played indicates that the next 30 percent of the content to play has been buffered.
        //the percentage (0-100) of the content that has been buffered or played thus far 
        public int GetCurrentSeekPercent() {
            if (mCurrentState == MediaPlayerState.PLAYING || mCurrentState == MediaPlayerState.PAUSED ||
                mCurrentState == MediaPlayerState.END || mCurrentState == MediaPlayerState.READY)
                return Call_GetCurrentSeekPercent();
            else
                return 0;
        }

        public int GetVideoInfo() {
            return Call_GetInfo();
        }

        public int GetVideoWidth() {
            return Call_GetVideoWidth();
        }

        public int GetVideoHeight() {
            return Call_GetVideoHeight();
        }

        public void UnLoad() {
            mCheckFBO = false;
            Call_UnLoad();

            mCurrentState = MediaPlayerState.NOT_READY;
        }

        private AndroidJavaObject mJavaObj;

        public MediaPlayerCtrl() {
            mVideoTextureDummy = null;
            ObjResize = null;
            Loop = false;
            mStop = false;
            Init = false;
            mCheckFBO = false;
            mPause = false;
        }

        protected AndroidJavaObject GetJavaObject() {
            if (mJavaObj == null) {
                mJavaObj = new AndroidJavaObject("com.EasyMovieTexture.EasyMovieTexture");
            }

            return mJavaObj;
        }

        private void Call_Destroy() {
            if (SystemInfo.graphicsMultiThreaded) {
                GL.IssuePluginEvent(EasyMovieTextureRender(), 5 + mAndroidMgrID * 10 + 7000);
            }
            else {
                GetJavaObject().Call("Destroy");
            }
        }

        private void Call_UnLoad() {
            if (SystemInfo.graphicsMultiThreaded) {
                GL.IssuePluginEvent(EasyMovieTextureRender(), 4 + mAndroidMgrID * 10 + 7000);
            }
            else {
                GetJavaObject().Call("UnLoad");
            }
        }

        private bool Call_Load(string strFileName, int iSeek) {
            if (SystemInfo.graphicsMultiThreaded)
            {
                GetJavaObject().Call("NDK_SetFileName", strFileName);
                GL.IssuePluginEvent(EasyMovieTextureRender(), 1 + mAndroidMgrID * 10 + 7000);
                Call_SetNotReady();
                return true;
            }
            else
            {
                GetJavaObject().Call("NDK_SetFileName", strFileName);
                if (GetJavaObject().Call<bool>("Load"))
                {
                    return true;
                }
                else
                {
                    OnError(MediaPlayerError.MEDIA_ERROR_UNKNOWN, MediaPlayerError.MEDIA_ERROR_UNKNOWN);
                    return false;
                }
            }
        }

        private void Call_UpdateVideoTexture() {
            if (Call_IsUpdateFrame() == false)
                return;

            if (mVideoTextureDummy != null) {
                Destroy(mVideoTextureDummy);
                mVideoTextureDummy = null;
            }

            if (SystemInfo.graphicsMultiThreaded == true) {
                GL.IssuePluginEvent(EasyMovieTextureRender(), 3 + mAndroidMgrID * 10 + 7000);
            }
            else {
                GetJavaObject().Call("UpdateVideoTexture");
            }

            if (!mIsFirstFrameReady) {
                mIsFirstFrameReady = true;

                if (OnVideoFirstFrameReady != null) {
                    OnVideoFirstFrameReady();
                }
            }
        }

        private void Call_SetVolume(float fVolume) {
            GetJavaObject().Call("SetVolume", fVolume);
        }

        private void Call_SetSpeed(float speed)
        {
            GetJavaObject().Call("SetSpeed", speed);
        }

        private float Call_GetSpeed()
        {
            return GetJavaObject().Call<float>("GetSpeed");
        }

        private void Call_SetSeekPosition(int iSeek) {
            GetJavaObject().Call("SetSeekPosition", iSeek);
        }

        private int Call_GetSeekPosition() {
            return GetJavaObject().Call<int>("GetSeekPosition");
        }

        private void Call_Play(int iSeek) {
            GetJavaObject().Call("Play", iSeek);
        }

        private void Call_Reset() {
            GetJavaObject().Call("Reset");
        }

        private void Call_Stop() {
            GetJavaObject().Call("Stop");
        }

        private void Call_RePlay() {
            GetJavaObject().Call("RePlay");
        }

        private void Call_Pause() {
            GetJavaObject().Call("Pause");
        }

        private int Call_InitNDK() {
            return GetJavaObject().Call<int>("InitNative", GetJavaObject());
        }

        private int Call_GetVideoWidth() {
            return GetJavaObject().Call<int>("GetVideoWidth");
        }

        private int Call_GetVideoHeight() {
            return GetJavaObject().Call<int>("GetVideoHeight");
        }

        private bool Call_IsUpdateFrame() {
            return GetJavaObject().Call<bool>("IsUpdateFrame");
        }

        private void Call_SetUnityTexture(int iTextureID) {
            GetJavaObject().Call("SetUnityTexture", iTextureID);
        }

        private void Call_SetWindowSize() {
            if (SystemInfo.graphicsMultiThreaded == true) {
                GL.IssuePluginEvent(EasyMovieTextureRender(), 2 + mAndroidMgrID * 10 + 7000);
            }
            else {
                GetJavaObject().Call("SetWindowSize");
            }
        }

        private void Call_SetLooping(bool bLoop) {
            GetJavaObject().Call("SetLooping", bLoop);
        }

        private void Call_SetRockchip(bool bValue) {
            GetJavaObject().Call("SetRockchip", bValue);
        }

        private int Call_GetDuration() {
            return GetJavaObject().Call<int>("GetDuration");
        }

        private int Call_GetCurrentPosition() {
            return GetJavaObject().Call<int>("GetCurrentPosition");
        }

        private int Call_GetCurrentSeekPercent() {
            return GetJavaObject().Call<int>("GetCurrentSeekPercent");
        }

        private int Call_GetInfo() {
            return GetJavaObject().Call<int>("GetInfo");
        }

        private int Call_GetError() {
            return GetJavaObject().Call<int>("GetError");
        }

        private void Call_SetSplitOBB(bool bValue, string strOBBName) {
            GetJavaObject().Call("SetSplitOBB", bValue, strOBBName);
        }

        private int Call_GetErrorExtra() {
            return GetJavaObject().Call<int>("GetErrorExtra");
        }

        private void Call_SetUnityActivity() {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            GetJavaObject().Call("SetUnityActivity", jo);

            if (SystemInfo.graphicsMultiThreaded == true) {
                GL.IssuePluginEvent(EasyMovieTextureRender(), 0 + mAndroidMgrID * 10 + 7000);
            }
            else {
                Call_InitJniManager();
            }
        }

        private void Call_SetNotReady() {
            GetJavaObject().Call("SetNotReady");
        }

        private void Call_InitJniManager() {
            GetJavaObject().Call("InitJniManager");
        }

        private MediaPlayerState Call_GetStatus() {
            return (MediaPlayerState) GetJavaObject().Call<int>("GetStatus");
        }

        public TrackItem[] getAudioTrackInfo() {
            AndroidJavaObject obj = GetJavaObject().Call<AndroidJavaObject>("getAudioTrackInfo");

            return TrackItem.CreateFromAndroidJavaObject(obj);
        }

        public TrackItem[] getVideoTrackInfo() {
            return TrackItem.CreateFromAndroidJavaObject(GetJavaObject().Call<AndroidJavaObject>("getVideoTrackInfo"));
        }

        public TrackItem[] getSubtitleTrackInfo() {
            return TrackItem.CreateFromAndroidJavaObject(
                GetJavaObject().Call<AndroidJavaObject>("getSubtitleTrackInfo"));
        }

        public int getVideoSelectedTrack() {
            return GetJavaObject().Call<int>("getVideoSelectedTrack");
        }

        public int getAudioSelectedTrack() {
            return GetJavaObject().Call<int>("getAudioSelectedTrack");
        }

        public int getSubtitleSelectedTrack() {
            return GetJavaObject().Call<int>("getSubtitleSelectedTrack");
        }

        public void selectTrack(int stream) {
            GetJavaObject().Call("selectTrack", stream);
        }

        public IEnumerator DownloadStreamingVideoAndLoad(string strURL) {
            strURL = strURL.Trim();
            WWW www = new WWW(strURL);

            yield return www;

            if (string.IsNullOrEmpty(www.error)) {
                if (Directory.Exists(Application.persistentDataPath + "/Data") == false)
                    Directory.CreateDirectory(Application.persistentDataPath + "/Data");

                string write_path = Application.persistentDataPath + "/Data" +
                                    strURL.Substring(strURL.LastIndexOf("/"));

                File.WriteAllBytes(write_path, www.bytes);
                Load("file://" + write_path);
            }
            else {
                Debug.Log(www.error);
            }

            www.Dispose();
            www = null;
            Resources.UnloadUnusedAssets();
        }

        IEnumerator CopyStreamingAssetVideoAndLoad(string strURL) {
            strURL = strURL.Trim();
            string write_path = Application.persistentDataPath + "/" + strURL;

            if (File.Exists(write_path) == false) {
                WWW www = new WWW(Application.streamingAssetsPath + "/" + strURL);

                yield return www;

                if (string.IsNullOrEmpty(www.error)) {
                    File.WriteAllBytes(write_path, www.bytes);
                    Load("file://" + write_path);
                }
                else {
                    Debug.Log(www.error);
                }

                www.Dispose();
                www = null;
            }
            else {
                Load("file://" + write_path);
            }
        }

        public class TrackItem {

            public int Index { get; private set; }

            public string InfoInline { get; private set; }

            public TrackItem(int index, string infoInline) {
                Index = index;
                InfoInline = infoInline;
            }

            public override string ToString() {
                return string.Format("[TrackItem: Index={0}, InfoInline={1}]", Index, InfoInline);
            }

            public static TrackItem[] CreateFromAndroidJavaObject(AndroidJavaObject obj) {
                int size = obj.Call<int>("size");

                List<TrackItem> result = new List<TrackItem>();

                for (int i = 0; i < size; i++) {
                    AndroidJavaObject trackItem = obj.Call<AndroidJavaObject>("get", i);
                    int index = trackItem.Call<int>("getIndex");
                    string infoInline = trackItem.Call<string>("getInfoInline");
                    TrackItem item = new TrackItem(index, infoInline);
                    result.Add(item);
                }

                return result.ToArray();
            }

        }

    }

}