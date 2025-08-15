using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoonXR.Player.Interfaces;
using MoonXR.Player.Interfaces.Scene;

namespace MoonXR.Player.Scene.Transitions
{
    public class SceneTransitionContext
    {
        public SceneSky From { get; set; }
        public SceneSky To { get; set; }
        public IScene Current { get; set; }
        public bool NeedFade { get; set; }
        public ISceneStateManager StateManager { get; set; }
        public ISceneLoader Loader { get; set; }
    }
}