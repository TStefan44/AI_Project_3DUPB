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
    private Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = initialSize * new Vector3(1, 1, 1);
        rigidbody = GetComponent<Rigidbody>();
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
                rigidbody.useGravity = true;
            }
            transform.localScale = curentSize * new Vector3(1, 1, 1);
        }
    }
}
