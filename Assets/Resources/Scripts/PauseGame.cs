using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public HayUnoRepetidoController controller;
    private new Collider2D collider2D;

    private void Start()
    {
        collider2D = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Vector3 wp = controller.camera.ScreenToWorldPoint(Input.GetTouch(0).position);
            Vector2 touchPos = new Vector2(wp.x, wp.y);
            if (collider2D == Physics2D.OverlapPoint(touchPos))
            {
                controller.pauseGame();
            }
        }
    }
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            controller.pauseGame();
        }
    }
}
