using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class behaviour : MonoBehaviour
{
    public Sprite sprite;
    public gestor controller;
    public int index;
    private Collider2D collider2D;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
        collider2D = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 1)
        {
            Vector3 wp = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            Vector2 touchPos = new Vector2(wp.x, wp.y);
            if (collider2D == Physics2D.OverlapPoint(touchPos))
            {
                if (index == 0 || index == 1)
                {
                    controller.GetComponent<gestor>().isTouching = true;
                }
                else
                {
                    controller.GetComponent<gestor>().mistakes++;
                    Debug.Log(controller.GetComponent<gestor>().mistakes);
                }

            }
        }
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)) {
            if (index == 0 || index == 1)
            {
                controller.GetComponent<gestor>().isTouching = true;
            }
            else
            {
                controller.GetComponent<gestor>().mistakes++;
                Debug.Log(controller.GetComponent<gestor>().mistakes);
            }
        }
    }

    
}
