using UnityEngine;

public class FigureBehaviour : MonoBehaviour
{
    public Sprite sprite;
    public HayUnoRepetidoController controller;
    public int index;
    private new Collider2D collider2D;
    public ParticleSystem ps;

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
        collider2D = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (Input.touchCount == 1) // si se pulsa la pantalla
        {
            Vector3 wp = controller.mainCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
            Vector2 touchPos = new Vector2(wp.x, wp.y);
            if (collider2D == Physics2D.OverlapPoint(touchPos)) // si la posición donde se pulsa es donde se encuentra la figura
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    checkIfUserTappedFigure();
                }

            }
        }
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)) // si se pulsa con el mouse
        {
            checkIfUserTappedFigure();
        }
    }

    /// <summary>
    /// Verifica si el usuario tapeó la fruta correcta, y el comportamiento 
    /// correspondiente.
    /// </summary>
    void checkIfUserTappedFigure()
    {
        if (index == 0 || index == 1)
        {
            controller.GetComponent<HayUnoRepetidoController>().isTouching = true;
            ps.Stop();
            ps.Play();
        }
        else
        {
            if (controller.mainCamera.GetComponent<ScreenShake>().shakeDuration <= 0 && !controller.hayUnoRepetido.onTutorial)
            {
                controller.isMakingMistake = true;
            }
        }
    }
}
