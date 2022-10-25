using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleGameGrid : MonoBehaviour
{
    public GameObject templateCell;
    private RectTransform gridRectTransform;
    private GridLayoutGroup gridLayoutGroup;
    private List<GameObject> cells;
    public List<GameObject> availableCells;
    public GameObject figurePrefab;
    public float CellWidth { get => GridLayoutGroup.cellSize.x; }
    public float CellHeight { get => GridLayoutGroup.cellSize.y; }
    private float HorizontalSpacing { get => GridLayoutGroup.spacing.x; }
    private float VerticalSpacing { get => GridLayoutGroup.spacing.y; }
    public float GridWidth { get => GridRectTransform.rect.width; }
    public float GridHeight { get => GridRectTransform.rect.height; }
    public float NumberOfRows { get => Mathf.FloorToInt((GridWidth - CellWidth) / (CellWidth + HorizontalSpacing)) + 1 ; }
    public float NumberOfColumns { get => Mathf.FloorToInt((GridHeight - CellHeight) / (CellHeight + VerticalSpacing)) + 1 ; }
    private RectTransform GridRectTransform { get => gridRectTransform == null ? GetComponent<RectTransform>() : gridRectTransform; }
    private GridLayoutGroup GridLayoutGroup { get => gridLayoutGroup == null ? GetComponent<GridLayoutGroup>() : gridLayoutGroup; }

    void Start()
    {
        CreateCells();
    }

    /// <summary>
    /// Crea una grilla que se ajusta al tamaño de la pantalla para 
    /// EAN y EAR.
    /// </summary>
    public void CreateCells()
    {
        availableCells = new List<GameObject>();

        for(int i = 1; i < NumberOfColumns * NumberOfRows; i ++)
        {
            availableCells.Add(Instantiate(templateCell, transform));
        }
    }

    /// <summary>
    /// Crea una figura en un lugar random de la pantalla.
    /// No se debe usar para el tutorial.
    /// </summary>
    /// <param name="figure">El prefab de la figura a instanciar.</param>
    public void CreateFigureOnRandomCell(Sprite[] sprites, int i, HayUnoRepetidoController controller)
    {
        int index = Random.Range(0, availableCells.Count);
        GameObject randomCell = availableCells[index];
        availableCells.Remove(randomCell);
        GameObject fig = Instantiate(figurePrefab, randomCell.transform);
        fig.GetComponent<FigureBehaviour>().sprite = sprites[i];
        fig.GetComponent<FigureBehaviour>().controller = controller;
        fig.GetComponent<FigureBehaviour>().index = i;
    }
}
