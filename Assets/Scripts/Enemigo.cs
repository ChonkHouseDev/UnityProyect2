using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class Enemigo : MonoBehaviour
{
    public float rangoDeAlerta;
    private bool estarAlerta;
    public float rangoDeAtaque;
    private bool puedeAtacar;
    private bool enemigoAtacando;
    private bool enemigoCaminando;
    public LayerMask capaDelJugador;
    public Transform jugador;
    public float velocidad;
    private Animator animator;
    [SerializeField] private AnimationClip animAtaque;
    [SerializeField] private AnimationClip animMorir;
    private bool muriendo;
    [SerializeField] private Transform enemigoWing;
    private BoxCollider colliderWing; 
    [SerializeField] protected int vidaMax;
    private int _vida;
    [SerializeField]private GameObject render;
    private SkinnedMeshRenderer render2;
    
    
    private void Awake()
    {
         
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

        render2=render.GetComponent<SkinnedMeshRenderer>();
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
        
        //print("Vida "+gameObject.name+": "+ Vida);

        if (estarAlerta)
        {
            animator.SetBool("caminar",true);
        }
        else
        {
            animator.SetBool("caminar",false);
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

    public IEnumerator CrMorir()
    {
        muriendo = true;
        float duracionMorir = animMorir.length;
        yield return new WaitForSeconds(duracionMorir);
        muriendo = false;
        Destroy(gameObject);
    }

    public IEnumerator CrDano()
    {
        render2.material.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        render2.material.color = Color.white;
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
                    StartCoroutine(CrDano());
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
    
    public void MeLastimaron(int dano)
    {
        Vida = Vida - dano;
    }

    
    
   


}
