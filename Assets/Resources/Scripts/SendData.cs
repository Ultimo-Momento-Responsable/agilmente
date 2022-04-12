using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendData : MonoBehaviour
{
    public static string IP_PROD = "http://www.agilmente.tk:8009/";
    public static string IP_DEV = "http://localhost:8009/";
    public static string IP = IP_PROD;
    public static string IP_PIPEDREAM = "https://c8a17f3bf731698d2108b5d4ccbf71ab.m.pipedream.net";

    /// <summary>
    /// Función que se encarga de armar el HTTP Request y enviarlo al backend 
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

    public void sendDataPipedream(string json)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        json = json.Replace("False", "false");
        json = json.Replace("True", "true");
        parameters.Add("Content-Type", "application/json");
        parameters.Add("Content-Length", json.Length.ToString());
        json = json.Replace("'", "\"");
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(json);
        WWW pipedream = new WWW(IP_PIPEDREAM, postData, parameters);
        StartCoroutine(Upload(pipedream));
    }

    /// <summary>
    /// Función que se encarga de enviar los datos al backend (agilmente-core).
    /// </summary>
    /// <param name="www">Request HTTP (POST).</param>
    /// <returns>Corrutina.</returns>
    public IEnumerator Upload(WWW www)
    {
        yield return www;
    }
}
