using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.PlasticSCM.Editor.WebApi;


[Serializable]
public class Obstacle
{
    public GameObject obstacle;
    public Vector2 radius;
}

[Serializable]
public class ShootEvent
{
    public int type; // 0: straight;  1: diagonal
    public int direction; // 0: left, 1: right
    public int cowboyPos; // local pixel position, left: 0-1; right: 2-3
}

public class GenerationManager : MonoBehaviour
{
    // 0: shooting
    // 1: empty
    // 2: customer
    // 3: obstacle
    // 4: ornament
    public List<int> startEvent;
    private List<int> prevEvent;
    private List<int> currEvent;
    private List<List<int>> currEventChoice;
    private int choiceNum = 4;

    [Tooltip("How many floor pixels per 'floor' prefab")]
    public int numOfPixelX;

    [Header("Shoot")]
    [Tooltip("0: straightL; 1: straightR; 2: diagonalL; 3: diagonalR")]
    public List<GameObject> shootPrefab;
    public LinkedList<ShootEvent> shootEvents;
    [Range(0f,1f)]
    public float shootingProb;

    [Header("Customer")]
    public Customers customerTable;


    [Header("Obstacle")]
    public List<Obstacle> obstacles;
    //public float obstacleProb;

    [Header("NaughtyCowboy")]
    [Range(0f,1f)]
    public float naughtyProb;
    public GameObject dropCowboy;
    public GameObject runCowboy;
    [Range(0f,3f)]
    public float dropOffset;

    [Header("Ornament")]
    [Range(0f,1f)]
    public float ornamentProb;
    public List<GameObject> ornament;

    void Start()
    {
        prevEvent = new List<int>();
        currEvent = new List<int>();
        currEventChoice = new List<List<int>>();
        shootEvents = new LinkedList<ShootEvent>();

        // what is the event on the top? 
        // [1,2,1,2] -> [empty, cus, empty, cus]

        for (int i = 0; i < startEvent.Count; i++)
        {
            prevEvent.Add(startEvent[i]);
            currEvent.Add(startEvent[i]);

            // First shoot event must be staright
            if (startEvent[i] == 0)
            {
                ShootEvent firstShootL = new ShootEvent();
                firstShootL.type = 0;
                firstShootL.direction = 1;
                firstShootL.cowboyPos = 0;

                ShootEvent firstShootR = new ShootEvent();
                firstShootR.type = 0;
                firstShootR.direction = 1;
                firstShootR.cowboyPos = 0;

                shootEvents.AddLast(firstShootL);
                shootEvents.AddLast(firstShootR);
            }
        }

        // currEventChoice = [[1,2,3],[1,2,3],[1,2,3],[1,2,3]]
        for (int i = 0; i < numOfPixelX; i++)
        {
            currEventChoice.Add(new List<int>());
            for (int j = 1; j < choiceNum; j++)
            {
                currEventChoice[i].Add(j);
            }
        }   
    }

    public void SceneGeneration(Transform floor, float spriteSize)
    {
        // preEvent = currEvent;
        prevEvent.Clear();
        for (int i = 0; i < currEvent.Count; i++)
        {
            prevEvent.Add(currEvent[i]);
        }
        
        currEvent.Clear();
        for (int i = 0; i < numOfPixelX; i++)
        {
            currEvent.Add(-1);
        }

        // currEventChoice = [[1,2,3],[1,2,3],[1,2,3],[1,2,3]]
        for (int i = 0; i < numOfPixelX; i++)
        {
            currEventChoice[i].Clear();
            for (int j = 1; j < choiceNum; j++)
            {
                currEventChoice[i].Add(j);
            }
        }

        if (ShootGeneration(floor, spriteSize))
        {
            return;
        }

        PrevCustomerDetect(floor, spriteSize);
        ObjectGenerate(floor, spriteSize);

        NaughtyCowboyGenerate(floor, spriteSize);

        OrnamentGenerate(floor, spriteSize);
        
    }

    private bool ShootGeneration(Transform floor, float spriteSize)
    {
        // whether last one is a shooting event
        if (prevEvent.Contains(0))
        {
            //Debug.Log("last is shoot");
            ShootEvent prevShoot = shootEvents.Last.Value;

            // diagonal & only right cowboy, generate the upleft one
            if (prevShoot.type == 1 && prevShoot.direction == 1)
            {
                ShootEvent currShoot = new ShootEvent();
                currShoot.type = 1;
                currShoot.direction = 0;
                currShoot.cowboyPos = prevShoot.cowboyPos-3;
                shootEvents.AddLast(currShoot);

                GameObject cowboy = Instantiate(shootPrefab[3],floor);
                cowboy.transform.localPosition = new Vector2((2*currShoot.cowboyPos-3)*spriteSize/2, 0);
                //cowboy.transform.position = new Vector2((2*currShoot.cowboyPos-3)*spriteSize/2, cowboy.transform.position.y);

                currEvent[currShoot.cowboyPos] = 0;

                // Update the valid choices
                currEventChoice[currShoot.cowboyPos].Clear();
                currEventChoice[currShoot.cowboyPos+1].Clear();

                return false;
            }
            // else, this floor no shooting
            else
            {
                //currEvent.Clear();
                //currEvent.Add(1);

                return false;
            }
            
        }

        // no, decide whether to shoot
        if (UnityEngine.Random.Range(0f,1f) > shootingProb) return false; // no shoot

        //Debug.Log("shoot");

        // generate shoot
        ShootEvent shootR = new ShootEvent();
        shootR.type = UnityEngine.Random.Range(0,2);
        shootR.direction = 1;
        shootR.cowboyPos = UnityEngine.Random.Range(2,4);

        currEvent[shootR.cowboyPos] = 0;

        if (shootR.type == 0) // staright
        {
            ShootEvent shootL = new ShootEvent();
            shootL.type = 0;
            shootL.direction = 0;
            shootL.cowboyPos = shootR.cowboyPos-2;
            
            shootEvents.AddLast(shootR);
            shootEvents.AddLast(shootL);
            
            GameObject cowboyR = Instantiate(shootPrefab[1],floor);
            cowboyR.transform.localPosition = new Vector2((2*shootR.cowboyPos-3)*spriteSize/2, 0);
            //cowboyR.transform.position = new Vector2((2*shootR.cowboyPos-3)*spriteSize/2, cowboyR.transform.position.y);
            
            GameObject cowboyL = Instantiate(shootPrefab[0],floor);
            cowboyL.transform.localPosition = new Vector2((2*shootL.cowboyPos-3)*spriteSize/2, 0);
            //cowboyL.transform.position = new Vector2((2*shootL.cowboyPos-3)*spriteSize/2, cowboyL.transform.position.y);

            currEvent[shootL.cowboyPos] = 0;

            return true;
        }
        else
        {
            // last floor[3] has customers, will overlap the cowboy
            if (prevEvent[3] == 2)
            {
                return false;
            }

            shootR.cowboyPos = 3;
            shootEvents.AddLast(shootR);

            GameObject cowboyR = Instantiate(shootPrefab[2],floor);
            cowboyR.transform.localPosition = new Vector2((2*shootR.cowboyPos-3)*spriteSize/2, 0);
            //cowboyR.transform.position = new Vector2((2*shootR.cowboyPos-3)*spriteSize/2, cowboyR.transform.position.y);

            // Update the valid choices
            currEventChoice[shootR.cowboyPos].Clear();
            currEventChoice[shootR.cowboyPos-1].Clear();
        }

        return false;
    }

    
    private void PrevCustomerDetect(Transform floor, float spriteSize)
    {
        List<int> tablePos = new List<int>();

        // [[1,2,3], [1,3], [1,2,3], [1,3]] = currEventChoice
        // [   1,      2,      1,      2]   = prevEvent
        for (int i = 0; i < prevEvent.Count; i++)
        {
            if (prevEvent[i] == 0) continue;
            if (prevEvent[i] == 2) // customer
            {
                currEventChoice[i].Remove(2);
            }
        }

        string formatted = "[" + string.Join(", ", currEventChoice.ConvertAll(
            sublist => "[" + string.Join(", ", sublist) + "]"
        )) + "]";

        Debug.Log(formatted);
    }

    private void ObjectGenerate(Transform floor, float spriteSize)
    {
        int currTableNum = 0;
        int spawnedFloor = 0;
        int emptyFloor = 0;


        for (int i = 0; i < currEvent.Count; i++)
        {

            if (spawnedFloor >= 2) 
            {
                currEvent[i] = 1;
                continue;
            }

            // no choice, leave empty
            if (currEventChoice[i].Count == 0)
            {
                currEvent[i] = currEvent[i] == 0? 0 : 1;
                continue;
            }

            // cannot generate empty floor if there are already two empty road
            if (emptyFloor >= 2)
                 currEventChoice[i].Remove(1);

            int index = UnityEngine.Random.Range(0, currEventChoice[i].Count);
            int type = currEventChoice[i][index];

            if (type == 1) // Empty Road
            {
                currEvent[i] = type;
                emptyFloor++;
                continue;
            }
            else
            {
                // at least one table
                if (spawnedFloor == 1 && currTableNum == 0)
                {
                    // put table
                    SpawnObject(floor, spriteSize, i, 2);
                    currEvent[i] = 2;
                    currTableNum++;
                }
                else
                {
                    // put random type
                    SpawnObject(floor, spriteSize, i, type);
                    currEvent[i] = type;
                    if (type == 2) currTableNum++;
                    //currTableNum = type == 2? currTableNum+1 : currTableNum;
                }
                spawnedFloor++;
            }
        }
    }

    private void SpawnObject(Transform floor, float spriteSize, int pos, int type)
    {
        if (type == 2) // customer
        {
            Customers newTable = Instantiate(customerTable, floor);
            newTable.transform.localPosition = new Vector2((2*pos-3)*spriteSize/2, 0);
        }
        else if (type == 3) // obstacle
        {
            int obstacleType = UnityEngine.Random.Range(0, obstacles.Count);
            Vector2 obstacleRadius = obstacles[obstacleType].radius;

            float xmin = (pos-2)*spriteSize + obstacleRadius.x;
            float xmax = (pos-1)*spriteSize - obstacleRadius.x;
            float x = xmin <= xmax ? UnityEngine.Random.Range(xmin, xmax) : (2*pos-3)*spriteSize/2;

            GameObject newObstacle = Instantiate(obstacles[obstacleType].obstacle, floor);
            newObstacle.transform.localPosition = new Vector2(x, 0);
        }
    }

    private void NaughtyCowboyGenerate(Transform floor, float spriteSize)
    {
        bool ifNaughty = UnityEngine.Random.Range(0f,1f) <= naughtyProb;

        if (!ifNaughty) return;

        //int naughtyType = UnityEngine.Random.Range(0,2); // 0: drop; 1: run
        int naughtyType = UnityEngine.Random.Range(1,2); // 0: drop; 1: run

        if (naughtyType == 0)
        {
            float xmin = -spriteSize * numOfPixelX/2 + dropOffset;
            float xmax = spriteSize * numOfPixelX/2 - dropOffset;
            float x = xmin <= xmax ? UnityEngine.Random.Range(xmin, xmax) : 0;

            GameObject newDrop = Instantiate(dropCowboy, floor);
            //newDrop.transform.localPosition = new Vector3(x,0);
        }
        else if (naughtyType == 1)
        {
            float x = -spriteSize * (numOfPixelX/2+1) + dropOffset;

            GameObject newRun = Instantiate(runCowboy, floor);
            //newRun.transform.localPosition = new Vector3(x,0);
        }
    }

    private void OrnamentGenerate(Transform floor, float spriteSize)
    {
        bool ifOrnament = UnityEngine.Random.Range(0f,1f) <= ornamentProb? true : false;
        if (!ifOrnament) return;

        int ornamentPos = 0;
        
        int bit1 = (currEvent[0] == 1)? 1 : 0;
        int bit2 = (currEvent[3] == 1)? 1 : 0;
        int bit3 = (prevEvent[0] == 2 || prevEvent[0] == 4)? 1 : 0;
        int bit4 = (prevEvent[3] == 2 || prevEvent[3] == 4)? 1 : 0;
        int result = (bit1 << 3) | (bit2 << 2) | (bit3 << 1) | bit4;

        Debug.Log("result: "+result);

        // 00xx
        if(result < 4) return;
        // 01xx / 1110
        else if ((result >= 4 && result < 8) || result == 14) ornamentPos = 3;
        // 10xx / 1101
        else if ((result >= 8 && result < 12) || result == 13) ornamentPos = 0;
        // 1100 / 1111
        else if (result == 12 || result == 15) ornamentPos = UnityEngine.Random.Range(0, 2) * 3;


        int ornamentType = UnityEngine.Random.Range(0, ornament.Count);
        GameObject newObstacle = Instantiate(ornament[ornamentType], floor);
        newObstacle.transform.localPosition = new Vector2(ornamentPos*11/3 - 5.5f, 0);
    }
}
