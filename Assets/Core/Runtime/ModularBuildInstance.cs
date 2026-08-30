using UnityEngine;

/// <summary>
/// Scene-side component that links a spawned build root to its
/// persistent ModularBuildDefinition asset.
/// </summary>
public class ModularBuildInstance : MonoBehaviour
{
    [SerializeField] private ModularBuildDefinition _definition;

    public ModularBuildDefinition Definition => _definition;

    /// <summary>
    /// Associates this scene instance with its build definition asset.
    /// </summary>
    public void Initialize(ModularBuildDefinition definition)
    {
        _definition = definition;
    }
}
