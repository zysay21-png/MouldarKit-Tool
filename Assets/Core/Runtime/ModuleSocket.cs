using UnityEngine;

/// <summary>
/// Lightweight metadata component used to mark a Transform as a modular
/// connection socket. Matching socket types can be used by future
/// attachment or socket-based placement workflows.
/// </summary>
public class ModuleSocket : MonoBehaviour
{
    [SerializeField] private string _socketType;

    public string SocketType => _socketType;
}
