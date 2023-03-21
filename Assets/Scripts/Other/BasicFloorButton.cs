using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class BasicFloorButton : MonoBehaviour
{
    [SerializeField] GameObject door;
    [SerializeField] private float doorMoveSpeed = 2;

    private void OnTriggerEnter(Collider other) //runs once for every time something collides with the trigger collider
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Floor button found");
        }
        
    }
    private void OnTriggerStay(Collider other) //runs once for every time something collides with the trigger collider
    {
        if (door.transform.position.y > -1)
        {
            door.transform.Translate(0, -transform.position.y * doorMoveSpeed * Time.deltaTime, 0);
        }
    }

}
