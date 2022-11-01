using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSessionCard : MonoBehaviour
{
    public Color agilmenteBlue;
    public Image logo;
    public List<Sprite> gameLogoSprites;
    public GameObject completedMedal;
    public Text txtGameName;
    public Text txtNumberOfSessions;

    private Dictionary<string, Sprite> gameLogo;

    /// <summary>
    /// Añade los logos de los juegos a un diccionario, para que 
    /// sea más fácil accederlos.
    /// </summary>
    private void AddLogosToDictionary()
    {
        gameLogo = new Dictionary<string, Sprite>();
        foreach (Sprite gl in gameLogoSprites)
        {
            gameLogo.Add(gl.name, gl);
        }
    }

    /// <summary>
    /// Cambia el estilo de la card a completada.
    /// </summary>
    private void SetAsCompleted()
    {
        completedMedal.SetActive(true);
    }

    /// <summary>
    /// Cambia el estilo de la card para sesiones libres.
    /// </summary>
    private void SetAsUnlimited() {
        GetComponent<Image>().color = Color.white;
        txtGameName.GetComponent<Text>().color = agilmenteBlue;
        txtNumberOfSessions.GetComponent<Text>().color = agilmenteBlue;
    }
    
    /// <summary>
    /// Setea el logo correspondiente al juego de la card.
    /// </summary>
    /// <param name="gameName">Nombre del juego.</param>
    private void SetGameLogo(string gameName)
    {
        logo.sprite = gameLogo[gameName];
    }

    /// <summary>
    /// Setea los textos de la card.
    /// </summary>
    /// <param name="gameName">Nombre del juego.</param>
    /// <param name="numberOfSessions">Número de sesiones restantes.</param>
    private void SetTexts(string gameName, int numberOfSessions)
    {
        SetGameName(gameName);
        SetNumberOfSessions(numberOfSessions);
    }

    /// <summary>
    /// Setea el nombre del juego.
    /// </summary>
    /// <param name="gameName">Nombre del juego.</param>
    private void SetGameName(string gameName)
    {
        txtGameName.text = gameName;
    }

    /// <summary>
    /// Setea el texto que indica la cantidad de sesiones restantes,
    /// en base a la cantidad de sesiones.
    /// </summary>
    /// <param name="numberOfSessions">Cantidad de sesiones restantes.</param>
    private void SetNumberOfSessions(int numberOfSessions)
    {
        switch (numberOfSessions)
        {
            default:
                txtNumberOfSessions.text = "Quedan " + numberOfSessions + " partidas por jugar";
                break;
            case 0:
                txtNumberOfSessions.text = "No quedan partidas restantes";
                break;
            case 1:
                txtNumberOfSessions.text = "Queda " + numberOfSessions + " partida por jugar";
                break;
            case -1:
                txtNumberOfSessions.text = "¡Juega libremente!";
                break;
        }
    }

    /// <summary>
    /// Inicializa la card, con su estilo correspondiente y los textos.
    /// </summary>
    /// <param name="gameName">Nombre del juego.</param>
    /// <param name="numberOfSessions">Sesiones restantes.</param>
    public void InitializeCard(string gameName, int numberOfSessions)
    {
        AddLogosToDictionary();
        SetGameLogo(gameName);
        SetTexts(gameName, numberOfSessions);

        switch(numberOfSessions) {
            case 0:
                SetAsCompleted();
                break;
            case -1:
                SetAsUnlimited();
                break;
        }
    }
}
