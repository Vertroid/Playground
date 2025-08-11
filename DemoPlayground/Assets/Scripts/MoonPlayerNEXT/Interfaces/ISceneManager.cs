
namespace MoonXR.Player.Interfaces
{
    public interface ISceneManager
    { 
        IScene CurrentScene { get; }
        SceneSky CurrentSceneType { get; }
        event Action<UISceneData> OnSceneLoaded;
        void LoadScene(SceneSky sceneType, Action<UISceneData> onSuccess = null, Action<string> onError = null);
        void UnloadCurrentScene();
    }
}