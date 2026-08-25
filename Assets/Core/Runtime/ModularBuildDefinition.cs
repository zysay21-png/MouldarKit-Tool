using UnityEngine;

[CreateAssetMenu(
    fileName = "New Modular Build",
    menuName = "Modular Kit Builder/Build Definition"
)]
public class ModularBuildDefinition : ScriptableObject
{
    [SerializeField] private string _buildId;
    [SerializeField] private string _displayName;
    [SerializeField] private ModularKitDefinition _kit;


    public string BuildId => _buildId;
    public string DisplayName => _displayName;
    public ModularKitDefinition Kit => _kit;


    public void Initialize(string buildId, string displayName, ModularKitDefinition kit)
    {
        _buildId = buildId;
        _displayName = displayName;
        _kit = kit;
    }
}