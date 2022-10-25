using UnityEngine;
using UnityEngine.UI;

public class FigureBehaviourWithCanvas : MonoBehaviour
{
    public Sprite sprite;
    public HayUnoRepetidoController controller;
    public int index;
    // TODO: Fix particle system
    //public ParticleSystem ps;

    void Start()
    {
        GetComponent<Image>().sprite = sprite;
    }


    /// <summary>
    /// Verifica si el usuario tapeó la fruta correcta, y el comportamiento 
    /// correspondiente.
    /// </summary>
    public void checkIfUserTappedFigure()
    {
        if (index == 0 || index == 1)
        {
            controller.GetComponent<HayUnoRepetidoController>().isTouching = true;
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
