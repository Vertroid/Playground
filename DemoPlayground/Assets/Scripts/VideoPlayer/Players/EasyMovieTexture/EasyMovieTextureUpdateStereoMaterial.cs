using UnityEngine;

namespace MoonXR.Video.Players.EasyMovieTexture
{
    public class EasyMovieTextureUpdateStereoMaterial : MonoBehaviour
    {
        [Header("Stereo camera")] public Camera _camera;

        [Header("Rendering elements")] public MeshRenderer _renderer;

        public Material _material;

        private int _cameraPositionId;

        private int _viewMatrixId;

        void Awake()
        {
            _cameraPositionId = Shader.PropertyToID("_cameraPosition");
            _viewMatrixId = Shader.PropertyToID("_ViewMatrix");

            if (_camera == null)
            {}
        }

        private void SetupMaterial(Material m, Camera camera)
        {
            m.SetVector(_cameraPositionId, camera.transform.position);
            m.SetMatrix(_viewMatrixId, camera.worldToCameraMatrix.transpose);
        }

        // We do a LateUpdate() to allow for any changes in the camera position that may have happened in Update()
        void LateUpdate()
        {
            Camera camera = _camera;

            if (camera == null)
            {
                camera = Camera.main;
            }

            if (_renderer == null && _material == null)
            {
                _renderer = gameObject.GetComponent<MeshRenderer>();
            }

            if (camera != null)
            {
                if (_renderer != null)
                {
                    SetupMaterial(_renderer.material, camera);
                }

                if (_material != null)
                {
                    SetupMaterial(_material, camera);
                }
            }
        }
    }
}