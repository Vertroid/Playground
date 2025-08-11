namespace MoonXR.Player.Interfaces
{
    public interface ISceneLoader
    {
        void LoadScene(SceneSky sceneType, Action<IScene> onSuccess, Action<string> onError);
        void PreloadScene(SceneSky sceneType);
        void UnloadScene(IScene scene);
    }
}