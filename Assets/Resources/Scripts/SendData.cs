using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendData : MonoBehaviour
{
    public static string IP_PROD = "http://agilmente.tk:8080/";
    public static string IP_DEV = "http://localhost:8080/";
    public static string IP = IP_PROD;

    /// <summary>
    /// Funci�n que se encarga de armar el HTTP Request y enviarlo al backend 
    /// (agilmente-core).
    /// </summary>
    public void sendData(string json, string endpoint)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        json = json.Replace("False", "false");
        json = json.Replace("True", "true");
        parameters.Add("Content-Type", "application/json");
        parameters.Add("Content-Length", json.Length.ToString());
        json = json.Replace("'", "\"");
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(json);
        WWW www = new WWW(IP + endpoint, postData, parameters);
        StartCoroutine(Upload(www));
    }

    /// <summary>
    /// Funci�n que se encarga de enviar los datos al backend (agilmente-core).
    /// </summary>
    /// <param name="www">Request HTTP (POST).</param>
    /// <returns>Corrutina.</returns>
    public IEnumerator Upload(WWW www)
    {
        yield return www;
    }
}
