using UnityEngine;

public class Proc_Tail : MonoBehaviour
{
    public Transform tailTarget;
    public Transform smoothTailTarget;
    private Vector3 fixedTailPos;
    private Vector3 startLocalPos;
    public Transform root;
    public float heigth;
    public float range;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fixedTailPos = tailTarget.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        fixedTailPos = Vector3.Lerp(fixedTailPos, tailTarget.position, Time.deltaTime *5);
        
        

        smoothTailTarget.position = fixedTailPos;

        


    }
}
