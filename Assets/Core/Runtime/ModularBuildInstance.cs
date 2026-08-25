using UnityEngine;

public class ModularBuildInstance : MonoBehaviour
{
    [SerializeField] private ModularBuildDefinition _definition;


    public ModularBuildDefinition Definition => _definition;


    public void Initialize(ModularBuildDefinition definition)
    {
        _definition = definition;
    }
}