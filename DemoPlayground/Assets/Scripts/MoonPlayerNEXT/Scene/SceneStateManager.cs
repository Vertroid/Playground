using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoonXR.Player.Interfaces;
using MoonXR.Player.Interfaces.Scene;

namespace MoonXR.Player.Scene
{
    public class SceneStateHandler : ISceneStateHandler
    {
        public IScene CurrentScene
        {
            get => _currentScene;
            private set
            {
                var oldScene = CurrentScene;
                CurrentScene = value;
                OnCurrentSceneChanged?.Invoke(oldScene, value);
            }
        }

        public event Action<IScene, IScene> OnCurrentSceneChanged;

        private IScene _currentScene;
    }
}