using UnityEngine;

public class FigureBehaviourEAN : MonoBehaviour
{
    public Sprite sprite;
    public EncuentraAlNuevoController controller;
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
            Vector3 wp = controller.camera.ScreenToWorldPoint(Input.GetTouch(0).position);
            Vector2 touchPos = new Vector2(wp.x, wp.y);
            if (collider2D == Physics2D.OverlapPoint(touchPos)) // si la posici�n donde se pulsa es donde se encuentra la figura
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
    /// Verifica si el usuario tape� la fruta correcta, y el comportamiento 
    /// correspondiente.
    /// </summary>
    void checkIfUserTappedFigure()
    {
        if (index == 0 && !controller.prevTutorial)
        {
            controller.GetComponent<EncuentraAlNuevoController>().isTouching = true;
            ps.Stop();
            ps.Play();
        }
        else
        {
            if (controller.camera.GetComponent<ScreenShake>().shakeDuration <= 0)
            {
                controller.encuentraAlNuevo.mistakes++;
                controller.isMakingMistake = true;
            }
        }
    }
}
