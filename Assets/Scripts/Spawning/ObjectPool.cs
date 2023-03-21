using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{

    public GameObject ObjectPrefab; //The prefabs that'll be pooled
    public List<GameObject> SpawnedObjects; //A list for tracking the pooled objects

    public GameObject GetObject(Vector3 spawnPos, Quaternion SpawnRotation, Transform parent)
    {
        if (parent == null)
        {
            parent = GameObject.FindGameObjectWithTag("Pool").GetComponent<Transform>(); //set a general object to parent the objects getting pooled;
        }
        for (int i = 0; i < SpawnedObjects.Count; i++) //search through all spawned objects
        {
            if (!SpawnedObjects[i].activeSelf) //if there is a spawned object that is not active
            {
                SpawnedObjects[i].transform.position = spawnPos; //reset object's postion if they moved since spawning
                SpawnedObjects[i].transform.rotation = SpawnRotation; //reset object's rotation in case they rotated since spawning
                SpawnedObjects[i].SetActive(true);
                return SpawnedObjects[i]; //return the now active object
            }
        }
        GameObject newObject = Instantiate(ObjectPrefab, spawnPos, SpawnRotation, parent); //create a new object to return
        SpawnedObjects.Add(newObject); //add to pool
        return newObject; //return the new object

    }
}
