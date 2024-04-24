using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




public class MenuPrincipal : MonoBehaviour
{
   
    
    public static void NuevaPartida()
    {
        SceneManager.LoadScene("MainScene");
    }

    public static void SalirJuego()
    {
        Application.Quit();
    }

    public static void Regresar()
    {
        print("boton regresar presionado");
    }

   
    
}
