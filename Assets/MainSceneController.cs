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
    public Text gameName;
    public Text numberOfSessions;
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
        gameName.text = planningRequestJson.planningList[0].game;
        numberOfSessions.text = "Quedan " + planningRequestJson.planningList[0].numberOfSession + " sesiones restantes";

        clearCards();

        highlightButton(gameButton.GetComponent<Image>(), gameSprite[0]);
        highlightButton(profileButton.GetComponent<Image>(), profileSprite[1]);
        highlightButton(homeButton.GetComponent<Image>(), homeSprite[1]);
        bodyText.SetActive(true);
        bodyText.GetComponent<Text>().text = "Juegos pendientes"; 
        print("Boton jugar clickeado");


        /**
         * Por cada juego pendiente, genera una card con la informacion del juego
         * Define un contenedor scrolleable con tamaño igual a la cantidad de cards generadas
         */

        int i = 0;
        cardContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(cardContainer.GetComponent<RectTransform>().sizeDelta.x, cardContainer.GetComponent<RectTransform>().sizeDelta.y + (planningRequestJson.planningList.Length *1.5f));
        foreach (Planning planningCards in planningRequestJson.planningList)
        {
            Button gameCardInstance = Instantiate(gameCard);
            btnClickPlayGame(gameCardInstance, i);
            gameCardInstance.transform.parent = cardContainer.transform;
            gameCardInstance.transform.localScale = new Vector2(0.0078f, 0.0078f);          //Escala actual del canvas
            gameCardInstance.transform.localPosition = new Vector3(0, -1 + (i * -1.5f), 0); //Tamaño de cards + offset
            print(4.45f + (i * -1.8f));

            foreach (Text gameName in gameCardInstance.GetComponentsInChildren<Text>())
            {
                if (gameName.gameObject.name == "GameName")
                {
                    gameName.text = planningCards.game;
                }
                if (gameName.gameObject.name == "NumberOfSessions")
                {
                    gameName.text = "Quedan " + planningCards.numberOfSession + " sesiones restantes";
                }
            }
            i++;

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
        bodyText.GetComponent<Text>().text = "¡Hola de nuevo " + settings.Login.patient.firstName + "!";
        print("Boton inicio clickeado");
    }
    
    /**
     * Muestra todos los elementos de la seccion "Perfil" de la aplicacion
     */
    public void btnProfile()
    {
        clearCards();
        title.text = "Perfil";
        highlightButton(gameButton.GetComponent<Image>(), gameSprite[1]);
        highlightButton(profileButton.GetComponent<Image>(), profileSprite[0]);
        highlightButton(homeButton.GetComponent<Image>(), homeSprite[1]);


        bodyText.SetActive(false);
        print("Boton perfil clickeado");
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
        gameName.text = planningRequestJson.planningList[0].game;
        numberOfSessions.text = "Quedan " + planningRequestJson.planningList[0].numberOfSession + " sesiones restantes";
    }

    /**
     * Ejecuta una instancia de juego segun los parametros definidos en la planificacion
     */
    public void playGame(int index)
    {
        if (planningRequestJson.planningList[index].game == "Encuentra al Repetido")
        {
            if (planningRequestJson.planningList[index].parameters[0].name=="figureQuantity")
            {
                SessionHayUnoRepetido.maxFigures = int.Parse(planningRequestJson.planningList[index].parameters[0].value);

            } else
            {
                SessionHayUnoRepetido.maxTime = float.Parse(planningRequestJson.planningList[index].parameters[0].value, CultureInfo.InvariantCulture);
            }
            SceneManager.LoadScene("HayUnoRepetidoScene");
        }
    }

    [System.Serializable]
    public class Planning
    {
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
        
    }


}
