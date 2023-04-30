using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindScript : MonoBehaviour
{
    public WeaponBaseClass Script; //the script to reference
    // Start is called before the first frame update
    void Start()
    {
        if (Script == null)
        {
            Script = GetComponentInParent<WeaponBaseClass>();
        }
    }
    public void EjectCasing()
    {
        Script.EjectCasing();
    }
    public void MuzzleFlash()
    {
        Script.MuzzleFlash();
    }
}
