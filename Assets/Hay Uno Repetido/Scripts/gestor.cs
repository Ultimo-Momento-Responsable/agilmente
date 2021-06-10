using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gestor : MonoBehaviour
{
    public int a_figureQuantity = 3;
    public int a_successes = 0;
    public int maxFigures = 21;
    public int maxTime = 60;
    public bool limitTime = true;
    public bool limitFigure;
    public List<int> a_timeBetweenSuccess;
    private int auxTime; 
    public GameObject figure;
    public Sprite[] sprites;
    public AudioSource audioSource;
    public AudioClip sndSuccess;
    private List<int> index;
    public int a_mistakes;
    public int a_totalTime;
    public int initTime;
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
        initTime = (int)Time.time;
        auxTime = initTime;
    }

    void Update()
    {
        if (!isFinished)
        {
            a_totalTime = (int)Time.time - initTime;
            // Acá verifica si la figura correcta fue tocada, en ese caso sube un nivel.
            if (isTouching && a_figureQuantity > 0)
            {

                a_timeBetweenSuccess.Add((int)Time.time - auxTime);
                auxTime = (int)Time.time;
                audioSource.PlayOneShot(sndSuccess);

                if (a_figureQuantity < maxFigures)
                {
                    a_figureQuantity++;
                    a_successes++;
                }

                resetValues();
            }
            if ((limitFigure && a_figureQuantity >= maxFigures) || (limitTime && (int)Time.time >= maxTime))
            {
                sendData();
            }
        }
        if (isMakingMistake) {
            isMakingMistake = false;
            Camera.main.GetComponent<ScreenShake>().TriggerShake(0.5f);
        }
        
    }

    private void OnApplicationQuit()
    {
        canceled = true;
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

    // Función que selecciona las figuras que se mostrarán en pantalla por cada nivel.
    void chooseSprites()
    {
        int repeatedIndex = (int)Random.Range(0, sprites.Length);
        index.Add(repeatedIndex);
        index.Add(repeatedIndex);
        for (int i = 2; i < a_figureQuantity; i++)
        {
            int randIndex = (int)Random.Range(0, sprites.Length);
            while (index.Contains(randIndex))
            {
                randIndex = (int)Random.Range(0, sprites.Length);
            }
            index.Add(randIndex);
        }
    }

    //Función que instancia las figuras que se mostrarán al inicio y al aumentar cada nivel
    void createFigures()
    {
        for (int i = 0; i < a_figureQuantity; i++)
        {

            Vector2 randomPositionOnScreen = Camera.main.ViewportToWorldPoint(new Vector2(Random.value, Random.value));
            randomPositionOnScreen = centerFigures(randomPositionOnScreen);

            while (thereIsSomethingIn(randomPositionOnScreen))
            {
                randomPositionOnScreen = Camera.main.ViewportToWorldPoint(new Vector2(Random.value, Random.value));
                randomPositionOnScreen = centerFigures(randomPositionOnScreen);
            }

            GameObject fig = Instantiate(figure, randomPositionOnScreen, Quaternion.identity);
            
            fig.GetComponent<Behaviour>().sprite = sprites[index[i]];
            fig.GetComponent<Behaviour>().controller = this;
            fig.GetComponent<Behaviour>().index = i;
            if (i < 2)
            {
                GameObject part = Instantiate(particles, randomPositionOnScreen, Quaternion.identity);
                fig.GetComponent<Behaviour>().ps = part.GetComponent<ParticleSystem>();
            }

        }
    }

    // Función que chequea que no haya nada en el lugar donde se crea la figura.
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

    //Función que centra las figuras, esto se hace para evitar que las mismas se generen en los bordes.
    Vector2 centerFigures(Vector2 randomPositionOnScreen)
    {
        if (randomPositionOnScreen.x < 0)
        {
            randomPositionOnScreen.x += 0.5f;
        }
        else
        {
            randomPositionOnScreen.x -= 0.5f;
        }
        if (randomPositionOnScreen.y < 0)
        {
            randomPositionOnScreen.y += 0.5f;
        }
        else
        {
            randomPositionOnScreen.y -= 0.5f;
        }
        return randomPositionOnScreen;
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
        WWW www = new WWW("http://localhost:8080/hay-uno-repetido", postData, parameters);
        StartCoroutine(Upload(www));
        resetValues();
    }
}