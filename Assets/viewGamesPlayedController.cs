using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class viewGamesPlayedController : MonoBehaviour
{
    public Button gameCard;
    private Settings settings;
    public GameObject gameCanvas;
    public GameObject cardContainer;
    public Text dateText;
    public Text noGames;
    private static string endpoint = "/results/by-patient-ordered/";
    void Start()
    {
        getResults();
    }

    /**
     * Obtiene los resultados del paciente logueado ordenados.
     */
    public void getResults()
    {

        settings = JsonUtility.FromJson<Settings>(System.IO.File.ReadAllText(Application.persistentDataPath + "/settings.json"));
        this.StartCoroutine(this.getResultsRoutine(SendData.IP + endpoint + settings.Login.patient.id, this.getResultsResponseCallback));
    }

    /**
     * Realiza el GET
     */
    private IEnumerator getResultsRoutine(string url, Action<string> callback = null)
    {
        var request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();
        var data = request.downloadHandler.text;

        if (callback != null)
            callback(data);
    }

    /**
     * Callback utilizado en getResults(), asigna a variables de Unity los datos obtenidos del JSON
     */
    private void getResultsResponseCallback(string data)
    {
        ResultsJson resultsRequestJson = JsonUtility.FromJson<ResultsJson>("{\"results\":" + data + "}");
        if (resultsRequestJson.results.Length == 0)
        {
            noGames.gameObject.SetActive(true);
        }
        else
        {
            int i = 0;
            cardContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(cardContainer.GetComponent<RectTransform>().sizeDelta.x, (resultsRequestJson.results.Length * 1.55f));
            float fixPerDateText = 0f;
            string date = "";
            foreach (Result r in resultsRequestJson.results)
            {
                DateTime dateTime = Convert.ToDateTime(r.completeDatetime);
                if (date != dateTime.ToString("dd/MM/yyyy"))
                {
                    date = dateTime.ToString("dd/MM/yyyy");
                    Text dateTextInstance = Instantiate(dateText);
                    dateTextInstance.transform.parent = cardContainer.transform;
                    dateTextInstance.transform.localScale = new Vector2(0.0078f, 0.0078f);
                    dateTextInstance.transform.localPosition = new Vector3(-2f, (i * -1.5f) - fixPerDateText - 0.6f, 0);
                    dateTextInstance.text = date;
                    fixPerDateText += 0.5f;
                }
                string hour = "";
                if (dateTime.Hour < 10)
                {
                    hour += "0";
                }
                hour += dateTime.Hour;
                string minute = "";
                if (dateTime.Minute < 10)
                {
                    minute += "0";
                }
                minute += dateTime.Minute;
                string time = hour + ":" + minute;

                generateCard(i, r.game, time, r.score, fixPerDateText);
                i++;
            }
        }
    }

    /**
     * Genera una card con el resultado del paciente
     */
    private void generateCard(int i, string game, string hour, int score, float fixPerDateText)
    {
        Button gameCardInstance = Instantiate(gameCard);
        gameCardInstance.transform.parent = cardContainer.transform;
        gameCardInstance.transform.localScale = new Vector2(0.0078f, 0.0078f);          //Escala actual del canvas
        gameCardInstance.transform.localPosition = new Vector3(0, -1 + (i * -1.5f) - fixPerDateText, 0); //Tamaño de cards + offset

        foreach (Text texts in gameCardInstance.GetComponentsInChildren<Text>())
        {
            if (texts.gameObject.name == "GameName")
            {
                texts.text = game;
            }
            if (texts.gameObject.name == "Hour")
            {
                texts.text = hour;
            }
            if (texts.gameObject.name == "ScoreValue")
            {
                texts.text = score.ToString();
            }

        }
    }

    /*
     * Vuelve a la MainScene
     */
    public void exitPlayedHistoryScene()
    {
        SceneManager.LoadScene("mainScene");
    }

    [System.Serializable]
    public class Result
    {
        public string game;
        public string completeDatetime;
        public int score;
    }

    public class ResultsJson
    {
        public Result[] results;
    }

}
