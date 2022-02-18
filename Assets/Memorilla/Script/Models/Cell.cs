using UnityEngine;
using UnityEngine.UI;
public enum STATES
{
    UNSELECTED,     // Esta celda no fue seleccionada para ser activa, color default
    SELECTED,       // Esta celda fue seleccionada para ser activa, color ROJO?
    MISSELECTED,    // Esta celda fue seleccionada pero no es activa, color ERROR
    MISS,           // Esta celda no fue seleccionada pero es activa, color so boludo
    CORRECT,        // Esta celda fue seleccionada y es activa, color verde
}
public class Cell
{
    private int row;
    private int column;
    private GameObject gameObject;
    private STATES state;
    private bool isActive;
    public Cell(int row, int column, GameObject gameObject)
    {
        this.row = row;
        this.column = column;
        this.isActive = false;
        this.gameObject = gameObject;
        State = STATES.UNSELECTED;
    }

    public int Row { get => row; set => row = value; }
    public int Column { get => column; set => column = value; }
    public GameObject GameObject { get => gameObject; set => gameObject = value; }
    public STATES State
    {
        get => state; 
        set
        {
            Color color;
            switch (value)
            {
                case STATES.SELECTED:
                    color = new Color(0, 0, 1, 1);
                    break;
                case STATES.MISSELECTED:
                    color = new Color(1, 0, 0, 1);
                    break;
                case STATES.MISS:
                    color = new Color(1, 0, 0, 1);
                    break;
                case STATES.CORRECT:
                    color = new Color(0, 1, 0, 1);
                    break;
                default:
                    color = new Color(1, 1, 1, 1);
                    break;
            }
            state = value;
            gameObject.GetComponentInChildren<Image>().color = color;
        }
    }
    public bool IsActive { get => isActive; set => isActive = value; } 
    public void Clear() {
        IsActive = false;
        State = STATES.UNSELECTED;
    }    
}

