using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public Vector3 velocity;

    // Update is called once per frame
    void Update()
    {
        transform.position += velocity * Time.deltaTime;
        transform.rotation = Quaternion.identity;
    }
}
