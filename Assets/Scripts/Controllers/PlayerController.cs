using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;


    //Player Movement Controls
    Rigidbody rb;
    float horizontal;
    float vertical;
    [SerializeField] float speed;
    public enum Direction { left, right, up, down, idle };
    public Direction direction;


    [SerializeField]
    bool interacting;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Interact") && !interacting)
        {
            StartCoroutine(Interact());
        }


        //Convert input to local value
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");


        //Movement Controller
        if (interacting)
        {
            //Force Player to Idle
            horizontal = 0;
            vertical = 0;

            direction = Direction.idle;
        }
        else
        {
            //Direction Controls for Animation
            if (horizontal > 0)
                direction = Direction.right;
            else if (horizontal < 0)
                direction = Direction.left;
            else if (vertical > 0)
                direction = Direction.up;
            else if (vertical < 0)
                direction = Direction.down;
            else
                direction = Direction.idle;
        }
        
    }

    private void FixedUpdate()
    {
        //Move player in FixedUpdate for consistent performance
        rb.velocity = new Vector3(horizontal * speed, 0, vertical * speed);
    }

    IEnumerator Interact()
    {
        interacting = true;

        Debug.Log("Interacting");

        yield return new WaitForSeconds(0.5f);

        interacting = false;
    }
}
