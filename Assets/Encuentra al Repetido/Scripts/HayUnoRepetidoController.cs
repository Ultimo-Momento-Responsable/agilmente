using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MainSceneController;

public class HayUnoRepetidoController : GameController
{

    private const string ENDPOINT = "results/encuentra-al-repetido";

    private List<int> index;
    private string json;
    private bool canceled = false;
    private int dontTouchTimer = 20;

    public Camera camera;
    public GameObject figure;
    public Sprite[] sprites;
    public Sprite[] distractorsSprites;
    public AudioSource audioSource;
    public AudioClip sndSuccess;
    public HayUnoRepetido hayUnoRepetido;
    public GameObject particles;
    public Text timer;
    public GameObject tutorial;
    private GameObject tutorialHand;
    public GameObject title;
    public GameObject handPref;
    public GameObject pauseButton;
    public Text scoreHUD;


    public int figureQuantity;
    private int maxLevel;
    private int maxFigures;
    private float maxTime;
    private bool limitTime = true;
    private bool limitFigure = true;
    public bool variableSizes;
    public bool distractors;
    public float auxTime;
    public float initTime;
    public bool isTouching = false;
    public bool isMakingMistake = false;
    public bool dontTouchAgain = false;
    public bool onTutorial = true;
    private GameObject[] a_figures;
    public GameObject endScreen;
    public GameObject background;
    public GameObject HUD;

    public GameObject[] figures
    {
        get
        {
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
        hayUnoRepetido = new HayUnoRepetido(this);
        maxLevel = SessionHayUnoRepetido.maxLevel;
        maxTime = SessionHayUnoRepetido.maxTime;
        variableSizes = SessionHayUnoRepetido.variableSizes;
        distractors = SessionHayUnoRepetido.distractors;
        maxFigures = SessionHayUnoRepetido.figureQuantity;
        sprites = Resources.LoadAll<Sprite>("Sprites/Figures/SpriteSet" + SessionHayUnoRepetido.spriteSet + "/");
        if (maxTime == -1)
        {
            limitTime = false;
        }
        if (maxLevel == -1)
        {
            limitFigure = false;
            maxLevel = 20;
        }
        index = hayUnoRepetido.chooseSprites(sprites, figureQuantity);
        hayUnoRepetido.createFigures(figureQuantity, camera, figure, sprites, index, this, particles);
        figures = GameObject.FindGameObjectsWithTag("figures");
    }

    void Update()
    {
        if (hayUnoRepetido.onTutorial)
        {
            timer.text = "Tutorial";
            if (isTouching)
            {
                dontTouchAgain = true;
                audioSource.PlayOneShot(sndSuccess);
                hayUnoRepetido.onTutorial = false;
                tutorial.SetActive(false);
                GameObject.FindGameObjectWithTag("tutorialhand").SetActive(false);
                title.SetActive(false);
                tutorial.GetComponent<Text>().text = "";
                resetValues();
                initTime = Time.time;
                auxTime = initTime;
                hayUnoRepetido.setStartTime();
                pauseButton.SetActive(true);
            }
        } 
        else 
        {
            hayUnoRepetido.totalTime = Time.time - initTime;
            if (limitTime)
            {
                timer.text = ((int)maxTime - (int)hayUnoRepetido.totalTime).ToString();
            }
            else
            {
                timer.text = (hayUnoRepetido.successes + 1).ToString() + " / " + maxLevel.ToString();
                scoreHUD.text = hayUnoRepetido.score.ToString();
            }

            if (isTouching && figureQuantity > 0 && !dontTouchAgain)
            {
                dontTouchAgain = true;
                hayUnoRepetido.addSuccess(figureQuantity);
                audioSource.PlayOneShot(sndSuccess);
                                
                if (limitFigure && ((hayUnoRepetido.successes + 1) > maxLevel))
                {
                    HUD.gameObject.SetActive(false);
                    timer.text = hayUnoRepetido.successes.ToString() + " / " + maxLevel.ToString();
                    removeFigures();
                    sendData();
                }
                else
                {
                    if (figureQuantity < maxFigures)
                    {
                        figureQuantity++;
                    }
                    resetValues();
                }
            }

            if (limitTime && (hayUnoRepetido.totalTime >= maxTime))
            {
                removeFigures();
                sendData();
            }

            if (isMakingMistake)
            {
                isMakingMistake = false;
                hayUnoRepetido.addMistake(figureQuantity);
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
    public void resetValues()
    {
        removeFigures();
        
        index = hayUnoRepetido.chooseSprites(sprites, figureQuantity);
        hayUnoRepetido.createFigures(figureQuantity, camera, figure, sprites, index, this, particles);
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
        if(hayUnoRepetido.successes > 0 | hayUnoRepetido.mistakes > 0) { 
            showEndScreen(this.hayUnoRepetido.score);
            figureQuantity = -1;

            string tBS;
            if (hayUnoRepetido.successes > 0)
            {
                tBS = "[";
                foreach (float v in hayUnoRepetido.timeBetweenSuccesses)
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
            string totalTime = hayUnoRepetido.totalTime.ToString().Replace(",", ".");
            if (limitTime)
            {
                totalTime = maxTime.ToString();
            }
            limitTime = false;
            limitFigure = false;
            json =
                "{" +
                    "'completeDatetime': '" + System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") +
                    "', 'canceled': " + canceled +
                    ", 'mistakes': " + hayUnoRepetido.mistakes +
                    ", 'successes': " + hayUnoRepetido.successes +
                    ", 'timeBetweenSuccesses': " + tBS +
                    ", 'totalTime': " + totalTime +
                    ", 'game': 'Encuentra al Repetido'" +
                    ", 'hayUnoRepetidoSessionId': " + SessionHayUnoRepetido.gameSessionId +
                    ", 'score': " + hayUnoRepetido.score + "}";
            SendData sD = (new GameObject("SendData")).AddComponent<SendData>();
            sD.sendData(json, ENDPOINT);
        }
    }

    /// <summary>
    /// Función que se encarga de pausar/despausar el juego.
    /// </summary>
    public override void pauseGame()
    {
        if (hayUnoRepetido.onTutorial)
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
        }
        foreach (GameObject f in figures)
        {
            f.SetActive(false);
        }

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
        if (hayUnoRepetido.onTutorial)
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
        if (hayUnoRepetido.onTutorial)
        {
            goToMainScene();
        }
        else
        {
            this.pause.pauseGame();
        }
    }
}