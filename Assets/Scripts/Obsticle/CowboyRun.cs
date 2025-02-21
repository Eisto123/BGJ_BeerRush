using UnityEngine;

public class CowboyRun : MonoBehaviour
{
    public float runSpeed = 5f; // Speed of the cowboy
    public float leftEdge = -10f; // Left boundary
    public float rightEdge = 10f; // Right boundary

    private int direction; // 1 for right, -1 for left
    private GameObject player;
    private bool playerIsNearBy;

    void Start()
    {
        
        // Randomly decide the start position
        if (Random.value > 0.5f)
        {
            transform.position = new Vector3(leftEdge, transform.position.y, 0);
            direction = 1; // Move right
        }
        else
        {
            transform.position = new Vector3(rightEdge, transform.position.y, 0);
            direction = -1; // Move left
            transform.localScale = new Vector3(transform.localScale.x*-1,transform.localScale.y,transform.localScale.z); // Flip sprite for leftward movement
        }
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < 3f)
        {
            playerIsNearBy = true;
        }
        else
        {
            playerIsNearBy = false;
        }

        if(playerIsNearBy)
        {
            transform.position += Vector3.right * direction * runSpeed * Time.deltaTime;
        }

        // Destroy cowboy if it exits the screen
        if (transform.position.x > rightEdge + 2 || transform.position.x < leftEdge - 2)
        {
            Destroy(gameObject);
        }
    }
}
