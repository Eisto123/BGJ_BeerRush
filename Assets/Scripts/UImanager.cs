using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UImanager : MonoBehaviour
{
    
    public List<Sprite> numbers;
    public GameObject digitPrefab; // A UI Image prefab for each digit
    public int spacing = 20; // Distance between digits
    public Vector2 offset = new Vector2(0, 0); // Offset from the center of the screen

    private List<GameObject> digitObjects = new List<GameObject>();

    void Start()
    {
        StartCoroutine(scoretest());
    } 

    IEnumerator scoretest(){
        UpdateScore(3);
        yield return new WaitForSeconds(2f);
        UpdateScore(13);
        yield return new WaitForSeconds(2f);
        UpdateScore(123);
        yield return new WaitForSeconds(2f);
        UpdateScore(1234);
    }
    public void UpdateScore(int score)
    {
        // Convert score to string to process each digit
        string scoreStr = score.ToString();

        // Remove excess digits if necessary
        while (digitObjects.Count > scoreStr.Length)
        {
            Destroy(digitObjects[digitObjects.Count - 1]);
            digitObjects.RemoveAt(digitObjects.Count - 1);
        }

        // Create/update UI elements for each digit
        for (int i = 0; i < scoreStr.Length; i++)
        {
            int digit = scoreStr[i] - '0'; // Convert char to int

            if (i >= digitObjects.Count)
            {
                // Create new digit object if needed
                GameObject newDigit = Instantiate(digitPrefab, transform);
                digitObjects.Add(newDigit);
            }

            // Update sprite
            digitObjects[i].GetComponent<Image>().sprite = numbers[digit];

            // Adjust position (left to right)
            digitObjects[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(i * spacing + offset.x, offset.y);
        }
    }
}
