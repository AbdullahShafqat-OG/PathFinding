using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class FollowPath : MonoBehaviour {

    // Tank targer
    Transform goal;
    // Tank speed
    [SerializeField]
    float speed = 5.0f;
    // Final distance from target
    [SerializeField]
    float accuracy = 1.0f;
    // Tank rotation speed
    [SerializeField]
    float rotSpeed = 2.0f;
    [SerializeField]
    float turnSpeed = 180.0f;
    [SerializeField]
    float waitAtLocation = 1.0f;

    // Access to the WPManager script
    public GameObject wpManager;
    // Array of waypoints
    GameObject[] wps;
    // Current waypoint
    [SerializeField]
    GameObject currentNode;
    // Starting waypoint index
    [SerializeField]
    int currentWP = 0;
    int currentWPInitial;
    // Access to the Graph script
    Graph g;

    public enum State { Moving, Waiting, Static };
    public State myState = State.Static;

    public float obstacleRange = 5.0f;
    public float radius = 0.75f;

    public float avoidRadius = 5.0f;

    private Coroutine moveCoroutine;

    private void OnEnable()
    {
        if (this.name == "Player1") WPManager.onEndLocationUpdated += GoToGoalLocation;
        if (this.name == "Player2") WPManager.onEndLocationUpdated2 += GoToGoalLocation;
    }

    private void OnDisable()
    {
        if (this.name == "Player1") WPManager.onEndLocationUpdated -= GoToGoalLocation;
        if (this.name == "Player2") WPManager.onEndLocationUpdated2 -= GoToGoalLocation;
    }

    // Use this for initialization
    void Start() {

        // Get hold of wpManager and Graph scripts
        wps = wpManager.GetComponent<WPManager>().waypoints;
        if (this.name == "Player1") g = wpManager.GetComponent<WPManager>().graphs[0];
        if (this.name == "Player2") g = wpManager.GetComponent<WPManager>().graphs[1];
        // Set the current Node
        currentNode = wps[currentWP];

        currentWPInitial = currentWP;
    }

    private void GoToGoalLocation(int index)
    {
        Debug.Log("GOING TO LOCATION FOR " + this.name);
        if (myState != State.Static) return;

        currentNode = wps[currentWPInitial];
        g.AStar(currentNode, wps[index]);
        currentWP = 0;
    }

    public void GoToHeli() {

        // Use the AStar method passing it currentNode and distination
        g.AStar(currentNode, wps[4]);
        // Reset index
        currentWP = 0;
    }

    // Update is called once per frame
    void LateUpdate() {
        myState = State.Static;
        if (!currentNode.CompareTag("startLocation")) myState = State.Waiting;
        if (moveCoroutine == null) moveCoroutine = StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.SphereCast(ray, radius, out hit))
        {
            if (hit.distance < obstacleRange && hit.transform.CompareTag("character"))
            {
                yield break;
            }
        }

        // If we've nowhere to go then just return
        if (g.getPathLength() == 0 || currentWP == g.getPathLength())
        {
            if (myState == State.Static) yield break;
            else
            {
                yield return new WaitForSeconds(waitAtLocation);
                PathBack();
                g.AStar(currentNode, wps[currentWPInitial]);
                currentWP = 0;
            }
        }

        myState = State.Moving;

        //the node we are closest to at this moment
        currentNode = g.getPathPoint(currentWP);

        //if we are close enough to the current waypoint move to next
        if (Vector3.Distance(
            g.getPathPoint(currentWP).transform.position,
            transform.position) < accuracy)
        {
            currentWP++;
        }

        //if we are not at the end of the path
        if (currentWP < g.getPathLength())
        {
            goal = g.getPathPoint(currentWP).transform;
            Vector3 lookAtGoal = new Vector3(goal.position.x,
                                            this.transform.position.y,
                                            goal.position.z);
            Vector3 direction = lookAtGoal - this.transform.position;

            // Rotate towards the heading
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                                    Quaternion.LookRotation(direction),
                                                    Time.deltaTime * rotSpeed);

            Collider[] colliders = Physics.OverlapSphere(transform.position, avoidRadius);
            foreach (Collider collider in colliders)
            {
                if (!collider.CompareTag("Player")) continue;
                if (collider.gameObject == gameObject) continue;

                transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
            }

            // Move the tank
            this.transform.Translate(0, 0, speed * Time.deltaTime);
        }

        moveCoroutine = null;
    }

    private void PathBack()
    {
        GameObject best = null;
        float bestDistance = 100000f;
        int index = 0;
        int bestIndex = 0;

        foreach (GameObject wp in wps)
        {
            if (wp.CompareTag("startLocation"))
            {
                float value = g.AStarPath(currentNode, wp);
                if (value < bestDistance)
                {
                    bestDistance = value;
                    best = wp;
                    bestIndex = index;
                }
                //Debug.Log($"GP:{wp.name} and SL:{currentNode.name} distance:{value}");
            }
            
            index++;
        }
       
        //Debug.Log($"Best is {best.name} index {bestIndex}");
        currentWPInitial = bestIndex;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * obstacleRange);
        Gizmos.DrawWireSphere(transform.position + transform.forward * obstacleRange, radius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, avoidRadius);
    }
}