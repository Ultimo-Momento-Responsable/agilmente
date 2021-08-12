using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MainSceneController : MonoBehaviour
{
    public Text title;
    public GameObject bodyText;
    private Settings settings;
    public Button gameButton;
    public Button profileButton;
    public Button homeButton;
    public Sprite[] homeSprite;
    public Sprite[] profileSprite;
    public Sprite[] gameSprite;
    public Text gameName;

    public void highlightButton(Image buttonImage, Sprite highlightedButton)
    {
        buttonImage.sprite = highlightedButton;
    }

    public void btnGames()
    {
        title.text = "Jugar";
        highlightButton(gameButton.GetComponent<Image>(), gameSprite[0]);
        highlightButton(profileButton.GetComponent<Image>(), profileSprite[1]);
        highlightButton(homeButton.GetComponent<Image>(), homeSprite[1]);

        bodyText.SetActive(true);
        bodyText.GetComponent<Text>().text = "Juegos pendientes"; 
        print("Boton jugar clickeado");

    }
    public void btnHome()
    {
        title.text = "Inicio";
        highlightButton(gameButton.GetComponent<Image>(), gameSprite[1]);
        highlightButton(profileButton.GetComponent<Image>(), profileSprite[1]);
        highlightButton(homeButton.GetComponent<Image>(), homeSprite[0]);

        bodyText.SetActive(true);
        settings = JsonUtility.FromJson<Settings>(System.IO.File.ReadAllText(Application.dataPath + "/settings.json"));
        bodyText.GetComponent<Text>().text = "¡Hola de nuevo " + settings.Login.patient.firstName + "!";
        print("Boton inicio clickeado");
    }
    public void btnProfile()
    {
        title.text = "Perfil";
        highlightButton(gameButton.GetComponent<Image>(), gameSprite[1]);
        highlightButton(profileButton.GetComponent<Image>(), profileSprite[0]);
        highlightButton(homeButton.GetComponent<Image>(), homeSprite[1]);

        bodyText.SetActive(false);
        print("Boton perfil clickeado");
    }

    //GET PARA PLANNING DESDE BACKEND - 
    public void getPlanning()
    {
        settings = JsonUtility.FromJson<Settings>(System.IO.File.ReadAllText(Application.dataPath + "/settings.json"));
        this.StartCoroutine(this.getPlanningRoutine("localhost:8080/planning/patient_", this.getPlanningResponseCallback));
    }
    /**
     * Se hace un get a los pacientes para ver si ese código de Logueo existe
     */
    private IEnumerator getPlanningRoutine(string url, Action<string> callback = null)
    {
        var request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();
        var data = request.downloadHandler.text;

        if (callback != null)
            callback(data);
    }

    /**
     * Una vez que obtiene los datos del paciente se modifica el json settings.json para que no pida nuevamente el logueo
     */
    private void getPlanningResponseCallback(string data) { }

}
