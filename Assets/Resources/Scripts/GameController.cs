using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    private GameObject a_endScreen;
    public GameObject endScreen
    {
        get
        {
            if (a_endScreen == null)
            {
                a_endScreen = GameObject.Find("EndScreen");
            }

            return a_endScreen;
        }
        set => a_endScreen = value;
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
        sendData();
    }

    /// <summary>
    /// Cambia la escena al menú principal.
    /// </summary>
    public void goToMainScene()
    {
        SceneManager.LoadScene("mainScene");
    }

    /// <summary>
    /// Muestra la pantalla de fin del juego con el puntaje.
    /// </summary>
    /// <param name="score">Puntaje final.</param>
    public void showEndScreen(int score)
    {
        pause.gameObject.SetActive(false);
        GameObject.Find("Timer").SetActive(false);
        endScreen.SetActive(true);
        endScreen.transform.Find("Score").GetComponent<Text>().text = score.ToString();
    }
}
