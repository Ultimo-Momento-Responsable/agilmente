using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneController : MonoBehaviour
{
    public Text title;

    public void btnGames()
    {
        title.text = "Jugar";
        print("Boton jugar clickeado");
    }
    public void btnHome()
    {
        title.text = "Inicio";
        print("Boton inicio clickeado");
    }
    public void btnProfile()
    {
        title.text = "Perfil";
        print("Boton perfil clickeado");
    }
}
