using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

    public GameObject CellPrefab;
    public GameObject GridGameObject;

    void Start()
    {
        TakeControlFromPlayer();
        cellSize = CellPrefab.gameObject.transform.localScale.x;
        float originY = -(Height * CellSize + (Height - 1) * CellSpaceBetweenRows) / 2;
        GridGameObject.transform.position = new Vector3(GridGameObject.transform.position.x, originY);
        height = SessionMemorilla.numberOfRows;
        width = SessionMemorilla.numberOfColumns;
        numberOfLevels = SessionMemorilla.maxLevel;
        numberOfStimuli = SessionMemorilla.figureQuantity;
        successesPerLevel = new List<int>();
        mistakesPerLevel = new List<int>();
        timePerLevel = new List<float>();
        CreateGrid();
        StartLevel();
        initTime = Time.time;
    }

    public override void cancelGame()
    {
        throw new System.NotImplementedException();
    }

    public override void OnApplicationPause()
    {
        throw new System.NotImplementedException();
    }

    public override void pauseGame()
    {
        throw new System.NotImplementedException();
    }

    public override void sendData()
    {
        calculateStreak();
        string successesPerLevelString = arrayToString(successesPerLevel);
        string mistakesPerLevelString = arrayToString(mistakesPerLevel);
        string timePerLevelString = arrayToString(timePerLevel);
        totalTime = timePerLevel.Sum().ToString().Replace(",", ".");
        string json =
            "{" +
                "'completeDatetime': '" + System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") +
                "', 'canceled': " + false +
                ", 'mistakesPerLevel': " + mistakesPerLevelString +
                ", 'successesPerLevel': " + successesPerLevelString +
                ", 'streak': " + streak +
                ", 'timePerLevel': " + timePerLevelString +
                ", 'totalTime': " + totalTime +
                ", 'game': 'Memorilla'" +
                ", 'memorillaSessionId': " + SessionMemorilla.gameSessionId +
                ", 'score': " + 0 + "}";
        SendData sD = (new GameObject("SendData")).AddComponent<SendData>();
        sD.sendData(json, ENDPOINT);
        goToMainScene();
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

    public override void unpauseGame()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Inicializa el nivel actual.
    /// </summary>
    private void StartLevel()
    {
        if (levelsPlayed >= numberOfLevels)
        {
            sendData();
        }
        NumberOfGuesses = NumberOfStimuli;
        CleanGrid();
        CreateStimuli();
        StartCoroutine(WaitWhileShowingSolution(timePreLevel));
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
        for (int i = 0; i < NumberOfStimuli; i++)
        {
            while (true) {
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
        NumberOfGuesses--;

        if (NumberOfGuesses == 0)
        {
            TakeControlFromPlayer();
            ShowResult();
            levelsPlayed++;
            StartNextLevel();
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
        if (levelsPlayed == 0)
        {
            timePerLevel.Add(Time.time - initTime - timePreLevel);
        }
        else
        {
            timePerLevel.Add(Time.time - initTime - timePerLevel[levelsPlayed-1] - ((timePreLevel + timePostLevel)*(levelsPlayed)) - timePreLevel);
        }
    }
}