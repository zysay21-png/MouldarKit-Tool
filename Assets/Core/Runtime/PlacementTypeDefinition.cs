using UnityEngine;

/// <summary>
/// Serializable definition used to classify modules by placement behavior.
/// The stable ID is stored on ModuleDefinition while the display name is
/// exposed to artists in the kit editor.
/// </summary>
[System.Serializable]
public class PlacementTypeDefinition
{
    [SerializeField, HideInInspector]
    private string _placementTypeId;

    [SerializeField]
    private string _displayName;

    public string PlacementTypeId => _placementTypeId;
    public string DisplayName => _displayName;

    /// <summary>
    /// Creates a placement type with a persistent ID and display name.
    /// </summary>
    public PlacementTypeDefinition(string placementTypeId, string displayName)
    {
        _placementTypeId = placementTypeId;
        _displayName = displayName;
    }
}
