using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HayUnoRepetidoController : MonoBehaviour
{

    private const string DEV_ENDPOINT = "localhost:8080/hay-uno-repetido";
    private const string PROD_ENDPOINT = "200.127.223.168:8082/hay-uno-repetido";

    public int a_figureQuantity = 3;
    public int a_successes = 0;
    public int maxFigures = 20;
    public float maxTime = 60f;
    public bool limitTime = true;
    public bool limitFigure;
    public List<float> a_timeBetweenSuccess;
    private float auxTime; 
    public GameObject figure;
    public Sprite[] sprites;
    public AudioSource audioSource;
    public AudioClip sndSuccess;
    private List<int> index;
    public int a_mistakes;
    public float a_totalTime;
    public float initTime;
    public bool isTouching = false;
    public bool isMakingMistake = false;
    private string json;
    private bool canceled = false;

    private bool isFinished = false;

    public GameObject particles;

    void Start()
    {
        index = new List<int>();
        chooseSprites();
        createFigures();
        initTime = Time.time;
        auxTime = initTime;
    }

    void Update()
    {
        if (!isFinished)
        {
            a_totalTime = Time.time - initTime;

            if (isTouching && a_figureQuantity > 0)
            {
                a_timeBetweenSuccess.Add(Time.time - auxTime);
                auxTime = Time.time;
                audioSource.PlayOneShot(sndSuccess);

                if (a_figureQuantity < maxFigures)
                {
                    a_figureQuantity++;
                    a_successes++;
                }

                resetValues();
            }
            if ((limitFigure && a_figureQuantity >= maxFigures) || (limitTime && Time.time >= maxTime))
            {
                sendData();
            }
        }
        if (isMakingMistake) {
            isMakingMistake = false;
            Camera.main.GetComponent<ScreenShake>().TriggerShake(0.2f);
        }
        
    }

    private void OnApplicationQuit()
    {
        canceled = true;
        sendData();
    }

    /// <summary>
    /// Recalcula la posición de los sprites y los reubica en la pantalla.
    /// </summary>
    private void resetValues()
    {
        var objects = GameObject.FindGameObjectsWithTag("figures");
        foreach (GameObject o in objects)
        {
            Destroy(o.gameObject);
        }
        index = new List<int>();
        chooseSprites();
        createFigures();
        isTouching = false;
    }

    /// <summary>
    /// Función que selecciona las figuras que se mostrarán en pantalla por 
    /// cada nivel.
    /// </summary>
    void chooseSprites()
    {
        int repeatedIndex = (int)UnityEngine.Random.Range(0, sprites.Length);
        index.Add(repeatedIndex);
        index.Add(repeatedIndex);
        for (int i = 2; i < a_figureQuantity; i++)
        {
            int randIndex = (int)UnityEngine.Random.Range(0, sprites.Length);
            while (index.Contains(randIndex))
            {
                randIndex = (int)UnityEngine.Random.Range(0, sprites.Length);
            }
            index.Add(randIndex);
        }
    }
    /// <summary>
    /// Función que instancia las figuras que se mostrarán al inicio y al 
    /// aumentar cada nivel.
    /// </summary>
    void createFigures()
    {
        for (int i = 0; i < a_figureQuantity; i++)
        {

            Vector2 RandomPositionOnScreen = Camera.main.ViewportToWorldPoint(new Vector2(UnityEngine.Random.value, UnityEngine.Random.value));
            RandomPositionOnScreen = centerFigures(RandomPositionOnScreen);

            while (thereIsSomethingIn(RandomPositionOnScreen))
            {
                RandomPositionOnScreen = Camera.main.ViewportToWorldPoint(new Vector2(UnityEngine.Random.value, UnityEngine.Random.value));
                RandomPositionOnScreen = centerFigures(RandomPositionOnScreen);
            }

            GameObject fig = Instantiate(figure, RandomPositionOnScreen, Quaternion.identity);
            
            fig.GetComponent<FigureBehaviour>().sprite = sprites[index[i]];
            fig.GetComponent<FigureBehaviour>().controller = this;
            fig.GetComponent<FigureBehaviour>().index = i;
            if (i < 2)
            {
                GameObject part = Instantiate(particles, RandomPositionOnScreen, Quaternion.identity);
                fig.GetComponent<FigureBehaviour>().ps = part.GetComponent<ParticleSystem>();
            }

        }
    }

    /// <summary>
    /// Función que chequea que no haya nada en el lugar donde se crea la figura.
    /// </summary>
    bool thereIsSomethingIn(Vector2 posición)
    {
        Vector2 p1 = posición - new Vector2(0.2f, 0.2f);
        Vector2 p2 = posición + new Vector2(0.2f, 0.2f);
        Collider2D collider = Physics2D.OverlapArea(p1, p2);

        if (collider != null)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Función que centra las figuras, esto se hace para evitar que las mismas
    /// se generen en los bordes.
    /// </summary>
    Vector2 centerFigures(Vector2 RandomPositionOnScreen)
    {
        if (RandomPositionOnScreen.x < 0)
        {
            RandomPositionOnScreen.x += 0.5f;
        }
        else
        {
            RandomPositionOnScreen.x -= 0.5f;
        }
        if (RandomPositionOnScreen.y < 0)
        {
            RandomPositionOnScreen.y += 0.5f;
        }
        else
        {
            RandomPositionOnScreen.y -= 0.5f;
        }
        return RandomPositionOnScreen;
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

    /// <summary>
    /// Función que se encarga de armar el HTTP Request y enviarlo al backend 
    /// (agilmente-core).
    /// </summary>
    void sendData()
    {
        isFinished = true;
        a_figureQuantity = -1;
        
        string tBS = "[";
        foreach (float v in a_timeBetweenSuccess)
        {
            tBS +=  v.ToString().Replace(",", ".") + ",";
        }
        tBS = tBS.Remove(tBS.Length - 1);
        tBS += "]";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        json = "{'name': 'Hay Uno Repetido', 'totalTime': " + a_totalTime.ToString().Replace(",", ".") + ", 'mistakes': " + a_mistakes +
            ", 'successes': " + a_successes + ", 'timeBetweenSuccesses': " + tBS + ", 'canceled': " + canceled +", 'dateTime': '" +
            System.DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss") + "'}";
        json = json.Replace("False", "false");
        json = json.Replace("True", "true");
        parameters.Add("Content-Type", "application/json");
        parameters.Add("Content-Length", json.Length.ToString());
        json = json.Replace("'", "\"");
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(json);
        WWW www = new WWW(DEV_ENDPOINT, postData, parameters);
        StartCoroutine(Upload(www));
        resetValues();
    }
}