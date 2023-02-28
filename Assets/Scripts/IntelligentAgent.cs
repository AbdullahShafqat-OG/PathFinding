using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class IntelligentAgent : MonoBehaviour {

    Transform goal;
    [SerializeField]
    float speed = 5.0f;
    [SerializeField]
    float accuracy = 1.0f;
    [SerializeField]
    float rotSpeed = 2.0f;
    [SerializeField]
    float avoidRotSpeed = 180.0f;
    [SerializeField]
    float waitAtLocation = 10.0f;

    public GameObject wpManager;
    GameObject[] wps;
    [SerializeField]
    GameObject currentNode;
    [SerializeField]
    int currentWP = 0;
    int currentWPInitial;
    Graph g;

    public enum State { Moving, Waiting, Static };
    public State myState = State.Static;

    public float obstacleRange = 5.0f;
    public float radius = 0.75f;

    public float avoidRadius = 5.0f;
    public GameObject parcel;

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

    void Start() {

        wps = wpManager.GetComponent<WPManager>().waypoints;
        if (this.name == "Player1") g = wpManager.GetComponent<WPManager>().graphs[0];
        if (this.name == "Player2") g = wpManager.GetComponent<WPManager>().graphs[1];
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

        if (g.getPathLength() == 0 || currentWP == g.getPathLength())
        {
            if (myState == State.Static)
            {
                parcel.SetActive(true);
                yield break;
            }
            else
            {
                parcel.SetActive(false);
                yield return new WaitForSeconds(waitAtLocation);
                PathBack();
                g.AStar(currentNode, wps[currentWPInitial]);
                currentWP = 0;
            }
        }

        myState = State.Moving;

        currentNode = g.getPathPoint(currentWP);

        if (Vector3.Distance(
            g.getPathPoint(currentWP).transform.position,
            transform.position) < accuracy)
        {
            currentWP++;
        }

        if (currentWP < g.getPathLength())
        {
            goal = g.getPathPoint(currentWP).transform;
            Vector3 lookAtGoal = new Vector3(goal.position.x,
                                            this.transform.position.y,
                                            goal.position.z);
            Vector3 direction = lookAtGoal - this.transform.position;

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                                    Quaternion.LookRotation(direction),
                                                    Time.deltaTime * rotSpeed);

            Collider[] colliders = Physics.OverlapSphere(transform.position, avoidRadius);
            foreach (Collider collider in colliders)
            {
                if (!collider.CompareTag("Player")) continue;
                if (collider.gameObject == gameObject) continue;

                transform.Rotate(Vector3.up, avoidRotSpeed * Time.deltaTime);
            }

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