using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{

    [SerializeField] private int projDamage = 50;
    [SerializeField] private float projSpeed = 20f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float lifetime = 4f;

    // Start is called before the first frame update
    void Start()
    {
        rb.AddForce(transform.forward * projSpeed, ForceMode.Impulse);
        Destroy(gameObject, lifetime); //incase no collision is detected
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent<PlayerHealth>(out PlayerHealth player))//if the projectile collides with an enemy, deal damage
        {
            player.HealthCurrent = -projDamage; //health script adds value passed to it, subtract damage from health
        }
        Destroy(gameObject); //destroy self whenever collision is detected
    }
    public void InitDamage(int setDamage) //Allows the player to set the projectile's damage
    {
        projDamage = setDamage;
    }
}
