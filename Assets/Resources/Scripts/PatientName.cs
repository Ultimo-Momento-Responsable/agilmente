using UnityEngine;
using UnityEngine.UI;

public class PatientName : MonoBehaviour
{
    private Settings settings;

    void Start()
    {
        settings = JsonUtility.FromJson<Settings>(System.IO.File.ReadAllText(Application.dataPath + "/settings.json"));
        Text welcomePatientText = GetComponent<Text>();
        welcomePatientText.text = "¡Bienvenido " + settings.Login.patient.firstName + "!";

    }
}
