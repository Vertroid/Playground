using UnityEngine;

namespace EasyMovieTexture.Scripts {

    public class SphereMirror : MonoBehaviour {

        public bool IsDefaultMirror;

        Vector2[] defaultUVs;

        Vector2[] mirrorUVs;

        private void Awake() {
            Vector2[] vec2UVs = transform.GetComponent<MeshFilter>().mesh.uv;
            defaultUVs = new Vector2[vec2UVs.Length];
            mirrorUVs = new Vector2[vec2UVs.Length];

            if (!IsDefaultMirror) {
                for (int i = 0; i < vec2UVs.Length; i++) {
                    defaultUVs[i] = new Vector2(vec2UVs[i].x, vec2UVs[i].y);
                    mirrorUVs[i] = new Vector2(1 - vec2UVs[i].x, vec2UVs[i].y);
                }
            }
            else {
                for (int i = 0; i < vec2UVs.Length; i++) {
                    mirrorUVs[i] = new Vector2(vec2UVs[i].x, vec2UVs[i].y);
                    defaultUVs[i] = new Vector2(1 - vec2UVs[i].x, vec2UVs[i].y);
                }
            }
        }

        public void Mirror(bool isMirror) {
            if (isMirror) {
                gameObject.GetComponent<MeshFilter>().mesh.uv = mirrorUVs;
            }
            else {
                gameObject.GetComponent<MeshFilter>().mesh.uv = defaultUVs;
            }
        }

    }

}