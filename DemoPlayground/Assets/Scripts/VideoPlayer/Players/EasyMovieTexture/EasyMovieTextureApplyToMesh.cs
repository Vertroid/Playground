using UnityEngine;
using static MoonXR.Video.Players.EasyMovieTexture.RendererHelper;

namespace MoonXR.Video.Players.EasyMovieTexture
{
    public class EasyMovieTextureApplyToMesh : MonoBehaviour
    {
        [Header("Media Source")] [SerializeField]
        private EasyMovieTexturePlayer _media = null;

        public EasyMovieTexturePlayer Player
        {
            get { return _media; }
            set
            {
                if (_media != value)
                {
                    _media = value;
                    _isDirty = true;
                }
            }
        }

        [Tooltip("Default texture to display when the video texture is preparing")] [SerializeField]
        private Texture2D _defaultTexture;

        [Space(8f)] [Header("Renderer Target")] [SerializeField]
        private Renderer _mesh = null;

        public Renderer MeshRenderer
        {
            get { return _mesh; }
            set
            {
                if (_mesh != value)
                {
                    _mesh = value;
                    _isDirty = true;
                }
            }
        }

        [SerializeField] private string _texturePropertyName = "_MainTex";

        [SerializeField] private Vector2 _offset = Vector2.zero;

        [SerializeField] private Vector2 _scale = Vector2.one;

        private bool _isDirty = false;

        private Texture _lastTextureApplied;

        private int _propTexture;

        private static int _propStereo;

        private static int _propAlphaPack;

        private static int _propApplyGamma;

        private const string PropChromaTexName = "_ChromaTex";

        private static int _propChromaTex;

        private const string PropUseYpCbCrName = "_UseYpCbCr";

        private static int _propUseYpCbCr;

        public void ForceUpdate()
        {
            _isDirty = true;
            LateUpdate();
        }

        private void Awake()
        {
            if (_propStereo == 0 || _propAlphaPack == 0)
            {
                _propStereo = Shader.PropertyToID("Stereo");
                _propAlphaPack = Shader.PropertyToID("AlphaPack");
                _propApplyGamma = Shader.PropertyToID("_ApplyGamma");
            }

            if (_propChromaTex == 0)
            {
                _propChromaTex = Shader.PropertyToID(PropChromaTexName);
            }

            if (_propUseYpCbCr == 0)
            {
                _propUseYpCbCr = Shader.PropertyToID(PropUseYpCbCrName);
            }
        }

        // We do a LateUpdate() to allow for any changes in the texture that may have happened in Update()
        private void LateUpdate()
        {
            bool applied = false;

            // Try to apply texture from media
            if (_media != null && _media.VideoTexture != null)
            {
                Texture texture = _media.VideoTexture;

                if (texture != null)
                {
                    // Check for changing texture
                    if (texture != _lastTextureApplied)
                    {
                        _isDirty = true;
                    }

                    if (_isDirty)
                    {
                        ApplyMapping(texture, false, 0);
                    }

                    applied = true;
                }
            }

            // If the media didn't apply a texture, then try to apply the default texture
            if (!applied)
            {
                if (_defaultTexture != _lastTextureApplied)
                {
                    _isDirty = true;
                }

                if (_isDirty)
                {
                    ApplyMapping(_defaultTexture, false);
                }
            }
        }

        private void ApplyMapping(Texture texture, bool requiresYFlip, int plane = 0)
        {
            if (_mesh != null)
            {
                _isDirty = false;

                Material[] meshMaterials = _mesh.materials;

                if (meshMaterials != null)
                {
                    for (int i = 0; i < meshMaterials.Length; i++)
                    {
                        Material mat = meshMaterials[i];

                        if (mat != null)
                        {
                            if (plane == 0)
                            {
                                mat.SetTexture(_propTexture, texture);

                                _lastTextureApplied = texture;

                                if (texture != null)
                                {
                                    if (requiresYFlip)
                                    {
                                        mat.SetTextureScale(_propTexture, new Vector2(_scale.x, -_scale.y));
                                        mat.SetTextureOffset(_propTexture, Vector2.up + _offset);
                                    }
                                    else
                                    {
                                        mat.SetTextureScale(_propTexture, _scale);
                                        mat.SetTextureOffset(_propTexture, _offset);
                                    }
                                }
                            }
                            else if (plane == 1)
                            {
                                if (mat.HasProperty(_propUseYpCbCr) && mat.HasProperty(_propChromaTex))
                                {
                                    mat.EnableKeyword("USE_YPCBCR");
                                    mat.SetTexture(_propChromaTex, texture);

                                    if (requiresYFlip)
                                    {
                                        mat.SetTextureScale(_propChromaTex, new Vector2(_scale.x, -_scale.y));
                                        mat.SetTextureOffset(_propChromaTex, Vector2.up + _offset);
                                    }
                                    else
                                    {
                                        mat.SetTextureScale(_propChromaTex, _scale);
                                        mat.SetTextureOffset(_propChromaTex, _offset);
                                    }
                                }
                            }

                            if (_media != null)
                            {
                                // Apply changes for stereo videos
                                if (mat.HasProperty(_propStereo))
                                {
                                    RendererHelper.SetupStereoMaterial(mat, StereoPacking.None,
                                    false);
                                }

                                // Apply changes for alpha videos
                                if (mat.HasProperty(_propAlphaPack))
                                {
                                    RendererHelper.SetupAlphaPackedMaterial(mat, AlphaPacking.None);
                                }
#if UNITY_PLATFORM_SUPPORTS_LINEAR
								// Apply gamma
								if (mat.HasProperty(_propApplyGamma))
								{
									Helper.SetupGammaMaterial(mat, false);
								}
#else
                                _propApplyGamma |= 0;
#endif
                            }
                        }
                    }
                }
            }
        }

        private void OnEnable()
        {
            if (_mesh == null)
            {
                _mesh = this.GetComponent<MeshRenderer>();

                if (_mesh == null)
                {
                }
            }

            _propTexture = Shader.PropertyToID(_texturePropertyName);

            _isDirty = true;

            if (_mesh != null)
            {
                LateUpdate();
            }
        }

        private void OnDisable()
        {
            ApplyMapping(_defaultTexture, false);
        }
    }
}