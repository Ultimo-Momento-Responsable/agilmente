using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class gestor : MonoBehaviour
{
    public int dificultad;
    public GameObject vegetal;
    public Sprite[] sprites;
    private List<int> indices;
    public int errores;
    private bool terminoJuego;
    public int tiempo;
    public bool clickearon = false;
    private string json;
    // Start is called before the first frame update
    void Start()
    {
        Rect canvasSize = GameObject.Find("canvas").GetComponent<RectTransform>().rect;
        
        indices = new List<int>();
        elegirSprites();
        crearVegetales();
        tiempo = (int)Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (clickearon && dificultad < 21)
        {
            var objects = GameObject.FindGameObjectsWithTag("vegetal");
            foreach (GameObject o in objects)
            {
                Destroy(o.gameObject);
            }
            dificultad++;
            indices = new List<int>();
            elegirSprites();
            crearVegetales();
            clickearon = false;
        }
        if (dificultad >= 21)
        {
            tiempo = (int)Time.time - tiempo;
            dificultad = -1;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            json = "{'nombre': 'Hay uno repetido', 'tiempo': " + tiempo + ", 'errores': " + errores + ", 'fechaYHora': '" +
                System.DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss") + "'}";
            parameters.Add("Content-Type", "application/json");
            parameters.Add("Content-Length", json.Length.ToString());
            json = json.Replace("'", "\"");
            byte[] postData = System.Text.Encoding.UTF8.GetBytes(json);
            //Now we call a new WWW request
            WWW www = new WWW("http://localhost:8080/juego", postData, parameters);
            //And we start a new co routine in Unity and wait for the response.
            StartCoroutine(Upload(www));
        }

    }

    public IEnumerator Upload(WWW www)
    {
        yield return www;
        if (www.error == null)
        {
            //Print server response
            Debug.Log(www.text);
        }
        else
        {
            //Something goes wrong, print the error response
            Debug.Log(www.error);
        }
        
    }
    
    void elegirSprites()
    {
        int indiceRepetido = (int)Random.Range(0, sprites.Length);
        indices.Add(indiceRepetido);
        indices.Add(indiceRepetido);
        for (int i = 2; i < dificultad; i++)
        {
            int indRandom = (int)Random.Range(0, sprites.Length);
            while (indices.Contains(indRandom))
            {
                indRandom = (int)Random.Range(0, sprites.Length);
            }
            indices.Add(indRandom);
        }
    }
    void crearVegetales()
    {
        for (int i = 0; i < dificultad; i++)
        {

            //float x = Random.Range
            //        (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
            //float y = Random.Range
            //    (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);
            Vector2 randomPositionOnScreen = Camera.main.ViewportToWorldPoint(new Vector2(Random.value, Random.value));

            while (hayEnemigoEn(randomPositionOnScreen))
            {
                randomPositionOnScreen = Camera.main.ViewportToWorldPoint(new Vector2(Random.value, Random.value));
                //x = Random.Range
                //    (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
                //y = Random.Range
                //(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);
            }
            GameObject veg = Instantiate(vegetal, randomPositionOnScreen, Quaternion.identity);
            veg.GetComponent<comportamiento>().sprite = sprites[indices[i]];
            veg.GetComponent<comportamiento>().controlador = this;
            veg.GetComponent<comportamiento>().indice = i;
        }
    }
    bool hayEnemigoEn(Vector2 posición)
    {
        Vector2 p1 = posición - new Vector2(0.25f, 0.25f);
        Vector2 p2 = posición + new Vector2(0.25f, 0.25f);
        Collider2D collider = Physics2D.OverlapArea(p1, p2);

        if (collider != null)
        {
            return true;
        }
        return false;
    }
}
