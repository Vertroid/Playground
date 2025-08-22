using System;
using MoonXR.Player.Interfaces.Scene;

namespace MoonXR.Player.Interfaces
{
    public interface ISceneStateHandler
    {
        // Properties
        IScene CurrentScene { get; }
        IScene CurrentSkybox { get; }
        SceneSky GetCurrentSceneType();
        bool IsPassthrough();
        void SaveSceneSettings(SceneSky sceneType);
        // Events
        event Action<IScene, IScene> OnCurrentSceneChanged;
    }
}