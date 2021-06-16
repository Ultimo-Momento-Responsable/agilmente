using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gestor : MonoBehaviour
{

    private const string DEV_ENDPOINT = "localhost:8080/hay-uno-repetido";
    private const string PROD_ENDPOINT = "200.127.223.168:8082/hay-uno-repetido";

    public Camera camera;

    public int figureQuantity;
    public int maxFigures;
    public float maxTime;
    public bool limitTime;
    public bool limitFigure;
    private float auxTime; 
    public GameObject figure;
    public Sprite[] sprites;
    public AudioSource audioSource;
    public AudioClip sndSuccess;
    private List<int> index;
    public float initTime;
    public bool isTouching = false;
    public bool isMakingMistake = false;
    private string json;
    private bool canceled = false;
    public HayUnoRepetido hayUnoRepetido;
    private int dontTouchTimer = 20;
    public bool dontTouchAgain = false;

    public GameObject particles;

    public Text timer;

    void Start()
    {
        
        hayUnoRepetido = ScriptableObject.CreateInstance<HayUnoRepetido>();
        index = hayUnoRepetido.chooseSprites(sprites, figureQuantity);
        hayUnoRepetido.createFigures(figureQuantity,camera,figure,sprites,index,this,particles);
        initTime = Time.time;
        auxTime = initTime;
        print(hayUnoRepetido.TotalTime);
    }

    void Update()
    {
        
        hayUnoRepetido.TotalTime = Time.time - initTime;
        if (limitTime)
        {
            timer.text = ((int) maxTime - (int)hayUnoRepetido.TotalTime).ToString();
        } else
        {
            timer.text = "Nivel " + (hayUnoRepetido.Successes + 1).ToString();
        }

        // Acá verifica si la figura correcta fue tocada, en ese caso sube un nivel.
        if (isTouching && figureQuantity > 0 && !dontTouchAgain)
        {
            dontTouchAgain = true;
            hayUnoRepetido.TimeBetweenSuccesses[hayUnoRepetido.Successes] = Time.time - auxTime;
            auxTime = Time.time;
            audioSource.PlayOneShot(sndSuccess);

            if (figureQuantity < maxFigures)
            {
                figureQuantity++;
            }
            hayUnoRepetido.Successes++;
            if (limitFigure && figureQuantity >= maxFigures)
            {
                sendData();
            }
            resetValues();
        }

        if (dontTouchAgain) {
            dontTouchTimer--;
        }
        if (dontTouchTimer <= 0)
        {
            dontTouchAgain = false;
        }

        if (limitTime && (hayUnoRepetido.TotalTime >= maxTime))
        {
            sendData();
        }
        
        if (isMakingMistake) {
            isMakingMistake = false;
            camera.GetComponent<ScreenShake>().TriggerShake(0.1f);
        }
    }


    private void OnApplicationQuit()
    {
        canceled = true;
        sendData();
    }

    private void resetValues()
    {
        var objects = GameObject.FindGameObjectsWithTag("figures");
        foreach (GameObject o in objects)
        {
            Destroy(o.gameObject);
        }
        index = hayUnoRepetido.chooseSprites(sprites, figureQuantity);
        hayUnoRepetido.createFigures(figureQuantity, camera, figure, sprites, index, this, particles);
        isTouching = false;
    }

    public IEnumerator Upload(WWW www)
    {
        yield return www;
        if (www.error == null)
        {
            //Print server response
        }
        else
        {
            //Something goes wrong, print the error response
        }
    }

    void sendData()
    {
        figureQuantity = -1;

        string tBS = "[";
        foreach (float v in hayUnoRepetido.TimeBetweenSuccesses)
        {
            if (v == 0)
            {
                break;
            }
            tBS +=  v.ToString().Replace(",", ".") + ",";
        }
        tBS = tBS.Remove(tBS.Length - 1);
        tBS += "]";
        print(tBS);
        //Se genera el JSON para ser enviado al endpoint
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        json = "{'name': 'Hay Uno Repetido', 'totalTime': " + hayUnoRepetido.TotalTime.ToString().Replace(",", ".") + ", 'mistakes': " + hayUnoRepetido.Mistakes +
            ", 'successes': " + hayUnoRepetido.Successes + ", 'timeBetweenSuccesses': " + tBS + ", 'canceled': " + canceled +", 'dateTime': '" +
            System.DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss") + "'}";
        json = json.Replace("False", "false");
        json = json.Replace("True", "true");
        parameters.Add("Content-Type", "application/json");
        parameters.Add("Content-Length", json.Length.ToString());
        json = json.Replace("'", "\"");
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(json);
        WWW www = new WWW(DEV_ENDPOINT, postData, parameters);
        StartCoroutine(Upload(www));
        SceneManager.LoadScene("mainScene");
    }
}