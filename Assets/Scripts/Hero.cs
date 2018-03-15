using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{

    public float speed = 5f;
    public float jumpForce = 400;

    public GameObject legs;
    public GameObject torso;

    private Animator animLegs;
    private Animator animTorso;

    private Rigidbody2D rb2d;
    private bool facingRight = true;
    private bool jump;
    private bool onGround = false;
    private Transform groundCheck;
    private float hForce = 0;

    //Tiro
    private float fireRate = 0.5f;
    private float nextFire;
    public GameObject bulletPreab;
    public Transform shootSpawner;


    private bool isDead = false;


    // Use this for initialization
    void Start()
    {
        animLegs = legs.GetComponent<Animator>();
        animTorso = torso.GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        groundCheck = gameObject.transform.Find("GroundCheck");

    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            //define o player no chao
            onGround = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

            //Se o player estiver no chao quer dizer que pode pular
            if (onGround)
            {
                animTorso.SetBool("Jump", false);
                animLegs.SetBool("Jump", false);
            }

            //Configuração do pulo
            if (Input.GetButtonDown("Jump") && onGround)
            {
                jump = true;
            }
            else if (Input.GetButtonUp("Jump"))
            {
                if (rb2d.velocity.y > 0)
                {
                    rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * 0.5f);
                }
            }

            //Se o botão de tiro estiver pressionado, muda a animação tiro
            if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                animTorso.SetTrigger("Shoot");
                GameObject tempBullet = Instantiate(bulletPreab, shootSpawner.position, shootSpawner.rotation);

                if (!facingRight)
                {
                    tempBullet.transform.eulerAngles = new Vector3(0, 0, 180);
                }

            }
        }
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            hForce = Input.GetAxisRaw("Horizontal");

            animLegs.SetFloat("Speed", Mathf.Abs(hForce));
            animTorso.SetFloat("Speed", Mathf.Abs(hForce));

            rb2d.velocity = new Vector2(hForce * speed, rb2d.velocity.y);

            if (hForce > 0 && !facingRight)
            {
                Flip();
            }
            else if (hForce < 0 && facingRight)
            {
                Flip();
            }

            //Se jump for verdadeiro seta no animator, coloca false para nao pular de novo e muda a velocidade
            if (jump)
            {
                animTorso.SetBool("Jump", true);
                animLegs.SetBool("Jump", true);
                jump = false;
                rb2d.AddForce(Vector2.up * jumpForce);
            }
        }
    }

    void Flip()
    {
        facingRight = !facingRight;

        Vector3 scaleLeg = legs.transform.localScale;
        scaleLeg.x *= -1;
        legs.transform.localScale = scaleLeg;

        Vector3 scaleTorso = torso.transform.localScale;
        scaleTorso.x *= -1;
        torso.transform.localScale = scaleTorso;
    }
}