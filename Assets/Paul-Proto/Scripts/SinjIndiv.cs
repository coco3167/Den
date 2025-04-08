using UnityEngine;
using TMPro;

public class SinjIndiv : MonoBehaviour
{
    public TextMeshPro influText;
    public TextMeshPro curioText;

    public float influencability;
    public float curio;
    
    public float cursorFactor;
    public float influenceFactor;
    public float range = 200f;

    public GameObject otherSinj;
    public Transform cursor;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Flee(cursor.position);
        
        float distance = Vector3.Distance(otherSinj.transform.position,transform.position);
        if (distance < range && otherSinj.GetComponent<SinjIndiv>().cursorFactor > 0)
        {
            influenceFactor = influencability*otherSinj.GetComponent<SinjIndiv>().cursorFactor;
        }
        else
        {
            influenceFactor = 0;
        }

        curio = Mathf.Clamp((cursorFactor + influenceFactor),0,1);
        
        


        influText.text = "influencability = " + influencability;
        curioText.text = "curio = " + curio;
    }

    void Flee(Vector3 cursorPos)
    {
        float distance = Vector3.Distance(cursorPos,transform.position);
        if (distance < range)
        {
            transform.position += (transform.position-cursorPos)*Time.deltaTime;
        }
    }
}
