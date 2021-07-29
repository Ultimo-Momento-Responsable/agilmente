using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    private Settings settings;
    public Text loginCode;
    public GameObject loginError;
    public Button loginButton;
    private PatientJson patientJson;

    void Start()
    {
        settings = JsonUtility.FromJson<Settings>(System.IO.File.ReadAllText(Application.dataPath + "/settings.json"));
        bool isLogged = settings.Login.isLogged;
        if (isLogged)
        {
            this.StartCoroutine(this.getPatientRoutine("localhost:8080/patient/" + settings.Login.patient.id, this.getPatientResponseCallback));
        }
    }

    void Update()
    {
        if (loginCode.text.Length < 6)
        {
            loginButton.interactable = false;
        }
        else
        {
            loginButton.interactable = true;
        }
    }

    // cuando se clickea el bot�n de ingresar, busca el paciente en la base de datos.
    public void getPatient()
    {
        this.StartCoroutine(this.getPatientRoutine("localhost:8080/patient/lc" + loginCode.text, this.getPatientResponseCallback));
    }

    // Se hace un get a los pacientes para ver si ese c�digo de Logueo existe
    private IEnumerator getPatientRoutine(string url, Action<string> callback = null)
    {
        var request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();
        var data = request.downloadHandler.text;

        if (callback != null)
            callback(data);
    }

    // Una vez que obtiene los datos del paciente se modifica el json settings.json para que no pida nuevamente el logueo
    private void getPatientResponseCallback(string data)
    {
        if (data != "null")
        {
            patientJson = JsonUtility.FromJson<PatientJson>(data);
        }
        else
        {
            patientJson = null;
        }
        if (patientJson != null)
        {
            if (patientJson.loginCode != settings.Login.loginCode && patientJson.logged == false && loginCode.text.Length<6)
            {
                settings = new Settings();
                settings.Login = new Login();
                settings.Login.isLogged = false;
                settings.Login.patient = new Patient();
                settings.Login.patient.id = patientJson.id;
                settings.Login.patient.name = patientJson.firstName + " " + patientJson.lastName;
                File.WriteAllText(Application.dataPath + "/settings.json", JsonUtility.ToJson(settings));
            }
            else if (patientJson.loginCode == "" && patientJson.logged == true)
            {
                SceneManager.LoadScene("mainScene");
            } 
            else
            {
                settings = new Settings();
                settings.Login = new Login();
                settings.Login.isLogged = true;
                settings.Login.loginCode = patientJson.loginCode;
                settings.Login.patient = new Patient();
                settings.Login.patient.id = patientJson.id;
                settings.Login.patient.name = patientJson.firstName + " " + patientJson.lastName;
                patientJson.logged = true;
                patientJson.loginCode = null;
                File.WriteAllText(Application.dataPath + "/settings.json", JsonUtility.ToJson(settings));
                this.StartCoroutine(this.putPatientRoutine("localhost:8080/patient/" + patientJson.id));
            }
        }
        else
        {
            loginError.SetActive(true);
        }
    }

    // Se hace un put en la base de datos para decirle que el paciente est� logueado y que el c�digo no sirve m�s
    private IEnumerator putPatientRoutine(string url)
    {
        var json = JsonUtility.ToJson(patientJson);
        byte[] putData = System.Text.Encoding.Unicode.GetBytes(json);
        var request = UnityWebRequest.Put(url,putData);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            SceneManager.LoadScene("mainScene");
        }
    }
}
[System.Serializable]
public class Settings
{
    public Login Login;
}

[System.Serializable]
public class Login
{
    public bool isLogged;
    public string loginCode;
    public Patient patient;
}
[System.Serializable]
public class Patient
{
    public long id;
    public string name;
}

public class PatientJson
{
    public long id;
    public string firstName;
    public string lastName;
    public string description;
    public string bornDate;
    public string city;
    public string loginCode;
    public bool logged;
}
