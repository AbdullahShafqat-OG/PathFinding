using System.Collections.Generic;
using UnityEngine;

public class Graph {
    List<Edge> edges = new List<Edge>();
    List<Node> nodes = new List<Node>();
    List<Node> pathList = new List<Node>();

    public Graph() { }

    public void AddNode(GameObject go) {
        Node node = new Node(go);
        nodes.Add(node);

        TextMesh[] textms = go.GetComponentsInChildren<TextMesh>() as TextMesh[];

        foreach (TextMesh tm in textms)
            GameObject.Destroy(tm.gameObject);
    }

    public void AddEdge(GameObject fromNode, GameObject toNode) {
        Node from = FindNodeInGraph(fromNode);
        Node to = FindNodeInGraph(toNode);

        if (from != null && to != null) {
            Edge e = new Edge(from, to);
            edges.Add(e);
            from.edgelist.Add(e);
        }
    }

    Node FindNodeInGraph(GameObject id) {
        foreach (Node n in nodes) {
            if (n.getId() == id)
                return n;
        }
        return null;
    }


    public int getPathLength() {
        return pathList.Count;
    }

    public GameObject getPathPoint(int index) {
        return pathList[index].id;
    }

    public bool AStar(GameObject goStart, GameObject goEnd) {
        Node sNode = FindNodeInGraph(goStart);
        Node eNode = FindNodeInGraph(goEnd);

        if (sNode == null || eNode == null) return false;

        List<Node> open = new List<Node>();
        List<Node> closed = new List<Node>();
        float tentative_g_score = 0;
        bool tentative_is_better;

        sNode.g = 0;
        sNode.h = distance(sNode, eNode);
        sNode.f = sNode.h;
        open.Add(sNode);

        while (open.Count > 0) {
            int i = lowestF(open);
            Node thisnode = open[i];
            if (thisnode.id == goEnd)
            {
                Debug.Log("BEST DISTANCE : " + tentative_g_score);
                reconstructPath(sNode, eNode);
                return true;
            }

            open.RemoveAt(i);
            closed.Add(thisnode);

            Node neighbour;
            foreach (Edge e in thisnode.edgelist) {
                neighbour = e.endNode;
                neighbour.g = thisnode.g + distance(thisnode, neighbour);

                if (closed.IndexOf(neighbour) > -1)
                    continue;

                tentative_g_score = thisnode.g + distance(thisnode, neighbour);

                if (open.IndexOf(neighbour) == -1) {
                    open.Add(neighbour);
                    tentative_is_better = true;
                } else if (tentative_g_score < neighbour.g) {
                    tentative_is_better = true;
                } else
                    tentative_is_better = false;

                if (tentative_is_better) {
                    neighbour.cameFrom = thisnode;
                    neighbour.g = tentative_g_score;
                    //neighbour.h = distance(thisnode,neighbour);
                    neighbour.h = distance(thisnode, eNode);
                    neighbour.f = neighbour.g + neighbour.h;
                }
            }

        }

        return false;
    }

    public float AStarPath(GameObject goStart, GameObject goEnd)
    {
        Node sNode = FindNodeInGraph(goStart);
        Node eNode = FindNodeInGraph(goEnd);

        if (sNode == null || eNode == null)
        {
            return float.NaN;
        }

        List<Node> open = new List<Node>();
        List<Node> closed = new List<Node>();
        float tentative_g_score = 0;
        bool tentative_is_better;

        sNode.g = 0;
        sNode.h = distance(sNode, eNode);
        sNode.f = sNode.h;
        open.Add(sNode);

        while (open.Count > 0)
        {
            int i = lowestF(open);
            Node thisnode = open[i];
            if (thisnode.id == goEnd)  //path found
            {
                //Debug.Log("Score : " + tentative_g_score);
                return tentative_g_score;
            }

            open.RemoveAt(i);
            closed.Add(thisnode);

            Node neighbour;
            foreach (Edge e in thisnode.edgelist)
            {
                neighbour = e.endNode;
                neighbour.g = thisnode.g + distance(thisnode, neighbour);

                if (closed.IndexOf(neighbour) > -1)
                    continue;

                tentative_g_score = thisnode.g + distance(thisnode, neighbour);

                if (open.IndexOf(neighbour) == -1)
                {
                    open.Add(neighbour);
                    tentative_is_better = true;
                }
                else if (tentative_g_score < neighbour.g)
                {
                    tentative_is_better = true;
                }
                else
                    tentative_is_better = false;

                if (tentative_is_better)
                {
                    neighbour.cameFrom = thisnode;
                    neighbour.g = tentative_g_score;
                    //neighbour.h = distance(thisnode,neighbour);
                    neighbour.h = distance(thisnode, eNode);
                    neighbour.f = neighbour.g + neighbour.h;
                }
            }

        }

        return float.NaN;
    }

    public void reconstructPath(Node startId, Node endId) {
        pathList.Clear();
        pathList.Add(endId);

        var p = endId.cameFrom;
        while (p != startId && p != null) {
            pathList.Insert(0, p);
            p = p.cameFrom;
        }
        pathList.Insert(0, startId);
    }

    public void reconstructPathBack(Node startId, Node endId)
    {
        pathList.Clear();
        pathList.Add(endId);

        var p = endId.cameFrom;
        while (p != startId && p != null)
        {
            pathList.Insert(0, p);
            p = p.cameFrom;
        }
        pathList.Insert(0, startId);

        List<GameObject> startLocations = new List<GameObject>();
        foreach (Node node in nodes)
        {
            if (node.getId().CompareTag("startLocation")) startLocations.Add(node.getId());
        }
        GameObject best = null;
        float bestDistance = 100000f;
        foreach (GameObject startLocation in startLocations)
        {
            float value = AStarPath(endId.getId(), startLocation);
            if (value < bestDistance)
            {
                bestDistance = value;
                best = startLocation;
            }
            Debug.Log($"GP {startLocation.name} and SL {endId.getId().name} distance {value}");
        }
        Debug.Log($"BEST PATH SCORE {best.name}");
        //AStar(endId.getId(), best, false, true);

        List<Node> reversedList = new List<Node>(pathList);
        reversedList.Reverse();
        pathList.AddRange(reversedList);
    }

    float distance(Node a, Node b) {
        float dx = a.xPos - b.xPos;
        float dy = a.yPos - b.yPos;
        float dz = a.zPos - b.zPos;
        float dist = dx * dx + dy * dy + dz * dz;
        return (dist);
    }

    int lowestF(List<Node> l) {
        float lowestf = 0;
        int count = 0;
        int iteratorCount = 0;

        for (int i = 0; i < l.Count; i++) {
            if (i == 0) {
                lowestf = l[i].f;
                iteratorCount = count;
            } else if (l[i].f <= lowestf) {
                lowestf = l[i].f;
                iteratorCount = count;
            }
            count++;
        }
        return iteratorCount;
    }
}