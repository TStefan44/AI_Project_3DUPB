using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageEnvironment : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject forest;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject agent;
    [SerializeField] private GameObject checkPoints;
    [SerializeField] private int offsetTree;
    [SerializeField] private int offsetAgent; 
    [SerializeField] private int randomEpisode;
    [SerializeField] private bool playerTrainingMove;
    private readonly List<Vector3> initialPosTrees = new();
    private List<GameObject> checkpoints = new();
    private Vector3 initialAgentPos;
    private int currentEpisode;
    private int currentCheckpoint = 0;
    void Start()
    {
        currentEpisode = randomEpisode;

        // Add each tree position in list
        foreach (Transform child in forest.transform)
        {
            initialPosTrees.Add(child.transform.localPosition);
        }

        // Memorise agent initial position
        initialAgentPos = agent.GetComponent<AgentNPC>().GetSafeHouse();

        // Set mode for player (true = controlled by input)
        player.GetComponent<Player>().SetTrainingMove(playerTrainingMove);

        // Memorise each checkpoint
        foreach (Transform child in checkPoints.transform)
        {
            checkpoints.Add(child.gameObject);
        }
        checkpoints = Fisher_Yates_CardDeck_Shuffle(checkpoints);
        checkpoints[currentCheckpoint].GetComponent<MeshRenderer>().enabled = true;
    }

    public void BeginNewEpisode()
    {
        currentEpisode--;
        if (currentEpisode == 0)
        {
            currentEpisode = randomEpisode;
            RadomiseEnvironment();
        }
    }

    private void RadomiseEnvironment()
    {
        ChangeRandomTreesPosition();
        ChangeRandomAgentSafeHouse();
        ChangeRandomCheckpoints();
    }

    // Shufle the order of checkpoints. Make current one visible.
    private void ChangeRandomCheckpoints()
    {
        checkpoints = Fisher_Yates_CardDeck_Shuffle(checkpoints);
        checkpoints[0].GetComponent<MeshRenderer>().enabled = true;
        for (int i = 1; i < checkpoints.Count; i++)
        {
            checkpoints[i].GetComponent<MeshRenderer>().enabled = false;
        }
        currentCheckpoint = 0;
    }

    // Randomise trees position and rotation.
    private void ChangeRandomTreesPosition()
    {
        Vector3 offsetPosition;
        int i = 0;
        foreach (Transform child in forest.transform)
        {
            offsetPosition = new Vector3(Random.Range(-offsetTree, offsetTree), 0, Random.Range(-offsetTree, offsetTree));
            child.transform.localPosition = offsetPosition + initialPosTrees[i];
            child.transform.localRotation = new Quaternion(0, Random.Range(0, 360), 0, 0);
            i++;
        }
    }

    // Randomise agent sapwnpoint/ safehouse
    private void ChangeRandomAgentSafeHouse()
    {
        Vector3 offsetPosition = new Vector3(Random.Range(-offsetAgent, offsetAgent), 0, Random.Range(-offsetAgent, offsetAgent));
        agent.GetComponent<AgentNPC>().SetSafeHouse(initialAgentPos + offsetPosition);
    }

    // Shufle function
    private static List<GameObject> Fisher_Yates_CardDeck_Shuffle(List<GameObject> aList)
    {

        System.Random _random = new();

        GameObject myGO;

        int n = aList.Count;
        for (int i = 0; i < n; i++)
        {
            // NextDouble returns a random number between 0 and 1
            int r = i + (int)(_random.NextDouble() * (n - i));
            myGO = aList[r];
            aList[r] = aList[i];
            aList[i] = myGO;
        }

        return aList;
    }

    // Try to give player a moveDir towards current checkpoint.
    // TODO: Not working, need repair.
    public Vector3 GetMoveDir(Vector3 currentPos)
    {
        Vector3 moveDir = checkpoints[currentCheckpoint].transform.localPosition - currentPos;
        moveDir = Vector3.ProjectOnPlane(moveDir, Vector3.up);
        return moveDir.normalized;
    }

    // Player hit checkpoint. Set next checkpoint and make visible.
    public void nextCheckpoint()
    {
        checkpoints[currentCheckpoint].GetComponent<MeshRenderer>().enabled = false;
        currentCheckpoint++;
        checkpoints[currentCheckpoint].GetComponent<MeshRenderer>().enabled = true;
        if (currentCheckpoint >= checkpoints.Count)
        {
            currentCheckpoint = 0;
        }
    }
}
