using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10;
    private string tagBullet;
    private Material materialBullet;
    
    [Header("Keybinds")]
    private KeyCode weapon1 = KeyCode.Alpha1;
    private KeyCode weapon2 = KeyCode.Alpha2;
    private KeyCode weapon3 = KeyCode.Alpha3;

    [Header("Materiales")]
    [SerializeField] public Material[] materiales;

    private void Awake()
    {
        tagBullet = "DisparoFire";
        materialBullet = materiales[0];
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            
            bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.forward * bulletSpeed;
           
            bullet.tag = tagBullet;
            
            bullet.GetComponent<Renderer>().material = materialBullet;
            
            print("Bullet con tag: "+bullet.tag);
        }

        if (Input.GetKeyDown(weapon1))
        {
            tagBullet = "DisparoFire";
            materialBullet = materiales[0];
        }
        
        if (Input.GetKeyDown(weapon2))
        {
            tagBullet = "DisparoTierra";
            materialBullet = materiales[1];
        }
        
        if (Input.GetKeyDown(weapon3))
        {
            tagBullet = "DisparoAgua";
            materialBullet = materiales[2];
        }
        
    }
}
