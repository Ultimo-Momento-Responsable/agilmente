using System.Runtime.InteropServices;
using UnityEngine;

public static class SettingsStorage
{
    /// <summary>
    /// Verifica si el paciente ya tiene un objeto de settings creado.
    /// </summary>
    /// <returns>Verdadero si ya existen configuraciones creadas.</returns>
    public static bool hasCreatedSettings()
    {
#if UNITY_WEBGL
        return hasSettingsInLocalStorage() == 1;
#else
        return System.IO.File.Exists(Application.persistentDataPath + "/settings.json");
#endif
    }

    /// <summary>
    /// Crea y guarda las configuraciones.
    /// En caso que ya existan configuraciones se sobreescriben con 
    /// configuraciones en blanco.
    /// </summary>
    public static void createSettings()
    {
        Settings settings = new Settings();
        settings.Login = new Login();
        settings.Login.isLogged = false;
        settings.Login.patient = new Patient();

        saveSettings(settings);
    }

    /// <summary>
    /// Guarda las configuraciones para que persistan después que se cierra la app.
    /// </summary>
    /// <param name="settings">Configuraciones.</param>
    public static void saveSettings(Settings settings)
    {
#if UNITY_WEBGL
        saveSettingsToLocalStorage(JsonUtility.ToJson(settings));
#else
        File.WriteAllText(Application.persistentDataPath + "/settings.json", JsonUtility.ToJson(settings));
#endif
    }

    /// <summary>
    /// Obtiene las configuraciones guardadas del paciente.
    /// </summary>
    /// <returns>Las configuraciones.</returns>
    public static Settings loadSettings()
    {
#if UNITY_WEBGL
        return JsonUtility.FromJson<Settings>(loadSettingsFromLocalStorage());
#else
        return JsonUtility.FromJson<Settings>(System.IO.File.ReadAllText(Application.persistentDataPath + "/settings.json"));
#endif
    }

#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern string loadSettingsFromLocalStorage();
    [DllImport("__Internal")]
    private static extern void saveSettingsToLocalStorage(string data);

    [DllImport("__Internal")]
    private static extern int hasSettingsInLocalStorage();
#endif
}