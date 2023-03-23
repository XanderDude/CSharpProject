using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    
    private float life = 2;
    public int damage = 50;
    // Start is called before the first frame update

    void OnEnable()
    {
        this.GetComponent<SphereCollider>().enabled = true; //enable trigger
        StartCoroutine(ObjectDisable()); //disable object after certain time
        StartCoroutine(TriggerDisable()); //disable trigger quickly
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EnemyHealth>(out EnemyHealth enemy))//if the projectile collides with an enemy, deal damage
        {
            enemy.Damage(damage);
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
}
