
using System.Collections;
using System.Collections.Generic;

namespace MoonXR.Player.Interfaces.Scene
{
    public interface ISceneConfigManager
    {
        string GetResourcePath(SceneSky sceneType);
        bool IsSkyboxScene(SceneSky sceneType);
        IEnumerable<SceneSky> GetAvailableScenes();
    }
}