using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public GameObject weapon;
    public bool melee;
    public GameObject trail;

    public float weakness;
    public float speed;
    public float rotateSpeed;
    public float dashSpeed;
    public float dashCooldown;
    public float recoilSpeed;
    public float recoilAngle;
    public float agingSpeed;
    public float maxHp;
    public float hp;
    public bool age;

    GameObject slashObject;
    Quaternion rotation;
    Vector3 velocity;
    Vector3 input;
    float dashTimer;
    float recoilAnim;
    float recoilEase;
    float slashEase;
    float swapWeaponAnim;
    GameObject newWeapon;
    bool isMelee;

    public void SwapWeapon(GameObject newWeapon, bool isMelee)
    {
        swapWeaponAnim = 1;
        this.newWeapon = newWeapon;
        this.isMelee = isMelee;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (melee)
        {
            slashObject = Instantiate(trail, weapon.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (age)
        {
            weakness += Time.deltaTime * agingSpeed * 0.01f;
            maxHp = 100 * (0.5f / (weakness + 1) + 0.5f);
        }

        //Temporary swap weapon button, also works for reloading guns?
        if (Input.GetKeyDown(KeyCode.F))
        {
            SwapWeapon(weapon, melee);
        }
        Move();
        Attack();
    }
    private void FixedUpdate()
    {
        velocity *= 0.1f / (weakness + 1) + 0.88f;
        if (melee && swapWeaponAnim == 0)
        {
            slashEase = Mathf.Max(slashEase, (rotation.eulerAngles * 50000 - weapon.transform.rotation.eulerAngles * 50000).magnitude) * 0.9f;
            slashObject.GetComponent<TrailRenderer>().startColor = new Color(1, 1, 1, slashEase);
        }
        else
        {
            slashEase = 0;
        }
    }
    private void Attack()
    {
        if (!melee & Input.GetKeyDown(KeyCode.Mouse0))
        {
            recoilEase = 1;
            recoilAnim = recoilAngle;
        }
    }
    private void Move()
    {
        //Get angle of mouse pointer with lerp
        Vector3 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + swapWeaponAnim * 360;
        rotation = Quaternion.Slerp(rotation, Quaternion.AngleAxis(angle, Vector3.forward), rotateSpeed * Time.deltaTime);

        //Flip the player to face the mouse pointer
        if(angle > -90 && angle < 90)
        {
            spriteRenderer.flipX = false;

            //Rotate the held weapon to face the mouse pointer
            weapon.transform.rotation = Quaternion.Slerp(rotation, Quaternion.Slerp(Quaternion.Euler(0, 0, angle - recoilAnim), Quaternion.Euler(0, 0, angle + recoilAnim), recoilEase), recoilEase);
            if(swapWeaponAnim == 0) weapon.GetComponent<SpriteRenderer>().flipY = false;
        }
        else
        {
            spriteRenderer.flipX = true;

            //Rotate the held weapon to face the mouse pointer
            weapon.transform.rotation = Quaternion.Slerp(rotation, Quaternion.Slerp(Quaternion.Euler(0, 0, angle + recoilAnim), Quaternion.Euler(0, 0, angle - recoilAnim), recoilEase), recoilEase);
            if (swapWeaponAnim == 0) weapon.GetComponent<SpriteRenderer>().flipY = true;
        }
        swapWeaponAnim -= Time.deltaTime * 5;
        if (swapWeaponAnim < 0)
            swapWeaponAnim = 0;
        else if (swapWeaponAnim < 0.5f)
        {
            weapon = newWeapon;
            melee = isMelee;
        }

        weapon.transform.position = transform.position + dir.normalized * (0.5f + ((swapWeaponAnim - 0.5f) * (swapWeaponAnim - 0.5f) - 0.25f) * 2);
        recoilEase -= recoilSpeed * Time.deltaTime;

        if (recoilEase < 0)
        {
            Destroy(slashObject);
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
