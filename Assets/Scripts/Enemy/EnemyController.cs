using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private NavMeshAgent enemyAgent; //the enemy's own nav mesh agent component
    [SerializeField] private Transform player; //finding the player
    [SerializeField] private LayerMask playerMask; //for detecting if the player is within a certain distance to the enemy

    //starting position

    [SerializeField] private float patrolDistance = 10f; //patrol distance +10 -10 current location
    [SerializeField] private float sightRange = 15f; //distance the player needs to be from the enemy for it to chase
    [SerializeField] private float attackRange = 2f; //distance the player needs to be for the enemy to attack
    [SerializeField] private float attackRotationSpeed = 1f; //rotation speed while attacking

    [SerializeField] private Vector3 startPos; //for returning to if new destination cannot be found
    private Vector3 destination; //current destination
    [SerializeField] private bool isDestinationSet = false; //for checking if the enemy has reached destination or needs a new one

    private bool playerInSightRange; //player is in range to be chased
    private bool playerInAttackRange; //player is in range to be attacked

    [SerializeField] private EnemyAttack enemyAttack; //reference for the attack script that handles how the enemy should attack

    private bool dead = false;

    public bool PlayerInSightRange
    {
        get { return playerInSightRange; }
        set 
        {  //if value (player in sight) is not equal to the previously set value of playerInSight AND player is not in sight
            if (value != playerInSightRange && !value) //only run if value has changed, and the value is false
            {
                isDestinationSet = false;
                //destination = GetDestination();
                
            }
            playerInSightRange = value; //update every time

        }
    }



    // Start is called before the first frame update
    void Awake()
    {
        startPos = GetComponent<Transform>().position; //the place to return to if there cannot be new destination to set
        enemyAgent = GetComponent<NavMeshAgent>(); //assigns ref own nav mesh agent component
        if (player == null) //if player has not been assigned...
        {
            player = GetComponentInParent<WaveManager>().player; //get a ref to the player transform
        }
    }

    private void OnEnable()
    {
        dead = false;
        enemyAgent.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (dead) return; //no checks if dead
        PlayerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask); //returns true if player in chase range
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask); //returns true if player in attack range

        if (!PlayerInSightRange) //player is not in sight range
        {
            Patrol(); //Move to random positions within set distance
        }
        else if(PlayerInSightRange && !playerInAttackRange) //player in sight range, but not attack range
        {
            Chase(); //Move towards player
        }
        else //player must be in attack range
        {
            Attack(); //Stop moving, continue looking at player
        }
    }

    private void Patrol() //Move to random positions within set distance
    {
        if(!isDestinationSet)
        {
            //isDestinationSet = true; 
            destination = GetDestination(); //find a new destination
            enemyAgent.SetDestination(destination); //apply the new destination to the enemy
        }

        Vector3 destinationDist = transform.position - destination; //the vector direction to the destination

        if (destinationDist.magnitude < 0.1f) //when distance to destination has been crossed
        {
            isDestinationSet = false; //set false to find new destination
        }
    }

    private void Chase() //Move towards player
    {

        enemyAgent.SetDestination(player.position);
        //if (NavMesh.SamplePosition(player.position, out NavMeshHit hit, 2f, NavMesh.AllAreas)) //check that new destination is within nav mesh
        //{
        //    enemyAgent.SetDestination(hit.position); //Set the enemy's destination to the player's position
        //}
    }
    
    private void Attack() //Stop moving, continue looking at player
    {
        enemyAgent.SetDestination(transform.position); //Set the enemy's position to itself, hault to attack

        Vector3 lookVector = player.position - transform.position; //find vector from enemy to player
        //lookVector.y = 0f; //remove vectical direction
        Quaternion rotation = Quaternion.LookRotation(lookVector); //make rotation from direction
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * attackRotationSpeed);//rotate enemy to face player
        enemyAttack.DoAttack(); //start attacking depending on the enemy's attack type
    }

    private Vector3 GetDestination() //find new random patrol destination, or start position if cannot find one
    {
        float x = Random.Range(-patrolDistance, patrolDistance); //Find a random value between + and - patrolDistance for x and y
        float z = Random.Range(-patrolDistance, patrolDistance);

        Vector3 randomDest = new Vector3(x + transform.position.x, transform.position.y, z + transform.position.z);

        if (NavMesh.SamplePosition(randomDest, out NavMeshHit hit, 2f, NavMesh.AllAreas)) //check that new destination is within nav mesh
        {
            isDestinationSet = true;
            return hit.position;
        }
        return startPos; //if cannot find acceptable destination, return to starting position
    }

    public void Dead() //called to give controller death behavior
    {
        dead = true;
        enemyAgent.enabled = false;
    }
}
