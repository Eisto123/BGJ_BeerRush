using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowboyD : Cowboy
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

    protected override void CheckPlayerIsNearBy()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < 8f)
        {
            playerIsNearBy = true;
        }
        else
        {
            playerIsNearBy = false;
        }
    }

    protected override void CheckForShooting()
    {
        CowboyC[] cowboys = FindObjectsOfType<CowboyC>();
        foreach (var other in cowboys)
        {
            if (other != this && Mathf.Abs(transform.position.y - other.transform.position.y) < 4.01f&& Mathf.Abs(transform.position.y - other.transform.position.y) > 3.99f)
            {
                StartCoroutine(Shoot());
            }
        }
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
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        float angle = -30f * Mathf.Deg2Rad; // Convert degrees to radians
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)); // Compute direction

        bullet.transform.eulerAngles = new Vector3(0, 0, -30); 
        bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;

        shootIndicator.SetActive(false);
        yield return null;
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            
            Debug.Log("CowboyD is hit by a bullet!");
            shootIndicator.SetActive(false);
            animator.SetTrigger("isDead");
            Destroy(collision.gameObject);
            Destroy(this);
        }
    }
}
