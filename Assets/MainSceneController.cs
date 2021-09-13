using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneController : MonoBehaviour
{
    public Text title;
    public GameObject bodyText;
    private Settings settings;
    public Button gameButton;
    public Button profileButton;
    public Button homeButton;
    public Sprite[] homeSprite;
    public Sprite[] profileSprite;
    public Sprite[] gameSprite;
    public GameObject gameText;
    private PlanningList planningRequestJson;
    public Camera camera;
    public Button gameCard;
    public GameObject gameCanvas;
    public GameObject cardContainer;

    /**
     * Inicio de escena, genera una request que obtiene un JSON con los juegos pendientes asignados a una planificacion
     */
    public void Start()
    {
        getPlanning();
    }

    /**
     * Destruye todas las tarjetas actualmente creadas, utilizado para navegabilidad entre menus. 
     */ 
    public void clearCards()
    {
        foreach (GameObject cardsToDestroy in GameObject.FindGameObjectsWithTag("gameCard"))
        {
            Destroy(cardsToDestroy);
        }
    }

    public void highlightButton(Image buttonImage, Sprite highlightedButton)
    {
        buttonImage.sprite = highlightedButton;
    }

    /**
     * Muestra todos los elementos de la seccion "juegos" de la aplicacion
     */
    public void btnGames()
    {
        title.text = "Jugar";

        clearCards();

        highlightButton(gameButton.GetComponent<Image>(), gameSprite[0]);
        highlightButton(profileButton.GetComponent<Image>(), profileSprite[1]);
        highlightButton(homeButton.GetComponent<Image>(), homeSprite[1]);
        bodyText.SetActive(true);
        bodyText.GetComponent<Text>().text = "Juegos pendientes";

        if (isThereAPlanning())
        {
            gameText.SetActive(false);
        }
        else
        {
            gameText.SetActive(true);
        }

        /**
         * Por cada juego pendiente, genera una card con la informacion del juego
         * Define un contenedor scrolleable con tama�o igual a la cantidad de cards generadas
         */
        if (planningRequestJson.planningList.Length != 0)
        {
            int i = 0;
            cardContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(cardContainer.GetComponent<RectTransform>().sizeDelta.x, (planningRequestJson.planningList.Length * 1.55f));
            foreach (Planning planningCards in planningRequestJson.planningList)
            {
                Button gameCardInstance = Instantiate(gameCard);
                
                gameCardInstance.transform.parent = cardContainer.transform;
                gameCardInstance.transform.localScale = new Vector2(0.0078f, 0.0078f);          //Escala actual del canvas
                gameCardInstance.transform.localPosition = new Vector3(0, -1 + (i * -1.5f), 0); //Tama�o de cards + offset

                foreach (Text gameName in gameCardInstance.GetComponentsInChildren<Text>())
                {
                    if (gameName.gameObject.name == "GameName")
                    {
                        gameName.text = planningCards.game;
                    }
                    if (gameName.gameObject.name == "NumberOfSessions")
                    {
                        if (planningCards.numberOfSession != -1)
                        {
                            gameName.text = "Quedan " + planningCards.numberOfSession + " sesiones restantes";
                        } else
                        {
                            gameName.text = "Sin l�mite de partidas";
                        }
                    }
                }
                btnClickPlayGame(gameCardInstance, i);
                i++;
            }
        }
    }

    /**
     * Llama al inicio del juego cuando se clickea en alguna de las cards
     */
    void btnClickPlayGame(Button btnPlayGame, int index)
    {
        btnPlayGame.onClick.AddListener(() => playGame(index));
    }

    /**
     * Muestra todos los elementos de la seccion "Inicio" de la aplicacion
     */
    public void btnHome()
    {
        clearCards();
        title.text = "Inicio";
        highlightButton(gameButton.GetComponent<Image>(), gameSprite[1]);
        highlightButton(profileButton.GetComponent<Image>(), profileSprite[1]);
        highlightButton(homeButton.GetComponent<Image>(), homeSprite[0]);


        bodyText.SetActive(true);
        settings = JsonUtility.FromJson<Settings>(System.IO.File.ReadAllText(Application.dataPath + "/settings.json"));
        bodyText.GetComponent<Text>().text = "�Hola de nuevo " + settings.Login.patient.firstName + "!";
        gameText.SetActive(true);
        if (isThereAPlanning())
        {
            gameText.GetComponent<Text>().text = "�Juega ahora!";
            generateCard();
        }
    }
    
    /**
     * Muestra todos los elementos de la seccion "Perfil" de la aplicacion
     */
    public void btnProfile()
    {
        gameText.SetActive(false);
        clearCards();
        title.text = "Perfil";
        highlightButton(gameButton.GetComponent<Image>(), gameSprite[1]);
        highlightButton(profileButton.GetComponent<Image>(), profileSprite[0]);
        highlightButton(homeButton.GetComponent<Image>(), homeSprite[1]);


        bodyText.SetActive(false);
    }

    /**
     * GET para planificaciones desde backend para un paciente
     */
    public void getPlanning()
    {
        settings = JsonUtility.FromJson<Settings>(System.IO.File.ReadAllText(Application.dataPath + "/settings.json"));
        this.StartCoroutine(this.getPlanningRoutine("localhost:8080/planning/mobile_patient_" + settings.Login.patient.id, this.getPlanningResponseCallback));
    }

    /**
     * Se hace un get a los pacientes para ver si ese c�digo de Logueo existe
     */
    private IEnumerator getPlanningRoutine(string url, Action<string> callback = null)
    {
        var request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();
        var data = request.downloadHandler.text;

        if (callback != null)
            callback(data);
    }

    /**
     * Callback utilizado en getPlanning(), asigna a variables de Unity los datos obtenidos del JSON
     */
    private void getPlanningResponseCallback(string data) { 
        planningRequestJson = JsonUtility.FromJson<PlanningList>(data);
        if (isThereAPlanning())
        {
            gameText.GetComponent<Text>().text = "�Juega ahora!";
            generateCard();

        }
    }

    /**
     * Checkea si hay planning
     */
    private bool isThereAPlanning()
    {
        if (planningRequestJson.planningList.Length != 0)
        {
            return true;
        }
        return false;
    }

    /**
     * Crea la card del inicio
     */
    private void generateCard()
    {
        Button gameCardInstance = Instantiate(gameCard);
        gameCardInstance.transform.SetParent(gameCanvas.transform);
        gameCardInstance.transform.localScale = new Vector2(1, 1);
        gameCardInstance.transform.position = new Vector3(0, -0.1f, 0);
        foreach (Text gameName in gameCardInstance.GetComponentsInChildren<Text>())
        {
            if (gameName.gameObject.name == "GameName")
            {
                gameName.text = planningRequestJson.planningList[0].game;
            }
            if (gameName.gameObject.name == "NumberOfSessions")
            {
                if (planningRequestJson.planningList[0].numberOfSession != -1)
                {
                    gameName.text = "Quedan " + planningRequestJson.planningList[0].numberOfSession + " sesiones restantes";
                }
                else
                {
                    gameName.text = "Sin l�mite de partidas";
                }
            }
            
        }
        btnClickPlayGame(gameCardInstance, 0);
    }

    /**
     * Ejecuta una instancia de juego segun los parametros definidos en la planificacion
     */
    public void playGame(int index)
    {
        if (planningRequestJson.planningList[index].game == "Encuentra al Repetido")
        {
            if (planningRequestJson.planningList[index].parameters[0].name == "figureQuantity")
            {
                SessionHayUnoRepetido.maxFigures = int.Parse(planningRequestJson.planningList[index].parameters[0].value);
                SessionHayUnoRepetido.maxTime = -1;
            }
            else
            {
                SessionHayUnoRepetido.maxTime = float.Parse(planningRequestJson.planningList[index].parameters[0].value, CultureInfo.InvariantCulture);
                SessionHayUnoRepetido.maxFigures = -1;
            }
            SessionHayUnoRepetido.gameSessionId = planningRequestJson.planningList[index].gameSessionId;
            SceneManager.LoadScene("HayUnoRepetidoScene");
        }
        if (planningRequestJson.planningList[index].game == "Encuentra al Nuevo")
        {
            if (planningRequestJson.planningList[index].parameters[0].name == "figureQuantity")
            {
                SessionEncuentraAlNuevo.maxFigures = int.Parse(planningRequestJson.planningList[index].parameters[0].value);
                SessionEncuentraAlNuevo.maxTime = -1;
            }
            else
            {
                SessionEncuentraAlNuevo.maxTime = float.Parse(planningRequestJson.planningList[index].parameters[0].value, CultureInfo.InvariantCulture);
                SessionEncuentraAlNuevo.maxFigures = -1;
            }
            SessionEncuentraAlNuevo.gameSessionId = planningRequestJson.planningList[index].gameSessionId;
            SceneManager.LoadScene("EncuentraAlNuevoScene");
        }
    }

    [System.Serializable]
    public class Planning
    {
        public int gameSessionId;
        public int numberOfSession;
        public string game;
        public Params[] parameters;
    }

    [System.Serializable]
    public class PlanningList
    {
        public Planning[] planningList;
    }

    [System.Serializable]
    public class Params
    {
        public long id;
        public string name;
        public string value;
    }

    [System.Serializable]
    public class SessionHayUnoRepetido
    {
        static public float maxTime = -1;
        static public int maxFigures = -1;
        static public int gameSessionId;
    }

    [System.Serializable]
    public class SessionEncuentraAlNuevo
    {
        static public float maxTime = -1;
        static public int maxFigures = -1;
        static public int gameSessionId;
    }
}
