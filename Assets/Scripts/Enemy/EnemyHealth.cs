using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100; //starting and respawned health
    [SerializeField] private int currentHealth; //starting and changing health
    [SerializeField] private Material damageMat; //the mat to use when damage is taken'
    private Color defaultMat;
    [SerializeField] private MeshRenderer mesh; //the mesh on the enemy
    private Color lerpColor; //the changing color, going from damageMat color to defaultMat color
    private float lerpTimer = 0;
    private bool damageTaken;

    private WaveManager spawnManager;

    // Start is called before the first frame update
    void Awake()
    {
        //currentHealth = maxHealth;
        if (GetComponentInParent<WaveManager>() != null)
        {
            spawnManager = GetComponentInParent<WaveManager>(); //ref for updating remaining enemies
        }
        
        defaultMat = mesh.material.color;
    }

    private void OnEnable()
    {
        currentHealth = maxHealth; //when respawning, set health back to full
        mesh.material.color = defaultMat; //set mat back to default
        damageTaken = false;
    }
    // Update is called once per frame

    private void Update()
    {
        if (damageTaken)
        {
            lerpTimer += Time.deltaTime;

            mesh.material.color = Color.Lerp(Color.red, defaultMat, lerpTimer * 2);

        }
    }

    public void Damage(int damageAmount) //called when this enemy takes damage
    {
        Debug.Log("Enemy took damage");

        damageTaken = true;
        lerpTimer = 0;
        if (currentHealth <= 0)
        {
            if (spawnManager != null)
            {
                spawnManager.EnemyDefeated();//update spawn manager, which then also updates hud
            }
            gameObject.SetActive(false);//disable self on death
            //dead
        }
        else
        {
            currentHealth -= damageAmount; //reduce health by damamge amount
        }

    }
}
