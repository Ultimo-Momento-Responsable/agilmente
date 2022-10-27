using UnityEngine;
using UnityEngine.UI;

public class FigureBehaviour : MonoBehaviour
{
    private HayUnoRepetidoController controller;
    private int index;
    public ParticleSystem ps;
    public bool isClicked = false;
    public bool IsCorrectFigure { get => index == 0 || index == 1; }

    public void OnDestroy()
    {
        if(!isClicked && ps != null)
        {
            Destroy(ps.gameObject);
        }
    }

    public void Initialize(HayUnoRepetidoController controller, Sprite sprite, int figureIndex, GameObject cell)
    {
        GetComponent<Image>().sprite = sprite;
        this.controller = controller;
        index = figureIndex;

        if (IsCorrectFigure)
        {
            ps = Instantiate(controller.particles, cell.transform).GetComponent<ParticleSystem>();
            ps.transform.localPosition = GetComponent<RectTransform>().anchoredPosition;
        }
    }


    /// <summary>
    /// Verifica si el usuario tapeó la fruta correcta, y el comportamiento 
    /// correspondiente.
    /// </summary>
    public void checkIfUserTappedFigure()
    {
        if (index == 0 || index == 1)
        {
            isClicked = true;
            controller.isTouching = true;
            ps.Stop();
            ps.Play();
        }
        else
        {
            if (controller.grid.GetComponent<ScreenShake>().shakeDuration <= 0 && !controller.hayUnoRepetido.onTutorial)
            {
                controller.isMakingMistake = true;
            }
        }
    }
}
