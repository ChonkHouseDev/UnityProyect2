using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemigo : MonoBehaviour
{
    public float rangoDeAlerta;
    private bool estarAlerta;
    public float rangoDeAtaque;
    private bool puedeAtacar;
    private bool enemigoAtacando;
    public LayerMask capaDelJugador;
    public Transform jugador;
    public float velocidad;
    private Animator animator;
    [SerializeField] private AnimationClip animAtaque;
    [SerializeField] private Transform enemigoWing;
    private BoxCollider colliderWing;
    private void Awake()
    {
        ObtenerComponentes();
    }

    private void ObtenerComponentes()
    {
        if (transform.childCount > 0)
        {
            animator = transform.GetChild(0).GetComponent<Animator>();
        }

        colliderWing = enemigoWing.GetComponent<BoxCollider>();
        colliderWing.enabled = false;
        
    }
    
    private void Update()
    {
        estarAlerta = Physics.CheckSphere(transform.position, rangoDeAlerta, capaDelJugador);
        if (estarAlerta&!enemigoAtacando)
        {
            //transform.LookAt(jugador);
            Vector3 posJugador = new Vector3(jugador.position.x, transform.position.y, jugador.position.z);
            transform.LookAt(posJugador);
            transform.position = Vector3.MoveTowards(transform.position, posJugador, velocidad * Time.deltaTime);
        }

        puedeAtacar = Physics.CheckSphere(transform.position, rangoDeAtaque, capaDelJugador);
        if (puedeAtacar&!enemigoAtacando)
        {
            
            animator.SetTrigger("ataque");
            StartCoroutine(CrAtaque());
            
        }
    }

    private void OnDrawGizmos()
    {
        //gizmo rango de alerta
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position,rangoDeAlerta);
        //gizmo rango para atacar
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,rangoDeAtaque);
    }

    public IEnumerator CrAtaque()
    {
        enemigoAtacando = true;
        
        //aqui se va a encender el collider que hara daño;
        colliderWing.enabled = true;
        
        //obtenemos la duracion de la animacion
        float duracionAtaque = animAtaque.length;
        yield return new WaitForSeconds(duracionAtaque);
        
        //se apaga el collider...
        colliderWing.enabled = false;
        
        enemigoAtacando = false;
    }
}
