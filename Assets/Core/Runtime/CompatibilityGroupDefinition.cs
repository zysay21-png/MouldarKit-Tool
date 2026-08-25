using UnityEngine;

[System.Serializable]
public class CompatibilityGroupDefinition
{
    [SerializeField, HideInInspector]
    private string _groupId;

    [SerializeField]
    private string _displayName;

    public string GroupId => _groupId;
    public string DisplayName => _displayName;

    public CompatibilityGroupDefinition(string groupId, string displayName)
    {
        _groupId = groupId;
        _displayName = displayName;
    }
}
