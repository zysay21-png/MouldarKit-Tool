using UnityEngine;

/// <summary>
/// Serializable data model that defines a module category inside a modular kit.
/// Categories are used to organize modules in the editor-facing palette.
/// </summary>
[System.Serializable]
public class CategoryDefinition
{
    // Stable internal identifier used by modules to reference this category.
    [SerializeField, HideInInspector]
    private string _categoryId;

    // Human-readable name shown in the kit editor and palette.
    [SerializeField]
    private string _displayName;

    public string CategoryId => _categoryId;
    public string DisplayName => _displayName;

    /// <summary>
    /// Creates a category with a persistent ID and display name.
    /// </summary>
    public CategoryDefinition(string categoryId, string displayName)
    {
        _categoryId = categoryId;
        _displayName = displayName;
    }
}
