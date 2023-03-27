using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimStateCheck : MonoBehaviour
{
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private Collider hitbox; //the collider dealing damage
    // Start is called before the first frame update

    private void Start()
    {
        //start with these disabled
        trail.enabled = false;
        hitbox.enabled = false;
    }

    public void ToggleLineRend()
    {
        trail.enabled = !trail.enabled;

    }

    public void ToggleHitBox()
    {
        hitbox.enabled = !hitbox.enabled;
    }
}
