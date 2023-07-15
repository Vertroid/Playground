using UnityEngine;
using System.Collections.Generic;
using MoonXR.Video.Players.EasyMovieTexture;

namespace MoonXR.Video
{
    public class VideoRenderer : MonoBehaviour
    {
        private const string TAG = "[RENDER]";

        private VideoPlayer mVideoPlayer;

        // AVPro Video Renderer
        [SerializeField] private MeshRenderer quadMeshAVP;
        [SerializeField] private MeshRenderer sphereMeshAVP;
        [SerializeField] private MeshRenderer domeMeshAVP;
        [SerializeField] private MeshRenderer cubeHMeshAVP;     //RA-ps: H=horizontal
        [SerializeField] private MeshRenderer cubeVMeshAVP;     //RA-ps: V=vertical
        [SerializeField] private MeshRenderer fish180DMeshAVP;  //RA-ps: D=default
        [SerializeField] private MeshRenderer fish190DMeshAVP;
        [SerializeField] private MeshRenderer fish200DMeshAVP;

        // Media property
        private int mVideoWidth;
        private int mVideoHeight;
        private float mVideoRatio = 1f;

        // For universe access target mesh
        private MeshRenderer mQuadMesh;
        private MeshRenderer mSphereMesh;
        private MeshRenderer mDomeMesh;
        private MeshRenderer mCubeHMesh;
        private MeshRenderer mCubeVMesh;
        private MeshRenderer mFish180DMesh;
        private MeshRenderer mFish190DMesh;
        private MeshRenderer mFish200DMesh;
        private List<MeshRenderer> mRenderMesh;

        public enum RenderMode
        {
            RenderQuad,
            RenderSphere,
            RenderDome,
            RenderCubeH,
            RenderCubeV,
            RenderFish180D,
            RenderFish190D,
            RenderFish200D,
        }

        public enum StereoMode
        {
            StereoMono,
            StereoLeftRight,
            StereoRightLeft,
            StereoTopBottom,
            StereoBottomTop,
            StereoHalfLeft,
            StereoHalfRight,
            StereoHalfTop,
            StereoHalfBottom,
        }

        private RenderMode mRenderMode = RenderMode.RenderQuad;
        private StereoMode mStereoMode = StereoMode.StereoMono;
        private Material[] mMeshMaterials;
        private int mPropStereoAVP;
        private int mPropFlipX;

        // private JSONObject getStatusJson() {
        //     JSONObject json = new JSONObject {
        //         ["RenderMode"] = mRenderMode.ToString(),
        //         ["StereoMode"] = mStereoMode.ToString(),
        //         ["MeshMaterials"] = mMeshMaterials?.Length,
        //         ["PropFlipX"] = mPropFlipX.ToString(),
        //         ["PropStereoAVP"] = mPropStereoAVP.ToString(),
        //         ["VideoWidth"] = mVideoWidth.ToString(),
        //         ["VideoHeight"] = mVideoHeight.ToString(),
        //         ["VideoPlayer"] = mVideoPlayer?.ToString(),
        //         ["QuadMesh"] = mQuadMesh?.ToString(),
        //         ["SphereMesh"] = mSphereMesh?.ToString(),
        //         ["DomeMesh"] = mDomeMesh?.ToString()
        //     };
        //     return json;
        // }

        public void SetVideoPlayer(VideoPlayer player)
        {
            if (player == mVideoPlayer || player == null)
            {
                return;
            }

            mVideoPlayer = player;

            mPropStereoAVP = Shader.PropertyToID("Stereo");
            mPropFlipX = Shader.PropertyToID("_FlipX");

            mQuadMesh = quadMeshAVP;
            mSphereMesh = sphereMeshAVP;
            mDomeMesh = domeMeshAVP;
            mCubeHMesh = cubeHMeshAVP;
            mCubeVMesh = cubeVMeshAVP;
            mFish180DMesh = fish180DMeshAVP;
            mFish190DMesh = fish190DMeshAVP;
            mFish200DMesh = fish200DMeshAVP;

            mRenderMesh = new List<MeshRenderer>() { mQuadMesh, mSphereMesh, mDomeMesh, mCubeHMesh, mCubeVMesh, mFish180DMesh, mFish190DMesh, mFish200DMesh };
            //与MoonXR.Video.VideoRenderer.RenderMode次序一致

            // Default to Quad and Mono
            SetRenderMode(RenderMode.RenderQuad);
            SetStereoMode(StereoMode.StereoMono);
        }

        private bool isInit()
        {
            return mVideoPlayer != null;
        }

        public void SetRenderMode(RenderMode mode)
        {
            if (!isInit())
            {
                return;
            }

            mRenderMode = mode;
            ResetRenderMesh();

            for (int i = 0; i < mRenderMesh.Count; i++)
            {
                if (i == (int)mode)
                {
                    mVideoPlayer.SetMeshRenderer(mRenderMesh[i]);
                    mRenderMesh[i].gameObject.SetActive(true);
                }
            }
        }

        // TODO, maybe we can unify masterial for all plugin
        public void SetStereoMode(StereoMode mode)
        {
            if (!isInit())
            {
                return;
            }

            mStereoMode = mode;

            for (int i = 0; i < mRenderMesh.Count; i++)
            {
                if (i == (int)mRenderMode)
                {
                    mMeshMaterials = mRenderMesh[i].materials;
                }
            }

            if (mMeshMaterials != null)
            {
                for (int i = 0; i < mMeshMaterials.Length; i++)
                {
                    Material mat = mMeshMaterials[i];
                    StereoPacking stereo = StereoPacking.None;

                    switch (mode)
                    {
                        case StereoMode.StereoMono:
                            stereo = StereoPacking.None;
                            break;
                        case StereoMode.StereoLeftRight:
                            stereo = StereoPacking.LeftRight;
                            break;
                        case StereoMode.StereoRightLeft:
                            stereo = StereoPacking.RightLeft;
                            break;
                        case StereoMode.StereoTopBottom:
                            stereo = StereoPacking.TopBottom;
                            break;
                        case StereoMode.StereoBottomTop:
                            stereo = StereoPacking.BottomTop;
                            break;
                        case StereoMode.StereoHalfLeft:
                            stereo = StereoPacking.HalfLeft;
                            break;
                        case StereoMode.StereoHalfRight:
                            stereo = StereoPacking.HalfRight;
                            break;
                        case StereoMode.StereoHalfTop:
                            stereo = StereoPacking.HalfTop;
                            break;
                        case StereoMode.StereoHalfBottom:
                            stereo = StereoPacking.HalfBottom;
                            break;
                    }

                    // Apply changes for stereo videos
                    if (mat.HasProperty(mPropStereoAVP))
                    {
                        mVideoPlayer.ActualPlayer.SetStereoMode(mat, stereo, false);
                    }
                    else
                    {
                        // Debug.LogError(getStatusJson());
                    }
                }
            }
            else
            {
                // Debug.LogError(getStatusJson());
            }

            if (mRenderMode == RenderMode.RenderQuad)
            {
                AdjustScreenSize();
            }
        }

        public void SetVideoSize(int width, int height)
        {
            if (!isInit())
            {
                return;
            }

            mVideoWidth = width;
            mVideoHeight = height;

            mVideoRatio = (float)width / (float)height;
        }

        public void SetRenderBrightness(float brightness)
        {
            foreach (MeshRenderer mr in mRenderMesh)
                mr.material.SetFloat("_Brightness", brightness);

        }

        public float GetRenderBrightness()
        {
            return mQuadMesh.material.GetFloat("_Brightness");
        }

        public void SetRenderFlipX(bool flip)
        {
            if (!mQuadMesh.material.HasProperty(mPropFlipX))
                return;


            if (flip)
            {
                foreach(MeshRenderer mr in mRenderMesh)
                    mr.material.EnableKeyword("FLIP_X");
            }
            else
            {
                foreach (MeshRenderer mr in mRenderMesh)
                    mr.material.DisableKeyword("FLIP_X");
            }
        }

        public void AdjustScreenSize()
        {
            if (!isInit())
            {
                return;
            }

            if (mVideoWidth == 0 || mVideoHeight == 0)
                return;

            // For L-R SBS 3D movies, in practice, both sides' widths are
            // divided by 2 (the video width is the original movie width).
            // There's no need to divide scaleX by 2
            float scaleY = mQuadMesh.transform.localScale.y;
            float scaleX = mVideoRatio * scaleY;
            float scaleZ = mQuadMesh.transform.localScale.z;

            if ((mStereoMode == StereoMode.StereoLeftRight || mStereoMode == StereoMode.StereoLeftRight ||
                 mStereoMode == StereoMode.StereoHalfLeft || mStereoMode == StereoMode.StereoHalfRight) &&
                mVideoWidth / (float) mVideoHeight > 3.0)
            {
                // Some L-R SBS 3D Movies are full width at both sides,
                // results in 32 : 10 or 32 : 9 ratio (both > 3.0)
                // So if width : height > 3 : 1, divide width by 2
                scaleX *= 0.5f;
            }

            //mQuadMesh.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
            //mDomeMesh.transform.localScale = new Vector3(scaleX, scaleY, scaleX);
        }

        private void ResetRenderMesh()
        {
            if (!isInit())
            {
                return;
            }

            foreach (MeshRenderer mr in mRenderMesh)
            {
                if(mr.gameObject.activeSelf)
                    mr.gameObject.SetActive(false);
            }
        }
    }
}