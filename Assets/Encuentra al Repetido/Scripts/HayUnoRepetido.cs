using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HayUnoRepetido : ScriptableObject
{
    private int a_mistakes;
    private int a_successes;
    private float[] a_timeBetweenSuccesses;
    private float a_totalTime;
    private int a_score;
    private float a_timeMarkFromLastSuccess;
    private float a_timeElapsedSinceLastSuccess = 1;
    public bool onTutorial = true;
    public HayUnoRepetidoController hayUnoRepetidoController;
    private float size;

    public int mistakes { get => a_mistakes; set => a_mistakes = value; }
    public int successes { get => a_successes; set => a_successes = value; }
    public float[] timeBetweenSuccesses { get => a_timeBetweenSuccesses; set => a_timeBetweenSuccesses = value; }
    public float totalTime { get => a_totalTime; set => a_totalTime = value; }
    public int score { get => a_score < 0 ? 0 : a_score; set => a_score = value; }
    private float timeMarkFromLastSuccess { get => a_timeMarkFromLastSuccess; set => a_timeMarkFromLastSuccess = value; }
    private float timeElapsedSinceLastSuccess { get => a_timeElapsedSinceLastSuccess; set => a_timeElapsedSinceLastSuccess = value; }

    /// <summary>
    /// Setea los valores iniciales para el objeto.
    /// </summary>
    /// <param name="hayUnoRepetidoController">Controlador del juego.</param>
    public void Initialize(HayUnoRepetidoController hayUnoRepetidoController)
    {
        timeMarkFromLastSuccess = Time.time;
        a_mistakes = 0;
        a_successes = 0;
        a_totalTime = 0f;
        a_timeBetweenSuccesses = new float[100];
        a_score = 0;
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
    /// Posiciona las figuras en el lugar correcto
    /// </summary>
    /// <param name="size">Tamaño estándar</param>
    /// <param name="minSize">Tamaño mínimo</param>
    /// <param name="maxSize">Tamaño máximo</param>
    /// <param name="controller">Controlador del juego.</param>
    /// <returns>Retorna un vector con la posición de la figura</returns>
    Vector2 locateFigures(float minSize, float maxSize, HayUnoRepetidoController controller)
    {
        Vector2 figurePosition = new Vector2(Random.Range(0, 6) * 0.9f - 2.5f + Random.Range(-0.15f, 0.15f), Random.Range(0, 9) * 1.2f - 4.5f + Random.Range(-0.2f, 0));
        figurePosition = centerFigures(figurePosition);
        if (controller.variableSizes)
        {
            size = Random.Range(minSize, maxSize);
        }
        while (thereIsSomethingIn(figurePosition, size))
        {
            figurePosition = new Vector2(Random.Range(0, 6) * 0.9f - 2.5f + Random.Range(-0.15f, 0.15f), Random.Range(0, 9) * 1.2f - 4.5f + Random.Range(-0.2f, 0));
            figurePosition = centerFigures(figurePosition);
        }
        return figurePosition;
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
            size = 0.15f;
            float minSize = 0.122f;
            float maxSize = 0.2f;
            
            if (!onTutorial)
            {
                figurePosition = locateFigures(minSize, maxSize, controller);
            }
            else
            {
                figurePosition = camera.ViewportToWorldPoint(new Vector2(Random.Range(1, 4) * 0.25f, 0.4f));
                while (thereIsSomethingIn(figurePosition,size))
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
                    GameObject tHand = Instantiate(hayUnoRepetidoController.handPref, new Vector2(figurePosition.x, figurePosition.y), Quaternion.identity);
                    tHand.GetComponent<TutorialHand>().yPos = -2.8f;
                }
                GameObject part = Instantiate(particles, figurePosition, Quaternion.identity);
                fig.GetComponent<FigureBehaviour>().ps = part.GetComponent<ParticleSystem>();
            }
        }

        int countSpritesets = 2;
        if (hayUnoRepetidoController.distractors && Random.value <= 0.25f && !onTutorial)
        {
            figurePosition = locateFigures(0.2f, 0.2f, controller);
            
            GameObject distractor = Instantiate(figure, figurePosition, Quaternion.identity);

            int spriteSetDistractor = Random.Range(1, countSpritesets + 1);
            while (spriteSetDistractor == MainSceneController.SessionHayUnoRepetido.spriteSet)
            {
                spriteSetDistractor = Random.Range(1, countSpritesets + 1);
            }
            if (spriteSetDistractor == 1)
            {
                distractor.GetComponent<Transform>().localScale = new Vector3(0.2f, 0.2f, 1);
            } else
            {
                distractor.GetComponent<Transform>().localScale = new Vector3(0.15f, 0.15f, 1);
            }
            
            hayUnoRepetidoController.distractorsSprites = Resources.LoadAll<Sprite>("Sprites/Figures/SpriteSet" + spriteSetDistractor + "/");
            int pos = Random.Range(0, hayUnoRepetidoController.distractorsSprites.Length);
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
            randomPositionOnScreen.y += 0.8f;
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
    /// <param name="position">Posición a controlar.</param>
    /// <returns>Verdadero si hay algo.</returns>
    public bool thereIsSomethingIn(Vector2 position, float size)
    {
        Vector2 p1 = position - new Vector2(0.45f, 0.5f + size/2);
        Vector2 p2 = position + new Vector2(0.45f, 0.5f + size/2);
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
        calculateTimeSinceLastSuccess();
        addPointsToScore(calculateScoreSuccess(figureQuantity));
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
