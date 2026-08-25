using UnityEngine;

[System.Serializable]
public class PlacementTypeDefinition
{
    [SerializeField, HideInInspector]
    private string _placementTypeId;

    [SerializeField]
    private string _displayName;

    public string PlacementTypeId => _placementTypeId;
    public string DisplayName => _displayName;

    public PlacementTypeDefinition(string placementTypeId, string displayName)
    {
        _placementTypeId = placementTypeId;
        _displayName = displayName;
    }
}
