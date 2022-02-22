using System;
using UnityEngine;
using UnityEngine.UI;
public enum STATES
{
    UNSELECTED,     // Esta celda no fue seleccionada para ser activa, color default
    SELECTED,       // Esta celda fue seleccionada para ser activa, color ROJO?
    MISS,           // Esta celda no fue seleccionada pero es activa, color so boludo
    CORRECT,        // Esta celda fue seleccionada y es activa, color verde
}
public class Cell: MonoBehaviour
{
    private int row;
    private int column;
    private STATES state;
    private bool isActive;
    private MemorillaController controller;

    public float PosX { get => column * (controller.CellSize + controller.CellSpaceBetweenColumns); }
    public float PosY { get => row * (controller.CellSize + controller.CellSpaceBetweenRows); }
    public int Row { get => row; set => row = value; }
    public int Column { get => column; set => column = value; }
    public STATES State
    {
        get => state; 
        set
        {
            state = value;
            gameObject.GetComponentInChildren<Animator>().SetInteger("State", (int) value);
        }
    }
    public bool IsActive { get => isActive; set => isActive = value; }

    /// <summary>
    /// Inicializa la celda.
    /// </summary>
    /// <param name="row">Fila donde se encuentra la celda.</param>
    /// <param name="column">Columna donde se encuentra la celda.</param>
    /// <param name="controller">Controlador del juego.</param>
    /// <returns>La instancia de la celda.</returns>
    public Cell Create(int row, int column, MemorillaController controller)
    {
        this.row = row;
        this.column = column;
        this.isActive = false;
        this.controller = controller;
        State = STATES.UNSELECTED;

        return this;
    }

    /// <summary>
    /// Limpia la celda.
    /// Pasa el estado a UNSELECTED y
    /// si tiene guardada una respuesta, la borra.
    /// </summary>
    public void Clear() {
        IsActive = false;
        State = STATES.UNSELECTED;
    }

    /// <summary>
    /// Evento que gestiona lo que ocurre cuando 
    /// se clickea la celda.
    /// </summary>
    public void OnCellClick()
    {
        if (!controller.ControlsEnabled)
            return;

        if (State == STATES.UNSELECTED)
        {
            State = STATES.SELECTED;
            controller.OnCellClicked();
        }
    }

    /// <summary>
    /// Verifica si el jugador seleccionó la opción
    /// correcta.
    /// </summary>
    public void CheckSolution()
    {
        if (IsActive && State == STATES.SELECTED)
        {
            State = STATES.CORRECT;
        } else if (IsActive && State == STATES.UNSELECTED)
        {
            State = STATES.MISS;
        } else if (!IsActive && State == STATES.SELECTED)
        {
            State = STATES.UNSELECTED;
        }
    }
}

