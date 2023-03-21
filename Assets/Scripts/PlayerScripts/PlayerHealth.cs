using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int healthCurrent = 50; //the player's current health
    [SerializeField] private int healthMax = 100; //the max health the player can reach
    [SerializeField] private int healthMaxOverheal = 25; //how much over max the health can go
    [SerializeField] private float healthDecaySpeed = 1.5f; //the rate that the health will reduce back to max when health goes over max
    private float decayTimer = 0;
    [SerializeField] private bool godMode = false;

    [SerializeField] private Transform spawnLocation; //location to spawn and respawn the player
    public int HealthCurrent //Property for increasing and decreasing the player's health
    {
        get { return healthCurrent; }

        set 
        {
            if(godMode)
            {
                return;
            }
            if (healthCurrent + value >= healthMaxOverheal) //if health increases past max health + overheal
            {
                healthCurrent = healthMaxOverheal; //set current health to max possible health
            }
            else if (healthCurrent + value <= 0) //if health hits zero or below, death
            {
                healthCurrent = 0; //keep health at zero
                gameManager.playerDead = true;
            }
            else
            {
                healthCurrent += value; //adjust current health +- value
            }
            pHUD.HUDUpdateHealth(healthCurrent); //update hud with current health value
        }
    }


    [SerializeField] private PlayerHUD pHUD; //the script that handles updating hud elements
    [SerializeField] private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        healthMaxOverheal += healthMax; //Set max overheal to equal max health plus overheal value
        pHUD.HUDUpdateHealth(healthCurrent); //update hud with current health value
    }
    void Update() 
    { 
        if (healthCurrent > healthMax) //if healh exceeds max health
        {
            DecayHealth();//decrease health back to max
        }
    }

    private void DecayHealth()
    {
        decayTimer += Time.deltaTime;//timer for length of each tick of health decay
        if (decayTimer >= healthDecaySpeed)//decrease health by 1 
        {
            HealthCurrent = -1;
            decayTimer = 0;
        }
        
    }
}
