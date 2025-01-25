using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 5f;
    [SerializeField] private int dmg;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void SetDamage(int damage)
    {
        dmg = damage;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger)
            return;
        if (collision.CompareTag("Enemy"))
        {
            Enemy mutant = collision.GetComponent<Enemy>();
            if (mutant != null)
            {
                mutant.TakeDamage(dmg);
            }
            Destroy(gameObject);
        }

        else if (collision.CompareTag("BlockTerrain"))
        {
            Destroy(gameObject);
        }
    }

}
