using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColors : MonoBehaviour
{
    [SerializeField] private Material objMat; //The default material component for the object, color red

    [SerializeField] private Material objMat2; //A blue material component

    [SerializeField] private Material objMat3; //A green material component

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().material = objMat; //Assigns the red material to the game object's mesh renderer
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)) //Returns True for 1 Frame when the '1' alpha key is pressed down
        {
            Debug.Log("Set Object to red");
            GetComponent<MeshRenderer>().material = objMat; //Assigns the red material to the game object's mesh renderer
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) //Returns True for 1 Frame when the '2' alpha key is pressed down
        {
            Debug.Log("Set Object to blue");
            GetComponent<MeshRenderer>().material = objMat2; //Assigns the blue material to the game object's mesh renderer
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) //Returns True for 1 Frame when the '3' alpha key is pressed down
        {
            Debug.Log("Set Object to green");
            GetComponent<MeshRenderer>().material = objMat3; //Assigns the green material to the game object's mesh renderer
        }
    }
}
