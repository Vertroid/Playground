using System;
using MoonXR.Player.Data;
using MoonXR.Player.Interfaces.Scene;

namespace MoonXR.Player.Scene.Transitions
{
    public class StandardSceneTransitionStrategy : ISceneTransitionStrategy
    {
        public bool CanHandle(SceneSky from, SceneSky to)
        {
            return to != SceneSky.Passthrough &&
                    !IsSameResource(from, to);
        }

        public void Execute(SceneTransitionContext context, Action<SceneData> onLoadSuccess)
        {
            context.Current?.Unload();
            // StateManager.ClearHomeScene();

            context.Loader.LoadScene(context.To, (newScene) =>
            {
                // 设置当前场景
                // Load
                // onComplete
            }, (err) =>
            {

            });
        }

        private bool IsSameResource(SceneSky from, SceneSky to)
        {
            return false;
        }
    }
}