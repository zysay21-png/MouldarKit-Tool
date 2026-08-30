using UnityEngine;

public class ModuleSocket : MonoBehaviour
{
    [SerializeField] private string _socketType;

    public string SocketType => _socketType;
}