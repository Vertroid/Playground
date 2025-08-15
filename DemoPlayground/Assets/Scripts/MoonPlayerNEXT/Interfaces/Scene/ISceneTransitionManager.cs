using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoonXR.Player.Data;

namespace MoonXR.Player.Interfaces.Scene
{
    public interface ISceneTransitionManager
    {
        public void TransitionToScene(SceneSky to, Action<SceneData> onComplete);
    }
}