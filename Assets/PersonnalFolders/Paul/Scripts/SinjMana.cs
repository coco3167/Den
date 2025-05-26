using UnityEngine;
using TMPro;

public class SinjMana : MonoBehaviour
{
    public float curioCommune;
    public SinjIndiv[] sinjs;
    public TextMeshPro curioText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (SinjIndiv sinj in sinjs)
        {
            curioCommune += sinj.curio;
        }

        curioText.text = "Curio Jauge = " + Mathf.Round(100*curioCommune)/100;
    }
}
