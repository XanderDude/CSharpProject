using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerRaycast : MonoBehaviour
{
    private Transform cameraTransform; //the camera's transform
    [SerializeField] private float raycastDistance = 5f; //set the distance that the raycast will travel

    [SerializeField] private Material[] cubeMats; //array of materials for cubes
    [SerializeField] private Material[] sphereMats; //array of materials for spheres

    private bool interactWait = false; //a check to ensure the raycast isn't fired every frame the player clicks

    //[SerializeField] private GameObject interactPromptText; //Press E to pick up text
    private bool interactTooltipVisible = false; //bool to switching the visible state of the text 

    public PlayerHUD pHUD; //the script handling updating the hud, on the canvas

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform; //Assigns the main camera's transform to the var
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Fire1") == 0) //Player is not left clicking
        {
            interactWait = false; //Allow the player to fire a raycast
        }
        FireRaycast(); //Fires a raycast
    }

    private void FireRaycast() //called when the player is not currently clicking
    {
        Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, raycastDistance); //always fires
            
        if ((Input.GetAxis("Fire1") > 0 || Input.GetKeyDown(KeyCode.E)) && !interactWait && !hit.collider.IsUnityNull()) //check for input
        {
            interactWait = true; //Set bool to prevent FireRaycast() to be called
            GameObject hitObj = hit.collider.gameObject; //local gameobject for the object hit
            if (hit.collider.CompareTag("Cube")) //only get its mat name and run SetNextMat if it's a cube
            {
                string hitMatName = hitObj.GetComponent<MeshRenderer>().material.name[0..^11]; //creates a local string for the name of the material on the collider, and removes the (Instance) in the name
                SetNextMat(hitObj, hitMatName, cubeMats); //Find and set the next material for the object hit, by comparing the hit object's material name with the array of materials

            }
            else if (hit.collider.CompareTag("Sphere")) //only get its mat name and run SetNextMat if it's a sphere
            {
                string hitMatName = hitObj.GetComponent<MeshRenderer>().material.name[0..^11]; //creates a local string for the name of the material on the collider, and removes the (Instance) in the name
                SetNextMat(hitObj, hitMatName, sphereMats); //Find and set the next material for the object hit, by comparing the hit object's material name with the array of materials
            }
            else if (hit.collider.CompareTag("Pickup")) //player has interacted with a pickup
            {
                Debug.Log("Picked up pickup");
            }
        }

        if (!hit.collider.IsUnityNull() && hit.collider.CompareTag("Pickup")) //if the object is a pickup
        {
            if (!interactTooltipVisible) //if the tooltip is not already visible
            {
                interactTooltipVisible = true; //set bool to indicate it should be visible
                pHUD.HUDItemVisible(interactTooltipVisible); //call HudItemVisible to update the text with the visibility bool
            }
        }
        else //collider is not a pickup
        {
            if (interactTooltipVisible) //if the tooltip is visible
            {
                interactTooltipVisible = false; //set bool to indicate it should not be visible
                pHUD.HUDItemVisible(interactTooltipVisible);  //call HudItemVisible to update the text with the visibility bool
            }
        }
        
        if (interactTooltipVisible) //if the tooltip is visible
        {
            interactTooltipVisible = false; //set bool to indicate it should not be visible
            pHUD.HUDItemVisible(interactTooltipVisible); //call HudItemVisible to update the text with the visibility bool
        }
        
    }

    private void SetNextMat(GameObject hitObj, string hitMatName, Material[] objMats) //Called when an object that should have its material changed is hit
    {
        for (int i = 0; i < objMats.Length; i++) //Loop through the array of materials
        {
            if (hitMatName == objMats[i].name)
            {
                if (i + 1 >= objMats.Length) //if the next index is out of scope, set material to the first in the array
                {
                    hitObj.GetComponent<MeshRenderer>().material = objMats[0]; //First mat in array
                }
                else
                {
                    hitObj.GetComponent<MeshRenderer>().material = objMats[i + 1]; //Next mat in array
                }
                return; //exit loop once material match is found
            }
        }
        hitObj.GetComponent<MeshRenderer>().material = objMats[0]; //if the material doesn't match any in the array, set to first material in array
    }
}
