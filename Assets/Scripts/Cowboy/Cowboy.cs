using System.Collections;
using UnityEngine;

public abstract class Cowboy : MonoBehaviour
{
    protected float moveSpeed = 0.5f;
    protected float minY = -1f;
    protected float maxY = 1f;
    protected bool isShooting = false;

    protected Vector3 targetPosition;

    protected virtual void Start()
    {
        PickNewPosition();
    }

    protected virtual void Update()
    {
        if(!isShooting){
            Move();
            CheckForShooting();
        }
        
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            PickNewPosition();
        }
    }

    private void PickNewPosition()
    {
        float newY = Random.Range(minY, maxY);
        targetPosition = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void CheckForShooting()
    {
        Cowboy[] cowboys = FindObjectsOfType<Cowboy>();
        foreach (var other in cowboys)
        {
            if (other != this && Mathf.Abs(transform.position.y - other.transform.position.y) < 0.01f)
            {
                StartCoroutine(Shoot());
            }
        }
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
    }

    protected virtual IEnumerator Shoot()
    {
        isShooting = true;
        Debug.Log("Cowboy is shooting!");
        yield return null;
    }
}
