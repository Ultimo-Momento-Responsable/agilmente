using UnityEngine;
using UnityEngine.UI;
using Assets.Resources.Scripts;
public class FigureBehaviour : MonoBehaviour
{
    private ControllerWithFigureBehaviour controller;
    private int index;
    public ParticleSystem ps;
    public bool isClicked = false;
    public bool IsCorrectFigure { get => controller.GetType() == typeof(HayUnoRepetidoController) ? index == 0 || index == 1 : index == 0; }

    public void OnDestroy()
    {
        if(!isClicked && ps != null)
        {
            Destroy(ps.gameObject);
        }
    }

    /// <summary>
    /// Inicializa los datos de la figura.
    /// </summary>
    /// <param name="controller">Controlador del juego.</param>
    /// <param name="sprite">Sprite de la figura.</param>
    /// <param name="figureIndex">Índice que identifica a la figura.</param>
    /// <param name="cell">Celda donde se encuentra la figura.</param>
    public void Initialize(ControllerWithFigureBehaviour controller, Sprite sprite, int figureIndex, GameObject cell)
    {
        GetComponent<Image>().sprite = sprite;
        this.controller = controller;
        index = figureIndex;

        if (IsCorrectFigure)
        {
            ps = Instantiate(controller.Particles, cell.transform).GetComponent<ParticleSystem>();
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
            controller.IsTouching = true;
            ps.Stop();
            ps.Play();
        }
        else
        {
            if (controller.Grid.GetComponent<ScreenShake>().shakeDuration <= 0 && !controller.Game.OnTutorial)
            {
                controller.IsMakingMistake = true;
            }
        }
    }
}
