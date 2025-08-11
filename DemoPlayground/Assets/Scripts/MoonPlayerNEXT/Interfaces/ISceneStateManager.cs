namespace MoonXR.Player.Interfaces
{
    public interface ISceneStateManager
    {
        SceneSky GetCurrentSceneType();
        bool IsPassthrough();
        void SaveSceneSettings(SceneSky sceneType);
        SceneSky GetSceneTypeSetting();
    }
}