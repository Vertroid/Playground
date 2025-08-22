using System;
using MoonXR.Player.Interfaces;
using MoonXR.Player.Data;
using MoonXR.Player.Interfaces.Scene;

namespace MoonXR.Player.Scene
{
    public class SceneManager
    {
        private ISceneConfigManager _configManager;
        private ISceneStateHandler _stateManager;
        private ISceneTransitionHandler _transitionManager;
        private ISceneLoader _sceneLoader;

        public event Action<SceneData> OnSceneLoaded;

        public void LoadScene(SceneSky sceneType, Action<SceneData> onSuccess = null, Action<string> onError = null)
        {
            throw new NotImplementedException();
        }

        public void UnloadCurrentScene()
        {
            throw new NotImplementedException();
        }

    }
}