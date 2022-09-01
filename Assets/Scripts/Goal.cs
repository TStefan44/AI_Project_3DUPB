using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private float timeToGrow;
    [SerializeField] private float initialSize;
    [SerializeField] private float endSize;
    [SerializeField] private float timeToRot;
    private float currentTime = 0;
    private float curentSize;
    private Rigidbody rigidBody;
    private bool hadExitSpawn = false;
    private GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = initialSize * new Vector3(1, 1, 1);
        rigidBody = GetComponent<Rigidbody>();
        parent = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // In tree, fruit is growing proportionaly with time
        if (!hadExitSpawn && currentTime < timeToGrow)
        {
            currentTime += Time.deltaTime;
            curentSize = currentTime * endSize / timeToGrow;

            // maximum size reached, activate gravity
            if (currentTime >= timeToGrow)
            {
                curentSize = endSize;
                rigidBody.useGravity = true;
                hadExitSpawn = true;
                currentTime = 0;
            }
            transform.localScale = curentSize * new Vector3(1, 1, 1);
        }
        if (hadExitSpawn && currentTime < timeToRot)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= timeToRot)
            {
                parent.GetComponent<Spawner>().addCurrentNumberSpawn(-1);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Collision with parent spawner
        if (other.gameObject.CompareTag("Spawner") && GameObject.ReferenceEquals(other.gameObject, parent))
        {
            if (hadExitSpawn == true)
            {
                return;
            }
            // The program doesn't execute this usually
            Debug.Log("exit");
            hadExitSpawn = true;
            currentTime = 0;
            parent.GetComponent<Spawner>().addCurrentNumberSpawn(-1);
        }
    }
}
