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
    public void btnGames()
    {
        title.text = "Jugar";
        bodyText.SetActive(true);
        bodyText.GetComponent<Text>().text = "Juegos pendientes"; 
        print("Boton jugar clickeado");
    }
    public void btnHome()
    {
        title.text = "Inicio";
        bodyText.SetActive(true);
        settings = JsonUtility.FromJson<Settings>(System.IO.File.ReadAllText(Application.dataPath + "/settings.json"));
        bodyText.GetComponent<Text>().text = "¡Hola de nuevo " + settings.Login.patient.firstName + "!";
        print("Boton inicio clickeado");
    }
    public void btnProfile()
    {
        title.text = "Perfil";
        bodyText.SetActive(false);
        print("Boton perfil clickeado");
    }

    //GET PARA PLANNING DESDE BACKEND - 
    public void getPlanning()
    {
        this.StartCoroutine(this.getPlanningRoutine("localhost:8080/planning", this.getPlanningResponseCallback));
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
