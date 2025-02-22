using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Cowboy : MonoBehaviour
{
    protected float moveSpeed = 0.5f;
    protected float minY = -0.5f;
    protected float maxY = 0.5f;
    protected bool isShooting = false;
    protected bool playerIsNearBy = false;
    protected GameObject player;
    protected Vector3 targetPosition;
    private float initialY;
    private bool isStarted = false;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(StartProcess());
        
    }
    private IEnumerator StartProcess()
    {
        yield return new WaitForSeconds(2f);
        initialY = transform.position.y+transform.lossyScale.y+0.3f;
        PickNewPosition();
        isStarted = true;
        Debug.Log(initialY);
    }

    protected virtual void Update()
    {
        CheckPlayerIsNearBy();
        if(!isShooting&&isStarted){
            Move();
            if(playerIsNearBy){
                CheckForShooting();
            }
            
        }
        
    }

    protected virtual void CheckPlayerIsNearBy(){
        if(Mathf.Abs(transform.position.y - player.transform.position.y) < 5f){
            playerIsNearBy = true;
        }else{
            playerIsNearBy = false;
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
        float newY = initialY + UnityEngine.Random.Range(minY, maxY);
        targetPosition = new Vector3(transform.position.x, newY, transform.position.z);
    }

    protected virtual void CheckForShooting()
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
