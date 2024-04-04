using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
     public float life = 3;
     internal Enemigo Enemy;

 

     private void Awake()
     {
          Destroy(gameObject,life);
     }

     private void OnCollisionEnter(Collision collision)
     {
          //Destroy(collision.gameObject);
          //print("el disparo colisiono con: "+collision.gameObject.name);
          
     }

     private void OnTriggerEnter(Collider other)
     {
          print("bala colisiono con: "+other.transform);
          Enemy.OnHitBoxEnter(other.transform);
          Destroy(gameObject);
     }
}
