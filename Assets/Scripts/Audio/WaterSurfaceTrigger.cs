using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WaterSurfaceTrigger : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event SetSwitchToWater;
    [SerializeField] private AK.Wwise.Event SetSwitchToGrass;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Creature"))
            return;

        // other.gameObject is the rig child—where your MovementAudioManager & AkGameObj live
        var emitterGO = other.gameObject;

        Debug.Log($"[WaterTrigger] Posting SetSwitchToWater on {emitterGO.name}");
        SetSwitchToWater.Post(emitterGO);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Creature"))
            return;

        var emitterGO = other.gameObject;

        Debug.Log($"[WaterTrigger] Posting SetSwitchToGrass on {emitterGO.name}");
        SetSwitchToGrass.Post(emitterGO);
    }
}
