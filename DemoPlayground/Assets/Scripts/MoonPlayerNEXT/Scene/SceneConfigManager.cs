
using System.Collections.Generic;
using MoonXR.Player.Interfaces.Scene;
using UnityEngine;

namespace MoonXR.Player.Scene
{
    public class SceneConfigManager : ISceneConfigManager
    {
        private Dictionary<SceneSky, string> _sceneMappings;
        private HashSet<SceneSky> _skyboxScenes;
        private SceneConfiguration _config;

        public IEnumerable<SceneSky> GetAvailableScenes()
        {
            throw new System.NotImplementedException();
        }

        public string GetResourcePath(SceneSky sceneType)
        {
            throw new System.NotImplementedException();
        }

        public bool IsSkyboxScene(SceneSky sceneType)
        {
            throw new System.NotImplementedException();
        }

        private void LoadConfig()
        {
            _config = Resources.Load<SceneConfiguration>("Config/SceneConfig");
        }

        private void LoadDefaultConfig()
        { 

        }
    }
}