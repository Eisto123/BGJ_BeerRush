using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customers : MonoBehaviour
{
    public int customerNum;
    public int beerNum;
    private bool beerSolved = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetNumber(int customer, int beer)
    {
        customerNum = customer;
        beerNum = beer;
    }

    private void OTriggerEnter2D(Collider2D collision)
    {
        // Beer has been handed over
        if (beerSolved)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            // if the player get close, hand the beer
            beerSolved = true;
        }
    }
}
