using System;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [Range(0f,1f)]
    public float moveSpeed;
    public SpriteRenderer pixelSprite;
    [Tooltip("How many pixel in y axis per camera length, 4 in default")]
    public int numOfPixelY;
    private float camLength; // camera length
    private float camHeight; // camera height
    
    private Vector2 startPos;
    private GameObject mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        camLength = pixelSprite.bounds.size.y * numOfPixelY;
        mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0];
        camHeight = mainCamera.GetComponent<Camera>().orthographicSize*2;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = mainCamera.transform.position.y * moveSpeed;
        transform.position = new Vector3(transform.position.x, startPos.y + distance, transform.position.z);

        float temp = mainCamera.transform.position.y * (1-moveSpeed);
        if (temp > startPos.y+camLength)
        {
            Scroll();
        }
    }

    [Header("Obstacle Generation")]
    public List<Obstacle> obstacles;
    public float obstacleProb;
    public Vector2 offset;
    public int numOfPixelX;

    private void Scroll()
    {
        startPos.y+=2*camLength;

        // Clear current obstacles
        foreach (Transform child in transform)
        {
            if (child.gameObject.name != "floor")
            {
                Destroy(child.gameObject);
            }
        }

        // whether there is obstacle;
        bool ifObstacle = UnityEngine.Random.Range(0f,1f) <= obstacleProb;

        if (!ifObstacle) return;

        // put obstacles
        // choose one type of the obstacles
        int index = UnityEngine.Random.Range(0, obstacles.Count);
        Vector2 obstacleRadius = obstacles[index].radius;

        // decide the position
        float xmin = -pixelSprite.bounds.size.x * numOfPixelX/2 + offset.x + obstacleRadius.x;
        float xmax = pixelSprite.bounds.size.x * numOfPixelX/2 - offset.x - obstacleRadius.x;
        float ymin = -pixelSprite.bounds.size.y/2 + offset.y + obstacleRadius.y;
        float ymax = pixelSprite.bounds.size.y/2 - offset.y - obstacleRadius.y;

        Debug.Log(xmin +" "+ xmax+" "+ ymin +" "+ ymax);

        float x = xmin <= xmax ? UnityEngine.Random.Range(xmin, xmax) : 0;
        float y = ymin <= ymax ? UnityEngine.Random.Range(ymin, ymax) : 0;

        Debug.Log(x+" "+y);

        GameObject newBucket = Instantiate(obstacles[index].obstacle, this.transform);
        newBucket.transform.localPosition = new Vector3(x,y);
        
    }
}

[Serializable]
public class Obstacle
{
    public GameObject obstacle;
    public Vector2 radius;
}
