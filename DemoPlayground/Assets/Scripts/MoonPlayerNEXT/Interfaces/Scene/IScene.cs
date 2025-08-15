using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoonXR.Player.Interfaces.Scene
{
    public interface IScene
    {
        void Load();
        void Unload();
    }
}