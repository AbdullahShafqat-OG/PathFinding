using UnityEngine;

[System.Serializable]
public struct Link {
    public GameObject node1;
    public GameObject node2;
}

public class WPManager : MonoBehaviour {

    public GameObject[] waypoints;
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

    void Start() {

        if (waypoints.Length > 0) {
            foreach (GameObject wp in waypoints) {
                foreach (Graph graph in graphs)
                    graph.AddNode(wp);
            }

            foreach (Link l in links) {
                foreach (Graph graph in graphs)
                {
                    graph.AddEdge(l.node1, l.node2);
                    graph.AddEdge(l.node2, l.node1);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int i = 0; i < links.Length; i++)
        {
            Gizmos.DrawLine(links[i].node1.transform.position, links[i].node2.transform.position);
        }
    }
}
