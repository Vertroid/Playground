using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoonXR.Player.Interfaces;
using MoonXR.Player.Interfaces.Scene;
using UnityEngine;

namespace MoonXR.Player.Scene
{
    public class SceneLoader : ISceneLoader
    {
        private readonly ISceneConfigManager _configManager;
        private readonly Dictionary<SceneSky, float> _loadProgress;
        private readonly HashSet<SceneSky> _preloadedScenes;

        public SceneLoader(ISceneConfigManager configManager)
        {
            _preloadedScenes = new();
            _loadProgress = new Dictionary<SceneSky, float>();
        }

        public void LoadScene(SceneSky type, Action<IScene> onComplete, Action<string> onError)
        {
            try
            {
                _loadProgress[type] = 0f;

                // Load scene from pool or addressables.
            }
            catch (Exception e)
            {
                onError?.Invoke(e.Message);
            }
        }

        private IScene CreateSceneComponent(GameObject sceneObject, SceneSky sceneType)
        {
            throw new NotImplementedException();
        }

        public void PreloadScene(SceneSky type, Action onComplete = null)
        {
            if (_preloadedScenes.Contains(type))
            {
                onComplete?.Invoke();
                return;
            }

            if (!ShouldPreload(type))
            {
                onComplete?.Invoke();
                return;
            }

            var resourcePath = _configManager.GetResourcePath(type);
            if (string.IsNullOrEmpty(resourcePath))
            {
                onComplete?.Invoke();
                return;
            }

            // Load addressable resource
        }

        public void UnloadScene(IScene scene)
        {
            if (scene == null)
                return;

            try
            {
                scene.Unload();

                if (ShouldDestroyOnUnload(scene))
                {
                    // Destroy
                }
            }
            catch (Exception e)
            { 
                
            }
        }
    }
}