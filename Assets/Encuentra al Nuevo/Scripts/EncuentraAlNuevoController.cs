using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static MainSceneController;

public class EncuentraAlNuevoController : MonoBehaviour
{

    private const string DEV_ENDPOINT = "localhost:8080/hay-uno-repetido";
    private const string PROD_ENDPOINT = "3.23.85.46:8080/hay-uno-repetido";

    private string json;
    private bool canceled = false;
    private int dontTouchTimer = 20;
    private GameObject[] figures;
    private GameObject pause;

    public bool prevTutorial = true;
    public Camera camera;
    public GameObject figure;
    public Sprite[] sprites;
    public AudioSource audioSource;
    public AudioClip sndSuccess;
    public EncuentraAlNuevo encuentraAlNuevo;
    public GameObject particles;
    public Text timer;
    public Text tutorial;
    public GameObject tutorialHand;
    public GameObject pauseButton;
    public GameObject startButton;


    public int figureQuantity;
    private int maxFigures;
    private float maxTime;
    private bool limitTime = true;
    private bool limitFigure = true;
    private List<int> actualSprites;
    public float auxTime;
    public float initTime;
    public bool isTouching = false;
    public bool isMakingMistake = false;
    public bool dontTouchAgain = false;
    public bool onTutorial = true;
      

    void Start()
    {
        encuentraAlNuevo = new EncuentraAlNuevo(this);
        maxFigures = SessionEncuentraAlNuevo.maxFigures;
        maxTime = SessionEncuentraAlNuevo.maxTime;
        if (maxTime == -1)
        {
            limitTime = false;
        }
        if (maxFigures == -1)
        {
            limitFigure = false;
            maxFigures = 20;
        }
        figureQuantity = 2;
        actualSprites = encuentraAlNuevo.intialSprites(sprites);
        encuentraAlNuevo.createFigures(figureQuantity, camera, figure, sprites, actualSprites, this, particles);
        pause = GameObject.FindGameObjectWithTag("pause");
        pause.SetActive(false);
        figures = GameObject.FindGameObjectsWithTag("figures");
    }

    void Update()
    {
        if (encuentraAlNuevo.onTutorial)
        {
            if (isTouching && !prevTutorial)
            {
                dontTouchAgain = true;
                audioSource.PlayOneShot(sndSuccess);
                encuentraAlNuevo.onTutorial = false;
                GameObject.FindGameObjectWithTag("tutorial").SetActive(false);
                GameObject.FindGameObjectWithTag("tutorialhand").SetActive(false);
                GameObject.FindGameObjectWithTag("title").SetActive(false);
                tutorial.text = "";
                figureQuantity++;
                resetValues();
                initTime = Time.time;
                auxTime = initTime;
                pauseButton.SetActive(true);
            }
        } 
        else 
        {
            encuentraAlNuevo.totalTime = Time.time - initTime;
            if (limitTime)
            {
                timer.text = ((int)maxTime - (int)encuentraAlNuevo.totalTime).ToString();
            }
            else
            {
                timer.text = "Nivel " + (encuentraAlNuevo.successes + 1).ToString();
            }

            if (isTouching && figureQuantity > 0 && !dontTouchAgain)
            {
                dontTouchAgain = true;
                encuentraAlNuevo.timeBetweenSuccesses[encuentraAlNuevo.successes] = Time.time - auxTime;
                auxTime = Time.time;
                audioSource.PlayOneShot(sndSuccess);

                if (figureQuantity <= maxFigures)
                {
                    figureQuantity++;
                }

                encuentraAlNuevo.successes++;
                if ((limitFigure && (figureQuantity >= maxFigures)) || (figureQuantity >= 20))
                {
                    sendData();
                }
                resetValues();
            }

            if (limitTime && (encuentraAlNuevo.totalTime >= maxTime))
            {
                sendData();
            }

            if (isMakingMistake)
            {
                isMakingMistake = false;
                camera.GetComponent<ScreenShake>().TriggerShake(0.1f);
            }
        }

        if (dontTouchAgain)
        {
            dontTouchTimer--;
        }
        if (dontTouchTimer <= 0)
        {
            dontTouchAgain = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseGame();
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
        actualSprites = encuentraAlNuevo.chooseSprites(sprites, actualSprites);
        encuentraAlNuevo.createFigures(figureQuantity, camera, figure, sprites, actualSprites, this, particles);
        isTouching = false;
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
        figureQuantity = -1;
        string tBS = "[";
        foreach (float v in encuentraAlNuevo.timeBetweenSuccesses)
        {
            if (v == 0)
            {
                break;
            }
            tBS +=  v.ToString().Replace(",", ".") + ",";
        }
        tBS = tBS.Remove(tBS.Length - 1);
        tBS += "]";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        json = "{'name': 'Encuentra al Nuevo', 'totalTime': " + encuentraAlNuevo.totalTime.ToString().Replace(",", ".") + ", 'mistakes': " + encuentraAlNuevo.mistakes +
            ", 'successes': " + encuentraAlNuevo.successes + ", 'timeBetweenSuccesses': " + tBS + ", 'canceled': " + canceled +", 'dateTime': '" +
            System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "'}";
        json = json.Replace("False", "false");
        json = json.Replace("True", "true");
        parameters.Add("Content-Type", "application/json");
        parameters.Add("Content-Length", json.Length.ToString());
        json = json.Replace("'", "\"");
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(json);
        WWW www = new WWW(PROD_ENDPOINT, postData, parameters);
        StartCoroutine(Upload(www));
        SceneManager.LoadScene("mainScene");
    }

    /// <summary>
    /// Función que se encarga de pausar/despausar el juego.
    /// </summary>
    public void pauseGame()
    {
        if (Time.timeScale == 1)
        {    //si la velocidad es 1
            Time.timeScale = 0;     //que la velocidad del juego sea 0
            pause.SetActive(true);
            figures = GameObject.FindGameObjectsWithTag("figures");
            foreach (GameObject f in figures)
            {
                f.SetActive(false);
            }
        }
        else if (Time.timeScale == 0)
        {   // si la velocidad es 0
            Time.timeScale = 1;     // que la velocidad del juego regrese a 1
            pause.SetActive(false);
            foreach (GameObject f in figures)
            {
                f.SetActive(true);
            }
        }
    }
    public void backToMainMenu()
    {
        canceled = true;
        sendData();
        pauseGame();
        SceneManager.LoadScene("mainScene");
    }

    /// <summary>
    /// Botón Comenzar, agrega una nueva fruta e inicia la mano del tuto.
    /// </summary>
    public void startBtn()
    {
        figureQuantity = 3;
        prevTutorial = false;
        resetValues();
        startButton.SetActive(false);
        tutorial.text = "Presiona la nueva figura";
    }
}