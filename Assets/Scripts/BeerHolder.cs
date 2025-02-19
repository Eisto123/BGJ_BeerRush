using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeerHolder : MonoBehaviour
{
    public GameObject[] beers = new GameObject[6];

    // Start is called before the first frame update

    void Awake()
    {
        
    }
    public void AddBeer()
    {
        for (int i = 0; i < beers.Length; i++)
        {
            if (beers[i].activeSelf == false)
            {
                beers[i].SetActive(true);
                break;
            }
        }
    }
    public void RemoveBeer()
    {
        for (int i = beers.Length - 1; i >= 0; i--)
        {
            if (beers[i].activeSelf == true)
            {
                beers[i].SetActive(false);
                break;
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
