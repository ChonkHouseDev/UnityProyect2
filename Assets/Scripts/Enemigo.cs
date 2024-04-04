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
    [SerializeField] private AnimationClip animMorir;
    private bool muriendo;
    [SerializeField] private Transform enemigoWing;
    private BoxCollider colliderWing;
    [SerializeField] private Transform enemigoCabeza;
    private BoxCollider colliderCabeza;

    [SerializeField] private Bullet disparo;
    
    [SerializeField] protected int vidaMax;
    private int _vida;
    private void Awake()
    {
        disparo.Enemy = this;
        Vida = vidaMax;
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

        colliderCabeza = enemigoCabeza.GetComponent<BoxCollider>();
        

    }
    
    private void Update()
    {
        estarAlerta = Physics.CheckSphere(transform.position, rangoDeAlerta, capaDelJugador);
        if (estarAlerta&!enemigoAtacando&!muriendo)
        {
            //transform.LookAt(jugador);
            Vector3 posJugador = new Vector3(jugador.position.x, transform.position.y, jugador.position.z);
            transform.LookAt(posJugador);
            transform.position = Vector3.MoveTowards(transform.position, posJugador, velocidad * Time.deltaTime);
        }

        puedeAtacar = Physics.CheckSphere(transform.position, rangoDeAtaque, capaDelJugador);
        if (puedeAtacar&!enemigoAtacando&!muriendo)
        {
            animator.SetTrigger("ataque");
            StartCoroutine(CrAtaque());
        }
        
        print("Vida "+gameObject.name+": "+ Vida);
        
        
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

    public IEnumerator CrMorir()
    {
        muriendo = true;
        //float duracionMorir = animMorir.length;
        yield return new WaitForSeconds(1f);
        muriendo = false;
        Destroy(gameObject);
    }

    public int Vida
    {
        get
        {
            return _vida;
        }

        set
        {
            if (value <= 0)
            {
                Morir();
            }
            else if (value >= vidaMax)
            {
                _vida = vidaMax;
            }
            else
            {
                if (value < _vida)
                {
                    //StartCoroutine(CrColorDano());
                }

                _vida = value;
            }

             

        }
    }

    private void Morir()
    {
        animator.SetTrigger("morir");
        StartCoroutine(CrMorir());
        
    }

    public void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "DisparoFire":
                print("recibi fuego");
                MeLastimaron(5);
                break;

            case "DisparoAgua":
                print("recibi agua");
                MeLastimaron(0);
                break;

            case "DisparoTierra":
                print("recibi tierra");
                MeLastimaron(0);
                break;
        }
    }

    private void MeLastimaron(int dano)
    {
        Vida = Vida - dano;
    }

    public void OnHitBoxEnter(Transform other)
    {
        print("onhitboxenter");
        print("onhitboxenter name "+other.name);
        
        switch (other.gameObject.tag)
        {
            case "DisparoFire":
                print("recibi fuego");
                MeLastimaron(5);
                break;

            case "DisparoAgua":
                print("recibi agua");
                MeLastimaron(0);
                break;

            case "DisparoTierra":
                print("recibi tierra");
                MeLastimaron(0);
                break;
        }
    }
    
}
