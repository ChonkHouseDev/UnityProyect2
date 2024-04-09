using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;

    private float xRotation;
    private float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        VignettePresente();
        vignette.active = false;
    }

    private void Update()
    {
        //get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation,-90f,90f);
        
        //rotate cam and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
    
    #region vignette
    
    public Volume volume; // Referencia al componente Volume
    private Vignette vignette;  // Referencia al efecto Vignette

    private void VignettePresente()
    {
        // Asegúrate de que el componente Volume y el efecto Vignette estén presentes
        if (volume == null || !volume.profile.TryGet(out vignette))
        {
            Debug.LogWarning("No se encontró el componente Volume o el efecto Vignette.");
            return;
        }
    }
    
    // Método para activar o desactivar el Vignette
    public void SetVignetteActive()
    {
        StartCoroutine(CrVignetteActive());
    }

    private float intensity;

    private IEnumerator CrVignetteActive()
    {
        intensity = 0.5f;
        vignette.active = true;
        vignette.intensity.Override(0.5f);
        
        yield return new WaitForSeconds(0.4f);

        while (intensity > 0)
        {
            intensity -= 0.01f;
            if (intensity < 0) intensity = 0;
            vignette.intensity.Override(intensity);
            yield return new WaitForSeconds(0.1f);
        }

        vignette.active = false;
        yield break;

    }
    
    #endregion vignette
    
    
}
