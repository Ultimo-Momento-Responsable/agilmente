using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public HayUnoRepetidoController controller;
    private new Collider2D collider2D;
    private bool isTouching;

    private void Start()
    {
        collider2D = GetComponent<Collider2D>();
        isTouching = false;
    }

    void Update()
    {
        if (Input.touchCount > 0) // si se pulsa la pantalla
        {
            Vector3 wp = controller.camera.ScreenToWorldPoint(Input.GetTouch(0).position); 
            Vector2 touchPos = new Vector2(wp.x, wp.y);
            if (collider2D == Physics2D.OverlapPoint(touchPos) && !isTouching) // si la posición donde se pulsa es donde se encuentra el botón de pausa
            {
                isTouching = true;
                controller.pauseGame();
            }
        }
        else
        {
            isTouching = false;
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
