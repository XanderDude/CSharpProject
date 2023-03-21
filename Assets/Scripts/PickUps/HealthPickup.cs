using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    //public PlayerHUD pHUD;
    [SerializeField] private int healthGainAmount = 15; //the amount to heal the player
    [SerializeField] private float rotateSpeed = 50f;

    void Update()
    {
        transform.Rotate(transform.rotation.x, Time.deltaTime * rotateSpeed, transform.rotation.z); //spin the pickup
    }
    private void OnTriggerEnter(Collider other) //runs once for every time something collides with the trigger collider
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<UseObjects>().pHUD.HUDHealthGained(healthGainAmount); //update the hud with the ammount gained
            other.GetComponent<PlayerHealth>().HealthCurrent = healthGainAmount; //heal player
            gameObject.SetActive(false); //disables self

        }
    }
}
