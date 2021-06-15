using System.Collections.Generic;
using UnityEngine;

public class HayUnoRepetido : ScriptableObject
{
    private int mistakes;
    private int successes;
    private float[] timeBetweenSuccesses;
    private float totalTime;

    public HayUnoRepetido()
    {
        mistakes = 0;
        successes = 0;
        totalTime = 0f;
        timeBetweenSuccesses = new float[100];
    }

    // Función que selecciona los sprites de las figuras que se mostrarán en pantalla por cada nivel.
    public List<int> chooseSprites(Sprite[] sprites, int figureQuantity)
    {
        List<int> index = new List<int>();
        int repeatedIndex = (int)UnityEngine.Random.Range(0, sprites.Length);
        index.Add(repeatedIndex);
        index.Add(repeatedIndex);
        for (int i = 2; i < figureQuantity; i++)
        {
            int randIndex = (int)UnityEngine.Random.Range(0, sprites.Length);
            while (index.Contains(randIndex))
            {
                randIndex = (int)UnityEngine.Random.Range(0, sprites.Length);
            }
            index.Add(randIndex);
        }
        return index;
    }

    //Función que instancia las figuras que se mostrarán al inicio y al aumentar cada nivel
    public void createFigures(int figureQuantity, Camera camera, GameObject figure, Sprite[] sprites, List<int> index, Gestor controller, GameObject particles)
    {
        for (int i = 0; i < figureQuantity; i++)
        {

            Vector2 randomPositionOnScreen = camera.ViewportToWorldPoint(new Vector2(UnityEngine.Random.value, UnityEngine.Random.value));
            randomPositionOnScreen = centerFigures(randomPositionOnScreen);

            while (thereIsSomethingIn(randomPositionOnScreen))
            {
                randomPositionOnScreen = camera.ViewportToWorldPoint(new Vector2(UnityEngine.Random.value, UnityEngine.Random.value));
                randomPositionOnScreen = centerFigures(randomPositionOnScreen);
            }

            GameObject fig = Instantiate(figure, randomPositionOnScreen, Quaternion.identity);

            fig.GetComponent<FigureBehaviour>().sprite = sprites[index[i]];
            fig.GetComponent<FigureBehaviour>().controller = controller;
            fig.GetComponent<FigureBehaviour>().index = i;
            if (i < 2)
            {
                GameObject part = Instantiate(particles, randomPositionOnScreen, Quaternion.identity);
                fig.GetComponent<FigureBehaviour>().ps = part.GetComponent<ParticleSystem>();
            }

        }
    }

    //Función que centra las figuras, esto se hace para evitar que las mismas se generen en los bordes.
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
            randomPositionOnScreen.y += 0.5f;
        }
        else
        {
            randomPositionOnScreen.y -= 0.5f;
        }
        return randomPositionOnScreen;
    }

    // Función que chequea que no haya nada en el lugar donde se crea la figura.
    public bool thereIsSomethingIn(Vector2 posición)
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

    public int Mistakes { get => mistakes; set => mistakes = value; }
    public int Successes { get => successes; set => successes = value; }
    public float[] TimeBetweenSuccesses { get => timeBetweenSuccesses; set => timeBetweenSuccesses = value; }
    public float TotalTime { get => totalTime; set => totalTime = value; }


}
