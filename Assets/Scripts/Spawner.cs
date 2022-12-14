using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private int numberOfObjectsToSpawn;
    [SerializeField] private float timeToSpawn;
    private float currentTimeToSpawn;
    private int curentNumberOfObjectsToSpawn = 0;

    // Start is called before the first frame update
    void Start()
    {
        currentTimeToSpawn = timeToSpawn;
    }

    // Update is called once per frame
    void Update()
    {
        // maximum objects spawned
        if (curentNumberOfObjectsToSpawn == numberOfObjectsToSpawn)
        {
            return;
        }
        // cooldown time
        if (currentTimeToSpawn > 0)
        {
            currentTimeToSpawn -= Time.deltaTime;
        }
        // spawn new object
        else
        {
            currentTimeToSpawn = timeToSpawn;
            curentNumberOfObjectsToSpawn += 1;
            spawnObject();
        }
    }

    // Random spawn object near spawner
    private void spawnObject()
    {
        Vector3 randomPosition = new Vector3(Random.Range(-2, 2), Random.Range(-1, 1), Random.Range(-3, 3));
        Instantiate(objectToSpawn, transform.position + randomPosition, objectToSpawn.transform.rotation, transform);
        Debug.Log("Object Spawned");
    }

    public void addCurrentNumberSpawn(int num)
    {
        curentNumberOfObjectsToSpawn += num;
        if (curentNumberOfObjectsToSpawn > numberOfObjectsToSpawn)
        {
            curentNumberOfObjectsToSpawn = numberOfObjectsToSpawn;
        }
        if (curentNumberOfObjectsToSpawn < 0)
        {
            curentNumberOfObjectsToSpawn = 0;
        }
    }
}
