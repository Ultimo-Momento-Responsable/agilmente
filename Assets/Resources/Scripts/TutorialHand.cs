using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHand : MonoBehaviour
{
    public AnimationCurve myCurve;
    public Sprite touchingHand;
    public Sprite hand;
    SpriteRenderer sr;
    
    // Start is called before the first frame update
    void Start()
    {
        sr = GameObject.FindGameObjectWithTag("tutorial").GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, myCurve.Evaluate((Time.time % myCurve.length)) -2.3f, transform.position.z);
        if (myCurve.Evaluate((Time.time % myCurve.length)) > 0.98f)
        {
            sr.sprite = touchingHand;
            print(myCurve.Evaluate((Time.time % myCurve.length)));
        }
        else
        {
            sr.sprite = hand;
        }
    }
}
