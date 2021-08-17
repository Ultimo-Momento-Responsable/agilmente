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


    public void Start()
    {
        getPlanning();
    }
    
    //public void OnGUI()
    //{
    //    int i = 0;

    //    Vector2 cardSize = camera.ViewportToWorldPoint(new Vector2(670, 175));
    //    foreach (Planning planningCards in planningRequestJson.planningList)
    //    {
    //        Vector2 cardPosition = camera.ViewportToWorldPoint(new Vector2(0.0347f, 0.26f + (i * 0.15f)));
    //        Rect card = new Rect(cardPosition, cardSize);
    //        i++;
    //        //Color cardColor = new Color(0f, 1f, 0f, 0.5f);
    //        GUIStyle cardColor = new GUIStyle();
    //        GUI.backgroundColor = Color.blue;
    //        GUI.Button(card, "");
    //    }

    //}

    public void highlightButton(Image buttonImage, Sprite highlightedButton)
    {
        buttonImage.sprite = highlightedButton;
    }

    public void btnGames()
    {
        title.text = "Jugar";
        gameName.text = planningRequestJson.planningList[0].game;
        numberOfSessions.text = "Quedan " + planningRequestJson.planningList[0].numberOfSession + " sesiones restantes";

        highlightButton(gameButton.GetComponent<Image>(), gameSprite[0]);
        highlightButton(profileButton.GetComponent<Image>(), profileSprite[1]);
        highlightButton(homeButton.GetComponent<Image>(), homeSprite[1]);
        GameObject.FindGameObjectWithTag("gameCard").SetActive(false);
        bodyText.SetActive(true);
        bodyText.GetComponent<Text>().text = "Juegos pendientes"; 
        print("Boton jugar clickeado");

        int i = 0;

        foreach (Planning planningCards in planningRequestJson.planningList)
        {
            Button gameCardInstance = Instantiate(gameCard);
            btnClickPlayGame(gameCardInstance, i);
            gameCardInstance.transform.parent = gameCanvas.transform;
            gameCardInstance.transform.localScale = new Vector2(1, 1);
            gameCardInstance.transform.localPosition = new Vector3(0, 222.5f + (i*-222.5f), 0);
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

    void btnClickPlayGame(Button btnPlayGame, int index)
    {
        btnPlayGame.onClick.AddListener(() => playGame(index));
    }



    public void btnHome()
    {
        title.text = "Inicio";
        highlightButton(gameButton.GetComponent<Image>(), gameSprite[1]);
        highlightButton(profileButton.GetComponent<Image>(), profileSprite[1]);
        highlightButton(homeButton.GetComponent<Image>(), homeSprite[0]);

        bodyText.SetActive(true);
        settings = JsonUtility.FromJson<Settings>(System.IO.File.ReadAllText(Application.dataPath + "/settings.json"));
        bodyText.GetComponent<Text>().text = "¡Hola de nuevo " + settings.Login.patient.firstName + "!";
        print("Boton inicio clickeado");
    }
    public void btnProfile()
    {
        title.text = "Perfil";
        highlightButton(gameButton.GetComponent<Image>(), gameSprite[1]);
        highlightButton(profileButton.GetComponent<Image>(), profileSprite[0]);
        highlightButton(homeButton.GetComponent<Image>(), homeSprite[1]);

        bodyText.SetActive(false);
        print("Boton perfil clickeado");
    }

    //GET PARA PLANNING DESDE BACKEND - 
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
     * Una vez que obtiene los datos del paciente se modifica el json settings.json para que no pida nuevamente el logueo
     */
    private void getPlanningResponseCallback(string data) { 
        //recorrer con un for, para Guille es solo el primero
        planningRequestJson = JsonUtility.FromJson<PlanningList>(data);
        gameName.text = planningRequestJson.planningList[0].game;
        numberOfSessions.text = "Quedan " + planningRequestJson.planningList[0].numberOfSession + " sesiones restantes";
    }

    public void playGame(int index)
    {
        print("llega a ejecutar Play Game");
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
    //recibir del JSON para planningRequest
    //al data PlanningRequest.Planning[i].numberOfSessions
    //al data PlanningRequest.Planning[i].name
    //Para guille es solo el mas proximo/primero
    //crear variable del tipo PlanningRequest con nombre
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
