using UnityEngine;

public class FigureBehaviour : MonoBehaviour
{
    public Sprite sprite;
    public HayUnoRepetidoController controller;
    public int index;
    private Collider2D collider2D;
    public ParticleSystem ps;

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
        collider2D = GetComponent<Collider2D>();
    }

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
                    controller.GetComponent<HayUnoRepetidoController>().isTouching = true;
                    ps.Stop();
                    ps.Play();
                }
                else
                {
                    if (Camera.main.GetComponent<ScreenShake>().shakeDuration <= 0)
                    {
                        controller.GetComponent<HayUnoRepetidoController>().a_mistakes++;
                        controller.GetComponent<HayUnoRepetidoController>().isMakingMistake = true;
                    }
                }

            }
        }
    }

    void OnMouseOver()
    {
        checkIfUserTappedFigure();
    }

    /// <summary>
    /// Verifica si el usuario tapeó la fruta correcta, y el comportamiento 
    /// correspondiente.
    /// </summary>
    void checkIfUserTappedFigure()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (index == 0 || index == 1)
            {
                controller.GetComponent<HayUnoRepetidoController>().isTouching = true;
                ps.Stop();
                ps.Play();
            }
            else
            {
                if (Camera.main.GetComponent<ScreenShake>().shakeDuration <= 0)
                {
                    controller.GetComponent<HayUnoRepetidoController>().a_mistakes++;
                    controller.GetComponent<HayUnoRepetidoController>().isMakingMistake = true;
                }
            }
        }
    }
}
