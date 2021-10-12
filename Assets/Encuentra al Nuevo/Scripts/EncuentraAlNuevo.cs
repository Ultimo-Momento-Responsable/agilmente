using System.Collections.Generic;
using UnityEngine;

public class EncuentraAlNuevo : ScriptableObject
{
    private int a_mistakes;
    private int a_successes;
    private float[] a_timeBetweenSuccesses;
    private float a_totalTime;
    public bool onTutorial = true;
    public EncuentraAlNuevoController encuentraAlNuevoController;

    public EncuentraAlNuevo(EncuentraAlNuevoController encuentraAlNuevoController)
    {
        a_mistakes = 0;
        a_successes = 0;
        a_totalTime = 0f;
        a_timeBetweenSuccesses = new float[100];
        this.encuentraAlNuevoController = encuentraAlNuevoController;
    }

    public List<int> intialSprites(Sprite[] sprites)
    {
        List<int> initialSprites = new List<int>();
        int indexSprite = Random.Range(0, sprites.Length);
        initialSprites.Insert(0, indexSprite);
        indexSprite = Random.Range(0, sprites.Length);
        while (initialSprites.Contains(indexSprite))
        {
            indexSprite = Random.Range(0, sprites.Length);
        }
        initialSprites.Insert(0, indexSprite);

        return initialSprites;
    }

    /// <summary>
    /// Funci�n que selecciona los sprites de las figuras que se mostrar�n en
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
    public void createFigures(int figureQuantity, Camera camera, GameObject figure, Sprite[] sprites, List<int> index, EncuentraAlNuevoController controller, GameObject particles)
    {
        for (int i = figureQuantity-1; i >= 0; i--)
        {
            float size = 0.15f;
            float minsize = 0.122f;
            float maxsize = 0.2f;

            // Ajusta el tama�o seg�n el spriteset (flores mas grandes)
            if (MainSceneController.SessionEncuentraAlNuevo.spriteSet == 1)
            {
                size = 0.2f;
                minsize = 0.172f;
                maxsize = 0.25f;
            }

            Vector2 figurePosition;
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
            }
            

            GameObject fig = Instantiate(figure, figurePosition, Quaternion.identity);
            fig.GetComponent<Transform>().localScale = new Vector3(size, size, 1);
            fig.GetComponent<FigureBehaviourEAN>().sprite = sprites[index[i]];
            fig.GetComponent<FigureBehaviourEAN>().controller = controller;
            fig.GetComponent<FigureBehaviourEAN>().index = i;

            // Si est� en tutorial crea la mano en una fruta nueva
            if (i == 0 && !encuentraAlNuevoController.prevTutorial)
            {
                if (onTutorial)
                {
                    GameObject tHand = Instantiate(encuentraAlNuevoController.tutorialHand, new Vector2(figurePosition.x, figurePosition.y), Quaternion.identity);
                    tHand.GetComponent<TutorialHand>().yPos = -2.8f;
                }
                GameObject part = Instantiate(particles, figurePosition, Quaternion.identity);
                fig.GetComponent<FigureBehaviourEAN>().ps = part.GetComponent<ParticleSystem>();
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
