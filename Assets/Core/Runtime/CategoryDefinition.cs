using UnityEngine;

[System.Serializable]
public class CategoryDefinition
{
    [SerializeField, HideInInspector]
    private string _categoryId;

    [SerializeField]
    private string _displayName;

    public string CategoryId => _categoryId;
    public string DisplayName => _displayName;

    public CategoryDefinition(string categoryId, string displayName)
    {
        _categoryId = categoryId;
        _displayName = displayName;
    }
}
