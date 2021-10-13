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
        Vector2 figurePosition;
        bool inColission = false;
        for (int i = 0; i < figureQuantity; i++)
        {
            float size = 0.15f;
            float minsize = 0.122f;
            float maxsize = 0.2f;
            if (MainSceneController.SessionHayUnoRepetido.spriteSet == 1)
            {
                size = 0.19f;
                minsize = 0.17f;
                maxsize = 0.22f;
            }
            
            if (!onTutorial)
            {
                figurePosition = new Vector2(Random.Range(0,6) * 0.9f - 2.5f + Random.Range(-0.15f,0.15f), Random.Range(0, 9) * 1.2f -4.5f + Random.Range(-0.2f,0));
                figurePosition = centerFigures(figurePosition);
                if (controller.variableSizes)
                {
                    size = Random.Range(minsize, maxsize);
                }
                var attempts = 0;
                while (thereIsSomethingIn(figurePosition,size))
                {
                    figurePosition = new Vector2(Random.Range(0, 6) * 0.9f - 2.5f + Random.Range(-0.15f, 0.15f), Random.Range(0, 9) * 1.2f - 4.5f + Random.Range(-0.2f, 0)); 
                    figurePosition = centerFigures(figurePosition);
                    attempts++;
                    if (attempts > 200)
                    {
                        inColission = true;
                        break;
                    }
                }
                
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

            // Si est� en tutorial crea la mano en una fruta repetida
            if (i < 2) 
            {
                if (i == handPosition && onTutorial) 
                {
                    GameObject tHand = Instantiate(hayUnoRepetidoController.tutorialHand, new Vector2(figurePosition.x, figurePosition.y), Quaternion.identity);
                    tHand.GetComponent<TutorialHand>().yPos = -2.8f;
                }
                GameObject part = Instantiate(particles, figurePosition, Quaternion.identity);
                part.transform.SetParent(fig.transform);
                fig.GetComponent<FigureBehaviour>().ps = part.GetComponent<ParticleSystem>();
            }

        }
        if (inColission)
        {
            controller.resetValues();
        }
        int countSpritesets = Directory.GetDirectories(Application.dataPath + "/Resources/Sprites/Figures/").Length;
        if (hayUnoRepetidoController.distractors && Random.value <= 0.25f && !onTutorial)
        {
            figurePosition = new Vector2(Random.Range(0, 6) * 0.9f - 2.5f + Random.Range(-0.15f, 0.15f), Random.Range(0, 9) * 1.2f - 4.5f + Random.Range(-0.2f, 0));
            figurePosition = centerFigures(figurePosition);

            while (thereIsSomethingIn(figurePosition,0.2f))
            {
                figurePosition = new Vector2(Random.Range(0, 6) * 0.9f - 2.5f + Random.Range(-0.15f, 0.15f), Random.Range(0, 9) * 1.2f - 4.5f + Random.Range(-0.2f, 0));
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
            randomPositionOnScreen.y += 0.8f;
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
    public bool thereIsSomethingIn(Vector2 posici�n,float size)
    {
        Vector2 p1 = posici�n - new Vector2(0.45f, 0.5f + size/2);
        Vector2 p2 = posici�n + new Vector2(0.45f, 0.5f + size/2);
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
