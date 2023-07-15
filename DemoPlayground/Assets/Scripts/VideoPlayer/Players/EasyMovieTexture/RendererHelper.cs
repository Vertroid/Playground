using UnityEngine;

namespace MoonXR.Video.Players.EasyMovieTexture
{
    public enum StereoPacking
    {
        None,
        TopBottom,
        BottomTop,
        LeftRight,
        RightLeft,
        HalfLeft,
        HalfRight,
        HalfTop,
        HalfBottom,
        CustomUV,
    }

    public enum AlphaPacking
    {
        None,
        TopBottom,
        LeftRight
    }

    public class RendererHelper
    {
        public static void SetupGammaMaterial(Material material, bool playerSupportsLinear)
        {
#if UNITY_PLATFORM_SUPPORTS_LINEAR
            if (QualitySettings.activeColorSpace == ColorSpace.Linear && !playerSupportsLinear) {
                material.EnableKeyword("APPLY_GAMMA");
            }
            else {
                material.DisableKeyword("APPLY_GAMMA");
            }
#endif
        }

        // @usunyu modified, support Right_Left, Bottom_Top, Half_Left, Half_Right, Half_Top, Half_Bottom
        public static void SetupStereoMaterial(Material material, StereoPacking packing, bool displayDebugTinting)
        {
            material.DisableKeyword("STEREO_CUSTOM_UV");
            material.DisableKeyword("STEREO_TOP_BOTTOM");
            material.DisableKeyword("STEREO_BOTTOM_TOP");
            material.DisableKeyword("STEREO_LEFT_RIGHT");
            material.DisableKeyword("STEREO_RIGHT_LEFT");
            material.DisableKeyword("STEREO_HALF_LEFT");
            material.DisableKeyword("STEREO_HALF_RIGHT");
            material.DisableKeyword("STEREO_HALF_TOP");
            material.DisableKeyword("STEREO_HALF_BOTTOM");
            material.DisableKeyword("MONOSCOPIC");

            // Enable the required mode
            switch (packing)
            {
                case StereoPacking.None:
                    break;
                case StereoPacking.TopBottom:
                    material.EnableKeyword("STEREO_TOP_BOTTOM");
                    break;
                case StereoPacking.BottomTop:
                    material.EnableKeyword("STEREO_BOTTOM_TOP");
                    break;
                case StereoPacking.LeftRight:
                    material.EnableKeyword("STEREO_LEFT_RIGHT");
                    break;
                case StereoPacking.RightLeft:
                    material.EnableKeyword("STEREO_RIGHT_LEFT");
                    break;
                case StereoPacking.HalfLeft:
                    material.EnableKeyword("STEREO_HALF_LEFT");
                    break;
                case StereoPacking.HalfRight:
                    material.EnableKeyword("STEREO_HALF_RIGHT");
                    break;
                case StereoPacking.HalfTop:
                    material.EnableKeyword("STEREO_HALF_TOP");
                    break;
                case StereoPacking.HalfBottom:
                    material.EnableKeyword("STEREO_HALF_BOTTOM");
                    break;
                case StereoPacking.CustomUV:
                    material.EnableKeyword("STEREO_CUSTOM_UV");
                    break;
            }

            if (displayDebugTinting)
            {
                material.EnableKeyword("STEREO_DEBUG");
            }
            else
            {
                material.DisableKeyword("STEREO_DEBUG");
            }
        }

        public static void SetupAlphaPackedMaterial(Material material, AlphaPacking packing)
        {
            material.DisableKeyword("ALPHAPACK_TOP_BOTTOM");
            material.DisableKeyword("ALPHAPACK_LEFT_RIGHT");
            material.DisableKeyword("ALPHAPACK_NONE");

            // Enable the required mode
            switch (packing)
            {
                case AlphaPacking.None:
                    break;
                case AlphaPacking.TopBottom:
                    material.EnableKeyword("ALPHAPACK_TOP_BOTTOM");
                    break;
                case AlphaPacking.LeftRight:
                    material.EnableKeyword("ALPHAPACK_LEFT_RIGHT");
                    break;
            }
        }
    }
}