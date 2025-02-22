using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CowboyFall : MonoBehaviour
{
    public Transform shadow;  // Reference to the shadow GameObject
    private SpriteRenderer spriteRenderer;
    public float fallSpeed = 5f;  // Speed at which the cowboy falls
    public float groundZ = -7f;  // Y position where the cowboy lands
    public float minShadowScale = 1f;  // Minimum size of the shadow


    private PolygonCollider2D polygonCollider2D;
    private Vector3 initialShadowScale;
    private float startZ;
    private bool playerIsNearBy;
    private GameObject player;
    private bool isFalling = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (shadow == null)
        {
            Debug.LogError("Shadow reference not set!");
            return;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        spriteRenderer.enabled = false;
        polygonCollider2D.enabled = false;
        initialShadowScale = shadow.localScale; // Save the original shadow size
        startZ = transform.localPosition.z; // Save initial Z position
    }

    void Update()
    {
        
        if (Mathf.Abs(transform.position.y - player.transform.position.y) < 5f)
        {
            playerIsNearBy = true;
        }
        if(playerIsNearBy){
            if(!isFalling){
                isFalling = true;
                Debug.Log("Cowboy is falling");
                AudioManager.Instance.PlayEnviroment(3);
            }
            if (transform.position.z > groundZ)
        {
            // Move cowboy down
            transform.position += Vector3.back * fallSpeed * Time.deltaTime;

            // Calculate shadow scaling based on height
            float heightRatio = Mathf.InverseLerp(startZ, groundZ, transform.position.z);
            float newShadowScale = Mathf.Lerp(initialShadowScale.x,minShadowScale, heightRatio);
            shadow.GetComponent<SpriteRenderer>().color = new Color(heightRatio, heightRatio, heightRatio, 1);
            // Apply new shadow scale
            shadow.localScale = new Vector3(newShadowScale, newShadowScale, 1);
        }
        else
        {
            // Ensure cowboy lands exactly at the ground position
            spriteRenderer.enabled = true;
            polygonCollider2D.enabled = true;
            transform.position = new Vector3(transform.position.x, transform.position.y, groundZ);
        }
        }
        
    }
}
