using System;
using System.Collections.Generic;
using MoonXR.Player.Interfaces.Scene;

namespace MoonXR.Player.Scene.Transitions
{
    public class TransitionStrategyFactory
    {
        private readonly List<ISceneTransitionStrategy> _strategies;
        private readonly ISceneTransitionStrategy _defaultStrategy;

        public TransitionStrategyFactory(Dictionary<SceneSky, string> sceneMapping)
        {
            _strategies = new List<ISceneTransitionStrategy>
            {
                // 优先添加条件严格的策略
                new StandardSceneTransitionStrategy()
            };

            _defaultStrategy = new StandardSceneTransitionStrategy();
        }

        public ISceneTransitionStrategy GetStrategy(SceneSky from, SceneSky to)
        {
            foreach (var s in _strategies)
            {
                if (s.CanHandle(from, to))
                {
                    return s;
                }
            }

            return _defaultStrategy;
        }

        public void RegisterStrategy(ISceneTransitionStrategy strategy)
        {
            _strategies.Insert(0, strategy);
        }
    }
}