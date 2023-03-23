using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    //------------------------------Enemy Wave Spawner---------------------------------------------
    //Current candence: 3 Waves, Wave 1: 4 Enemies, Wave 2: 6 Enemies, Wave 3: 10 Enemeis
    [SerializeField] private int[] waveSpawnAmount; //the amount of waves and enemies per each wave
    [SerializeField] private float waveDelay = 5f; //the time until after completing a wave till the next one starts
    private int wave = 0; //the current wave
    private bool finished = false; //check for when a wave has been cleared

    [SerializeField] private ObjectPool objectPool; //a reference for the script that handles pooling the spawned objects
    [SerializeField] private Transform[] spawnLocations; //An array for holding the spawn locations for the enemies
    [SerializeField] private GameObject enemyPrefab; //The prefabs that'll be created at the spawn locations
    [SerializeField] private float positionOffset = 2f; //ensure they don't spawn in the floor (add gravity to enemies)
    [SerializeField] private float spawnDelay = 1f; //The delay inbetween each spawn during the start of the wave (depricate?)
    private bool spawning = false; //enemies are spawning, beginning of each wave

    private int enemiesRemaining; //for tracking the amount of enemies alive 

    private GameManager manager;
    public PlayerHUD pHUD; //the script handling updating the hud, on the canvas
    public Transform player; //the location of the player. Enemies created under this manager use this reference

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        pHUD.HUDUpdateWave(wave); //update hud on start 
        
        StartCoroutine(SpawnWave(waveSpawnAmount[wave])); //start spawning first wave
    }

    // Update is called once per frame
    void Update()
    {
        if (spawning)
        {
            return; //wait for the wave to finish spawning
        }

        if (enemiesRemaining > 0)
        {
            return; //wait for all enemies to be defeated
        }
        finished = true; //wave is complete, allow for preparing next wave to spawn

        if (wave == waveSpawnAmount.Length - 1)
        {
            spawning = true;
            //final wave completed
            pHUD.HUDWavesCompleted();//update hud with waves completed text
            manager.WavesComplete();
            return;
        }

        //start next wave
        if (finished) //when a wave has been cleared, increase wave, start timer for next wave
        {
            finished = false; //this check plus bool spawning prevent multiple waves from queueing up
            wave += 1; //increase wave
            StartCoroutine(SpawnWave(waveSpawnAmount[wave])); //wait for wave delay, then start spawning enemies one at a time
        }
    }

    //private void SpawnEnemy() //basic spawning. OLD
    //{
    //    //int randomLocation = Random.Range(0, spawnLocations.Length);

    //    //spawnedEnemy = Instantiate(enemyPrefab, spawnLocations[randomLocation].position + Vector3.up * positionOffset, Quaternion.identity);

    //    for (int i = 0; i < spawnLocations.Length; i++)
    //    {
    //        Debug.Log("Spawning enemy");
    //        spawnedEnemies.Add(Instantiate(enemyPrefab, spawnLocations[i].position + Vector3.up * positionOffset, Quaternion.identity, GetComponent<Transform>()));

    //    }
    //}

    private IEnumerator SpawnWave(int spawnAmount)
    {
        spawning = true;
        yield return new WaitForSeconds(waveDelay);
        //spawnedEnemies.Clear(); //before spawning, make sure list is clear
        pHUD.HUDUpdateWave(wave); //update hud wave counter
        pHUD.HUDUpdateEnemies(spawnAmount); //update hud with the amount of enemies the player needs to defeat
        enemiesRemaining = spawnAmount; //set enemy counter
        for (int i = 0; i < spawnAmount; i++) //for how many to spawn, spawn 1 randomly
        {
            int randomLocation = Random.Range(0, spawnLocations.Length); //get a new random location for each enemy

            //spawnedEnemies.Add(Instantiate(enemyPrefab, spawnLocations[randomLocation].position + Vector3.up * positionOffset, Quaternion.identity, GetComponent<Transform>()));//create enemy at the random location, parent to spawn manager
            objectPool.GetObject(spawnLocations[randomLocation].position + Vector3.up * positionOffset, Quaternion.identity, GetComponent<Transform>());

            if (i < spawnAmount - 1)//apply spawn delay until spawning last enemy
            {
                yield return new WaitForSeconds(spawnDelay);
            }
        }

        spawning = false; //after all enemies have been spawned

    }
    public void EnemyDefeated()//called by enemies when defeated
    {
        enemiesRemaining -= 1;
        pHUD.HUDUpdateEnemies(enemiesRemaining);//update hud with remaining enemies
    }
}
