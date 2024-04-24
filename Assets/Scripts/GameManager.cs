using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class GameManager : MonoBehaviour
{
    private static Canvas canvasPausa;
    private static bool _enPausa = false;

    private static Canvas canvasFin;
    
    private void Awake()
    {
        canvasPausa = transform.GetChild(0).GetComponent<Canvas>();
        canvasPausa.enabled = false;

        canvasFin = transform.GetChild(1).GetComponent<Canvas>();
        canvasFin.enabled = false;
    }

    public static void Win()
    {
        canvasFin.enabled = true;
    }

    public static void SalirJuego()
    {
        Application.Quit();
    }
    
    private void Update()
    {
        JuegoEnPausa();
    }


    public static void JuegoEnPausa()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            EnPausa = !EnPausa;
        }

        if (!EnPausa)
        {
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        
    }

    public static bool EnPausa
    {
        get => _enPausa;

        set
        {
            _enPausa = value;
            canvasPausa.enabled = value;
            Time.timeScale = value ? 0 : 1;
        }
    }

    public static void SalirJuegoEnd()
    {
        Application.Quit();
    }
    
    
}
