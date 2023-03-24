using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    
    readonly float life = 2;
    public int damage = 50;
    public int pushBack = 50;
    // Start is called before the first frame update

    void OnEnable()
    {
        this.GetComponent<SphereCollider>().enabled = true; //enable trigger
        StartCoroutine(ObjectDisable()); //disable object after certain time
        StartCoroutine(TriggerDisable()); //disable trigger quickly
    }
    private void OnTriggerEnter(Collider other)
    {
        Vector3 pos = transform.position;
        if (other.TryGetComponent<EnemyHealth>(out EnemyHealth enemy))//if the projectile collides with an enemy, deal damage
        {
            enemy.Damage(damage, pushBack, pos);
        }
    }

    private IEnumerator ObjectDisable()
    {
        yield return new WaitForSeconds(life);
        gameObject.SetActive(false);
    }
    private IEnumerator TriggerDisable()
    {
        yield return new WaitForSeconds(.1f);
        this.GetComponent<SphereCollider>().enabled = false;
    }
    public void InitDamage(int setDamage, int setPushBack) //Allows the player to set the projectile's explosion damage and push back
    {
        damage = setDamage;
        pushBack = setPushBack;
    }
}
