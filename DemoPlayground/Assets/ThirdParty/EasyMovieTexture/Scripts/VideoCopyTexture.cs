using UnityEngine;

namespace EasyMovieTexture.Scripts {

    public class VideoCopyTexture : MonoBehaviour {

        public MediaPlayerCtrl m_srcVideo;

        void Start() {
        }

        // Update is called once per frame
        void Update() {
            if (m_srcVideo != null) {
                if (m_srcVideo.GetCurrentState() == MediaPlayerCtrl.MediaPlayerState.PAUSED ||
                    m_srcVideo.GetCurrentState() == MediaPlayerCtrl.MediaPlayerState.PLAYING) {
                    if (m_srcVideo.GetVideoTexture() != null) {
                        if (transform.GetComponent<MeshRenderer>().material.mainTexture !=
                            m_srcVideo.GetVideoTexture()) {
                            transform.GetComponent<MeshRenderer>().material.mainTexture = m_srcVideo.GetVideoTexture();
                        }
                    }
                }
            }
        }

    }

}