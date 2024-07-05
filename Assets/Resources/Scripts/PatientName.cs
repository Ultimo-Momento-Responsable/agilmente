using UnityEngine;
using UnityEngine.UI;

public class PatientName : MonoBehaviour
{
    private Settings settings;

    void Start()
    {
        settings = SettingsStorage.loadSettings();
        Text welcomePatientText = GetComponent<Text>();
        welcomePatientText.text = "¡Hola " + settings.Login.patient.firstName + "!";

    }
}
