using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the floor hierarchy of a modular build and tracks which floor
/// is currently active for placement.
/// </summary>
public class ModularBuildLevel : MonoBehaviour
{
    // Scene transforms that act as parents for modules placed on each floor.
    [SerializeField, HideInInspector]
    private List<Transform> _levels = new();

    public IReadOnlyList<Transform> Levels => _levels;

    // Index of the level currently used by the placement system.
    [SerializeField, HideInInspector]
    private int _activeLevelIndex;

    public int ActiveLevelIndex => _activeLevelIndex;

    // Source kit provides shared floor settings such as FloorHeight.
    [SerializeField, HideInInspector]
    private ModularKitDefinition _kit;

    /// <summary>
    /// Returns the transform for the currently active floor.
    /// </summary>
    public Transform ActiveLevel
    {
        get
        {
            if (_levels == null)
            {
                return null;
            }

            if (_levels[_activeLevelIndex] == null)
            {
                return null;
            }

            return _levels[_activeLevelIndex];
        }
    }

    /// <summary>
    /// Creates Level_00 when a build is initialized for the first time.
    /// </summary>
    private void CreateInitialLevel()
    {
        if (_levels.Count > 0)
        {
            return;
        }

        GameObject levelObject = new GameObject("Level_00");

        levelObject.transform.SetParent(transform);
        levelObject.transform.localPosition = Vector3.zero;

        _levels.Add(levelObject.transform);
        _activeLevelIndex = 0;
    }

    /// <summary>
    /// Initializes the level system with the kit used by this build.
    /// </summary>
    public void Initialize(ModularKitDefinition kit)
    {
        _kit = kit;
        CreateInitialLevel();
    }

    /// <summary>
    /// Adds a new floor at an integer multiple of the kit's FloorHeight
    /// and makes the new floor active.
    /// </summary>
    public void AddLevel()
    {
        int newLevelIndex = _levels.Count;

        GameObject newLevel =
            new GameObject("Level_" + newLevelIndex.ToString("00"));

        newLevel.transform.SetParent(transform);

        float y = newLevelIndex * _kit.FloorHeight;

        newLevel.transform.localPosition = new Vector3(0, y, 0);

        _levels.Add(newLevel.transform);
        _activeLevelIndex = newLevelIndex;
    }

    /// <summary>
    /// Changes the floor used by the placement system.
    /// Invalid indices are ignored.
    /// </summary>
    public void SetActiveLevel(int levelIndex)
    {
        if (_levels.Count > levelIndex && levelIndex >= 0)
        {
            _activeLevelIndex = levelIndex;
        }
    }
}
