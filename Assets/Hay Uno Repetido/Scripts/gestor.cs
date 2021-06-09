using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class gestor : MonoBehaviour
{
    public int figureQuantity = 3;
    public int maxFigures = 21;
    public int maxTime;
    public bool limitTime;
    public bool limitFigure;
    public List<int> timeBetweenSuccess;
    private int auxTime; 
    public GameObject figure;
    public Sprite[] sprites;
    public AudioSource audioSource;
    public AudioClip sndSuccess;
    private List<int> index;
    public int mistakes;
    public int totalTime;
    public bool isTouching = false;
    private string json;

    private bool isFinished = false;


    void Start()
    {
        
        index = new List<int>();
        chooseSprites();
        createFigures();
        totalTime = (int)Time.time;
        auxTime = totalTime;
    }

    void Update()
    {
        if (!isFinished)
        {
            // Acá verifica si la figura correcta fue tocada, en ese caso sube un nivel.
            if (isTouching && figureQuantity > 0)
            {

                timeBetweenSuccess.Add((int)Time.time - auxTime);
                auxTime = (int)Time.time;
                audioSource.PlayOneShot(sndSuccess);

                if (figureQuantity < maxFigures)
                {
                    figureQuantity++;
                }

                resetValues();
            }
            if ((limitFigure && figureQuantity >= maxFigures) || (limitTime && (int)Time.time >= maxTime))
            {
                sendData();
            }
        }
        
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
        for (int i = 2; i < figureQuantity; i++)
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
        for (int i = 0; i < figureQuantity; i++)
        {

            Vector2 randomPositionOnScreen = Camera.main.ViewportToWorldPoint(new Vector2(Random.value, Random.value));
            randomPositionOnScreen = centerFigures(randomPositionOnScreen);

            while (thereIsSomethingIn(randomPositionOnScreen))
            {
                randomPositionOnScreen = Camera.main.ViewportToWorldPoint(new Vector2(Random.value, Random.value));
                randomPositionOnScreen = centerFigures(randomPositionOnScreen);
            }

            GameObject veg = Instantiate(figure, randomPositionOnScreen, Quaternion.identity);
            veg.GetComponent<behaviour>().sprite = sprites[index[i]];
            veg.GetComponent<behaviour>().controller = this;
            veg.GetComponent<behaviour>().index = i;
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
            Debug.Log(www.text);
        }
        else
        {
            //Something goes wrong, print the error response
            Debug.Log(www.error);
        }
    }

    void sendData()
    {
        isFinished = true;
        totalTime = (int)Time.time - totalTime;
        figureQuantity = -1;
        resetValues();
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        json = "{'nombre': 'Hay uno repetido', 'tiempo': " + totalTime + ", 'errores': " + mistakes + ", 'fechaYHora': '" +
            System.DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss") + "'}";
        parameters.Add("Content-Type", "application/json");
        parameters.Add("Content-Length", json.Length.ToString());
        json = json.Replace("'", "\"");
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(json);
        //Now we call a new WWW request
        WWW www = new WWW("http://localhost:8080/juego", postData, parameters);
        //And we start a new co routine in Unity and wait for the response.
        StartCoroutine(Upload(www));

    }
}