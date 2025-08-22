using System;
using MoonXR.Player.Data;
using MoonXR.Player.Interfaces;
using MoonXR.Player.Interfaces.Scene;
using MoonXR.Player.Scene.Transitions;

namespace MoonXR.Player.Scene
{
    public class SceneTransitionManager : ISceneTransitionHandler
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly ISceneStateHandler _stateManager;

        private readonly TransitionStrategyFactory _strategyFactory;

        public void TransitionToScene(SceneSky to, Action<SceneData> onComplete)
        {

        }

        private void ExecuteTransition(SceneSky from, SceneSky to, IScene current, Action<SceneData> onComplete)
        {
            var strategy = _strategyFactory.GetStrategy(from, to);
            var context = new SceneTransitionContext
            {
                From = from,
                To = to,
                Current = current,
                Loader = _sceneLoader,
                // OnComplete = onComplete
            };

            strategy.Execute(context, (data) =>
            {
                // if needManuallyFade
            });
        }
    }
}