using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowboyB : Cowboy
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 5f;
    public GameObject shootIndicator;
    private Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();
        animator.SetBool("isWalking", true);
    }

    protected override IEnumerator Shoot()
    {
        isShooting = true;
        animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(Random.Range(0.5f, 2.5f));
        animator.SetTrigger("pullGun");
        yield return new WaitForSeconds(0.3f);
        shootIndicator.SetActive(true);
        yield return new WaitForSeconds(1f);
        animator.SetTrigger("shoot");
        Debug.Log("CowboyB fires a bullet!");
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().velocity = Vector2.right * bulletSpeed;

        shootIndicator.SetActive(false);
        yield return null;
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            
            Debug.Log("CowboyB is hit by a bullet!");
            shootIndicator.SetActive(false);
            animator.SetTrigger("isDead");
            Destroy(collision.gameObject);
            Destroy(this);
        }
    }
}
