using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleGameGrid : MonoBehaviour
{
    public GameObject templateCell;
    private RectTransform gridRectTransform;
    private GridLayoutGroup gridLayoutGroup;
    public List<GameObject> availableCells;
    public List<GameObject> cells;
    public GameObject figurePrefab;
    private float HorizontalSpacing { get => GridLayoutGroup.spacing.x; }
    private float VerticalSpacing { get => GridLayoutGroup.spacing.y; }
    private RectTransform GridRectTransform { get => gridRectTransform == null ? GetComponent<RectTransform>() : gridRectTransform; }
    private GridLayoutGroup GridLayoutGroup { get => gridLayoutGroup == null ? GetComponent<GridLayoutGroup>() : gridLayoutGroup; }
    public Vector2 CellSize { get => GridLayoutGroup.cellSize; }
    public float GridWidth { get => GridRectTransform.rect.width; }
    public float GridHeight { get => GridRectTransform.rect.height; }
    public float NumberOfColumns { get => Mathf.FloorToInt((GridWidth - CellSize.x) / (CellSize.x + HorizontalSpacing)) + 1; }
    public float NumberOfRows { get => Mathf.FloorToInt((GridHeight - CellSize.y) / (CellSize.y + VerticalSpacing)) + 1; }
    private static float MAX_VARIABLE_SIZE_ADJUSTMENT = 0f;
    public float MaxVariableSize { get => (HorizontalSpacing / 2) - MAX_VARIABLE_SIZE_ADJUSTMENT; }

    /// <summary>
    /// Crea una grilla que se ajusta al tamaño de la pantalla para 
    /// EAN y EAR.
    /// </summary>
    public void CreateCells()
    {
        if (cells != null && cells.Count > 0)
        {
            foreach (GameObject cell in cells)
            {
                Destroy(cell);
            }
        }

        cells = new List<GameObject>();
        availableCells = new List<GameObject>();

        for (int i = 1; i < NumberOfColumns * NumberOfRows; i++)
        {
            GameObject cell = Instantiate(templateCell, transform);
            cells.Add(cell);
            availableCells.Add(cell);
        }
    }

    /// <summary>
    /// Crea una figura en un lugar random de la pantalla.
    /// No se debe usar para el tutorial.
    /// </summary>
    /// <param name="figure">El prefab de la figura a instanciar.</param>
    public void CreateFigureOnRandomCell(Sprite[] sprites, int spriteIndex, int figureIndex, HayUnoRepetidoController controller)
    {
        int index = Random.Range(0, availableCells.Count);
        GameObject randomCell = availableCells[index];
        availableCells.Remove(randomCell);

        GameObject fig = Instantiate(figurePrefab, randomCell.transform);
        fig.GetComponent<FigureBehaviour>().Initialize(controller, sprites[spriteIndex], figureIndex);

        RectTransform figureTransform = fig.GetComponent<RectTransform>();

        if (controller.variableSizes)
        {
            SetRandomSizeToFigure(figureTransform);
        }

        SetRandomOffsetToFigure(figureTransform);
    }

    /// <summary>
    /// Cambia el tamaño de la figura aleatoriamente.
    /// </summary>
    /// <param name="fig">El RectTransform de la figura
    /// para cambiar el tamaño.</param>
    public void SetRandomSizeToFigure(RectTransform fig)
    {
        float randomSize = Random.Range(-1, 1) * MaxVariableSize;
        fig.sizeDelta = new Vector2(CellSize.x + randomSize, CellSize.y + randomSize);
    }

    /// <summary>
    /// Cambia el offset de una figura, teniendo en cuenta
    /// el tamaño variable y el espaciado entre celdas.
    /// </summary>
    /// <param name="fig">El RectTransform de la figura 
    /// para cambiar el offset.</param>
    public void SetRandomOffsetToFigure(RectTransform fig)
    {
        Vector2 figureSize = fig.GetComponent<RectTransform>().sizeDelta;
        Debug.Log(figureSize);
        Debug.Log(MaxVariableSize);
        Vector2 MaxOffset = CellSize + MaxVariableSize * Vector2.one - figureSize;

        float offsetX = Random.Range(-1, 1) * MaxOffset.x;
        float offsetY = Random.Range(-1, 1) * MaxOffset.y;

        Vector2 offset = new Vector2(offsetX, offsetY);
        fig.GetComponent<RectTransform>().anchoredPosition = offset;
    }

}
