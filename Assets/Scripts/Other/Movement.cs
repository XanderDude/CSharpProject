using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Movement : MonoBehaviour
{

    public float speed = 1f; //Speed of the moving object
    public Vector3 objPos; //the current position of the moving object


    public Transform[] locTargets; //List of targets for the object to move to. New target is set after current target location is reached.
    [SerializeField] private int targetNum = 0; //Counter for switching through list of targets

    public Material[] mats; //Array of materials to be applied to the moving object
    private int matNum = 0; //Counter for switching through list of materials

    private MeshRenderer objRend; //Variable to be used for the moving object's mesh renderer

    // Start is called before the first frame update
    void Start()
    {
        objRend = GetComponent<MeshRenderer>(); //Assign the moving object's mesh renderer to the variable
        objRend.material = mats[0]; //Apply the first material in the list to the moving object
    }

    // Update is called once per frame
    void Update()
    {
        getNewFloat(5);
        objPos = transform.position; //updates current position of the moving object
        transform.position = Vector3.MoveTowards(objPos, locTargets[targetNum].position, Time.deltaTime * speed); //translate the object to the target, adjusted by deltatime and speed
        if (objPos == locTargets[targetNum].position) //check if the moving object has reached the target's location
        {
            
            if (targetNum >= locTargets.Length - 1) //check if counter has reached the end of the list of targets
            {
                targetNum = 0; //If so, reset counter
            }
            else
            {
                targetNum += 1; //if not, continue counting up
            }
            if (matNum >= mats.Length - 1) //Check if counter has reached end of the list of materials
            {
                matNum = 0; //If so, reset counter
            }
            else
            {
                matNum += 1; //if not, continue counting up
            }
            objRend.material = mats[matNum]; //change the moving object's material to the material in the list according to the counter

        }

    }

    /// <summary>
    /// Multiplies paramter by 5
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns>Paramter * 5</returns>
    private float getNewFloat(float parameter)
    {

        return 5f * parameter;
    }

}
