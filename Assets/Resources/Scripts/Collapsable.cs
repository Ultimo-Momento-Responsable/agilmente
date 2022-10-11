using UnityEngine;
using UnityEngine.UI;
using System;

public class Collapsable : MonoBehaviour
{
    public GameObject body;
    public GameObject progressBar;
    public GameObject txtNumberOfGames;
    public GameObject txtNumberOfDaysLeft;
    public GameObject arrowDown;
    public GameObject arrowUp;
    public GameObject trophy;
    public Color completedGreen;

    private bool isCollapsed;
    private float a_offset;
    private int a_position;
    public float Offset { get => a_offset; set => a_offset = value; }
    public int Position { get => a_position; set => a_position = value; }
    public bool IsCollapsed { get => isCollapsed; set => isCollapsed = value; }
    private bool IsCompleted { get => trophy.activeSelf; }

    private void Start()
    {
        isCollapsed = true;
    }

    /// <summary>
    /// Activa o desactiva el collapsable para mostrar u ocultar las sesiones
    /// según corresponda.
    /// </summary>
    public void ToggleCollapsable()
    {
        if (!IsCompleted)
        {
            arrowDown.SetActive(!IsCollapsed);
            arrowUp.SetActive(IsCollapsed);
        }
        body.SetActive(IsCollapsed);
        IsCollapsed = !IsCollapsed;
    }

    /// <summary>
    /// Usa los datos de una planning en progreso para mostrarla en el collapsable.
    /// </summary>
    /// <param name="gamesPlayed">Cantidad de sesiones jugadas.</param>
    /// <param name="totalGames">Cantidad total de sesiones.</param>
    /// <param name="unlimited">Contiene juegos libres.</param>
    /// <param name="daysLeft">Días restantes en la planificación.</param>
    public void SetPlanningData(float gamesPlayed, float totalGames, bool unlimited, string daysLeft)
    {
        float completedPercentage = gamesPlayed / totalGames;

        if (unlimited && totalGames == 0)
        {
            completedPercentage = 1;
        }

        progressBar.transform.localScale = new Vector2(completedPercentage, 1);

        SetNumerOfGamesText(gamesPlayed, totalGames);
        txtNumberOfDaysLeft.GetComponent<Text>().text = GetNumberOfDaysLeftText(daysLeft);
    }

    /// <summary>
    /// Genera el texto correspondiente al número de días restantes.
    /// </summary>
    /// <param name="daysLeft">Número de días restantes.</param>
    /// <returns>Texto.</returns>
    private string GetNumberOfDaysLeftText(string daysLeft)
    {
        switch (int.Parse(daysLeft))
        {
            case 0:
                return "¡Último día!";
            case 1:
                return "¡Queda " + daysLeft + " día!";
            default:
                return "¡Quedan " + daysLeft + " días!";
        }
    }

    /// <summary>
    /// Usa los datos de una planning completada para mostrarla en el collapsable.
    /// </summary>
    /// <param name="gamesPlayed">Cantidad de sesiones jugadas.</param>
    /// <param name="totalGames">Cantidad total de sesiones.</param>
    public void SetCompleted(float gamesPlayed, float totalGames)
    {
        progressBar.GetComponent<Image>().color = completedGreen;
        txtNumberOfDaysLeft.GetComponent<Text>().color = completedGreen;
        txtNumberOfDaysLeft.GetComponent<Text>().text = "¡Completada!";
        txtNumberOfGames.GetComponent<Text>().color = completedGreen;
        SetNumerOfGamesText(gamesPlayed, totalGames);
        arrowDown.SetActive(false); 
        arrowUp.SetActive(false);
        trophy.SetActive(true);
    }

    /// <summary>
    /// Setea el texto correspondiente al progreso del paciente.
    /// </summary>
    /// <param name="gamesPlayed">Cantidad de juegos jugados.</param>
    /// <param name="totalGames">Cantidad de juegos terminados.</param>
    private void SetNumerOfGamesText(float gamesPlayed, float totalGames)
    {
        txtNumberOfGames.GetComponent<Text>().text = totalGames > 0 ? gamesPlayed + "/" + totalGames : "";
    }
}
