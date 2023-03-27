using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    private enum Target { Player, Enemy } //who the projectile is looking for
    [SerializeField] private Target currentTarget; //the current target
    [SerializeField] private int damage = 50;
    private int pushBack = 0;
    [SerializeField] private TrailRenderer trail;

    private void OnDisable()
    {
        if (trail != null)
        {
            trail.Clear();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        switch (currentTarget)
        {
            case Target.Player:
                if (collision.TryGetComponent<PlayerHealth>(out PlayerHealth player))//if the projectile collides with an enemy, deal damage
                {
                    player.HealthCurrent = -damage; //health script adds value passed to it, subtract damage from health
                }
                break;
            case Target.Enemy:
                if (collision.TryGetComponent<EnemyHealth>(out EnemyHealth enemy))//if the projectile collides with an enemy, deal damage
                {
                    enemy.Damage(damage, pushBack, transform.position);
                }
                break;
            default:
                if (collision.TryGetComponent<EnemyHealth>(out EnemyHealth enemy2))//if the projectile collides with an enemy, deal damage
                {
                    enemy2.Damage(damage, pushBack, transform.position);
                }
                break;
        }
    }

    public void InitDamage(int setDamage, int setPushBack) //Allows the player to set the projectile's damage and push back
    {
        damage = setDamage;
        pushBack = setPushBack;
    }
}
