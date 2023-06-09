using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public float weakness;
    public float speed;
    public float dashSpeed;
    public float dashCooldown;

    Vector3 velocity;
    Vector3 input;
    float dashTimer;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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
        velocity += input.normalized * speed;
        dashTimer += Time.deltaTime;
    }
    private void FixedUpdate()
    {
        velocity *= 0.05f / (weakness + 1) + 0.92f;
    }
}
