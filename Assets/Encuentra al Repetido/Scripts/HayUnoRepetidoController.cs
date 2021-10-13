using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static MainSceneController;

public class HayUnoRepetidoController : GameController
{

    private const string DEV_ENDPOINT = "localhost:8080/results/encuentra-al-repetido";
    private const string PROD_ENDPOINT = "3.23.85.46:8080/results/encuentra-al-repetido";

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
    public Text tutorial;
    public GameObject tutorialHand;
    public GameObject pauseButton;


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
            if (isTouching)
            {
                dontTouchAgain = true;
                audioSource.PlayOneShot(sndSuccess);
                hayUnoRepetido.onTutorial = false;
                GameObject.FindGameObjectWithTag("tutorial").SetActive(false);
                GameObject.FindGameObjectWithTag("tutorialhand").SetActive(false);
                GameObject.FindGameObjectWithTag("title").SetActive(false);
                tutorial.text = "";
                resetValues();
                initTime = Time.time;
                auxTime = initTime;
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
                timer.text = "Nivel " + (hayUnoRepetido.successes + 1).ToString();
            }

            if (isTouching && figureQuantity > 0 && !dontTouchAgain)
            {
                dontTouchAgain = true;
                hayUnoRepetido.timeBetweenSuccesses[hayUnoRepetido.successes] = Time.time - auxTime;
                auxTime = Time.time;
                audioSource.PlayOneShot(sndSuccess);

                if (figureQuantity < maxFigures)
                {
                    figureQuantity++;
                }
                hayUnoRepetido.successes++;
                if (limitFigure && ((hayUnoRepetido.successes + 1) > maxLevel))
                {
                    sendData();
                }
                resetValues();
            }

            if (limitTime && (hayUnoRepetido.totalTime >= maxTime))
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

    /// <summary>
    /// Recalcula la posici�n de los sprites y los reubica en la pantalla.
    /// </summary>
    public void resetValues()
    {
        var objects = GameObject.FindGameObjectsWithTag("figures");
        foreach (GameObject o in objects)
        {
            Destroy(o.gameObject);
        }
        
        index = hayUnoRepetido.chooseSprites(sprites, figureQuantity);
        hayUnoRepetido.createFigures(figureQuantity, camera, figure, sprites, index, this, particles);
        isTouching = false;
    }

    /// <summary>
    /// Funci�n que se encarga de enviar los datos al backend (agilmente-core).
    /// </summary>
    /// <param name="www">Request HTTP (POST).</param>
    /// <returns>Corrutina.</returns>
    public IEnumerator Upload(WWW www)
    {
        yield return www;
    }

    /// <summary>
    /// Funci�n que se encarga de armar el HTTP Request y enviarlo al backend 
    /// (agilmente-core).
    /// </summary>
    public override void sendData()
    {
        
        figureQuantity = -1;
        limitTime = false;
        limitFigure = false;
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

        json =
            "{" +
                "'completeDatetime': '" + System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") +
                "', 'canceled': " + canceled +
                ", 'mistakes': " + hayUnoRepetido.mistakes +
                ", 'successes': " + hayUnoRepetido.successes +
                ", 'timeBetweenSuccesses': " + tBS +
                ", 'totalTime': " + hayUnoRepetido.totalTime.ToString().Replace(",", ".") +
                ", 'game': 'Encuentra al Repetido'" +
                ", 'hayUnoRepetidoSessionId': " + SessionHayUnoRepetido.gameSessionId +
            "}";
        SendData sD = (new GameObject("SendData")).AddComponent<SendData>();
        sD.sendData(json, DEV_ENDPOINT);
        SceneManager.LoadScene("mainScene");
    }

    /// <summary>
    /// Funci�n que se encarga de pausar/despausar el juego.
    /// </summary>
    public override void pauseGame()
    {
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
    /// Funci�n que se encarga de reanudar el juego.
    /// </summary>
    public override void unpauseGame()
    {
        foreach (GameObject f in figures)
        {
            f.SetActive(true);
        }
    }
}