using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneController : MonoBehaviour
{
    public Text title;
    public Text medals;
    public Text trophies;
    public GameObject completedGamesText;
    public GameObject bodyText;
    public GameObject gameText;
    public GameObject collapsable;
    public Button gameCard;
    public GameObject gameCanvas;
    public GameObject cardContainer;
    public GameObject medalAnimation;
    public GameObject trophyAnimation;
    public Sprite maxTime;
    public Sprite maxLevel;
    public Camera mainCamera;
    public GameObject currentPlanningCanvas;
    public GameObject completedPlanningCanvas;
    public Color agilmenteBlue;

    private Animator medalAnimator;
    private Animator trophyAnimator;
    private Settings settings;
    private PlanningList planningRequestJson;
    private List<Planning> completedPlannings;
    private List<Planning> uncompletedPlannings;
    private static string endpoint = "planning/mobile_patient/";

    /**
     * Inicio de escena, genera una request que obtiene un JSON con los juegos pendientes asignados a una planificacion
     */
    public void Start()
    {
        settings = JsonUtility.FromJson<Settings>(System.IO.File.ReadAllText(Application.persistentDataPath + "/settings.json"));
        medals.text = settings.Login.patient.medals.ToString();
        trophies.text = settings.Login.patient.trophies.ToString();
        getPlanning();
        getMedalsAndTrophies();
    }

    /**
     * Inicia la animación del aumento de medallas
     */
    private void startMedalAnimation() {
        medalAnimator = medalAnimation.GetComponent<Animator>();
        medalAnimator.Play("medalsAndTrophiesAnimation");
    }

    /**
     * Inicia la animación del aumento de trofeos
     */
    private void startTrophyAnimation() {
        trophyAnimator = trophyAnimation.GetComponent<Animator>();
        trophyAnimator.Play("medalsAndTrophiesAnimation");
    }

    /**
     * Destruye todas las tarjetas actualmente creadas, utilizado para navegabilidad entre menus. 
     */ 
    public void clearCards()
    {
        foreach (GameObject cardsToDestroy in GameObject.FindGameObjectsWithTag("collapsable"))
        {
            Destroy(cardsToDestroy);
        }
    }

    /**
     * GET para las medallas y trofeos del paciente
     */
    void getMedalsAndTrophies(){
        this.StartCoroutine(this.getRoutine(SendData.IP + "patient/medalsAndTrophies/" + settings.Login.patient.id, this.getMedalsAndTrophiesResponseCallback));
    }

    /**
     * Callback utilizado en getMedalsAndTrophies(), asigna a variables de Unity los datos obtenidos del JSON
     */
    private void getMedalsAndTrophiesResponseCallback(string data) { 
        MedalsAndTrophies medalsAndTrophies = JsonUtility.FromJson<MedalsAndTrophies>(data);
        checkForMedalOrTrophieChanges(medalsAndTrophies);
    }

    /**
     * Chequea si hubo un aumento de trofeos o medallas para ejecutar la animación.
     * @Param medalsAndTrophies medallas y trofeos traídos del backend
     */
    private void checkForMedalOrTrophieChanges(MedalsAndTrophies medalsAndTrophies) {
        if (settings.Login.patient.medals != medalsAndTrophies.medals) {
            startMedalAnimation();
            StartCoroutine(waitForAnimation(medalsAndTrophies));
            settings.Login.patient.medals = medalsAndTrophies.medals;
            File.WriteAllText(Application.persistentDataPath + "/settings.json", JsonUtility.ToJson(settings));
        }
        if (settings.Login.patient.trophies != medalsAndTrophies.trophies) {
            startTrophyAnimation();
            StartCoroutine(waitForAnimation(medalsAndTrophies));
            settings.Login.patient.trophies = medalsAndTrophies.trophies;
            File.WriteAllText(Application.persistentDataPath + "/settings.json", JsonUtility.ToJson(settings));
        }
    }

    /**
     * Hace una espera de 1 segundo antes de actualizar el contador de trofeos y medallas
     * @Param medalsAndTrophies medallas y trofeos traídos del backend
     */
    private IEnumerator waitForAnimation(MedalsAndTrophies medalsAndTrophies) {
        yield return new WaitForSeconds(1);
        medals.text = medalsAndTrophies.medals.ToString();
        trophies.text = medalsAndTrophies.trophies.ToString();
    }

    /**
     * GET para planificaciones desde backend para un paciente
     */
    public void getPlanning()
    {
        this.StartCoroutine(this.getRoutine(SendData.IP + endpoint + settings.Login.patient.id, this.getPlanningResponseCallback));
    }

    /**
     * Se hace un get a los pacientes para ver si ese código de Logueo existe
     */
    private IEnumerator getRoutine(string url, Action<string> callback = null)
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
        planningRequestJson.planningList = planningRequestJson.planningList.OrderByDescending(p => p.totalGames).ToArray();
        completedPlannings = new List<Planning>();
        uncompletedPlannings = new List<Planning>();
        foreach (Planning p in planningRequestJson.planningList)
        {
            p.planningList = SortPlannings(p.planningList);
            if (p.gamesPlayed==p.totalGames)
            {
                if (p.unlimited){
                    uncompletedPlannings.Add(p);
                } else {
                    completedPlannings.Add(p);
                }
            } else
            {
                uncompletedPlannings.Add(p);
            }
        }
        ShowGameCards();
    }

    /**
     * Llama al inicio del juego cuando se clickea en alguna de las cards
     */
    void btnClickPlayGame(Button btnPlayGame, int planningIndex, int gameSessionIndex)
    {
        btnPlayGame.onClick.AddListener(() => playGame(planningIndex, gameSessionIndex));
    }

    /**
     * Muestra todos los elementos de la seccion "Inicio" de la aplicacion
     */
    public void ShowGameCards()
    {
        clearCards();
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
            foreach (Planning p in uncompletedPlannings)
            {
                string[] dateSplitted = p.dueDate.Split('-');
                DateTime dueDate = new DateTime(int.Parse(dateSplitted[2]), int.Parse(dateSplitted[1]), int.Parse(dateSplitted[0]));
                DateTime now = DateTime.Now;
                string daysLeft = ((int)(dueDate - now).TotalDays).ToString();
                int j = 0;
                GameObject collapsablePlanning = createCollapsable(p, false, daysLeft);
                foreach (GameSession planningCard in p.planningList)
                {
                    createPlanningCard(planningCard, collapsablePlanning, i, j);
                    j++;
                }
                i++;
            }
            if (completedPlannings.Count > 0)
            {
                completedGamesText.gameObject.SetActive(true);
                foreach (Planning p in completedPlannings) {
                    int j = 0;
                    GameObject collapsablePlanning = createCollapsable(p, true);
                    foreach (GameSession planningCard in p.planningList)
                    {
                        createPlanningCard(planningCard, collapsablePlanning, i, j);
                        j++;
                    }
                    i++;
                }
            }
        }
    }

    /**
     * Crea la card de la sesión.
     * @Param planningCard Sesión a la que corresponde,
     * @Param collapsablePlanning en el colapsable que tiene que estar,
     * @Param gameSessionIndex, índice del colapsable
     * @Param planningIndex, índice de la card.
     */
    private void createPlanningCard(GameSession planningCard, GameObject collapsablePlanning, int gameSessionIndex, int planningIndex)
    {
        Button gameCardInstance = Instantiate(gameCard, collapsablePlanning.GetComponent<Collapsable>().body.transform);
        GameSessionCard gameSessionCard = gameCardInstance.GetComponent<GameSessionCard>();
        gameSessionCard.InitializeCard(planningCard.game, planningCard.numberOfSession);

       if (planningCard.numberOfSession != 0)
        {
            btnClickPlayGame(gameCardInstance, gameSessionIndex, planningIndex);
        }
    }

    /**
     * Crea el colapsable.
     * @Param p Planning a la que corresponde,
     * @Param completed si está completada o no,
     * @Param daysLeft, días restantes de la planning.
     * @Return collapsablePlanning, devuelve el colapsable creado.
     */
    private GameObject createCollapsable(Planning p, bool completed, string daysLeft = "")
    {
        GameObject collapsablePlanning;
        if (completed)
        {
            collapsablePlanning = Instantiate(collapsable, completedPlanningCanvas.transform);
            collapsablePlanning.GetComponent<Collapsable>().SetCompleted(p.gamesPlayed, p.totalGames);
        }
        else
        {
            collapsablePlanning = Instantiate(collapsable, currentPlanningCanvas.transform);
            collapsablePlanning.GetComponent<Collapsable>().SetPlanningData(p.gamesPlayed, p.totalGames, p.unlimited, daysLeft);
        }
        return collapsablePlanning;
    }


    /**
     * Ordena los juegos de las plannings para que queden los que tienen sesiones arriba, en el medio los libres, y abajo los completados.
     */
    private GameSession[] SortPlannings(GameSession[] plannings)
    {
        List<GameSession> planningList = new List<GameSession>();
        foreach (GameSession p in plannings)
        {
            if (p.numberOfSession > 0)
            {
                if (!planningList.Contains(p))
                {
                    planningList.Add(p);
                }
            }
        }
        foreach (GameSession p in plannings)
        {
            if (p.numberOfSession == -1)
            {
                if (!planningList.Contains(p))
                {
                    planningList.Add(p);
                }
            }
        }
        foreach (GameSession p in plannings)
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

    /// <summary>
    /// Checkea si hay planificaciones sin completar.
    /// </summary>
    /// <returns>Devuelve verdadero si hay plannings vigentes.</returns>
    private bool isThereAPlanning()
    {
        return uncompletedPlannings.Count > 0;
    }


    /**
     * Ejecuta una instancia de juego segun los parametros definidos en la planificacion
     */
    public void playGame(int planningIndex, int gameSessionIndex)
    {
        if (uncompletedPlannings[planningIndex].planningList[gameSessionIndex].game == "Encuentra al Repetido")
        {
            foreach (Params param in uncompletedPlannings[planningIndex].planningList[gameSessionIndex].parameters)
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
            SessionHayUnoRepetido.gameSessionId = uncompletedPlannings[planningIndex].planningList[gameSessionIndex].gameSessionId;
            SceneManager.LoadScene("HayUnoRepetidoScene");
        }
        if (uncompletedPlannings[planningIndex].planningList[gameSessionIndex].game == "Encuentra al Nuevo")
        {
            foreach (Params param in uncompletedPlannings[planningIndex].planningList[gameSessionIndex].parameters)
            {
                if (param.name == "maxLevel")
                {
                    SessionEncuentraAlNuevo.maxLevel = int.Parse(uncompletedPlannings[planningIndex].planningList[gameSessionIndex].parameters[0].value);
                    SessionEncuentraAlNuevo.maxTime = -1;
                }
                if (param.name == "maximumTime")
                {
                    SessionEncuentraAlNuevo.maxTime = float.Parse(uncompletedPlannings[planningIndex].planningList[gameSessionIndex].parameters[0].value, CultureInfo.InvariantCulture);
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
            SessionEncuentraAlNuevo.gameSessionId = uncompletedPlannings[planningIndex].planningList[gameSessionIndex].gameSessionId;
            SceneManager.LoadScene("EncuentraAlNuevoScene");
        }

        if (uncompletedPlannings[planningIndex].planningList[gameSessionIndex].game == "Memorilla")
        {
            foreach (Params param in uncompletedPlannings[planningIndex].planningList[gameSessionIndex].parameters)
            {
                if (param.name == "maxLevel")
                {
                    SessionMemorilla.maxLevel = int.Parse(uncompletedPlannings[planningIndex].planningList[gameSessionIndex].parameters[0].value);
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
            SessionMemorilla.gameSessionId = uncompletedPlannings[planningIndex].planningList[gameSessionIndex].gameSessionId;
            SceneManager.LoadScene("MemorillaScene");
        }
    }

    [System.Serializable]
    public class GameSession
    {
        public int gameSessionId;
        public int numberOfSession;
        public string game;
        public Params[] parameters;
    }

    [System.Serializable]
    public class Planning
    {
        public int planningId;
        public int totalGames;
        public int gamesPlayed;
        public string dueDate;
        public bool unlimited;
        public GameSession[] planningList;
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

    [System.Serializable]
    public class MedalsAndTrophies 
    {
        public int medals = 0;
        public int trophies = 0;
    }
}
