using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneController : MonoBehaviour
{
    public Text title;
    public GameObject bodyText;
    private Settings settings;
    public Button profileButton;
    public Button backButton;
    public Button viewGamesButton;
    public Button notificationButton;
    public Sprite[] gameLogo;
    public GameObject gameText;
    private PlanningList planningRequestJson;
    public Camera camera;
    public Button gameCard;
    public GameObject gameCanvas;
    public GameObject cardContainer;
    public Sprite maxTime;
    public Sprite maxLevel;
    private static string endpoint = "/planning/mobile_patient_";

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
    public void ShowGameCards()
    {
        clearCards();
        viewGamesButton.gameObject.SetActive(false);
        bodyText.SetActive(true);

        if (isThereAPlanning())
        {
            gameText.GetComponent<Text>().text = "Juegos pendientes";
        }
        else
        {
            gameText.GetComponent<Text>().text = "No tiene ningún juego pendiente";
        }

        /**
         * Por cada juego pendiente, genera una card con la informacion del juego
         * Define un contenedor scrolleable con tamaño igual a la cantidad de cards generadas
         */
        if (planningRequestJson.planningList.Length != 0)
        {
            int i = 0;
            cardContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(cardContainer.GetComponent<RectTransform>().sizeDelta.x, (planningRequestJson.planningList.Length * 1.65f));
            bodyText.transform.localPosition = new Vector2(0, -0.6f);
            gameText.transform.localPosition = new Vector2(0, -1.6f);
            foreach (Planning planningCards in planningRequestJson.planningList)
            {
                Button gameCardInstance = Instantiate(gameCard);

                gameCardInstance.transform.parent = cardContainer.transform;
                gameCardInstance.transform.localScale = new Vector2(0.0078f, 0.0078f);          //Escala actual del canvas
                gameCardInstance.transform.localPosition = new Vector3(0, -2.3f + (i * -1.3f), 0); //Tamaño de cards + offset

                foreach (Sprite logo in gameLogo)
                {
                    if (planningCards.game == logo.name)
                    {
                        foreach (Image gameCardImage in gameCardInstance.GetComponentsInChildren<Image>())
                        {
                            if (gameCardImage.gameObject.name == "Logo")
                            {
                                gameCardImage.sprite = logo;
                            }
                            if (gameCardImage.gameObject.name == "MedalComplete")
                            {
                                if (planningCards.numberOfSession == 0)
                                {
                                    gameCardImage.gameObject.SetActive(true);
                                } else
                                {
                                    gameCardImage.gameObject.SetActive(false);
                                }
                            }
                        }
                    }
                }

                foreach (Text gameName in gameCardInstance.GetComponentsInChildren<Text>())
                {
                    if (gameName.gameObject.name == "GameName")
                    {
                        gameName.text = planningCards.game;
                    }
                    if (planningCards.numberOfSession > 0)
                    {
                        if (gameName.gameObject.name == "NumberOfSessions")
                        {
                            gameName.text = "Quedan " + planningCards.numberOfSession + " partidas por jugar";
                        }
                    }
                    else if (planningCards.numberOfSession == 0)
                    {
                        if (gameName.gameObject.name == "NumberOfSessions")
                        {
                            gameName.text = "No quedan partidas restantes";
                        }
                    } else
                    {
                        if (gameName.gameObject.name == "NumberOfSessions")
                        {
                            gameName.text = "¡Juega libremente!";
                        }
                        gameName.color = gameCardInstance.GetComponent<Image>().color;
                    }
                }
                if (planningCards.numberOfSession == -1)
                {
                    gameCardInstance.GetComponent<Image>().color = Color.white;
                }
                if (planningCards.numberOfSession != 0)
                {
                    btnClickPlayGame(gameCardInstance, i);
                }
                i++;
            }
        }
    }
    
    /**
     * Muestra todos los elementos de la sección "Perfil" de la aplicación
     */
    public void GoToProfile()
    {
        viewGamesButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
        profileButton.gameObject.SetActive(false);
        notificationButton.gameObject.SetActive(false);
        gameText.SetActive(false);
        clearCards();
        viewGamesButton.enabled = true;
        title.text = "Perfil";
        bodyText.SetActive(false);
    }

    /**
     * Vuelve a la pantalla de Home
     */
    public void BackToHome()
    {
        viewGamesButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        profileButton.gameObject.SetActive(true);
        notificationButton.gameObject.SetActive(true);
        gameText.SetActive(true);
        bodyText.SetActive(true);
        title.text = "AgilMente";
        clearCards();
        ShowGameCards();
    }

    /**
     * GET para planificaciones desde backend para un paciente
     */
    public void getPlanning()
    {
        settings = JsonUtility.FromJson<Settings>(System.IO.File.ReadAllText(Application.persistentDataPath + "/settings.json"));
        this.StartCoroutine(this.getPlanningRoutine(SendData.IP + endpoint + settings.Login.patient.id, this.getPlanningResponseCallback));
    }

    /**
     * Se hace un get a los pacientes para ver si ese código de Logueo existe
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
        planningRequestJson.planningList = SortPlannings(planningRequestJson.planningList);
        ShowGameCards();
    }

    /**
     * Ordena los juegos de las plannings para que queden los que tienen sesiones arriba, en el medio los libres, y abajo los completados.
     */
    private Planning[] SortPlannings(Planning[] plannings)
    {
        List<Planning> planningList = new List<Planning>();
        foreach (Planning p in plannings)
        {
            if (p.numberOfSession > 0)
            {
                if (!planningList.Contains(p))
                {
                    planningList.Add(p);
                }
            }
        }
        foreach (Planning p in plannings)
        {
            if (p.numberOfSession == -1)
            {
                if (!planningList.Contains(p))
                {
                    planningList.Add(p);
                }
            }
        }
        foreach (Planning p in plannings)
        {
            if (p.numberOfSession == 0)
            {
                if (!planningList.Contains(p))
                {
                    planningList.Add(p);
                }
            }
        }
        return planningList.ToArray();
    }

    /**
     * Checkea si hay planning
     */
    private bool isThereAPlanning()
    {
        return planningRequestJson.planningList.Length > 0;
    }


    /**
     * Ejecuta una instancia de juego segun los parametros definidos en la planificacion
     */
    public void playGame(int index)
    {
        if (planningRequestJson.planningList[index].game == "Encuentra al Repetido")
        {
            foreach (Params param in planningRequestJson.planningList[index].parameters)
            {
                if (param.name == "maxLevel")
                {
                    SessionHayUnoRepetido.maxLevel = int.Parse(param.value);
                    SessionHayUnoRepetido.maxTime = -1;
                }
                if (param.name == "maximumTime")
                {
                    SessionHayUnoRepetido.maxTime = float.Parse(param.value, CultureInfo.InvariantCulture);
                    SessionHayUnoRepetido.maxLevel = -1;
                }
                if (param.name == "variableSize")
                {
                    SessionHayUnoRepetido.variableSizes = bool.Parse(param.value);
                }
                if (param.name == "distractors")
                {
                    SessionHayUnoRepetido.distractors = bool.Parse(param.value);
                }
                if (param.name == "spriteSet")
                {
                    SessionHayUnoRepetido.spriteSet = int.Parse(param.value);
                }
                if (param.name == "figureQuantity")
                {
                    SessionHayUnoRepetido.figureQuantity = int.Parse(param.value);
                }
            }
            SessionHayUnoRepetido.gameSessionId = planningRequestJson.planningList[index].gameSessionId;
            SceneManager.LoadScene("HayUnoRepetidoScene");
        }
        if (planningRequestJson.planningList[index].game == "Encuentra al Nuevo")
        {
            foreach (Params param in planningRequestJson.planningList[index].parameters)
            {
                if (param.name == "maxLevel")
                {
                    SessionEncuentraAlNuevo.maxLevel = int.Parse(planningRequestJson.planningList[index].parameters[0].value);
                    SessionEncuentraAlNuevo.maxTime = -1;
                }
                if (param.name == "maximumTime")
                {
                    SessionEncuentraAlNuevo.maxTime = float.Parse(planningRequestJson.planningList[index].parameters[0].value, CultureInfo.InvariantCulture);
                    SessionEncuentraAlNuevo.maxLevel = -1;
                }
                if (param.name == "spriteSet")
                {
                    SessionEncuentraAlNuevo.spriteSet = int.Parse(param.value);
                }
                if (param.name == "variableSize")
                {
                    SessionEncuentraAlNuevo.variableSizes = bool.Parse(param.value);
                }
            }
            SessionEncuentraAlNuevo.gameSessionId = planningRequestJson.planningList[index].gameSessionId;
            SceneManager.LoadScene("EncuentraAlNuevoScene");
        }

        if (planningRequestJson.planningList[index].game == "Memorilla")
        {
            foreach (Params param in planningRequestJson.planningList[index].parameters)
            {
                if (param.name == "maxLevel")
                {
                    SessionMemorilla.maxLevel = int.Parse(planningRequestJson.planningList[index].parameters[0].value);
                }
                if (param.name == "figureQuantity")
                {
                    SessionMemorilla.figureQuantity = int.Parse(param.value);
                }
                if (param.name == "numberOfRows")
                {
                    SessionMemorilla.numberOfRows = int.Parse(param.value);
                }
                if (param.name == "numberOfColumns")
                {
                    SessionMemorilla.numberOfColumns = int.Parse(param.value);
                }
            }
            SessionMemorilla.gameSessionId = planningRequestJson.planningList[index].gameSessionId;
            SceneManager.LoadScene("MemorillaScene");
        }
    }

    /**
     * Muestra la pantalla del historial de partidas
     */
    public void viewHistory()
    {
        SceneManager.LoadScene("viewGamesPlayedScene");
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
        static public int maxLevel = -1;
        static public int gameSessionId;
        static public bool variableSizes = false;
        static public bool distractors = false;
        static public int spriteSet = 1;
        static public int figureQuantity = 20;
    }

    [System.Serializable]
    public class SessionEncuentraAlNuevo
    {
        static public float maxTime = -1;
        static public int maxLevel = -1;
        static public int gameSessionId;
        static public int spriteSet = 1;
        static public bool variableSizes = false;
    }

    [System.Serializable]
    public class SessionMemorilla
    {
        static public int maxLevel = -1;
        static public int gameSessionId;
        static public int figureQuantity = 15;
        static public int numberOfRows = 6;
        static public int numberOfColumns = 8;
    }
}
