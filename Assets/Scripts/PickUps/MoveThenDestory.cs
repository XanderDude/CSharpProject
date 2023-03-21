using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveThenDestory : MonoBehaviour
{
    //Note: This should be handled in an object pool to prevent constant instantiating/destroying

    private RectTransform rectTrans; //the ui object's transform
    private float scrollSpeed = 30f;
    private readonly float aliveTimer = 1f; //time to be alive
    

    // Start is called before the first frame update
    void Start()
    {
        rectTrans = GetComponent<RectTransform>(); //assign self's transform
        
    }

    // Update is called once per frame
    void Update()
    {
        //scroll ui object up the canvas on awake
        rectTrans.anchoredPosition = new Vector2(rectTrans.anchoredPosition.x, rectTrans.anchoredPosition.y + Time.deltaTime * scrollSpeed);
        GetComponent<TextMeshProUGUI>().CrossFadeAlpha(0, aliveTimer/5, false); //Fade the text out overtime
        Destroy(gameObject, aliveTimer); //after set time, destroy self
    }
}
