using UnityEngine;
/// <summary>
/// Una lista de estados en los que puede estar la celda.
/// </summary>
/// <list>
///     <item>
///         <term><c>UNSELECTED</c></term>
///         <description>
///         Celda deseleccionada (color blanco). Estado por defecto.
///         </description>
///     </item>
///     <item>
///         <term><c>SELECTED</c></term>
///         <description>
///         Celda seleccionada (color azul). Ocurre cuando el paciente hace click 
///         sobre la celda, o bien cuando se muestra la solución al comenzar el nivel.
///         </description>
///     </item>
///     <item>
///         <term><c>MISS</c></term>
///         <description>
///         Error (color rojo). Ocurre cuando se muestra la solución al final del 
///         nivel, cuando la celda es parte de la solución pero no fue seleccionada.
///         </description>
///     </item>
///     <item>
///         <term><c>CORRECT</c></term>
///         <description>
///         Acierto (color verde). Ocurre cuando se muestra la solución al final del 
///         nivel, cuando la celda es parte de la solución y el paciente la 
///         seleccionó.
///         </description>
///     </item>
/// </list>
public enum STATES
{
    UNSELECTED,
    SELECTED,
    MISS,
    CORRECT,
}
public class Cell: MonoBehaviour
{
    private int row;
    private int column;
    private STATES state;
    private bool isActive;
    private MemorillaController controller;

    public float PosX { get => column * (controller.CellSize + controller.CellSpaceBetweenColumns) - 310; }
    public float PosY { get => row * (controller.CellSize + controller.CellSpaceBetweenRows) - (controller.CellSize * controller.Height / 2); }
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
    /// <remarks>
    /// Cambia el estado de la celda a <c>SELECTED</c>,
    /// si estaba <c>UNSELECTED</c>, y delega el resto 
    /// al controlador.
    /// </remarks>
    public void OnCellClick()
    {
        if (!controller.ControlsEnabled)
            return;

        if (!controller.onTutorial)
        {
            if (State == STATES.UNSELECTED)
            {
                State = STATES.SELECTED;
                controller.OnCellClicked();
            }
        }
        else
        {
            if (IsActive)
            {
                State = STATES.SELECTED;
                controller.OnCellClicked();
            }
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

