using UnityEngine;

public class OnboardPartMog : MonoBehaviour
{
    
    public float height;
    public float hFrequency;
    public float width;
    public float wFrequency;

    public ParticleSystem particleSystem;
    public float detectionRange;
    public float minimalNoise;
    public float noiseStrength;
    public Transform mouse;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.Sin(Time.time * wFrequency + 0.5f) * width, Mathf.Sin(Time.time * hFrequency + hFrequency) * height, 20);

        var noise = particleSystem.noise;
        noise.strength = Mathf.Clamp(detectionRange - Vector3.Distance(transform.position,mouse.position),minimalNoise,999)*noiseStrength;
        
    }
}
