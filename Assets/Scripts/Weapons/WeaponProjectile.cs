using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProjectile : MonoBehaviour
{

    [SerializeField] private int projDamage = 50;
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
        if (impactExplosion != null)
        {
            GameObject projectileObj = objectPool.GetObject(gameObject.transform.position, impactExplosion.transform.rotation, null); //ref for and grab an explosion
            projectileObj.GetComponent<Explosion>().damage = projDamage; //set explosion's damage
            gameObject.SetActive(false);
            return;
        }
        if (collision.collider.TryGetComponent<EnemyHealth>(out EnemyHealth enemy))//if the projectile collides with an enemy, deal damage
        {
            enemy.Damage(projDamage);
        }
        gameObject.SetActive(false);
    }
    public void InitDamage(int setDamage) //Allows the player to set the projectile's damage
    {
        projDamage = setDamage;
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
