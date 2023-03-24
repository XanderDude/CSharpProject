using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProjectile : MonoBehaviour
{
    private enum Target { Player, Enemy} //who the projectile is looking for
    [SerializeField] private Target currentTarget; //the current target
    [SerializeField] private int projDamage = 50;
    private int pushBack = 0;
    [SerializeField] private float projSpeed = 20f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float lifetime = 4f;
    [SerializeField] private GameObject impactExplosion;
    [SerializeField] private ObjectPool objectPool;
    [SerializeField] private TrailRenderer trail;
    // Start is called before the first frame update
    void OnEnable()
    {
        rb.AddForce(transform.forward * projSpeed, ForceMode.Impulse);
        StartCoroutine(DisableSelf()); //on enable, start timer to disable
    }
    private void OnDisable()
    {
        rb.Sleep();
        if (trail != null)
        {
            trail.Clear();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Vector3 pos = transform.position;
        if (impactExplosion != null) //if there's an explosion attached, the explosion will deal the damage
        {
            GameObject projectileObj = objectPool.GetObject(gameObject.transform.position, impactExplosion.transform.rotation, null); //ref for and grab an explosion
            projectileObj.GetComponent<Explosion>().InitDamage(projDamage, pushBack); //set explosion's damage
            gameObject.SetActive(false); //disable self
            return;
        }
        switch (currentTarget)
        {
            case Target.Player:
                if (collision.collider.TryGetComponent<PlayerHealth>(out PlayerHealth player))//if the projectile collides with an enemy, deal damage
                {
                    player.HealthCurrent = -projDamage; //health script adds value passed to it, subtract damage from health
                }
                break;
            case Target.Enemy:
                if (collision.collider.TryGetComponent<EnemyHealth>(out EnemyHealth enemy))//if the projectile collides with an enemy, deal damage
                {
                    enemy.Damage(projDamage, pushBack, pos);
                }
                break;
            default:
                if (collision.collider.TryGetComponent<EnemyHealth>(out EnemyHealth enemy2))//if the projectile collides with an enemy, deal damage
                {
                    enemy2.Damage(projDamage, pushBack, pos);
                }
                break;
        }

        gameObject.SetActive(false);
    }
    public void InitDamage(int setDamage, int setPushBack) //Allows the player to set the projectile's damage and push back
    {
        projDamage = setDamage;
        pushBack = setPushBack;
    }

    private IEnumerator DisableSelf()
    {
        yield return new WaitForSeconds(lifetime);
        if (impactExplosion != null)
        {
            GameObject projectileObj = objectPool.GetObject(gameObject.transform.position, impactExplosion.transform.rotation, null); //ref for and grab an explosion
            projectileObj.GetComponent<Explosion>().damage = projDamage; //set explosion's damage
        }
        gameObject.SetActive(false);
    }
}
