using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class comportamiento : MonoBehaviour
{
    public Sprite sprite;
    public gestor controlador;
    public int indice;
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
                if (indice == 0 || indice == 1)
                {
                    controlador.GetComponent<gestor>().clickearon = true;
                }
                else
                {
                    controlador.GetComponent<gestor>().errores++;
                    Debug.Log(controlador.GetComponent<gestor>().errores);
                }

            }
        }
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)) {
            if (indice == 0 || indice == 1)
            {
                controlador.GetComponent<gestor>().clickearon = true;
            }
            else
            {
                controlador.GetComponent<gestor>().errores++;
                Debug.Log(controlador.GetComponent<gestor>().errores);
            }
        }
    }

    
}
