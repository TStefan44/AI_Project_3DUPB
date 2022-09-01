using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentNPC : Agent
{
    [SerializeField] private float rotSpeed = 5f;
    [SerializeField] private float speed;
    [SerializeField] private Vector3 safeHouse;
    private ManageEnvironment manager;
    private Vector3 moveDir;
    private Rigidbody rBody;
    const float joystickActiveTolerance = 3f * 10e-3f;
    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        manager = transform.parent.GetComponent<ManageEnvironment>();
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

        // Punish for redundant steps
        //AddReward(-0.02f);

        // Check if agent is out from it's environment
        // TODO: Change, refactor, eliminate. Tryed collisionExit with Plane
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
        manager.BeginNewEpisode();

        // If the Agent fell, zero its momentum
        // TODO: Change, refactor, eliminate. Tryed collisionExit with Plane
        if (this.transform.localPosition.y < 0)
        {
            rBody.angularVelocity = Vector3.zero;
            rBody.velocity = Vector3.zero;
        }

        //spawn to safeHouse location
        transform.localPosition = safeHouse;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // collision with goal/ pickup/ apple
        if (collision.gameObject.TryGetComponent<Goal>(out Goal goal))
        {
            // Spawn new pickup, destroy old one
            // TODO: need to refactor interaction in medium future
            NotifySpawner(collision.gameObject);
            Destroy(collision.gameObject);

            AddReward(1f);
            EndEpisode();
        }
        // collision with player
        if (collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            AddReward(-1f);
            Debug.Log("Player collision!");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Collision with plane
        if (collision.gameObject.TryGetComponent<MeshCollider>(out MeshCollider mesh))
        {
            SetReward(-1f);
            EndEpisode();
        }
    }

    // TODO: need to refactor interaction in medium future
    private void NotifySpawner(GameObject goal)
    {
        Spawner parentSpawner = goal.transform.parent.GetComponent<Spawner>();
        parentSpawner.addCurrentNumberSpawn(-1);
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

    public Vector3 GetSafeHouse()
    {
        return safeHouse;
    }

    public void SetSafeHouse(Vector3 safeHouse)
    {
        this.safeHouse = safeHouse;
    }
}
