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
        if (Input.touchCount == 1) // si se pulsa la pantalla
        {
            Vector3 wp = controller.camera.ScreenToWorldPoint(Input.GetTouch(0).position); 
            Vector2 touchPos = new Vector2(wp.x, wp.y);
            if (collider2D == Physics2D.OverlapPoint(touchPos)) // si la posición donde se pulsa es donde se encuentra el botón de pausa
            {
                controller.pauseGame();
            }
        }
    }
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)) // si se pulsa con el mouse
        {
            controller.pauseGame();
        }
    }
}
