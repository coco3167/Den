using Unity.AI.Navigation;
using UnityEngine;

[RequireComponent(typeof(NavMeshSurface))]
public class EnvironmentManager : MonoBehaviour
{
    private NavMeshSurface m_navMeshSurface;

    private void Awake()
    {
        m_navMeshSurface = GetComponent<NavMeshSurface>();
    }
}
