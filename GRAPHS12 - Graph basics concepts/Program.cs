using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class GraphBasics
{
    static Dictionary<int, List<int>> graph;
    static bool[] visited;
    static List<int> articulationPoints;
    static int[] discoveryTime;
    static int[] lowTime;
    static int[] parent;
    static int time;

    public static void Main()
    {
        using var reader = new StreamReader(Console.OpenStandardInput());
        using var writer = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };

        string inputLine = reader.ReadLine();
        if (string.IsNullOrEmpty(inputLine))
        {
            writer.WriteLine("No input provided.");
            return;
        }

        int d = int.Parse(inputLine);

        inputLine = reader.ReadLine();
        if (string.IsNullOrEmpty(inputLine))
        {
            writer.WriteLine("No edge data provided.");
            return;
        }

        var tokens = inputLine.Split().Select(int.Parse).ToArray();
        int n = tokens[0], e = tokens[1];

        InitializeGlobals(n);

        for (int i = 0; i < e; i++)
        {
            tokens = reader.ReadLine().Split().Select(int.Parse).ToArray();
            graph[tokens[0]].Add(tokens[1]);
            if (d == 0) // If the graph is non-directed
            {
                graph[tokens[1]].Add(tokens[0]);
            }
        }

        PerformBFSDFSAndArticulation(writer, n);
    }

    static void InitializeGlobals(int n)
    {
        graph = new Dictionary<int, List<int>>();
        for (int i = 1; i <= n; i++)
        {
            graph[i] = new List<int>();
        }

        visited = new bool[n + 1];
        articulationPoints = new List<int>();
        discoveryTime = new int[n + 1];
        lowTime = new int[n + 1];
        parent = new int[n + 1];
        Array.Fill(parent, -1); // Initialize all parents to -1
        time = 0;
    }

    static void PerformBFSDFSAndArticulation(StreamWriter writer, int n)
    {
        // BFS
        List<int> bfsResult = BFS(1);
        writer.WriteLine("BFS:");
        writer.WriteLine(string.Join(" ", bfsResult));

        // DFS
        Array.Clear(visited, 0, visited.Length); // Clear visited for DFS
        List<int> dfsResult = new List<int>();
        DFS(1, dfsResult);
        writer.WriteLine("DFS:");
        writer.WriteLine(string.Join(" ", dfsResult));

        // Articulation Points
        Array.Clear(visited, 0, visited.Length); // Clear visited for articulation points
        FindArticulationPoints(1);
        writer.WriteLine("Articulation Vertices:");
        writer.WriteLine(string.Join(" ", articulationPoints.Distinct()));
    }

    private static void FindArticulationPoints(int u, int p = -1)
    {
        visited[u] = true;
        discoveryTime[u] = lowTime[u] = ++time;
        int childCount = 0;
        bool isArticulation = false;

        foreach (int v in graph[u])
        {
            if (!visited[v])
            {
                parent[v] = u;
                childCount++;
                FindArticulationPoints(v, u);

                if (lowTime[v] >= discoveryTime[u])
                {
                    isArticulation = true;
                }
                lowTime[u] = Math.Min(lowTime[u], lowTime[v]);
            }
            else if (v != p)
            {
                lowTime[u] = Math.Min(lowTime[u], discoveryTime[v]);
            }
        }

        if ((p == -1 && childCount > 1) || (p != -1 && isArticulation))
        {
            articulationPoints.Add(u);
        }
    }

    private static void DFS(int node, List<int> dfsResult)
    {
        visited[node] = true;
        dfsResult.Add(node);

        foreach (int neighbor in graph[node])
        {
            if (!visited[neighbor])
            {
                DFS(neighbor, dfsResult);
            }
        }
    }

    static List<int> BFS(int start)
    {
        var queue = new Queue<int>();
        var result = new List<int>();
        visited[start] = true;
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            int node = queue.Dequeue();
            result.Add(node);

            foreach (var neighbor in graph[node])
            {
                if (!visited[neighbor])
                {
                    visited[neighbor] = true;
                    queue.Enqueue(neighbor);
                }
            }
        }

        return result;
    }
}