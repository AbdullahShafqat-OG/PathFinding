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
    public Graph[] graphs = new Graph[] { new Graph(), new Graph() };

    public delegate void UpdateEndLocationAction(int index);
    public static event UpdateEndLocationAction onEndLocationUpdated;
    public static event UpdateEndLocationAction onEndLocationUpdated2;

    private void OnEnable()
    {
        Waypoint.onSelected += GetWaypointIndex;
    }

    private void OnDisable()
    {
        Waypoint.onSelected -= GetWaypointIndex;
    }

    private void GetWaypointIndex(GameObject go, bool left)
    {
        int index = System.Array.IndexOf(waypoints, go);
        if (left && onEndLocationUpdated != null) onEndLocationUpdated(index);
        else if (!left && onEndLocationUpdated2 != null) onEndLocationUpdated2(index);
    }

    // Use this for initialization
    void Start() {

        // Check we have some waypoints to work with
        if (waypoints.Length > 0) {
            // Loop through all the waypoints and add them to the graph
            foreach (GameObject wp in waypoints) {
                foreach (Graph graph in graphs)
                    graph.AddNode(wp, false, false, false);
            }

            // Loop through all the possible links and add them to the graph
            foreach (Link l in links) {
                foreach (Graph graph in graphs)
                {
                    graph.AddEdge(l.node1, l.node2);
                    if (l.dir == Link.direction.BI)
                        graph.AddEdge(l.node2, l.node1);
                }
            }
        }
    }

    // Update is called once per frame
    void Update() {
        // Call the graph debugDraw code
        graphs[0].debugDraw();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //draw edges
        for (int i = 0; i < links.Length; i++)
        {
            Gizmos.DrawLine(links[i].node1.transform.position, links[i].node2.transform.position);
        }
    }
}
