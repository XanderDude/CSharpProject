using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BasicPickup : MonoBehaviour
{

    private void Start()
    {

    }
    private void OnTriggerEnter(Collider other) //runs once for every time something collides with the trigger collider
    {
        Debug.Log("Collision detected");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected");

        }
    }
}
