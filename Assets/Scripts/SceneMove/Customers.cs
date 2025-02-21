using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class CustomerImage
{
    public List<Sprite> images;
}

public class Customers : MonoBehaviour
{
    public List<GameObject> customerList;
    public List<CustomerImage> customerSprite;
    public List<GameObject> chairList;
    public Animator beerBubble;

    public int customerNum;
    public int beerNum;
    public float waitingTime;

    private bool beerSolved = false;
    private float timer;
    private bool startTimer = false;

    // Get score / Lose score
    private Player player;
    private Collider2D trigger;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        trigger = GetComponent<Collider2D>();
        beerBubble.gameObject.SetActive(false);

        for (int i = 0; i < customerList.Count; i++)
        {
            customerList[i].SetActive(false);
            chairList[i].SetActive(false);
        }

        // need replace!
        //SetNumber(2,3,3);
        SetNumber(customerNum,beerNum,waitingTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (!startTimer) return;

        timer -= Time.deltaTime;

        // Time's out, customer unsatisfied
        if (timer < 0)
        {
            startTimer = false;

            // customer unhappy, lose score
            Debug.Log("lose score");
            trigger.enabled = false;
            beerBubble.gameObject.SetActive(false);
        }
    }

    public void SetNumber(int customer, int beer, float wait)
    {
        customerNum = customer;
        beerNum = beer;
        waitingTime = wait;
        timer = waitingTime;
        startTimer = true;

        // Show #customerNum customers&chairs
        for (int i = 0; i < customerNum; i++)
        {
            customerList[i].SetActive(true);
            chairList[i].SetActive(true);

            int spriteIndex = UnityEngine.Random.Range(0, customerSprite[i].images.Count);
            customerList[i].GetComponent<SpriteRenderer>().sprite = customerSprite[i].images[spriteIndex];
        }

        
        beerBubble.gameObject.SetActive(true);

        // Player get #beerNum new beer
        Debug.Log("get "+beerNum+" beer");

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Beer has been handed over
        if (beerSolved || !startTimer)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            // if the player get close, hand the beer
            beerSolved = true;
            startTimer = false;

            beerBubble.gameObject.SetActive(false);
            
            // customer happy, get score
            Debug.Log("get score");
            trigger.enabled = false;
        }
    }
}
