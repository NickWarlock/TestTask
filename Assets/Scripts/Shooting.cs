using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    private Vector3 faceLeft = new Vector3(-1f, 1f, 1f);
    private Vector3 faceRight = new Vector3(1f, 1f, 1f);

    [SerializeField] private Transform sprite;

    [SerializeField] private float range = 10f;
    private Transform targetEnemy;
    [SerializeField] private Transform gun;
    [SerializeField] private Transform gunPivot;
    [SerializeField] private Transform gunBarrel;
    private float barreloffset = 0;
    public float bulletSpeed;
    [SerializeField] GameObject bulletPrefab;
    private Vector3 gunRest;
    [SerializeField] private Vector3 aimOffset;
    private bool isShooting = false;
    private float lastShot;

    [SerializeField] private float fireRate;
    [SerializeField] private int dmg;
    [SerializeField] private ItemAmmo ammoType;

    private void Awake()
    {
        gunRest = gun.transform.localPosition;
    }

    public void onShootStart()
    {
        isShooting = true;
    }

    public void onShootStop()
    {
        isShooting = false;
    }

    Transform FindNearestEnemy()
    {
        float shortestDistance = range;
        Transform closestEnemy = null;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }
        return closestEnemy;
    }

    public void Shoot()
    {
        GameController.Instance.gameObject.GetComponent<Inventory>().ConsumeAmmo(ammoType);
        GameObject bullet = Instantiate(bulletPrefab, gunBarrel.position, gunBarrel.localRotation * Quaternion.Euler(0f, 0f, barreloffset));
        bullet.GetComponent<Bullet>().SetDamage(dmg);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.velocity = bulletRb.transform.localToWorldMatrix.MultiplyVector(gunBarrel.right * bulletSpeed);

        }
    }

    private bool HasAmmo()
    {
        return GameController.Instance.gameObject.GetComponent<Inventory>().HasAmmo(ammoType);
    }

    private void Update()
    {
        targetEnemy = FindNearestEnemy();

        if (targetEnemy != null)
        {
            gun.position = targetEnemy.position + aimOffset;
            if ((gun.position - this.transform.position).x < 0)
            {
                sprite.localScale = faceLeft;
                barreloffset = 186.888f;
            }
            else
            {
                sprite.localScale = faceRight;
                barreloffset = 6.888f;
            }
        }
        else
        {
            gun.localPosition = gunRest;
            sprite.localScale = faceRight;
            barreloffset = 6.888f;
        }

        if (isShooting)
        {
            if (Time.time - lastShot >= 1f / fireRate)
            {
                if (HasAmmo())
                {
                    Shoot();
                    lastShot = Time.time;
                }
            }
        }
    }
}
