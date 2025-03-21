using UnityEngine;

public class Sinj : MonoBehaviour
{
    [SerializeField, Range(1, 5)] private float distanceToReact;
    
    public void ReactToMouseDistance(float distance)
    {
        if(distance < distanceToReact*distanceToReact)
            Debug.Log("ouhou haha");
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, distanceToReact);
    }
}
