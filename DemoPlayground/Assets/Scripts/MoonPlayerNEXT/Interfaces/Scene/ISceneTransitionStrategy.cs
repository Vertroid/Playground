using System;
using MoonXR.Player.Data;
using MoonXR.Player.Scene.Transitions;

namespace MoonXR.Player.Interfaces.Scene
{
    public interface ISceneTransitionStrategy
    {
        bool CanHandle(SceneSky from, SceneSky to);
        void Execute(SceneTransitionContext context, Action<SceneData> onLoadSuccess);
    }
}