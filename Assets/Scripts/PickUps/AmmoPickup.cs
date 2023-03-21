using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoGainAmount = 25; //the amount of ammo added to the player's reserve ammo
    [SerializeField] private float rotateSpeed = 50f;

    private void Update()
    {
        transform.Rotate(transform.rotation.x, Time.deltaTime * rotateSpeed, transform.rotation.z); //spin the pickup 
    }
    private void OnTriggerEnter(Collider other) //runs once for every time something collides with the trigger collider
    {
        if (other.CompareTag("Player"))
        {
            PlayerAttack playerShoot = other.GetComponent<PlayerAttack>();
            //PlayerHUD playerHUD = other.GetComponent<PlayerRaycast>().pHUD;

            int tmpAmmoReserve = playerShoot.AmmoCurrentReserve; //finds the amount of ammo in reserve
            playerShoot.AmmoCurrentReserve = ammoGainAmount; //add ammo to reserve according to the pickup

            other.GetComponent<UseObjects>().pHUD.HUDAmmoGained(playerShoot.AmmoCurrentReserve - tmpAmmoReserve); //pass the amount of ammo gained to hud for ammo gained text

            if (playerShoot.AmmoCurrentReserve - tmpAmmoReserve != 0) //only destroy self if consumed (ammo was gained)
            {
                gameObject.SetActive(false); //disables the ammo object
            }
            
            //gameObject.SetActive(false); //disables self
        }
    }
}
