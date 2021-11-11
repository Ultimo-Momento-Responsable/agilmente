using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    /// <summary>
    /// Cambia la escena al menú principal.
    /// </summary>
    public void goToMainScene()
    {
        SceneManager.LoadScene("mainScene");
    }
}
