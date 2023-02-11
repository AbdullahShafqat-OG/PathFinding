using UnityEngine;

[System.Serializable]
public struct Link {
    // Our two possible choices of single or bi direction
    public enum direction { UNI, BI };
    // Nodes
    public GameObject node1;
    public GameObject node2;
    // Direction UNI or BI
    public direction dir;
}

public class WPManager : MonoBehaviour {

    // An array of GameObjects to store all the waypoints;
    public GameObject[] waypoints;
    // An array of possible links between nodes
    public Link[] links;
    public Graph graph = new Graph();

    public delegate void UpdateEndLocationAction(int index);
    public static event UpdateEndLocationAction onEndLocationUpdated;

    private void OnEnable()
    {
        Waypoint.onSelected += GetWaypointIndex;
    }

    private void OnDisable()
    {
        Waypoint.onSelected -= GetWaypointIndex;
    }

    private void GetWaypointIndex(GameObject go)
    {
        int index = System.Array.IndexOf(waypoints, go);
        onEndLocationUpdated(index);
    }

    // Use this for initialization
    void Start() {

        // Check we have some waypoints to work with
        if (waypoints.Length > 0) {
            // Loop through all the waypoints and add them to the graph
            foreach (GameObject wp in waypoints) {
                graph.AddNode(wp, false, false, false);
            }

            // Loop through all the possible links and add them to the graph
            foreach (Link l in links) {
                graph.AddEdge(l.node1, l.node2);
                if (l.dir == Link.direction.BI)
                    graph.AddEdge(l.node2, l.node1);
            }
        }
    }

    // Update is called once per frame
    void Update() {

        // Call the graph debugDraw code
        graph.debugDraw();
    }

    private void OnDrawGizmos()
    {
        Color oldColor = Gizmos.color;
        Gizmos.color = Color.red;
        //draw edges
        for (int i = 0; i < links.Length; i++)
        {
            Gizmos.DrawLine(links[i].node1.transform.position, links[i].node2.transform.position);
        }
        Gizmos.color = oldColor;
    }
}
