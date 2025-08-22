using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;

namespace MoonXR.Player.Scene
{
    [CreateAssetMenu(fileName = "SceneConfig", menuName = "MoonXR/Scene Configuration")]
    public class SceneConfiguration : ScriptableObject
    {
        [System.Serializable]
        public class SceneMapping
        {
            public SceneSky SceneType;
            public string ResourcePath; // Addressable ID or
            public string displayName;
            public bool IsSkyboxScene;
        }
        [SerializeField] private SceneMapping[] sceneMappings;
        private Dictionary<SceneSky, string> _sceneDict;

        public Dictionary<SceneSky, string> GetSceneMapping()
        {
            if (_sceneDict == null)
            {
                _sceneDict = new Dictionary<SceneSky, string>();
                foreach (var mapping in sceneMappings)
                {
                    _sceneDict[mapping.SceneType] = mapping.ResourcePath;
                }
            }
            return _sceneDict;
        }

        public string GetResourcePath(SceneSky sceneType)
        {
            return GetSceneMapping().TryGetValue(sceneType, out string id) ? id : null;
        }

        public bool IsValidScene(SceneSky sceneType)
        {
            return GetSceneMapping().ContainsKey(sceneType);
        }

        public bool IsSkyboxScene(SceneSky scene)
        {
            var mapping = Array.Find(sceneMappings, m => m.SceneType == scene);
            return mapping?.IsSkyboxScene ?? false;
        }
    }
}
