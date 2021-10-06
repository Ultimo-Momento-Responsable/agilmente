using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public class HayUnoRepetido : ScriptableObject
{
    private int a_mistakes;
    private int a_successes;
    private float[] a_timeBetweenSuccesses;
    private float a_totalTime;
    private int a_score;
    private int a_mgp;
    private float a_timeFromLastSuccess;
    public bool onTutorial = true;
    public HayUnoRepetidoController hayUnoRepetidoController;

    public int mistakes { get => a_mistakes; set => a_mistakes = value; }
    public int successes { get => a_successes; set => a_successes = value; }
    public float[] timeBetweenSuccesses { get => a_timeBetweenSuccesses; set => a_timeBetweenSuccesses = value; }
    public float totalTime { get => a_totalTime; set => a_totalTime = value; }
    public int score { get => a_score; set => a_score = value; }
    public int mgp { get => a_mgp; set => a_mgp = value; }
    private float timeFromLastSuccess { get => a_timeFromLastSuccess; set => a_timeFromLastSuccess = value; }

    public HayUnoRepetido(HayUnoRepetidoController hayUnoRepetidoController)
    {
        timeFromLastSuccess = Time.time;
        a_mistakes = 0;
        a_successes = 0;
        a_totalTime = 0f;
        a_timeBetweenSuccesses = new float[100];
        a_score = 0;
        a_mgp = 0;
        this.hayUnoRepetidoController = hayUnoRepetidoController;
    }

    /// <summary>
    /// Función que selecciona los sprites de las figuras que se mostrarán en
    /// pantalla por cada nivel.
    /// </summary>
    public List<int> chooseSprites(Sprite[] sprites, int figureQuantity)
    {
        List<int> index = new List<int>();
        int repeatedIndex = UnityEngine.Random.Range(0, sprites.Length);
        index.Add(repeatedIndex);
        index.Add(repeatedIndex);
        for (int i = 2; i < figureQuantity; i++)
        {
            int randIndex = UnityEngine.Random.Range(0, sprites.Length);
            while (index.Contains(randIndex))
            {
                randIndex = UnityEngine.Random.Range(0, sprites.Length);
            }
            index.Add(randIndex);
        }
        return index;
    }

    /// <summary>
    /// Función que instancia las figuras que se mostrarán al inicio y al 
    /// aumentar cada nivel.
    /// </summary>
    /// <param name="figureQuantity">Cantidad máxima de figuras.</param>
    /// <param name="camera">Cámara.</param>
    /// <param name="figure">Figura.</param>
    /// <param name="sprites">Set de sprites a usar.</param>
    /// <param name="index">Índice.</param>
    /// <param name="controller">Controlador del juego.</param>
    /// <param name="particles">Partículas.</param>
    public void createFigures(int figureQuantity, Camera camera, GameObject figure, Sprite[] sprites, List<int> index, HayUnoRepetidoController controller, GameObject particles)
    {
        int handPosition = Random.Range(0, 2); // Posición de la mano del tutorial
        Vector2 figurePosition;
        for (int i = 0; i < figureQuantity; i++)
        {
            float size = 0.15f;
            float minsize = 0.122f;
            float maxsize = 0.2f;
            if (MainSceneController.SessionHayUnoRepetido.spriteSet == 1)
            {
                size = 0.2f;
                minsize = 0.172f;
                maxsize = 0.25f;
            }
            
            if (!onTutorial)
            {
                figurePosition = camera.ViewportToWorldPoint(new Vector2(UnityEngine.Random.value, UnityEngine.Random.value));
                figurePosition = centerFigures(figurePosition);

                while (thereIsSomethingIn(figurePosition))
                {
                    figurePosition = camera.ViewportToWorldPoint(new Vector2(UnityEngine.Random.value, UnityEngine.Random.value));
                    figurePosition = centerFigures(figurePosition);
                }
                if (controller.variableSizes)
                {
                    size = Random.Range(minsize, maxsize);
                }
                
            }
            else
            {
                figurePosition = camera.ViewportToWorldPoint(new Vector2(Random.Range(1, 4) * 0.25f, 0.4f));
                while (thereIsSomethingIn(figurePosition))
                {
                    figurePosition = camera.ViewportToWorldPoint(new Vector2(Random.Range(1, 4) * 0.25f, 0.4f));
                }
            }
            

            GameObject fig = Instantiate(figure, figurePosition, Quaternion.identity);
            
            fig.GetComponent<Transform>().localScale = new Vector3(size, size, 1);
            fig.GetComponent<FigureBehaviour>().sprite = sprites[index[i]];
            fig.GetComponent<FigureBehaviour>().controller = controller;
            fig.GetComponent<FigureBehaviour>().index = i;

            // Si está en tutorial crea la mano en una fruta repetida
            if (i < 2) 
            {
                if (i == handPosition && onTutorial) 
                {
                    GameObject tHand = Instantiate(hayUnoRepetidoController.tutorialHand, new Vector2(figurePosition.x, figurePosition.y), Quaternion.identity);
                    tHand.GetComponent<TutorialHand>().yPos = -2.8f;
                }
                GameObject part = Instantiate(particles, figurePosition, Quaternion.identity);
                fig.GetComponent<FigureBehaviour>().ps = part.GetComponent<ParticleSystem>();
            }

        }
        int countSpritesets = Directory.GetDirectories(Application.dataPath + "/Resources/Sprites/Figures/").Length;
        if (hayUnoRepetidoController.distractors && Random.value <= 0.25f && !onTutorial)
        {
            figurePosition = camera.ViewportToWorldPoint(new Vector2(UnityEngine.Random.value, UnityEngine.Random.value));
            figurePosition = centerFigures(figurePosition);

            while (thereIsSomethingIn(figurePosition))
            {
                figurePosition = camera.ViewportToWorldPoint(new Vector2(UnityEngine.Random.value, UnityEngine.Random.value));
                figurePosition = centerFigures(figurePosition);
            }
            
            GameObject distractor = Instantiate(figure, figurePosition, Quaternion.identity);

            
            int spriteSetDistractor = UnityEngine.Random.Range(1, countSpritesets + 1);
            while (spriteSetDistractor == MainSceneController.SessionHayUnoRepetido.spriteSet)
            {
                spriteSetDistractor = UnityEngine.Random.Range(1, countSpritesets + 1);
            }
            if (spriteSetDistractor == 1)
            {
                distractor.GetComponent<Transform>().localScale = new Vector3(0.2f, 0.2f, 1);
            } else
            {
                distractor.GetComponent<Transform>().localScale = new Vector3(0.15f, 0.15f, 1);
            }
            
            hayUnoRepetidoController.distractorsSprites = Resources.LoadAll<Sprite>("Sprites/Figures/SpriteSet" + spriteSetDistractor + "/");
            int pos = UnityEngine.Random.Range(0, hayUnoRepetidoController.distractorsSprites.Length);
            distractor.GetComponent<FigureBehaviour>().sprite = hayUnoRepetidoController.distractorsSprites[pos];
            distractor.GetComponent<FigureBehaviour>().controller = controller;
            distractor.GetComponent<FigureBehaviour>().index = -1;
        }

    }

    /// <summary>
    /// Función que centra las figuras, esto se hace para evitar que las mismas 
    /// se generen en los bordes.
    /// </summary>
    /// <param name="randomPositionOnScreen">Posición donde aparecerá la figura.</param>
    /// <returns>Posición corregida.</returns>
    public Vector2 centerFigures(Vector2 randomPositionOnScreen)
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
            randomPositionOnScreen.y += 1.5f;
        }
        else
        {
            randomPositionOnScreen.y -= 1.5f;
        }
        return randomPositionOnScreen;
    }

    /// <summary>
    /// Función que chequea que no haya nada en el lugar donde se crea la figura.
    /// </summary>
    /// <param name="posición">Posición a controlar.</param>
    /// <returns>Verdadero si hay algo.</returns>
    public bool thereIsSomethingIn(Vector2 posición)
    {
        Vector2 p1 = posición - new Vector2(0.5f, 0.5f);
        Vector2 p2 = posición + new Vector2(0.5f, 0.5f);
        Collider2D collider = Physics2D.OverlapArea(p1, p2);

        if (collider != null)
        {
            return true;
        }
        return false;
    }

    public void addSuccess()
    {
        calculateTimeSinceLastSuccess();
        addPointsToScore(calculateScoreSuccess());
        successes++;
    }

    public void calculateTimeSinceLastSuccess()
    {
        timeBetweenSuccesses[successes] = Time.time - timeFromLastSuccess;
        timeFromLastSuccess = Time.time;
    }

    public int calculateScoreSuccess()
    {
        return 0;
    }

    public int calculateScoreMistake()
    {
        return 0;
    }

    public void addMistake()
    {
        addPointsToScore(calculateScoreMistake());
        mistakes++;
    }

    /// <summary>
    /// Suma una cantidad de puntos al score.
    /// </summary>
    /// <param name="points">Puntos a sumar.</param>
    public void addPointsToScore(int points)
    {
        score += points;
    }

    /// <summary>
    /// Calcula los MGP a partir del resultado.
    /// </summary>
    public void calculateMGP()
    {
        throw new System.NotImplementedException();
    }

    public void setStartTime()
    {
        timeFromLastSuccess = Time.time;
    }
}
