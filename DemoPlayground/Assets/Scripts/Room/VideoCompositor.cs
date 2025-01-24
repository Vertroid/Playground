
using UnityEngine;
using UnityEngine.Video;

namespace NNCam 
{
    public sealed class VideoCompositor : MonoBehaviour
    {
        [SerializeField] Texture2D _background = null;
        [SerializeField, Range(0.01f, 0.99f)] float _threshold = .5f;
        [SerializeField] ResourceSet _resources = null;
        [SerializeField] RenderTexture _renderTexture = null;
        [SerializeField] Material _material;

        SegmentationFilter _filter;

        void Start()
        {
            _filter = new SegmentationFilter(_resources);
        }

        void OnDestroy()
        {
            _filter.Dispose();
        }

        void Update()
        {
            _filter.ProcessImage(_renderTexture);

            _material.SetTexture("_Background", _background);
            _material.SetTexture("_CameraFeed", _renderTexture);
            _material.SetTexture("_Mask", _filter.MaskTexture);
            _material.SetFloat("_Threshold", _threshold);
        }

    }

} // namespace NNCam
