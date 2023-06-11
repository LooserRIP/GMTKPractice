using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public GameObject weapon;
    public GameObject trail;
    public GameObject leftArm;
    public GameObject rightArm;
    public GameObject leftLeg;
    public GameObject rightLeg;

    public float weakness;
    public float speed;
    public float rotateSpeed;
    public float dashSpeed;
    public float dashCooldown;
    public float recoilSpeed;
    public float recoilAngle;
    public float slashSpeed;
    public float slashAngle;
    public float agingSpeed;
    public float maxHp;
    public float hp;
    public bool age;

    GameObject slashObject;
    Quaternion rotation;
    Vector2 input;
    float dashTimer;
    float recoilAnim;
    float recoilEase;
    float slashEase;
    float attackEase;
    float swapWeaponAnim;
    public GameObject newWeapon;
    int attackFlip;

    public int[] inventory;
    public int selectedSlot;
    public Sprite fistSprite;
    public hotbarUIManager hotbarui;
    public gameManager gm;

    float slashCooldown;
    float legAnim;

    public void init() {
        inventory = new int[]{3,-1,-1,-1,-1};
        selectedSlot = 0;
        hotbarui.render();
    }
    public void SwapWeapon(GameObject newWeapon) {
        swapWeaponAnim = 1;
        this.newWeapon = newWeapon;
    }

    // Start is called before the first frame update
    void Start() {
        init();   
        if (weapon.GetComponent<WeaponBehavior>().melee){
            slashObject = Instantiate(trail, weapon.transform);
        }
        gm.DropItem(4, transform);
    }

    // Update is called once per frame
    void Update() {
        if (age) {
            weakness += Time.deltaTime * agingSpeed * 0.01f;
            maxHp = 100 * (0.5f / (weakness + 1) + 0.5f);
        }
        if (Input.GetKeyDown("1")) {
            switchHotbarSlot(0);
        }
        if (Input.GetKeyDown("2")) {
            switchHotbarSlot(1);
        }
        if (Input.GetKeyDown("3")) {
            switchHotbarSlot(2);
        }
        if (Input.GetKeyDown("4")) {
            switchHotbarSlot(3);
        }
        if (Input.GetKeyDown("5")) {
            switchHotbarSlot(4);
        }
        //Temporary swap weapon button, also works for reloading guns?
        if (Input.GetKeyDown(KeyCode.F)) {
            SwapWeapon(weapon);
        }
        slashCooldown -= slashSpeed * Time.deltaTime;
        Move();
        Attack();
        Anim();
        Pickup();
    }
    public void switchHotbarSlot(int slot) {
        Debug.Log(slot + " placed");
        bool changed = false;
        if (slot != selectedSlot) changed = true;
        selectedSlot = slot;
        hotbarui.render();
        if (inventory[slot] == -1) {
            if (changed) {
                weapon.GetComponent<SpriteRenderer>().enabled = true;
                weapon.GetComponent<SpriteRenderer>().sprite = fistSprite;
                weapon.GetComponent<PolygonCollider2D>().TryUpdateShapeToAttachedSprite();
                weapon.GetComponent<SpriteRenderer>().enabled = false;
                SwapWeapon(weapon);
            }
        } else {
            Item pickedWeapon = gm.gameItems[inventory[slot]];
            if (changed) {
                weapon.GetComponent<SpriteRenderer>().enabled = true;
                weapon.GetComponent<SpriteRenderer>().sprite = pickedWeapon.Sprite;
                weapon.GetComponent<PolygonCollider2D>().TryUpdateShapeToAttachedSprite();
                SwapWeapon(weapon);
            }
        }
    }
    private void FixedUpdate(){
        GetComponent<Rigidbody2D>().velocity *= 0.1f / (weakness + 1) + 0.88f;
        if (weapon.GetComponent<WeaponBehavior>().melee && swapWeaponAnim == 0){
            slashEase = Mathf.Max(slashEase, (rotation.eulerAngles * 100000 - weapon.transform.rotation.eulerAngles * 100000).magnitude) * 0.9f;
            slashObject.GetComponent<TrailRenderer>().startColor = new Color(1, 1, 1, slashEase * slashCooldown * 2);
            weapon.GetComponent<WeaponBehavior>().dIndex = slashEase * (1 / (weakness + 1) + 0.25f) * 10;
        }
        else{
            slashEase = 0;
        }
    }
    private void Attack(){
        if (Input.GetKeyDown(KeyCode.Mouse0)){
            weapon.GetComponent<WeaponBehavior>().Attack();
            if (weapon.GetComponent<WeaponBehavior>().melee){
                slashCooldown = 1;
                attackEase = 1;
                if (spriteRenderer.flipX) {
                    attackFlip = -1;
                }
                else {
                    attackFlip = 1;
                }
            }
            else {
                recoilEase = 1;
                recoilAnim = recoilAngle;
            }
        }
    }
    private void Anim()
    {
        if (spriteRenderer.flipX)
        {
            leftArm.transform.localPosition = new Vector3(-0.17f, 0.05f);
            leftArm.transform.rotation = Quaternion.identity;

            rightArm.transform.position = weapon.transform.position;
            Vector3 raPos = new Vector3(0.17f, 0.25f);
            Vector3 ranPos = weapon.transform.position - transform.position - raPos;
            float angle = Mathf.Atan2(ranPos.x, ranPos.y) * Mathf.Rad2Deg - 180;
            rightArm.transform.rotation = Quaternion.AngleAxis(-angle, Vector3.forward);

            leftLeg.transform.localPosition = new Vector3(-0.1f, 0);
            rightLeg.transform.localPosition = new Vector3(0.02f, 0);

            leftLeg.GetComponent<SpriteRenderer>().flipX = true;
            rightLeg.GetComponent<SpriteRenderer>().flipX = true;

            leftLeg.transform.rotation = Quaternion.AngleAxis(Mathf.Sin(legAnim) * 30, Vector3.forward);
            rightLeg.transform.rotation = Quaternion.AngleAxis(Mathf.Sin(-legAnim) * 30, Vector3.forward);

            leftArm.GetComponent<SpriteRenderer>().sortingOrder = -1;
            rightArm.GetComponent<SpriteRenderer>().sortingOrder = 1;
            leftLeg.GetComponent<SpriteRenderer>().sortingOrder = -2;
            rightLeg.GetComponent<SpriteRenderer>().sortingOrder = 2;
        }
        else
        {
            rightArm.transform.localPosition = new Vector3(0.17f, 0.05f);
            rightArm.transform.rotation = Quaternion.identity;

            leftArm.transform.position = weapon.transform.position;
            Vector3 laPos = new Vector3(-0.17f, 0.25f);
            Vector3 lanPos = weapon.transform.position - transform.position - laPos;
            float angle = Mathf.Atan2(lanPos.x, lanPos.y) * Mathf.Rad2Deg - 180;
            leftArm.transform.rotation = Quaternion.AngleAxis(-angle, Vector3.forward);

            leftLeg.transform.localPosition = new Vector3(-0.02f, 0);
            rightLeg.transform.localPosition = new Vector3(0.1f, 0);

            leftLeg.GetComponent<SpriteRenderer>().flipX = false;
            rightLeg.GetComponent<SpriteRenderer>().flipX = false;

            leftLeg.transform.rotation = Quaternion.AngleAxis(Mathf.Sin(-legAnim) * 30, Vector3.forward);
            rightLeg.transform.rotation = Quaternion.AngleAxis(Mathf.Sin(legAnim) * 30, Vector3.forward);

            leftArm.GetComponent<SpriteRenderer>().sortingOrder = 1;
            rightArm.GetComponent<SpriteRenderer>().sortingOrder = -1;
            leftLeg.GetComponent<SpriteRenderer>().sortingOrder = 2;
            rightLeg.GetComponent<SpriteRenderer>().sortingOrder = -2;
        }
    }
    private void Move() {
        //Get angle of mouse pointer with lerp
        Vector3 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + swapWeaponAnim * 360 + (1 - 4 * (attackEase - 0.5f) * (attackEase - 0.5f)) * (attackEase - 0.5f) * 2 * slashAngle * attackFlip - 45;
        rotation = Quaternion.Slerp(rotation, Quaternion.AngleAxis(angle, Vector3.forward), rotateSpeed * Time.deltaTime);

        attackEase -= slashSpeed * Time.deltaTime;
        if (attackEase < 0)
            attackEase = 0;

        //Flip the player to face the mouse pointer
        if(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg > -90 && Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg < 90){
            spriteRenderer.flipX = false;

            //Rotate the held weapon to face the mouse pointer
            weapon.transform.rotation = Quaternion.Slerp(rotation, Quaternion.Slerp(Quaternion.Euler(0, 0, angle - recoilAnim), Quaternion.Euler(0, 0, angle + recoilAnim), recoilEase), recoilEase);
            //if(swapWeaponAnim == 0) weapon.GetComponent<SpriteRenderer>().flipY = false;
        }
        else{
            spriteRenderer.flipX = true;

            //Rotate the held weapon to face the mouse pointer
            weapon.transform.rotation = Quaternion.Slerp(rotation, Quaternion.Slerp(Quaternion.Euler(0, 0, angle + recoilAnim), Quaternion.Euler(0, 0, angle - recoilAnim), recoilEase), recoilEase);
            //if (swapWeaponAnim == 0) weapon.GetComponent<SpriteRenderer>().flipY = true;
        }
        swapWeaponAnim -= Time.deltaTime * 5;
        if (swapWeaponAnim < 0)
            swapWeaponAnim = 0;
        else if (swapWeaponAnim < 0.5f){
            weapon = newWeapon;
        }

        weapon.transform.position = transform.position + dir.normalized * (0.5f + ((swapWeaponAnim - 0.5f) * (swapWeaponAnim - 0.5f) - 0.25f) * 2) * 0.5f + new Vector3(0, 0.1f, 0);
        recoilEase -= recoilSpeed * Time.deltaTime;

        //Movement
        input = Vector2.zero;
        if (Input.GetKey(KeyCode.W)){
            input.y = 1;
        }
        if (Input.GetKey(KeyCode.S)){
            input.y = -1;
        }
        if (Input.GetKey(KeyCode.D)){
            input.x = 1;
        }
        if (Input.GetKey(KeyCode.A)){
            input.x = -1;
        }
        if (dashTimer > dashCooldown && Input.GetKeyDown(KeyCode.Space)) {
            dashTimer = 0;
            GetComponent<Rigidbody2D>().velocity = input.normalized * dashSpeed;
        }
        GetComponent<Rigidbody2D>().velocity += input.normalized * speed * Time.deltaTime;
        legAnim += input.magnitude * speed * Time.deltaTime;
        legAnim %= 360 * Mathf.Deg2Rad;
        if(input == Vector2.zero)
        {
            legAnim *= 0.9f;
        }
        dashTimer += Time.deltaTime;
    }
    void Pickup()
    {
        if (Array.IndexOf(inventory, -1) == -1) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            int i = 0;
            foreach(GameObject item in gm.droppedItems)
            {
                if ((item.transform.position - transform.position).magnitude < 1f)
                {
                    inventory[Array.IndexOf(inventory, -1)] = gm.droppedItemsID[i];
                    gm.droppedItems.Remove(item);
                    Destroy(item);
                    hotbarui.render();
                    return;
                }
                i++;
            }
        }
    }
}
