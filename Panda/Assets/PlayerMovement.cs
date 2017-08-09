using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private Rigidbody rb;
    private Vector3 objCentre;
    
    public float speed;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        Renderer renderer = GetComponent<Renderer>();
        Debug.Log(renderer.bounds.size);
        objCentre = new Vector3(0.5f, 0f, 0.5f);

    }
	
	void FixedUpdate ()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        
        transform.Translate(moveHorizontal*speed *Time.deltaTime, 0f, moveVertical * speed * Time.deltaTime);
        //rb.AddForce(momentum * speed);
    }
}
