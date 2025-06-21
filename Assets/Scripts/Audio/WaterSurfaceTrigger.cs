using Audio;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WaterSurfaceTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // whenever *any* creature enters, switch its footstep to Water
        AudioManager.Instance.SetFootstepSurface(
            WwiseSurfaceSwitch.Water,
            other.gameObject
        );
    }

    private void OnTriggerExit(Collider other)
    {
        // when they leave, switch it back to Grass
        AudioManager.Instance.SetFootstepSurface(
            WwiseSurfaceSwitch.Grass,
            other.gameObject
        );
    }
}
