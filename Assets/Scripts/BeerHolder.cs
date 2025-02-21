using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeerHolder : MonoBehaviour
{
    public GameObject[] beers = new GameObject[6];
    private bool beerIsFull;
    public void AddBeer()
    {
        for (int i = 0; i < beers.Length; i++)
        {
            if (beers[i].activeSelf == false)
            {
                beers[i].SetActive(true);
                break;
            }
            if(i == beers.Length - 1)
            {
                beerIsFull = true;
            }
        }
    }
    public void RemoveBeer()
    {
        for (int i = beers.Length - 1; i >= 0; i--)
        {
            if (beers[i].activeSelf == true)
            {
                Debug.Log("Removing beer");
                beers[i].SetActive(false);
                beerIsFull = false;
                break;
            }
            if (i == 0)
            {
                Debug.Log("No beer to remove");
            }

        }
    }

    public void BreakAllBeers()
    {
        for (int i = 0; i < beers.Length; i++)
        {
            if (beers[i].activeSelf == true)
            {
                beers[i].GetComponent<Animator>().SetTrigger("Break");
                
            }
        }
    }
}
