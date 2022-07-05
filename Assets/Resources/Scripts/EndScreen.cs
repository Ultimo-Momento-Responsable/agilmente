using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    private string endpoint = "result/ranking";
    public string game;
    public int gameSessionId;
    public string score;
    private List<string> scores;
    public Text rankingScoreText;
    public Text title;
    public Text yourScore;
    public Text rankingLabel;

    /**
     * cuando se clickea el botón de ingresar, busca el paciente en la base de datos.
     */
    public void getScores()
    {
        this.StartCoroutine(this.getScoresRoutine(SendData.IP + endpoint + "?game=" + game + "&id=" + gameSessionId, this.getScoresResponseCallback));
    }
    /**
     * Se hace un get a los pacientes para ver si ese código de Logueo existe
     */
    private IEnumerator getScoresRoutine(string url, Action<string> callback = null)
    {
        var request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();
        var data = request.downloadHandler.text;
        //networkError = request.result == UnityWebRequest.Result.ConnectionError;
        if (callback != null)
        {
            callback(data);
        }
    }

    /**
     * callback del Get a los scores
     * @param data. la info con todos los scores
     */
    private void getScoresResponseCallback (string data)
    {
        data = data.Remove(0,1);
        data = data.Remove(data.Length-1,1);
        var auxScores = data.Split(',');
        scores = auxScores.OfType<string>().ToList();
        scores.Add(score);
        scores.Sort();
        scores.Reverse();
        title.transform.localPosition = new Vector2(0, 472);
        rankingLabel.transform.localPosition = new Vector2(0, 200);

        yourScore.text += score; 
        var i = 1;
        var scoreIndex = scores.IndexOf(score);
        foreach (string s in scores)
        {
            print(s);
            if (i > 5)
            {
                if (s == score)
                {
                    createScoreText(scoreIndex+1, scoreIndex, s);
                    break;
                }
            }else
            {
                createScoreText(i, scoreIndex, s);
                i++;
            }   
        }
    }

    /**
     * Crea el texto de cada uno de los scores del ranking
     * @param i, posición del texto
     * @param scoreIndex, índice del score obtenido.
     * @param score, puntaje a escribir.
     */
    private void createScoreText(int i, int scoreIndex, string score)
    {
        var scoreText = Instantiate(rankingScoreText);
        scoreText.transform.SetParent(transform);
        scoreText.transform.localScale = new Vector2(1, 1);
        var index = i > 6 ? 7: i; 
        if (index == 7)
        {
            var separationText = Instantiate(rankingScoreText);
            separationText.transform.SetParent(transform);
            separationText.transform.localScale = new Vector2(1, 1);
            separationText.transform.localPosition = new Vector2(separationText.transform.position.x, separationText.transform.position.y - 180 - (5 * 70));
            separationText.text = "...";
        }
        scoreText.transform.localPosition = new Vector2(scoreText.transform.position.x, scoreText.transform.position.y - 180 - ((index - 1) * 70));
        if (scoreIndex == i - 1)
        {
            scoreText.fontStyle = FontStyle.Bold;
        }
        scoreText.text = i + "º ------------- " + score;
    }
    /// <summary>
    /// Cambia la escena al menú principal.
    /// </summary>
    public void goToMainScene()
    {
        SceneManager.LoadScene("mainScene");
    }
}
