using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MainSceneController;

public class EncuentraAlNuevoController : GameController
{
    private const string DEV_ENDPOINT = "localhost:8080/results/encuentra-al-nuevo";
    private const string PROD_ENDPOINT = "3.23.85.46:8080/results/encuentra-al-nuevo";

    private List<int> actualSprites;
    private List<int> index;
    private string json;
    private bool canceled = false;
    private int dontTouchTimer = 20;
    private GameObject[] a_figures;

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
    private int maxLevel;
    private float maxTime;
    private bool limitTime = true;
    private bool limitLevel = true;
    public float auxTime;
    public float initTime;
    public bool isTouching = false;
    public bool isMakingMistake = false;
    public bool dontTouchAgain = false;
    public bool onTutorial = true;
    public GameObject endScreen;
    public bool variableSizes;

    public GameObject[] figures {
        get {
            if (!pause.isPaused)
            {
                a_figures = GameObject.FindGameObjectsWithTag("figures");
            }
            return a_figures;
        }
        set => a_figures = value; 
    }

    void Start()
    {
        endScreen.SetActive(false);
        encuentraAlNuevo = new EncuentraAlNuevo(this);
        maxLevel = SessionEncuentraAlNuevo.maxLevel;
        maxTime = SessionEncuentraAlNuevo.maxTime;
        variableSizes = SessionEncuentraAlNuevo.variableSizes;
        sprites = Resources.LoadAll<Sprite>("Sprites/Figures/SpriteSet" + SessionEncuentraAlNuevo.spriteSet + "/");

        if (maxTime == -1)
        {
            limitTime = false;
        }
        if (maxLevel == -1)
        {
            limitLevel = false;
            maxLevel = 17;
        }
        figureQuantity = 2;
        actualSprites = encuentraAlNuevo.intialSprites(sprites);
        index = encuentraAlNuevo.chooseSprites(sprites, actualSprites);
        encuentraAlNuevo.createFigures(figureQuantity, camera, figure, sprites, index, this, particles);
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
                encuentraAlNuevo.setStartTime();
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
                encuentraAlNuevo.addSuccess(figureQuantity);
                audioSource.PlayOneShot(sndSuccess);
                
                if ((limitLevel && ((encuentraAlNuevo.successes + 1) > maxLevel)) || (figureQuantity > 20))
                {
                    sendData();
                }
                else
                {
                    figureQuantity++;
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
                encuentraAlNuevo.addMistake(figureQuantity);
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
            buttonPauseEvent();
        }

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
        
        index = encuentraAlNuevo.chooseSprites(sprites, actualSprites);

        encuentraAlNuevo.createFigures(figureQuantity, camera, figure, sprites, index, this, particles);
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
    public override void sendData()
    {
        showEndScreen(this.encuentraAlNuevo.score);
        figureQuantity = -1;
        limitTime = false;
        limitLevel = false;
        string tBS;
        if (encuentraAlNuevo.successes > 0)
        {
            tBS = "[";
            foreach (float v in encuentraAlNuevo.timeBetweenSuccesses)
            {
                if (v == 0)
                {
                    break;
                }
                tBS += v.ToString().Replace(",", ".") + ",";
            }
            tBS = tBS.Remove(tBS.Length - 1);
            tBS += "]";
        } else
        {
            tBS = "null";
        }

        json = "{'completeDatetime': '" + System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") +
            "', 'canceled': " + canceled +
            ", 'mistakes': " + encuentraAlNuevo.mistakes +
            ", 'successes': " + encuentraAlNuevo.successes +
            ", 'timeBetweenSuccesses': " + tBS +
            ", 'totalTime': " + encuentraAlNuevo.totalTime.ToString().Replace(",", ".") +
            ", 'game': 'Encuentra al Nuevo'" +
            ", 'encuentraAlNuevoSessionId': " + SessionEncuentraAlNuevo.gameSessionId + "}";

        SendData sD = (new GameObject("SendData")).AddComponent<SendData>();
        sD.sendData(json, DEV_ENDPOINT);
    }

    /// <summary>
    /// Función que se encarga de pausar el juego.
    /// </summary>
    public override void pauseGame()
    {
        foreach (GameObject f in figures)
        {
            f.SetActive(false);
        }
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

    /// <summary>
    /// Setea el estado del juego a cancelado.
    /// </summary>
    public override void cancelGame()
    {
        this.canceled = true;
    }

    /// <summary>
    /// Función que se encarga de reanudar el juego.
    /// </summary>
    public override void unpauseGame()
    {
        foreach (GameObject f in figures)
        {
            f.SetActive(true);
        }
    }

    /// <summary>
    /// Muestra la pantalla de fin del juego con el puntaje.
    /// </summary>
    /// <param name="score">Puntaje final.</param>
    public void showEndScreen(int score)
    {
        pause.gameObject.SetActive(false);
        GameObject.Find("Timer").SetActive(false);
        endScreen.SetActive(true);
        endScreen.transform.Find("Score").GetComponent<Text>().text = score.ToString();
    }
}