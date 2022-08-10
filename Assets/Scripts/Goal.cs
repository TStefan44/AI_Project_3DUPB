using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private float timeToGrow;
    [SerializeField] private float initialSize;
    [SerializeField] private float endSize;
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
        if (currentTime < timeToGrow)
        {
            currentTime += Time.deltaTime;
            curentSize = currentTime * endSize / timeToGrow;
            if (currentTime >= timeToGrow)
            {
                curentSize = endSize;
                rigidBody.useGravity = true;
            }
            transform.localScale = curentSize * new Vector3(1, 1, 1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Spawner") && GameObject.ReferenceEquals(other.gameObject, parent))
        {
            if(hadExitSpawn == true)
            {
                return;
            }
            Debug.Log("exit");
            hadExitSpawn = true;
            parent.GetComponent<Spawner>().addCurrentNumberSpawn(-1);
        }
    }
}
