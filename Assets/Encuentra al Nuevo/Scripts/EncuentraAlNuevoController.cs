using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MainSceneController;

public class EncuentraAlNuevoController : GameController
{
    private const string ENDPOINT = "result/encuentra-al-nuevo";

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
    public AudioClip sndSuccess;
    public EncuentraAlNuevo encuentraAlNuevo;
    public GameObject particles;
    public Text timer;
    public GameObject tutorial;
    private GameObject tutorialHand;
    public GameObject handPref;
    public GameObject title;
    public GameObject pauseButton;
    public GameObject startButton;
    public GameObject HUD;
    public Text scoreHUD;

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
            timer.text = "Tutorial";
            if (isTouching && !prevTutorial)
            {
                dontTouchAgain = true;
                PlaySound(sndSuccess);
                encuentraAlNuevo.onTutorial = false;
                tutorial.SetActive(false);
                GameObject.FindGameObjectWithTag("tutorialhand").SetActive(false);
                title.SetActive(false);
                tutorial.GetComponent<Text>().text = "";
                figureQuantity++;
                resetValues();
                initTime = Time.time;
                auxTime = initTime;
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
                timer.text = (encuentraAlNuevo.successes + 1).ToString() + " / " + maxLevel.ToString();
                scoreHUD.text = encuentraAlNuevo.score.ToString();
            }

            if (isTouching && figureQuantity > 0 && !dontTouchAgain)
            {
                dontTouchAgain = true;
                encuentraAlNuevo.addSuccess(figureQuantity);
                PlaySound(sndSuccess);
                if ((limitLevel && ((encuentraAlNuevo.successes + 1) > maxLevel)))
                {
                    HUD.gameObject.SetActive(false);
                    removeFigures();
                    sendData();
                }
                else
                {
                    figureQuantity++;
                    if (figureQuantity < 21)
                    {
                        resetValues();
                    }
                }
            }

            if ((limitTime && (encuentraAlNuevo.totalTime >= maxTime)) || figureQuantity > 20)
            {
                HUD.gameObject.SetActive(false);
                timer.text = encuentraAlNuevo.successes.ToString() + " / " + maxLevel.ToString();
                removeFigures();
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
    /// Elimina las figuras que hay en la pantalla
    /// </summary>
    private void removeFigures()
    {
        var objects = GameObject.FindGameObjectsWithTag("figures");
        foreach (GameObject o in objects)
        {
            Destroy(o.gameObject);
        }
    }

    /// <summary>
    /// Recalcula la posición de los sprites y los reubica en la pantalla.
    /// </summary>
    private void resetValues()
    {
        removeFigures();
        
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
        if (encuentraAlNuevo.successes > 0 | encuentraAlNuevo.mistakes > 0) { 
            showEndScreen(this.encuentraAlNuevo.score);
            figureQuantity = -1;
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
            }
            else
            {
                tBS = "null";
            }
            string totalTime = encuentraAlNuevo.totalTime.ToString().Replace(",", ".");
            if (limitTime)
            {
                totalTime = maxTime.ToString();
            }
            limitTime = false;
            limitLevel = false;

            json = "{'completeDatetime': '" + System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") +
                "', 'canceled': " + canceled +
                ", 'mistakes': " + encuentraAlNuevo.mistakes +
                ", 'successes': " + encuentraAlNuevo.successes +
                ", 'timeBetweenSuccesses': " + tBS +
                ", 'totalTime': " + totalTime +
                ", 'game': 'Encuentra al Nuevo'" +
                ", 'encuentraAlNuevoSessionId': " + SessionEncuentraAlNuevo.gameSessionId +
                ", 'score': " + encuentraAlNuevo.score + "}";

            SendData sD = (new GameObject("SendData")).AddComponent<SendData>();
            sD.sendData(json, ENDPOINT);
        }
    }

    /// <summary>
    /// Función que se encarga de pausar el juego.
    /// </summary>
    public override void pauseGame()
    {
        if (encuentraAlNuevo.onTutorial)
        {
            if (tutorial)
            {
                tutorial.SetActive(false);
            }
            tutorialHand = GameObject.FindGameObjectWithTag("tutorialhand");
            if (tutorialHand)
            {
                tutorialHand.SetActive(false);
            }
            if (title)
            {
                title.SetActive(false);
            }
            if (prevTutorial)
            {
                startButton.SetActive(false);
            }
        }
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
        tutorial.GetComponent<Text>().text = "Presiona la nueva figura";
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
        if (encuentraAlNuevo.onTutorial)
        {
            if (tutorial)
            {
                tutorial.SetActive(true);
            }
            if (tutorialHand)
            {
                tutorialHand.SetActive(true);
            }
            if (title)
            {
                title.SetActive(true);
            }
            if (prevTutorial)
            {
                startButton.SetActive(true);
            }
        }
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
        PlayGameOverSound();
        pauseButton.SetActive(false);
        pause.gameObject.SetActive(false);
        endScreen.SetActive(true);
        endScreen.transform.Find("Score").GetComponent<Text>().text = score.ToString();
    }

    /// <summary>
    /// Pausa o vuelve al main menu, cuando la app pierde el focus.
    /// </summary>
    public override void OnApplicationPause()
    {
        this.pause.pauseGame();
    }
}