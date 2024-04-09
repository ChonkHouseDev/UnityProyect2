using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TakeDamage : MonoBehaviour
{

    public float intensity = 0f;
    private PostProcessVolume volume;
    private Vignette vignette;
    
    // Start is called before the first frame update
    void Start()
    {
        volume = GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings<Vignette>(out vignette);

        if (!vignette)
        {
            print("error vignette empty");
        }
        else
        {
            vignette.enabled.Override(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(TakeDamageEffect());
        }
    }

    public IEnumerator TakeDamageEffect()
    {
        intensity = 0.4f;
        vignette.enabled.Override(true);
        vignette.intensity.Override(0.4f);
        yield return new WaitForSeconds(0.4f);

        while (intensity > 0)
        {
            intensity -= 0.01f;
            if (intensity < 0)
            {
                intensity = 0;
            }
            vignette.intensity.Override(intensity);
            yield return new WaitForSeconds(0.1f);
        }
        
        vignette.enabled.Override(false);
        yield break;

    }
}
