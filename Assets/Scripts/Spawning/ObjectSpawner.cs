using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private ObjectPool objectPool; //a reference for the script that handles pooling the spawned objects
    [SerializeField] private Transform[] spawnLocations; //An array for holding the spawn locations of the pickups
    //[SerializeField] private GameObject ammoPrefab; //The prefabs that'll be created at the spawn locations
    //private GameObject spawnedAmmo; //A variable for tracking the spawned ammo
    [SerializeField] private int spawnAmount = 1; //amount of objects to spawn at a time
    [SerializeField] private float positionOffset = 2f; //offset of object, upward from spawn location
    [SerializeField] private float spawnDelay = 2f; //time to wait after an object is picked up before replacing it.
    private bool spawning = false; //check if currently spawning

    //public PlayerHUD pHUD; //the script handling updating the hud, on the canvas

    // Start is called before the first frame update
    void Start()
    {
        while (objectPool.SpawnedObjects.Count <= spawnAmount) //spawn all the maximum amount of objects
        {
            SpawnObject(); //spawn object(s)
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(objectPool.SpawnedObjects.Any<GameObject>(x => !x.activeSelf) && !spawning) //if there's a inactive ammo pickup, and there isn't a current ammo spawning
        {
            StartCoroutine(SpawnDelay());//if an object is picked up, start spawning another object
        }
    }

    private void SpawnObject()//the spawning of the object, at a random location
    {
        int randomLocation = Random.Range(0, spawnLocations.Length);

        spawnLocations[randomLocation].gameObject.tag = "Used"; //assign used tag so spawner knows spawn location is being used

        //spawnedAmmo = Instantiate(ammoPrefab, spawnLocations[randomLocation].position + Vector3.up * positionOffset, Quaternion.identity);

        objectPool.GetObject(spawnLocations[randomLocation].position + Vector3.up * positionOffset, 
            Quaternion.identity, spawnLocations[randomLocation]); //Tell the object pool to enable an object, and place at a random location 
    }

    private IEnumerator SpawnDelay()//the wait before the actual spawning
    {
        spawning = true; //spawning has started
        yield return new WaitForSeconds(spawnDelay);
        SpawnObject();//after waiting, spawn the object at a randomly selected location
        spawning = false;//spawning completed
    }
}
