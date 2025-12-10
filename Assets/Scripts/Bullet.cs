using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 2f;
    public int damage = 20;
    public GameObject bloodEffect;   // <-- ADD THIS

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Spawn blood if we hit an enemy
        EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            // Spawn blood at hit point
            ContactPoint contact = collision.contacts[0];
            Instantiate(bloodEffect, contact.point, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}

