using UnityEngine;

namespace Sprites
{
    public class Billboard : MonoBehaviour
    {
        private void Update()
        {
            transform.LookAt(GameManager.Instance.GetCamera().transform.position, Vector3.up);
        }
    }
}
