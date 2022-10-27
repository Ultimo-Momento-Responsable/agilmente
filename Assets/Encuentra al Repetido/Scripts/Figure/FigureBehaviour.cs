using UnityEngine;
using UnityEngine.UI;

public class FigureBehaviour : MonoBehaviour
{
    private HayUnoRepetidoController controller;
    private int index;
    // TODO: Fix particle system
    //public ParticleSystem ps;

    public void Initialize(HayUnoRepetidoController controller, Sprite sprite, int figureIndex)
    {
        GetComponent<Image>().sprite = sprite;
        this.controller = controller;
        index = figureIndex;
    }


    /// <summary>
    /// Verifica si el usuario tapeó la fruta correcta, y el comportamiento 
    /// correspondiente.
    /// </summary>
    public void checkIfUserTappedFigure()
    {
        if (index == 0 || index == 1)
        {
            controller.isTouching = true;
            //ps.Stop();
            //ps.Play();
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
