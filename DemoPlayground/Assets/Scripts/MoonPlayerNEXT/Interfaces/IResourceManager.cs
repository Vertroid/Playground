using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoonXR.Player.Interfaces.Common
{
    public interface IResourceManager
    {
        void LoadResourceAsync<T>(string resourceKey, Action<T> onComplete, Action<string> onError);
    }
}