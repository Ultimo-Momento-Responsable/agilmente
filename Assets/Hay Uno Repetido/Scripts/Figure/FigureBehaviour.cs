using UnityEngine;

public class FigureBehaviour : MonoBehaviour
{
    public Sprite sprite;
    public Gestor controller;
    public int index;
    private new Collider2D collider2D;
    public ParticleSystem ps;
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
            Vector3 wp = controller.camera.ScreenToWorldPoint(Input.GetTouch(0).position);
            Vector2 touchPos = new Vector2(wp.x, wp.y);
            if (collider2D == Physics2D.OverlapPoint(touchPos))
            {
                if (index == 0 || index == 1)
                {
                    controller.GetComponent<Gestor>().isTouching = true;
                    ps.Stop();
                    ps.Play();
                }
                else
                {
                    if (controller.camera.GetComponent<ScreenShake>().shakeDuration <= 0)
                    {
                        controller.hayUnoRepetido.Mistakes++;
                        controller.isMakingMistake = true;
                    }
                }

            }
        }
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)) {
            if (index == 0 || index == 1)
            {
                controller.GetComponent<Gestor>().isTouching = true;
                ps.Stop();
                ps.Play();
            }
            else
            {
                if (controller.camera.GetComponent<ScreenShake>().shakeDuration <= 0)
                {
                    controller.hayUnoRepetido.Mistakes++;
                    controller.isMakingMistake = true;
                }
            }
        }
    }

    
}
