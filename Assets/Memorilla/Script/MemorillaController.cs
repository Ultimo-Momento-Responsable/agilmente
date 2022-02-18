using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemorillaController : GameController
{
    [SerializeField]
    private int height = 4;
    [SerializeField]
    private int width = 4;
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

    public int Height { get => height; }
    public int Width { get => width; }
    public List<List<Cell>> Grid { get => grid; set => grid = value; }
    public Vector3 Offset { get => offset; }
    public float CellSize { get => cellSize; }
    public int NumberOfStimuli { get => numberOfStimuli; }
    public int NumberOfLevels { get => numberOfLevels; }
    public bool ControlsEnabled { get => controlsEnabled; set => controlsEnabled = value; }

    public GameObject CellPrefab;
    public GameObject GridGameObject;

    void Start()
    {
        takeControlFromPlayer();
        cellSize = CellPrefab.gameObject.transform.localScale.x;
        float originY = -(Height * CellSize + (Height - 1) * cellSpaceBetweenRows) / 2;
        GridGameObject.transform.position = new Vector3(GridGameObject.transform.position.x, originY);
        CreateGrid();
        StartLevel();
    }



    void Update()
    {

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
        throw new System.NotImplementedException();
    }

    public override void unpauseGame()
    {
        throw new System.NotImplementedException();
    }

    private void StartLevel()
    {
        CleanGrid();
        CreateStimuli();
        StartCoroutine(StartCountdown());
        ReturnControlToPlayer();
    }

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
    private void CreateStimuli()

    {
        for (int i = 0; i < NumberOfStimuli; i++)
        {
            while (true) {
                int randomX = Random.Range(0, Width);
                int randomY = Random.Range(0, Height);
                Cell selectedCell = Grid[randomX][randomY];

                if (!selectedCell.IsActive)
                {
                    selectedCell.IsActive = true;
                    selectedCell.State = STATES.SELECTED;
                    Debug.Log("El espacio " + randomX + " " + randomY + " fue asignado como activo");

                    break;
                }
                Debug.Log("El espacio ya se encuentra ocupado, rerolleando");
            }
            // Elegir un (randomX, RandomY) asignarle el cuadro, si esta usado, rerollear
            Debug.Log("asignacion de espacio numero: " + i);
        }
    }
    private IEnumerator StartCountdown()
    {
        Debug.Log("Esperando 5 segundos antes de comenzar");
        yield return new WaitForSeconds(5);
        Debug.Log("Comienzo de juego");
        HideStimuli();
        ReturnControlToPlayer();
    }
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
    private void ReturnControlToPlayer()
    {
        controlsEnabled = true;
    }

    private void takeControlFromPlayer()
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
    /// <returns></returns>
    private Cell CreateCell(int row, int column)
    {
        GameObject cellGameObject = Instantiate(CellPrefab, GridGameObject.transform);
        Cell cell = new Cell(row, column, cellGameObject);
        //cellGameObject.transform.localPosition = new Vector3(column * (CellSize + cellSpaceBetweenColumns), row * (CellSize + cellSpaceBetweenRows), 0);
        cellGameObject.transform.localPosition = new Vector3(column * (CellSize + cellSpaceBetweenColumns), row * (CellSize + cellSpaceBetweenRows), 0);
        return cell;
    }
}