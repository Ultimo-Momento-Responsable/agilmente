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
    private static string endpoint = "patient/";

    void Start()
    {
        bool fileExists = System.IO.File.Exists(Application.persistentDataPath + "/settings.json");

        if (!fileExists)
        {
            createSettingsFile();
        }

        settings = JsonUtility.FromJson<Settings>(System.IO.File.ReadAllText(Application.persistentDataPath + "/settings.json"));
        bool isLogged = settings.Login.isLogged;
        if (isLogged)
        {
            this.StartCoroutine(this.getPatientRoutine(SendData.IP + endpoint + settings.Login.patient.id, this.getPatientResponseCallback));
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
    
    public void createSettingsFile()
    {
        Settings settings = new Settings();
        settings.Login = new Login();
        settings.Login.isLogged = false;
        settings.Login.patient = new Patient();
        File.WriteAllText(Application.persistentDataPath + "/settings.json", JsonUtility.ToJson(settings));
    }

    /**
     * cuando se clickea el botón de ingresar, busca el paciente en la base de datos.
     */
    public void getPatient()
    {
        this.StartCoroutine(this.getPatientRoutine(SendData.IP + endpoint + "/lc" + loginCode.text, this.getPatientResponseCallback));
    }
    /**
     * Se hace un get a los pacientes para ver si ese código de Logueo existe
     */
    private IEnumerator getPatientRoutine(string url, Action<string> callback = null)
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
                settings.Login.patient.firstName = patientJson.firstName;
                settings.Login.patient.lastName = patientJson.lastName;
                File.WriteAllText(Application.persistentDataPath + "/settings.json", JsonUtility.ToJson(settings));
            }
            else if (patientJson.loginCode == "" && patientJson.logged == true && patientJson.joinDate == settings.Login.joinDate)
            {
                SceneManager.LoadScene("mainScene");
            } 
            else {
                if (loginCode.text.Length == 6)
                {
                    settings = new Settings();
                    settings.Login = new Login();
                    settings.Login.isLogged = true;
                    settings.Login.loginCode = patientJson.loginCode;
                    settings.Login.patient = new Patient();
                    settings.Login.patient.id = patientJson.id;
                    settings.Login.patient.firstName = patientJson.firstName;
                    settings.Login.patient.lastName = patientJson.lastName;
                    patientJson.logged = true;
                    patientJson.loginCode = null;
                    var joinDate = DateTime.Now;
                    patientJson.joinDate = joinDate.ToString("dd-MM-yyyy HH:mm:ss");
                    settings.Login.joinDate = joinDate.ToString("dd-MM-yyyy HH:mm:ss");
                    File.WriteAllText(Application.persistentDataPath + "/settings.json", JsonUtility.ToJson(settings));
                    this.StartCoroutine(this.putPatientRoutine(SendData.IP + endpoint + patientJson.id));
                }
            }
        }
        else
        {
            loginError.SetActive(true);
        }
    }

    /**
     * Se hace un put en la base de datos para decirle que el paciente está logueado y que el código no sirve más
     */
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
    public string joinDate;
    public Patient patient;
}
[System.Serializable]
public class Patient
{
    public long id;
    public string firstName;
    public string lastName;
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
    public string joinDate;
}
