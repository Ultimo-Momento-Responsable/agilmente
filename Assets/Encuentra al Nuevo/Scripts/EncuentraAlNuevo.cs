using System.Collections.Generic;
using UnityEngine;
using Assets.Resources.Scripts;

public class EncuentraAlNuevo : ScriptableObject, GameWithFigureBehaviour
{
    private int a_mistakes;
    private int a_successes;
    private float[] a_timeBetweenSuccesses;
    private float a_timeMarkFromLastSuccess;
    private float a_timeElapsedSinceLastSuccess = 1;
    private float a_totalTime;
    private int a_score;
    public bool onTutorial = true;
    public EncuentraAlNuevoController encuentraAlNuevoController;

    public int mistakes { get => a_mistakes; set => a_mistakes = value; }
    public int successes { get => a_successes; set => a_successes = value; }
    public float[] timeBetweenSuccesses { get => a_timeBetweenSuccesses; set => a_timeBetweenSuccesses = value; }
    public float totalTime { get => a_totalTime; set => a_totalTime = value; }
    private float timeMarkFromLastSuccess { get => a_timeMarkFromLastSuccess; set => a_timeMarkFromLastSuccess = value; }
    private float timeElapsedSinceLastSuccess { get => a_timeElapsedSinceLastSuccess; set => a_timeElapsedSinceLastSuccess = value; }
    public int score { get => a_score < 0 ? 0 : a_score; set => a_score = value; }

    public bool OnTutorial => onTutorial;

    /// <summary>
    /// Setea los valores iniciales para el objeto.
    /// </summary>
    /// <param name="encuentraAlNuevoController">Controlador del juego.</param>
    public void Initialize(EncuentraAlNuevoController encuentraAlNuevoController)
    {
        timeMarkFromLastSuccess = Time.time;
        a_mistakes = 0;
        a_successes = 0;
        a_totalTime = 0f;
        a_timeBetweenSuccesses = new float[100];
        a_score = 0;
        this.encuentraAlNuevoController = encuentraAlNuevoController;
    }

    public List<int> intialSprites(Sprite[] sprites)
    {
        List<int> initialSprites = new List<int>();
        int indexSprite = Random.Range(0, sprites.Length);
        while (initialSprites.Contains(indexSprite))
        {
            indexSprite = Random.Range(0, sprites.Length);
        }
        initialSprites.Insert(0, indexSprite);

        return initialSprites;
    }

    /// <summary>
    /// Función que selecciona los sprites de las figuras que se mostrarán en
    /// pantalla por cada nivel.
    /// </summary>
    public List<int> chooseSprites(Sprite[] sprites, List<int> actualSprites)
    {
        int randIndex = Random.Range(0, sprites.Length);
        while (actualSprites.Contains(randIndex))
        {
            randIndex = Random.Range(0, sprites.Length);
        }
        actualSprites.Insert(0, randIndex);
        return actualSprites;
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
    public void createFigures(int figureQuantity, Camera camera, GameObject figure, Sprite[] sprites, List<int> index, EncuentraAlNuevoController controller)
    {
        // Nuevo spawn de figuras
        if (!OnTutorial)
        {
            encuentraAlNuevoController.Grid.CreateCells();

            for (int i = 0; i < figureQuantity; i++)
            {
                encuentraAlNuevoController.Grid.CreateFigureOnRandomCell(sprites, index[i], i, controller);
            }

            return;
        }

        // Lógica del tutorial
        const float SIZE = 0.15f;

        for (int i = figureQuantity-1; i >= 0; i--)
        {
            Vector2 figurePosition;
            
            float space;
            if (figureQuantity == 2)
            {
                space = 0.33f;
            } else
            {
                space = 0.25f;
            }
            figurePosition = camera.ViewportToWorldPoint(new Vector2(Random.Range(1, figureQuantity + 1) * space, 0.4f));
            while (thereIsSomethingIn(figurePosition))
            {
                figurePosition = camera.ViewportToWorldPoint(new Vector2(Random.Range(1, figureQuantity + 1) * space, 0.4f));
            }

            GameObject fig = Instantiate(figure, figurePosition, Quaternion.identity);
            fig.GetComponent<Transform>().localScale = new Vector3(SIZE, SIZE, 1);
            fig.GetComponent<TutorialFigureBehaviour>().sprite = sprites[index[i]];
            fig.GetComponent<TutorialFigureBehaviour>().controller = controller;
            fig.GetComponent<TutorialFigureBehaviour>().index = i;

            // Crea la mano en una fruta nueva
            if (i == 0 && !encuentraAlNuevoController.prevTutorial)
            {
                GameObject tHand = Instantiate(encuentraAlNuevoController.handPref, new Vector2(figurePosition.x, figurePosition.y), Quaternion.identity);
                tHand.SetActive(true);
                tHand.GetComponent<TutorialHand>().yPos = -2.8f;
                GameObject part = Instantiate(controller.Particles, figurePosition, Quaternion.identity);
                fig.GetComponent<TutorialFigureBehaviour>().ps = part.GetComponent<ParticleSystem>();
            }
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

    /// <summary>
    /// Añade un acierto y calcula el puntaje.
    /// </summary>
    /// <param name="figureQuantity">Cantidad de figuras en la pantalla cuando se 
    /// hizo el acierto.</param>
    public void addSuccess(int figureQuantity)
    {
        addPointsToScore(calculateScoreSuccess(figureQuantity));
        calculateTimeSinceLastSuccess();
        successes++;
    }

    /// <summary>
    /// Añade un error y calcula el puntaje.
    /// </summary>
    /// <param name="figureQuantity">Cantidad de figuras en la pantalla cuando se 
    /// comete el error.</param>
    public void addMistake(int figureQuantity)
    {
        addPointsToScore(calculateScoreMistake(figureQuantity));
        mistakes++;
    }

    /// <summary>
    /// Calcula el tiempo transcurrido desde el último acierto y lo guarda.
    /// </summary>
    public void calculateTimeSinceLastSuccess()
    {
        timeElapsedSinceLastSuccess = Time.time - timeMarkFromLastSuccess;
        timeBetweenSuccesses[successes] = timeElapsedSinceLastSuccess;
        timeMarkFromLastSuccess = Time.time;
    }

    /// <summary>
    /// Calcula el puntaje correspondiente al acierto.
    /// </summary>
    /// <param name="figureQuantity">Cantidad de figuras en pantalla al momento del
    /// acierto.</param>
    /// <returns>El puntaje correspondiente al acierto.</returns>
    public int calculateScoreSuccess(int figureQuantity)
    {
        if (timeElapsedSinceLastSuccess > 25)
        {
            return 0;
        }

        float timeMultiplier = 2;

        if (figureQuantity >= 11)
        {
            timeMultiplier = 1f / 3f;
        }
        else if (figureQuantity >= 6)
        {
            timeMultiplier = 1f;
        }

        return 100 - Mathf.RoundToInt(timeMultiplier * timeElapsedSinceLastSuccess);
    }

    /// <summary>
    /// Calcula el puntaje correspondiente al error.
    /// </summary>
    /// <param name="figureQuantity">Cantidad de figuras en pantalla al momento de cometer
    /// el error.</param>
    /// <returns>El puntaje correspondiente a cometer el error.</returns>
    public int calculateScoreMistake(int figureQuantity)
    {
        return -10;
    }

    /// <summary>
    /// Suma una cantidad de puntos al score.
    /// </summary>
    /// <param name="points">Puntos a sumar.</param>
    private void addPointsToScore(int points)
    {
        a_score += points;
    }

    /// <summary>
    /// Setea el tiempo de inicio.
    /// </summary>
    public void setStartTime()
    {
        timeMarkFromLastSuccess = Time.time;
    }
}
