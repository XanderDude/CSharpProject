using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UseObjects : MonoBehaviour
{
    private Transform cameraTransform; //the camera's transform
    [SerializeField] private float raycastDistance = 5f; //set the distance that the raycast will travel

    private bool interactTooltipVisible = false; //bool for checking if the interact tooltip text is visible on-screen

    public PlayerHUD pHUD; //the script handling updating the hud, on the canvas

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform; //Assigns the main camera's transform
    }

    // Update is called once per frame
    void Update()
    {
        FireRaycast(); //Fires a raycast
    }

    private void FireRaycast() //Fires a raycast forward from the camera, to a set distance, checking for interactable gameobjects
    {
        Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, raycastDistance); //always fires

        if (Input.GetKeyDown(KeyCode.E) && !hit.collider.IsUnityNull()) //check for input and if the player is currently looking at a gameobject
        {

            //Note: This only works for ammo interactables
            if (hit.collider.CompareTag("Interactable")) //player has interacted with a pickup
            {
                int tmpAmmoReserve = GetComponent<PlayerInputs>().currentWeapon.CurrentReserveAmmo; //finds the amount of ammo in reserve
                GetComponent<PlayerInputs>().currentWeapon.CurrentReserveAmmo = hit.collider.GetComponent<AmmoPickup>().ammoGainAmount; //add ammo to reserve according to the pickup

                pHUD.HUDAmmoGained(GetComponent<PlayerInputs>().currentWeapon.CurrentReserveAmmo - tmpAmmoReserve); //display how much reserve ammo increased by subracting new amount by amount prior to interacting
            }
        }

        if (!hit.collider.IsUnityNull() && hit.collider.CompareTag("Interactable")) //if there is a game object and it is interactable
        {
            if (!interactTooltipVisible) //ony run if the tooltip is not visible
            {
                interactTooltipVisible = true; //switch tooltip visibility check to true
                pHUD.HUDItemVisible(interactTooltipVisible); //call HudItemVisible with current visibility state of the tooltip
            }
        }
        else //not looking at a gameobject, or the gameobject is not interactable
        {
            if (interactTooltipVisible) //only run if the tooltip is visible
            {
                interactTooltipVisible = false; //switch tooltip visibility check to false
                pHUD.HUDItemVisible(interactTooltipVisible);  //call HudItemVisible with current visibility state of the tooltip
            }
        }
    }

}
