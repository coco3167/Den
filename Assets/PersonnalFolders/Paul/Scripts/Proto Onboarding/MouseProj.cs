using UnityEngine;

public class MouseProj : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Camera camera;
    public GameObject marker;
    public float planeZ = 0f;

    void Update()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, new Vector3(0, 0, planeZ));

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 worldPos = ray.GetPoint(distance);
            Debug.Log("Mouse World Position: " + worldPos);

            if (marker != null)
                marker.transform.position = worldPos;
        }
    }
}
