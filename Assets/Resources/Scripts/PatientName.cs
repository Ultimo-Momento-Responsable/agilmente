using UnityEngine;
using UnityEngine.UI;

public class PatientName : MonoBehaviour
{
    private Settings settings;

    void Start()
    {
        settings = JsonUtility.FromJson<Settings>(System.IO.File.ReadAllText(Application.persistentDataPath + "/settings.json"));
        Text welcomePatientText = GetComponent<Text>();
        welcomePatientText.text = "¡Hola de nuevo " + settings.Login.patient.firstName + "!";

    }
}
