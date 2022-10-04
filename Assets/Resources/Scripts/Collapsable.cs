using UnityEngine;
using UnityEngine.UI;

public class Collapsable : MonoBehaviour
{
    public GameObject body;
    public GameObject progressBar;
    public GameObject txtNumberOfGames;
    public GameObject txtNumberOfDaysLeft;

    private bool isCollapsed;
    private float a_offset;
    private int a_position;
    public float Offset { get => a_offset; set => a_offset = value; }
    public int Position { get => a_position; set => a_position = value; }
    public bool IsCollapsed { get => isCollapsed; set => isCollapsed = value; }


    private void Start()
    {
        this.isCollapsed = true;
    }

    /**
     * Desplaza los colapsables que hay debajo para no superponerse
     */
    public void DisplaceCollapsable()
    {
        IsCollapsed = !IsCollapsed;
        body.SetActive(IsCollapsed);
        int direction = this.IsCollapsed ? 1 : 0;
        GameObject[] collapsables = GameObject.FindGameObjectsWithTag("collapsable");
        GameObject cardContainer = GameObject.FindGameObjectWithTag("cardContainer");
        foreach (GameObject collapsable in collapsables)
        {
           if (collapsable.GetComponent<Collapsable>().Position > Position)
           {
                collapsable.transform.localPosition = new Vector2(collapsable.transform.localPosition.x,collapsable.transform.localPosition.y - (Offset * direction));
           }
        }
        cardContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(cardContainer.GetComponent<RectTransform>().sizeDelta.x, cardContainer.GetComponent<RectTransform>().sizeDelta.y + Offset * direction);
    }

    /// <summary>
    /// Le pasa los datos de la planning a la card para que se acomode.
    /// </summary>
    /// <param name="gamesPlayed"></param>
    /// <param name="totalGames"></param>
    /// <param name="unlimited"></param>
    /// <param name="daysLeft"></param>
    public void SetPlanningData(float gamesPlayed, float totalGames, bool unlimited, string daysLeft)
    {
        float completedPercentage = gamesPlayed / totalGames;

        if (unlimited && totalGames == 0)
        {
            completedPercentage = 1;
        }

        progressBar.transform.localScale = new Vector2(completedPercentage, 1);

        if (totalGames > 0)
        {
            txtNumberOfGames.GetComponent<Text>().text = gamesPlayed + "/" + totalGames;
        }

        txtNumberOfDaysLeft.GetComponent<Text>().text = "¡Quedan " + daysLeft + " días!";
    }
    
    /// <summary>
    /// Le pasa los datos de la planning completada para que se muestre.
    /// </summary>
    /// <param name="gamesPlayed"></param>
    /// <param name="totalGames"></param>
    public void SetCompleted(float gamesPlayed, float totalGames)
    {
        //collapsablePlanning.GetComponent<Collapsable>().SetCompleted(gamesPlayed, totalGames);
        //collapsablePlanning.transform.GetChild(3).gameObject.SetActive(true);
        //collapsablePlanning.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
        //collapsablePlanning.transform.GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(false);
        //collapsablePlanning.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false);
        //collapsablePlanning.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(true);
        //collapsablePlanning.transform.GetChild(4).gameObject.SetActive(true);
        //collapsablePlanning.transform.GetChild(5).gameObject.SetActive(true);
        //if (p.totalGames > 0)
        //    collapsablePlanning.transform.GetChild(6).GetComponent<Text>().text = gamesPlayed + "/" + totalGames;
    }
}
