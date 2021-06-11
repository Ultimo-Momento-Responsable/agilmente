using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gestor : MonoBehaviour
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
            // Ac� verifica si la figura correcta fue tocada, en ese caso sube un nivel.
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

    // Funci�n que selecciona las figuras que se mostrar�n en pantalla por cada nivel.
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
        Debug.Log(a_figureQuantity);
        Console.WriteLine(String.Join(", ", index));
    }

    //Funci�n que instancia las figuras que se mostrar�n al inicio y al aumentar cada nivel
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

    // Funci�n que chequea que no haya nada en el lugar donde se crea la figura.
    bool thereIsSomethingIn(Vector2 posici�n)
    {
        Vector2 p1 = posici�n - new Vector2(0.2f, 0.2f);
        Vector2 p2 = posici�n + new Vector2(0.2f, 0.2f);
        Collider2D collider = Physics2D.OverlapArea(p1, p2);

        if (collider != null)
        {
            return true;
        }
        return false;
    }

    //Funci�n que centra las figuras, esto se hace para evitar que las mismas se generen en los bordes.
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

    public IEnumerator Upload(WWW www)
    {
        yield return www;
        if (www.error == null)
        {
            //Print server response
        }
        else
        {
            //Something goes wrong, print the error response
        }
    }

    void sendData()
    {
        isFinished = true;
        a_figureQuantity = -1;
        


        string tBS = "[";
        foreach (int v in a_timeBetweenSuccess)
        {
            tBS +=  v.ToString() + ",";
        }
        tBS = tBS.Remove(tBS.Length - 1);
        tBS += "]";
        //Se genera el JSON para ser enviado al endpoint
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        json = "{'name': 'Hay Uno Repetido', 'totalTime': " + a_totalTime + ", 'mistakes': " + a_mistakes +
            ", 'successes': " + a_successes + ", 'timeBetweenSuccesses': " + tBS + ", 'canceled': " + canceled +", 'dateTime': '" +
            System.DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss") + "'}";
        json = json.Replace("False", "false");
        json = json.Replace("True", "true");
        parameters.Add("Content-Type", "application/json");
        parameters.Add("Content-Length", json.Length.ToString());
        json = json.Replace("'", "\"");
        Debug.Log(json);
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(json);
        WWW www = new WWW(DEV_ENDPOINT, postData, parameters);
        StartCoroutine(Upload(www));
        resetValues();
    }
}