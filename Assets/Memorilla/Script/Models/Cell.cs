using UnityEngine;

public class Cell
{
    private int row;
    private int column;
    private GameObject gameObject;
    private bool shouldBeActive;
    private bool isActive;

    public Cell(int row, int column, GameObject gameObject)
    {
        this.row = row;
        this.column = column;
        this.shouldBeActive = false;
        this.isActive = false;
        this.gameObject = gameObject;
    }

    public int Row { get => row; set => row = value; }
    public int Column { get => column; set => column = value; }
    public GameObject GameObject { get => gameObject; set => gameObject = value; }
    public bool ShouldBeActive { get => shouldBeActive; set => shouldBeActive = value; }
    public bool IsActive { get => isActive; set => isActive = value; }
}
