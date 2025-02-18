using UnityEngine;

public class BulletSpawn : MonoBehaviour
{
    public GameObject bulletPrefab;  // Assign the Bullet prefab
    public Transform firePoint;      // Where the bullet spawns
    public float fireRate = 0.1f;
    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            SpawnBullet();
        }
    }

    void SpawnBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.damage = 10f;  // Set bullet damage here
    }
}
