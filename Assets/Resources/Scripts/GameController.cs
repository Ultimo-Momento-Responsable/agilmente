using UnityEngine;
public abstract class GameController: MonoBehaviour
{
    private PauseMenu a_pause;
    public PauseMenu pause
    {
        get
        {
            if (a_pause == null)
            {
                a_pause = GameObject.Find("pauseMenu").GetComponent<PauseMenu>();
            }

            return a_pause;
        }
        set => a_pause = value;
    }

    /// <summary>
    /// Cancela el juego que se está jugando.
    /// </summary>
    public abstract void cancelGame();
    /// <summary>
    /// Envía el resultado del juego que se 
    /// está jugando.
    /// </summary>
    public abstract void sendData();
    /// <summary>
    /// Pausa el juego que se está jugando.
    /// </summary>
    public abstract void pauseGame();

    /// <summary>
    /// Quita la pausa del juego que se está jugando.
    /// </summary>
    public abstract void unpauseGame();

    /// <summary>
    /// Esta función se llama cuando se llama a la pausa.
    /// </summary>
    public void buttonPauseEvent()
    {
        if (this.pause.isPaused)
        {
            this.pause.unpauseGame();
        } else
        { 
            this.pause.pauseGame();
        }
    }

    public void OnApplicationPause()
    {
        this.pause.pauseGame();
    }

    public void OnApplicationQuit()
    {
        cancelGame();
        //sendData();
    }
}
