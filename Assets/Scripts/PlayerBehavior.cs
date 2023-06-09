using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public GameObject weapon;

    public float weakness;
    public float speed;
    public float rotateSpeed;
    public float dashSpeed;
    public float dashCooldown;
    public float slashSpeed;
    public float slashAngle;
    public float agingSpeed;
    public float maxHp;
    public float hp;
    public bool age;

    Quaternion rotation;
    Vector3 velocity;
    Vector3 input;
    float dashTimer;
    float slash;
    float slashAmount;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (age)
        {
            weakness += Time.deltaTime * agingSpeed * 0.01f;
            maxHp = 100 * (0.5f / (weakness + 1) + 0.5f);
        }
        Move();
        Attack();
    }
    private void FixedUpdate()
    {
        velocity *= 0.1f / (weakness + 1) + 0.88f;
    }
    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            slashAmount = 1;
            slash = slashAngle;
        }
    }
    private void Move()
    {
        //Get angle of mouse pointer with lerp
        Vector3 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rotation = Quaternion.Slerp(rotation, Quaternion.AngleAxis(angle, Vector3.forward), rotateSpeed * Time.deltaTime);

        //Flip the player to face the mouse pointer
        if(angle > -90 && angle < 90)
        {
            spriteRenderer.flipX = false;

            //Rotate the held weapon to face the mouse pointer
            weapon.transform.rotation = Quaternion.Slerp(rotation, Quaternion.Slerp(Quaternion.Euler(0, 0, angle - slash), Quaternion.Euler(0, 0, angle + slash), slashAmount), slashAmount);
            weapon.transform.position = transform.position + dir.normalized * 0.5f;
            slashAmount -= slashSpeed * Time.deltaTime;
        }
        else
        {
            spriteRenderer.flipX = true;

            //Rotate the held weapon to face the mouse pointer
            weapon.transform.rotation = Quaternion.Slerp(rotation, Quaternion.Slerp(Quaternion.Euler(0, 0, angle + slash), Quaternion.Euler(0, 0, angle - slash), slashAmount), slashAmount);
            weapon.transform.position = transform.position + dir.normalized * 0.5f;
            slashAmount -= slashSpeed * Time.deltaTime;
        }

        //Movement
        transform.position += velocity * Time.deltaTime;
        input = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            input.y = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            input.y = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            input.x = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            input.x = -1;
        }
        if (dashTimer > dashCooldown && Input.GetKeyDown(KeyCode.Space))
        {
            dashTimer = 0;
            velocity = input.normalized * dashSpeed;
        }
        velocity += input.normalized * speed * Time.deltaTime;
        dashTimer += Time.deltaTime;
    }
}
