using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoonXR.Player.Interfaces.Common;

namespace DemoPlayground.Assets.Scripts.MoonPlayerNEXT.Core
{
    public class ResourceManager : IResourceManager
    {
        public void LoadResourceAsync<T>(string resourceKey, Action<T> onComplete, Action<string> onError)
        { 
            
        }
    }
}