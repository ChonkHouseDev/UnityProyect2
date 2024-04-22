using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Bullet : MonoBehaviour
{
     public float life = 3;
      
     
     private void Awake()
     {
          Destroy(gameObject,life);
     }

     private void OnCollisionEnter(Collision collision)
     {
          //Destroy(collision.gameObject);
          
          
     }

     private void OnTriggerEnter(Collider other)
     {
          print("el disparo colisiono con: "+other.gameObject.name);
          Destroy(gameObject);
     }
     
     
     
     // public KeyCode weapon1 = KeyCode.Alpha1;
     // public KeyCode weapon2 = KeyCode.Alpha2;
     // public KeyCode weapon3 = KeyCode.Alpha3;
     
      
}
