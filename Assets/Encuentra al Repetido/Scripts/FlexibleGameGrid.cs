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
    public float CellWidth { get => GridLayoutGroup.cellSize.x; }
    public float CellHeight { get => GridLayoutGroup.cellSize.y; }
    public float GridWidth { get => GridRectTransform.rect.width; }
    public float GridHeight { get => GridRectTransform.rect.height; }
    public float NumberOfColumns { get => Mathf.FloorToInt((GridWidth - CellWidth) / (CellWidth + HorizontalSpacing)) + 1 ; }
    public float NumberOfRows { get => Mathf.FloorToInt((GridHeight - CellHeight) / (CellHeight + VerticalSpacing)) + 1 ; }

    /// <summary>
    /// Crea una grilla que se ajusta al tamaño de la pantalla para 
    /// EAN y EAR.
    /// </summary>
    public void CreateCells()
    {
        Debug.Log(NumberOfColumns);
        if(cells != null && cells.Count > 0)
        {
            foreach(GameObject cell in cells)
            {
                Destroy(cell);
            }
        }

        cells = new List<GameObject>();
        availableCells = new List<GameObject>();

        for (int i = 1; i < NumberOfColumns * NumberOfRows; i ++)
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
        // TODO: Avoid overlapping
        int index = Random.Range(0, availableCells.Count);
        GameObject randomCell = availableCells[index];
        availableCells.Remove(randomCell);
        GameObject fig = Instantiate(figurePrefab, randomCell.transform);
        fig.GetComponent<FigureBehaviourWithCanvas>().sprite = sprites[spriteIndex];
        fig.GetComponent<FigureBehaviourWithCanvas>().controller = controller;
        fig.GetComponent<FigureBehaviourWithCanvas>().index = figureIndex;
        SetRandomOffsetToFigure(fig);
        
        if (controller.variableSizes)
        {
            SetRandomSizeToFigure(fig);
        }
    }

    public void SetRandomOffsetToFigure(GameObject fig)
    {
        // TODO: Add variation with size
        float MAX_OFFSET = HorizontalSpacing + 5;
        Vector2 offset = Random.insideUnitCircle * MAX_OFFSET;
        fig.GetComponent<RectTransform>().anchoredPosition = offset;
    }

    public void SetRandomSizeToFigure(GameObject fig)
    {
        const float MAX_VARIATION = 24;
        float randomSize = Random.insideUnitCircle.x * MAX_VARIATION;
        fig.GetComponent<RectTransform>().sizeDelta = new Vector2(CellWidth + randomSize, CellHeight + randomSize);
    }
}
