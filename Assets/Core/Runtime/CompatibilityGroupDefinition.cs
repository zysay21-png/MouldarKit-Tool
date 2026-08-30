using UnityEngine;

/// <summary>
/// Serializable data model that groups modules that are considered compatible
/// for replacement or other kit-specific compatibility workflows.
/// </summary>
[System.Serializable]
public class CompatibilityGroupDefinition
{
    // Stable internal identifier referenced by ModuleDefinition.
    [SerializeField, HideInInspector]
    private string _groupId;

    // Human-readable group name shown in editor UI.
    [SerializeField]
    private string _displayName;

    public string GroupId => _groupId;
    public string DisplayName => _displayName;

    /// <summary>
    /// Creates a compatibility group with a persistent ID and display name.
    /// </summary>
    public CompatibilityGroupDefinition(string groupId, string displayName)
    {
        _groupId = groupId;
        _displayName = displayName;
    }
}
