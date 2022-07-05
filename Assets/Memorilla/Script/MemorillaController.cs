using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static MainSceneController;

public class MemorillaController : GameController
{

    private const string ENDPOINT = "results/memorilla";

    [SerializeField]
    private int height = 7;
    [SerializeField]
    private int width = 5;
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private float cellSize;
    [SerializeField]
    private int numberOfStimuli = 7;
    [SerializeField]
    private int numberOfLevels = 5;
    [SerializeField]
    private float cellSpaceBetweenColumns = 0.1f;
    [SerializeField]
    private float cellSpaceBetweenRows = 0.1f;
    private List<List<Cell>> grid;
    private bool controlsEnabled;
    private int numberOfGuesses;
    private int levelsPlayed = 0;
    private List<int> successesPerLevel;
    private List<int> mistakesPerLevel;
    private int streak = 0;
    private List<float> timePerLevel;
    private string totalTime;
    private float initTime;
    private float timePreLevel = 5f;
    private float timePostLevel = 3f;
    private int a_score;
    private bool isOnStreak;


    public int Height { get => height; }
    public int Width { get => width; }
    public List<List<Cell>> Grid { get => grid; set => grid = value; }
    public Vector3 Offset { get => offset; }
    public float CellSize { get => cellSize; }
    public int NumberOfStimuli { get => numberOfStimuli; }
    public int NumberOfLevels { get => numberOfLevels; }
    public bool ControlsEnabled { get => controlsEnabled; }
    public float CellSpaceBetweenColumns { get => cellSpaceBetweenColumns; set => cellSpaceBetweenColumns = value; }
    public float CellSpaceBetweenRows { get => cellSpaceBetweenRows; set => cellSpaceBetweenRows = value; }
    public int NumberOfGuesses { get => numberOfGuesses; set => numberOfGuesses = value; }
    public int score { get => a_score < 0 ? 0 : a_score; set => a_score = value; }
    public bool IsOnStreak { get => isOnStreak; set => isOnStreak = value; }

    public GameObject CellPrefab;
    public GameObject GridGameObject;
    public GameObject pauseButton;
    public GameObject endScreen;
    public Text level;
    public Text scoreHUD;
    private bool canceled = false;
    public GameObject HUD;
    public AudioClip tapSound;
    public AudioClip transitionSuccessSound;
    public AudioClip transitionWithErrorsSound;

    void Start()
    {
        TakeControlFromPlayer();
        height = SessionMemorilla.numberOfRows;
        width = SessionMemorilla.numberOfColumns;
        numberOfLevels = SessionMemorilla.maxLevel;
        numberOfStimuli = SessionMemorilla.figureQuantity;
        successesPerLevel = new List<int>();
        mistakesPerLevel = new List<int>();
        timePerLevel = new List<float>();
        cellSize = 600 / Width;
        float originY = -(Height * CellSize + (Height - 1) * CellSpaceBetweenRows) / 2;
        GridGameObject.transform.position = new Vector3(GridGameObject.transform.position.x, originY);
        CreateGrid();
        StartLevel();
        initTime = Time.time;
        level.text = (levelsPlayed + 1).ToString() + " / " + numberOfLevels.ToString();
        scoreHUD.text = score.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            buttonPauseEvent();
        }
    }

    public override void cancelGame()
    {
        this.canceled = true;
    }

    public override void OnApplicationPause()
    {
        this.pause.pauseGame();
    }

    public override void pauseGame()
    {
        deactivateOrActivateCells(false);
    }

    public override void unpauseGame()
    {
        if (Grid != null)
        {
            deactivateOrActivateCells(true);
        }
    }

    /// <summary>
    /// Desactiva o activa las celdas según necesidad
    /// </summary>
    /// <param name="state"></param>
    void deactivateOrActivateCells(bool state)
    {
        foreach (List<Cell> row in Grid)
        {
            foreach (Cell cell in row)
            {
                cell.gameObject.SetActive(state);
                if (state)
                {
                    cell.gameObject.GetComponentInChildren<Animator>().SetInteger("State", (int)cell.State);
                }
            }
        }
    }

    /// <summary>
    /// Muestra la pantalla de fin del juego con el puntaje.
    /// </summary>
    /// <param name="score">Puntaje final.</param>
    public void showEndScreen(int score)
    {
        deactivateOrActivateCells(false);
        pause.gameObject.SetActive(false);
        pauseButton.SetActive(false);
        endScreen.SetActive(true);
        endScreen.GetComponent<EndScreen>().score = score.ToString();
        endScreen.GetComponent<EndScreen>().game = "Memorilla";
        endScreen.GetComponent<EndScreen>().gameSessionId = SessionMemorilla.gameSessionId;
        endScreen.GetComponent<EndScreen>().getScores();
    }

    public override void sendData()
    {
        calculateStreak();
        showEndScreen(a_score);
        string successesPerLevelString = arrayToString(successesPerLevel);
        string mistakesPerLevelString = arrayToString(mistakesPerLevel);
        string timePerLevelString = arrayToString(timePerLevel);
        totalTime = timePerLevel.Sum().ToString().Replace(",", ".");
        string json =
            "{" +
                "'completeDatetime': '" + System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") +
                "', 'canceled': " + this.canceled +
                ", 'mistakesPerLevel': " + mistakesPerLevelString +
                ", 'successesPerLevel': " + successesPerLevelString +
                ", 'streak': " + streak +
                ", 'timePerLevel': " + timePerLevelString +
                ", 'totalTime': " + totalTime +
                ", 'game': 'Memorilla'" +
                ", 'memorillaSessionId': " + SessionMemorilla.gameSessionId +
                ", 'score': " + a_score + "}";
        SendData sD = (new GameObject("SendData")).AddComponent<SendData>();
        sD.sendData(json, ENDPOINT);
    }

    /// <summary>
    /// Recibe un array de enteros y lo convierte en texto para poder enviarlo al back.
    /// </summary>
    /// <param name="listInt"> array de enteros </param>
    /// <returns></returns>
    private string arrayToString(List<int> listInt)
    {
        string stringArray;
        stringArray = "[";
        foreach (int v in listInt)
        {
            stringArray += v.ToString().Replace(",", ".") + ",";
        }
        stringArray = stringArray.Remove(stringArray.Length - 1);
        stringArray += "]";
        return stringArray;
    }

    /// <summary>
    ///  Recibe un array de floats y lo convierte en texto para poder enviarlo al back.
    /// </summary>
    /// <param name="listFloat"> array de floats </param>
    /// <returns></returns>
    private string arrayToString(List<float> listFloat)
    {
        string stringArray;
        stringArray = "[";
        foreach (float v in listFloat)
        {
            stringArray += v.ToString().Replace(",", ".") + ",";
        }
        stringArray = stringArray.Remove(stringArray.Length - 1);
        stringArray += "]";
        return stringArray;
    }


    /// <summary>
    /// Calcula la racha de niveles jugados sin cometer errores.
    /// </summary>
    private void calculateStreak()
    {
        int auxStreak = 0;
        foreach (int mistakes in mistakesPerLevel)
        {
            if (mistakes == 0)
            {
                auxStreak++;
            }
            else
            {
                if (auxStreak > streak)
                {
                    streak = auxStreak;
                }
                auxStreak = 0;
            }
        }
        if (auxStreak > streak)
        {
            streak = auxStreak;
        }
    }

    /// <summary>
    /// Inicializa el nivel actual.
    /// </summary>
    private void StartLevel()
    {
        if (levelsPlayed >= numberOfLevels)
        {
            level.text = levelsPlayed.ToString() + " / " + numberOfLevels.ToString();
            scoreHUD.text = score.ToString();
            PlayGameOverSound();
            sendData();
            HUD.gameObject.SetActive(false);
        }
        else
        {
            NumberOfGuesses = NumberOfStimuli;
            CleanGrid();
            CreateStimuli();
            StartCoroutine(WaitWhileShowingSolution(timePreLevel));
        }
    }

    /// <summary>
    /// Limpia la grilla de estados y colores.
    /// Reinicia todas las celdas al estado UNSELECTED,
    /// y también setea el isActive a false.
    /// </summary>
    private void CleanGrid()
    {
        foreach (List<Cell> row in Grid)
        {
            foreach (Cell cell in row)
            {
                cell.Clear();
            }
        }
    }

    /// <summary>
    /// Selecciona n celdas random para el ejercicio
    /// las cuales conformarán la solución del mismo.
    /// </summary>
    private void CreateStimuli()
    {
        level.text = (levelsPlayed + 1).ToString() + " / " + numberOfLevels.ToString();
        scoreHUD.text = score.ToString();
        for (int i = 0; i < NumberOfStimuli; i++)
        {
            while (true)
            {
                int randomX = Random.Range(0, Width);
                int randomY = Random.Range(0, Height);
                Cell selectedCell = Grid[randomY][randomX];

                if (!selectedCell.IsActive)
                {
                    selectedCell.IsActive = true;
                    selectedCell.State = STATES.SELECTED;

                    break;
                }
            }
        }
    }

    /// <summary>
    /// Inicia una corrutina para esperar 5 segundos
    /// antes de ocultar la solución.
    /// </summary>
    /// <param name="seconds">Cantidad de segundos a esperar.</param>
    /// <returns>Una corrutina.</returns>
    private IEnumerator WaitWhileShowingSolution(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        HideStimuli();
        ReturnControlToPlayer();
    }

    /// <summary>
    /// Esconde los estímulos (la solución).
    /// Cambia el estado de todas las celdas a UNSELECTED.
    /// </summary>
    private void HideStimuli()
    {
        foreach (List<Cell> row in Grid)
        {
            foreach (Cell cell in row)
            {
                cell.State = STATES.UNSELECTED;
            }
        }
    }

    /// <summary>
    /// Devuelve el control al jugador.
    /// Permite al jugador seleccionar celdas de la
    /// grilla.
    /// </summary>
    private void ReturnControlToPlayer()
    {
        controlsEnabled = true;
    }

    /// <summary>
    /// Quita el control al jugador.
    /// Evita que el jugador seleccione celdas de la
    /// grilla.
    /// </summary>
    private void TakeControlFromPlayer()
    {
        controlsEnabled = false;
    }

    /// <summary>
    /// Crea una grilla del tamaño especificado.
    /// </summary>
    private void CreateGrid()
    {
        Grid = new List<List<Cell>>();

        for (int i = 0; i < Height; i++)
        {
            List<Cell> row = new List<Cell>();
            for (int j = 0; j < Width; j++)
            {
                Cell cell = CreateCell(i, j);
                row.Add(cell);
            }
            Grid.Add(row);
        }
    }

    /// <summary>
    /// Crea una celda en una posición particular.
    /// </summary>
    /// <param name="row">Fila de la celda.</param>
    /// <param name="column">Columna de la celda.</param>
    /// <returns>La celda creada.</returns>
    private Cell CreateCell(int row, int column)
    {
        GameObject cellGameObject = Instantiate(CellPrefab, GridGameObject.transform);
        cellGameObject.GetComponent<Transform>().localScale = new Vector3(CellSize, CellSize, 1);
        Cell cell = cellGameObject.GetComponent<Cell>();
        cell.Create(row, column, this);
        cellGameObject.transform.localPosition = new Vector3(cell.PosX, cell.PosY, 0);
        return cell;
    }

    /// <summary>
    /// Evento que gestiona lo que ocurre cuando se clickea
    /// una celda de la grilla.
    /// </summary>
    public void OnCellClicked()
    {
        PlayTapSound();
        NumberOfGuesses--;

        if (NumberOfGuesses == 0)
        {
            TakeControlFromPlayer();
            ShowResult();
            PlayTransitionSound();
            levelsPlayed++;
            StartNextLevel();
        }
    }

    /// <summary>
    /// Reproduce un sonido de tap.
    /// </summary>
    private void PlayTapSound()
    {
        PlaySound(tapSound);
    }

    /// <summary>
    /// Reproduce el sonido de la pantalla de transición.
    /// </summary>
    private void PlayTransitionSound()
    {
        if (mistakesPerLevel.Last() == 0)
        {
            PlaySound(transitionSuccessSound);
        }
        else
        {
            PlaySound(transitionWithErrorsSound);
        }
    }

    /// <summary>
    /// Inicia la corrutina para empezar el siguiente nivel.
    /// </summary>
    private void StartNextLevel()
    {
        StartCoroutine(WaitBeforeNextLevel(timePostLevel));
    }

    /// <summary>
    /// Inicia una corrutina para esperar seconds segundos
    /// antes de pasar al siguiente nivel mientras muestra el resultado.
    /// </summary>
    /// <param name="seconds">Cantidad de segundos a esperar.</param>
    /// <returns>Una corrutina.</returns>
    private IEnumerator WaitBeforeNextLevel(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        StartLevel();
    }

    /// <summary>
    /// Muestra el resultado del ejercicio, además contabiliza los resultados del nivel.
    /// </summary>
    private void ShowResult()
    {
        int contMistakes = 0;
        int contSuccesses = 0;
        foreach (List<Cell> row in Grid)
        {
            foreach (Cell cell in row)
            {
                cell.CheckSolution();
                if (cell.State == STATES.CORRECT)
                {
                    contSuccesses++;
                }
                if (cell.State == STATES.MISS)
                {
                    contMistakes++;
                }
            }
        }
        mistakesPerLevel.Add(contMistakes);
        successesPerLevel.Add(contSuccesses);
        addPointsToScore(calculateScore(contMistakes, NumberOfStimuli, height * width));
        checkStreak(NumberOfStimuli, contMistakes);
        if (levelsPlayed == 0)
        {
            timePerLevel.Add(Time.time - initTime - timePreLevel);
        }
        else
        {
            timePerLevel.Add(Time.time - initTime - timePerLevel[levelsPlayed - 1] - ((timePreLevel + timePostLevel) * (levelsPlayed)) - timePreLevel);
        }
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
    /// Calcula el puntaje parcial correspondiente a cada nivel.
    /// </summary>
    /// <param name="mistakes">Errores cometidos en el nivel actual.</param>
    /// <param name="maxStimuli">Cantidad de estimulos utilizados.</param>
    /// <param name="dimension">Dimension del tablero, filas * columnas.</param>
    /// <returns></returns>
    public int calculateScore(int mistakes, int maxStimuli, int dimension)
    {
        if (IsOnStreak == true)
        {
            return Mathf.RoundToInt(((dimension * 1.25f) + ((maxStimuli - mistakes) * 14)) * 1.1f);
        }
        else
        {
            return Mathf.RoundToInt((dimension * 1.25f) + ((maxStimuli - mistakes) * 14));
        }
    }

    /// <summary>
    /// Chequea si en el ultimo nivel completado no se cometio ningun error, utilizado para calcular el score del nivel siguiente.
    /// </summary>
    /// <param name="maxStimuli">Cantidad de estimulos utilizados.</param>
    /// <param name="mistakes">Errores cometidos en el nivel actual.</param>
    /// <returns></returns>
    public bool checkStreak(int maxStimuli, int mistakes)
    {
        if ((maxStimuli - mistakes) == maxStimuli)
        {
            return IsOnStreak = true;
        }
        else
        {
            return IsOnStreak = false;
        }
    }
}