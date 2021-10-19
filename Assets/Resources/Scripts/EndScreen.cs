using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    /// <summary>
    /// Cambia la escena al men� principal.
    /// </summary>
    public void goToMainScene()
    {
        SceneManager.LoadScene("mainScene");
    }
}
