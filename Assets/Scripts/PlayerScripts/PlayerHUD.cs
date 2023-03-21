using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{

    [SerializeField] private GameObject interactPromptText; //The gameobject with the TMP, Press E to pick up text
    [SerializeField] private GameObject reloadingText; //the object with the reloading text for when the gun is reloading

    
    //Ammo HUD elements: 
    [SerializeField] private TextMeshProUGUI ammoCurrentMagTMP; //Player's ammo count
    [SerializeField] private RectTransform ammoGainedTextSpawn; //Origin point for ammo gained text
    [SerializeField] private GameObject ammoGainedTextObject; //Ammo gained text object

    //Health HUD elements:
    [SerializeField] private TextMeshProUGUI healthCurrentTMP; //Player's health
    [SerializeField] private RectTransform healthGainedTextSpawn; //Origin point for health gained text
    [SerializeField] private GameObject healthGainedTextObject; //Health gained text object

    //Wave HUD elements
    [SerializeField] private TextMeshProUGUI waveCurrentTMP; //the current wave that the player's on
    [SerializeField] private TextMeshProUGUI enemyCountCurrentTMP; //the current wave that the player's on

    private WeaponBaseClass[] currentWeaponList; //all weapons in the scene
    private void Start()
    {
        currentWeaponList = FindObjectsOfType<WeaponBaseClass>(true);
        foreach(WeaponBaseClass weapon in currentWeaponList)
        {
            weapon.OnPlayerReload += Weapon_OnPlayerReload; //Subscribing to reloading event
            weapon.OnPlayerShoot += Weapon_OnPlayerShoot;
        }
    }

    private void Weapon_OnPlayerShoot(int mag, int reserve)
    {
        ammoCurrentMagTMP.SetText(mag + " / " + reserve);
    }

    private void OnDestroy()
    {
        foreach (WeaponBaseClass weapon in currentWeaponList)
        {
            if (weapon)
            {
                weapon.OnPlayerReload -= Weapon_OnPlayerReload; //Subscribing to reloading event
            }
        }
    }

    private void Weapon_OnPlayerReload(float reloadSpeed, int magAmmo, int reserveAmmo)
    {
        StartCoroutine(ReloadDelay(reloadSpeed, magAmmo, reserveAmmo));
    }

    private IEnumerator ReloadDelay(float reloadSpeed, int magAmmo, int reserveAmmo)
    {
        reloadingText.SetActive(true);
        ammoCurrentMagTMP.SetText(0 + " / " + reserveAmmo);
        yield return new WaitForSeconds(reloadSpeed); //wait according to reload speed
        ammoCurrentMagTMP.SetText(magAmmo + " / " + reserveAmmo);
        reloadingText.SetActive(false);
    }

    public void HUDItemVisible(bool hudVisible) //when called, update the visibility with the bool that was passed
    {
        interactPromptText.SetActive(hudVisible); //enable/disable tooltip object
    }
    public void HUDUpdateHealth(int currentHealth) //when called, update health with the value passed
    {
        healthCurrentTMP.SetText("HP: " + currentHealth);

    }
    public void HUDUpdateAmmo(int ammoCurrentMag, int ammoCurrentReserve) //for updating the HUD ammo counter
    {

        ammoCurrentMagTMP.SetText(ammoCurrentMag + " / " + ammoCurrentReserve);

    }
    public void HUDAmmoGained(int ammountGained) //for creating the ammo gained text, and setting the amount gained
    {
        if (ammountGained == 0) //If no ammo can be gained
        {
            ammoGainedTextObject.GetComponent<TextMeshProUGUI>().SetText("Ammo Maxed");
        }
        else //else show the amount gained
        {
            ammoGainedTextObject.GetComponent<TextMeshProUGUI>().SetText("+ " + ammountGained + " Ammo");
        }

        Instantiate(ammoGainedTextObject, ammoGainedTextSpawn); //creat ammo gained object. Notes: Might need to make prior to setting it's text
        
    }
    public void HUDHealthGained(int ammountGained) //when health picked up, create health gained text
    {
        ammoGainedTextObject.GetComponent<TextMeshProUGUI>().SetText("+ " + ammountGained + " Health");
        Instantiate(healthGainedTextObject, healthGainedTextSpawn); //create the health gained object. 
    }

    public void HUDUpdateWave(int wave)
    {
        wave += 1; //wave starts at 0, increase 
        waveCurrentTMP.SetText("Wave: " + wave); //update hud with current wave 
    }
    public void HUDUpdateEnemies(int remaining)
    {
        enemyCountCurrentTMP.SetText("Enemies Remaining: " + remaining); //update hud with remaining enemies
    }

    public void HUDWavesCompleted() //update hud when final wave has been completed
    {
        waveCurrentTMP.SetText("Waves Completed!");
        enemyCountCurrentTMP.SetText("");
    }

    public void HUDToggleReloading(bool reloading)//turn off/on reloading text
    {
        reloadingText.SetActive(reloading); 
    }
    private void ReloadText(object sender, EventArgs e)
    {
        reloadingText.SetActive(true);
    }
}

