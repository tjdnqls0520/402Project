using Unity.VisualScripting;
using UnityEngine;
using static PlayerMouseMovement;

public class playeranimation : MonoBehaviour
{
    Animator ani;
    public GameObject pl;
    float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bool leftInputDown = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.A);
        bool rightInputDown = Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.S);
        bool leftInputHeld = Input.GetMouseButton(0) || Input.GetKey(KeyCode.A);
        bool rightInputHeld = Input.GetMouseButton(1) || Input.GetKey(KeyCode.S);


        if (pl.GetComponent<PlayerMouseMovement>().isFlying == true)
        {
            timer = 0;
            ani.SetBool("jump", true);
        }
        else if ((pl.GetComponent<PlayerMouseMovement>().IsGrounded() || pl.GetComponent<PlayerMouseMovement>().IsBreak()) && !leftInputHeld && !rightInputHeld)
        {
            timer += Time.deltaTime;
            ani.SetBool("jump", false);
            ani.SetBool("att", true);
            if (timer >= 0.35f)
                ani.SetBool("att", false);
        }

        if( pl.GetComponent<PlayerMouseMovement>().getskill == true && (pl.GetComponent<PlayerMouseMovement>().isBoostFlying == false || pl.GetComponent<PlayerMouseMovement>().IsGrounded()))
        {
            if (pl.GetComponent<PlayerMouseMovement>().dash)
            {
                ani.SetBool("double", false);
                ani.SetBool("spin", false);
                pl.GetComponent<PlayerMouseMovement>().dash = false;
            }
            else if (pl.GetComponent<PlayerMouseMovement>().jump)
            {
                ani.SetBool("up", false);
                ani.SetBool("upspining", false);
                pl.GetComponent<PlayerMouseMovement>().jump = false;
            }

               
          
            pl.GetComponent<PlayerMouseMovement>().getskill = false;
        }

       

        if (pl.GetComponent<PlayerMouseMovement>().isBoostFlying == true)
        {
            if(pl.GetComponent<PlayerMouseMovement>().dash  == true)
            {
                ani.SetBool("spin", true);
                ani.SetBool("double", true);
                ani.SetBool("up", false);
                ani.SetBool("upspining", false);
            }
            else if(pl.GetComponent<PlayerMouseMovement>().jump == true)
            {
                ani.SetBool("upspining", true);
                ani.SetBool("double", false);
                ani.SetBool("up", true);
                ani.SetBool("spin", false);
            }
            
        }


    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("CrystalDash"))
        {
            ani.SetBool("double", true);
            ani.SetBool("up", false);

        }
        else if (other.CompareTag("CrystalJump"))
        {
            ani.SetBool("up", true);
            ani.SetBool("double", false);
        }
    }


}
