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
    private float lerpTimer = 0;
    private bool damageTaken;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private EnemyController controller;
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
        if (damageTaken && currentHealth > 0)
        {
            lerpTimer += Time.deltaTime;
            mesh.material.color = Color.Lerp(Color.red, defaultMat, lerpTimer * 2);
        }

    }

    public void Damage(int damageAmount, int pushBack, Vector3 hitPos) //called when this enemy takes damage
    {
        if (currentHealth <= 0)
        {
            return; //only run once on death
        }
        Vector3 pushDirection = transform.position - hitPos;
        rb.AddForce(pushDirection * pushBack, ForceMode.Impulse);
        damageTaken = true;
        lerpTimer = 0;
        currentHealth -= damageAmount; //reduce health by damamge amount
        mesh.material.color = Color.red; //show damage received
        if (currentHealth <= 0)
        {
            rb.AddForce(pushDirection * pushBack, ForceMode.Impulse);
            spawnManager?.EnemyDefeated();  //update spawn manager, which then also updates hud
            Death();
        }
    }

    private void Death()
    {
        controller.Dead();
        StartCoroutine(DeathTimer());
    }
    private IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);//disable self on death
    }
}
