using System.Collections.Generic;
using UnityEngine;

public class HayUnoRepetido : ScriptableObject
{
    private int a_mistakes;
    private int a_successes;
    private float[] a_timeBetweenSuccesses;
    private float a_totalTime;
    public bool onTutorial = true;
    public HayUnoRepetidoController hayUnoRepetidoController;

    public HayUnoRepetido(HayUnoRepetidoController hayUnoRepetidoController)
    {
        a_mistakes = 0;
        a_successes = 0;
        a_totalTime = 0f;
        a_timeBetweenSuccesses = new float[100];
        this.hayUnoRepetidoController = hayUnoRepetidoController;
    }

    /// <summary>
    /// Funci�n que selecciona los sprites de las figuras que se mostrar�n en
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
    /// Funci�n que instancia las figuras que se mostrar�n al inicio y al 
    /// aumentar cada nivel.
    /// </summary>
    /// <param name="figureQuantity">Cantidad m�xima de figuras.</param>
    /// <param name="camera">C�mara.</param>
    /// <param name="figure">Figura.</param>
    /// <param name="sprites">Set de sprites a usar.</param>
    /// <param name="index">�ndice.</param>
    /// <param name="controller">Controlador del juego.</param>
    /// <param name="particles">Part�culas.</param>
    public void createFigures(int figureQuantity, Camera camera, GameObject figure, Sprite[] sprites, List<int> index, HayUnoRepetidoController controller, GameObject particles)
    {
        int handPosition = Random.Range(0, 2); // Posici�n de la mano del tutorial

        for (int i = 0; i < figureQuantity; i++)
        {
            Vector2 figurePosition;
            float size = 0.15f;
            if (!onTutorial)
            {
                figurePosition = camera.ViewportToWorldPoint(new Vector2(UnityEngine.Random.value, UnityEngine.Random.value));
                figurePosition = centerFigures(figurePosition);

                while (thereIsSomethingIn(figurePosition))
                {
                    figurePosition = camera.ViewportToWorldPoint(new Vector2(UnityEngine.Random.value, UnityEngine.Random.value));
                    figurePosition = centerFigures(figurePosition);
                }
                if (controller.changingSize)
                {
                    size = Random.Range(0.1f, 0.2f);
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

            // Si est� en tutorial crea la mano en una fruta repetida
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

    }

    /// <summary>
    /// Funci�n que centra las figuras, esto se hace para evitar que las mismas 
    /// se generen en los bordes.
    /// </summary>
    /// <param name="randomPositionOnScreen">Posici�n donde aparecer� la figura.</param>
    /// <returns>Posici�n corregida.</returns>
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
    /// Funci�n que chequea que no haya nada en el lugar donde se crea la figura.
    /// </summary>
    /// <param name="posici�n">Posici�n a controlar.</param>
    /// <returns>Verdadero si hay algo.</returns>
    public bool thereIsSomethingIn(Vector2 posici�n)
    {
        Vector2 p1 = posici�n - new Vector2(0.5f, 0.5f);
        Vector2 p2 = posici�n + new Vector2(0.5f, 0.5f);
        Collider2D collider = Physics2D.OverlapArea(p1, p2);

        if (collider != null)
        {
            return true;
        }
        return false;
    }

    public int mistakes { get => a_mistakes; set => a_mistakes = value; }
    public int successes { get => a_successes; set => a_successes = value; }
    public float[] timeBetweenSuccesses { get => a_timeBetweenSuccesses; set => a_timeBetweenSuccesses = value; }
    public float totalTime { get => a_totalTime; set => a_totalTime = value; }
}
