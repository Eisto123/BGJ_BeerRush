using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float clampXOffset = 1f;
    public float playerYOffest = 2;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        float x = player.transform.position.x;
        float y = player.transform.position.y + playerYOffest;
         
        x = Mathf.Clamp(x, -clampXOffset, clampXOffset);
        if (y < -1) y = -1;

        transform.position = new Vector3(x, y, transform.position.z);

    }
}
