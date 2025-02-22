using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowboyC : Cowboy
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

    protected override void CheckForShooting()
    {
        CowboyD[] cowboys = FindObjectsOfType<CowboyD>();
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

        //Audio
        AudioManager.Instance.PlayCowboy(0);

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        bullet.GetComponent<SpriteRenderer>().flipX = true;
        bullet.transform.eulerAngles = new Vector3(0, 0, -30);

        float angle = 150f * Mathf.Deg2Rad; // Convert degrees to radians
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;

        shootIndicator.SetActive(false);
        yield return null;
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("CowboyC is hit by a bullet!");
            
            //Audio, beingshot
            AudioManager.Instance.PlayCowboy(2);

            shootIndicator.SetActive(false);
            animator.SetTrigger("isDead");
            Destroy(collision.gameObject);
            Destroy(this);
        }
    }
}
