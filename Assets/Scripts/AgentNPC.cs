using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentNPC : Agent
{
    private Rigidbody rBody;
    [SerializeField] private float rotSpeed = 5f;
    const float joystickActiveTolerance = 3f * 10e-3f;
    [SerializeField] private float speed;
    [SerializeField] private Vector3 safeHouse;
    private Vector3 moveDir;
    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(safeHouse);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float x = actions.ContinuousActions[0];
        float z = actions.ContinuousActions[1];
        moveDir = new Vector3(x, 0, z).normalized;
        ApplyRootRotation();
        rBody.velocity = moveDir * speed;

        if (transform.localPosition.y < 0)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        continousActions[0] = Input.GetAxisRaw("Horizontal");
        continousActions[1] = Input.GetAxisRaw("Vertical");
    }

    public override void OnEpisodeBegin()
    {
        // If the Agent fell, zero its momentum
        if (this.transform.localPosition.y < 0)
        {
            rBody.angularVelocity = Vector3.zero;
            rBody.velocity = Vector3.zero;
        }

        //transform.localPosition = new Vector3(0, 0.5f, 0);
        transform.localPosition = safeHouse;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Goal>(out Goal goal))
        {
            AddReward(0.5f);
            EndEpisode();
        }
    }

    private void ApplyRootRotation()
    {
        Vector3 lookDir = transform.forward;
        if (moveDir.magnitude > joystickActiveTolerance) //there is movement
            lookDir = moveDir;

        Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);
        float rotSlerpFactor = Mathf.Clamp01(rotSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotSlerpFactor);
    }
}
