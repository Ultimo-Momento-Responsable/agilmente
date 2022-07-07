using UnityEngine;
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

    public AudioSource audioSource;
    public AudioClip gameOverSound;

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

    public abstract void OnApplicationPause();

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
    /// Reproduce un sonido.
    /// </summary>
    /// <param name="audioClip">Sonido a reproducir.</param>
    public void PlaySound(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }

    /// <summary>
    /// Reproduce el sonido de juego terminado.
    /// </summary>
    public void PlayGameOverSound()
    {
        PlaySound(gameOverSound);
    }
}
