using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu: MonoBehaviour
{
    private GameController a_gameController;
    private bool a_isPaused;
    public GameObject menuMain;
    public GameObject menuConfirmLeave;
    public GameObject pauseMenu;
    public GameObject goGameController;
    public GameController gameController { get => a_gameController; set => a_gameController = value; }
    public bool isPaused { get => a_isPaused; set => a_isPaused = value; }

    public void Start()
    {
        this.gameController = this.goGameController.GetComponent<GameController>();
        this.unpauseGame();
    }

    /// <summary>
    /// Función del botón del menú para volver al menú principal.
    /// </summary>
    public void backToMainMenu()
    {
        this.showConfirmationMessageLeave();
    }

    /// <summary>
    /// Cancela la partida y regresa al menú principal.
    /// </summary>
    public void returnToMainScene ()
    {
        this.gameController.cancelGame();
        this.gameController.sendData();
        this.unpauseGame();

        SceneManager.LoadScene("mainScene");
    }

    /// <summary>
    /// Función del botón del menú para reanudar el juego.
    /// </summary>
    public void resumeGame()
    {
        this.unpauseGame();
    }

    /// <summary>
    /// Muestra el mensaje de confirmación preguntando al usuario si quiere
    /// abandonar la partida.
    /// </summary>
    public void showConfirmationMessageLeave()
    {
        this.menuMain.SetActive(false);
        this.menuConfirmLeave.SetActive(true);
    }

    /// <summary>
    /// Oculta el mensaje de confirmación preguntando al usuario si quiere
    /// abandonar la partida.
    /// </summary>
    public void hideConfirmationMessageLeave()
    {
        this.menuMain.SetActive(true);
        this.menuConfirmLeave.SetActive(false);
    }

    /// <summary>
    /// Pausa el juego
    /// </summary>
    public void pauseGame()
    {
        if (this.gameController)
        {
            this.gameController.pauseGame();
        }
        this.pauseMenu.SetActive(true);
        Time.timeScale = 0;
        this.isPaused = true;
    }

    /// <summary>
    /// Reanuda el juego.
    /// </summary>
    public void unpauseGame()
    {
        this.gameController.unpauseGame();
        this.pauseMenu.SetActive(false);
        Time.timeScale = 1;
        this.isPaused = false;
    }
}
