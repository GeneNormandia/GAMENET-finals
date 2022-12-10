using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiMovement : MonoBehaviour
{
    public int movementRandomizer;
    private float speed = .05f;

    float xDirection;
    float zDirection; 

    void Start()
    {
        xDirection = Random.Range(-0.1f, 0.2f); 
        zDirection = Random.Range(-0.1f, 0.2f);
    }
    // Update is called once per frame
    void Update()
    {
        //float xDirection = Input.GetAxis("Horizontal");
        //float zDirection = Input.GetAxis("Vertical");
        

        Vector3 moveDirection = new Vector3(xDirection, 0.0f, zDirection);

        transform.position += moveDirection * speed;


    }

    /*public IEnumerator RandomizerTimer()
    {
        GameObject killFeedText = GameObject.Find("Kill Feed Text");
        killFeedText.GetComponent<Text>().text = killer + " killed " + dead;

        float timer = 3.0f;

        while (timer > 0)
        {
            yield return new WaitForSeconds(1.0f);
            timer--;
        }
    }*/


}
