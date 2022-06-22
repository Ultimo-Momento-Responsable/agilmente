using UnityEngine;

public class Collapsable : MonoBehaviour
{
    private float a_offset;
    private int a_position;
    public float Offset { get => a_offset; set => a_offset = value; }
    public int Position { get => a_position; set => a_position = value; }

    /**
     * Desplaza los colapsables que hay debajo para no superponerse
     */
    public void DisplaceCollapsable(int direction)
    {
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
}
