
using System;
using MoonXR.Player.Data;
using MoonXR.Player.Interfaces.Scene;

namespace MoonXR.Player.Interfaces
{
    public interface ISceneManager
    {
        event Action<SceneData> OnSceneLoaded;
        void LoadScene(SceneSky sceneType, Action<SceneData> onSuccess = null, Action<string> onError = null);
        void UnloadCurrentScene();
    }
}