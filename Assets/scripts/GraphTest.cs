using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GraphTest : MonoBehaviour
{
    public enum Alforithm
    {
        DFS,
        BFS,
        DFSRecursive,
        PathFindingBSF,
        Dijkcstra,
        AStar
    }
    public Transform uiNodeRoot;
    public UiGraphNode nodePrefab;
    private List<UiGraphNode> uiNodes = new List<UiGraphNode>();
    private Graph graph;

    public Alforithm alforithm;
    public int startId;
    public int endId;

    private void Start()
    {
        int[,] map = new int[5, 5]
        {
            {1, -1, 1, 1, 1 },
            {1, -1, 1, 1, 1 },
            {1, -1, 1, 1, 1 },
            {1, -1, 1, 1, 1 },
            {1, 1, 1, 1, 1 },
        };
        graph = new Graph();
        graph.Init(map);
        InitUiNodes(graph);
    }

    private void InitUiNodes(Graph graph)
    {
        foreach (var node in graph.nodes)
        {
            var uiNode = Instantiate(nodePrefab, uiNodeRoot);
            uiNode.SetNode(node);
            uiNode.Reset();
            uiNodes.Add(uiNode);
        }
    }
    private void ResetUiNoddes()
    {
        foreach (var uinode in uiNodes)
        {
            uinode.Reset();
        }
    }
    [ContextMenu("Search")]
    public void Search()
    {
        var search = new GraphSearch();
        search.Init(graph);

        switch (alforithm)
        {
            case Alforithm.DFS:
                search.DFS(graph.nodes[startId]);
                break;
            case Alforithm.BFS:
                search.BFS(graph.nodes[startId]);
                break;
            case Alforithm.DFSRecursive:
                search.DFSRecursive(graph.nodes[startId]);
                break;
            case Alforithm.PathFindingBSF:
                search.PathFindingBSF(graph.nodes[startId], graph.nodes[endId]);
                break;
            case Alforithm.Dijkcstra:
                search.Dijkstra(graph.nodes[startId], graph.nodes[endId]);
                break;
            case Alforithm.AStar:
                search.AStar(graph.nodes[startId], graph.nodes[endId]);
                break;

        }

        ResetUiNoddes();
        if (search.path.Count <= 1)
        {
            if (search.path.Count == 1)
            {
                var only = search.path[0];
                uiNodes[only.id].SetColor(Color.red);

            }
            return;
        }

        for (int i = 0; i < search.path.Count; i++)
        {
            var node = search.path[i];
            var color = Color.Lerp(Color.red, Color.green, (float)i / (search.path.Count - 1));
            uiNodes[node.id].SetColor(color);
            uiNodes[node.id].SetText($"ID: {node.id}\nWeight: {node.weight}\nPath: {i}");
        }
    }
}
