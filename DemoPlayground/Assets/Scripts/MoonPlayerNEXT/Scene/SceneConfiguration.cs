using UnityEngine;

[System.Serializable]
public class SceneMapping
{
    public SceneSky sceneType;
    public string addressableId;
    public string displayName;
    public Sprite previewIcon;
}

[CreateAssetMenu(fileName = "SceneConfig", menuName = "MoonXR/Scene Config")]
public class SceneConfigAsset : ScriptableObject
{
    [SerializeField] private SceneMapping[] sceneMappings;
    
    private Dictionary<SceneSky, string> _sceneDict;
    
    public Dictionary<SceneSky, string> GetSceneMapping()
    {
        if (_sceneDict == null)
        {
            _sceneDict = new Dictionary<SceneSky, string>();
            foreach (var mapping in sceneMappings)
            {
                _sceneDict[mapping.sceneType] = mapping.addressableId;
            }
        }
        return _sceneDict;
    }
    
    public string GetAddressableId(SceneSky sceneType)
    {
        return GetSceneMapping().TryGetValue(sceneType, out string id) ? id : null;
    }
    
    public bool IsValidScene(SceneSky sceneType)
    {
        return GetSceneMapping().ContainsKey(sceneType);
    }
}