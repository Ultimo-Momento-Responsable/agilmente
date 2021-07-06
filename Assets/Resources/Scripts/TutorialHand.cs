using UnityEngine;

public class TutorialHand : MonoBehaviour
{
    public AnimationCurve myCurve;
    public Sprite touchingHand;
    public Sprite hand;
    SpriteRenderer sr;
    public float yPos;
    
    void Start()
    {
        sr = GameObject.FindGameObjectWithTag("tutorial").GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, myCurve.Evaluate((Time.time % myCurve.length)) + yPos, transform.position.z);
        if (myCurve.Evaluate((Time.time % myCurve.length)) > 0.98f)
        {
            if (sr != null)
            {
                sr.sprite = touchingHand;
            }
        }
        else
        {
            if (sr != null)
            {
                sr.sprite = hand;
            }
        }
    }
}
