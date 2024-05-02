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
        string inputLine = Console.ReadLine();
        if (string.IsNullOrEmpty(inputLine))
        {
            Console.WriteLine("No input provided.");
            return;
        }

        int d = int.Parse(inputLine);

        inputLine = Console.ReadLine();
        if (string.IsNullOrEmpty(inputLine))
        {
            Console.WriteLine("No edge data provided.");
            return;
        }

        var tokens = inputLine.Split().Select(int.Parse).ToArray();
        int n = tokens[0], e = tokens[1];

        InitializeGlobals(n);

        for (int i = 0; i < e; i++)
        {
            tokens = Console.ReadLine().Split().Select(int.Parse).ToArray();
            graph[tokens[0]].Add(tokens[1]);
            if (d == 0) 
            {
                graph[tokens[1]].Add(tokens[0]);
            }
        }

        PerformBFSDFSAndArticulation(n);
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
        Array.Fill(parent, -1); 
        time = 0;
    }

    static void PerformBFSDFSAndArticulation(int n)
    {
        var bfsParents = BFS(1);
        Console.WriteLine("BFS:");
        var bfsResult = bfsParents.Keys.ToList();
        Console.WriteLine(string.Join(" ", bfsResult));
        Console.WriteLine("BFS Paths:");
        foreach (var node in bfsResult)
        {
            var path = GetPath(bfsParents, 1, node);
            Console.WriteLine(string.Join(" ", path));
        }

        Array.Clear(visited, 0, visited.Length);
        List<int> dfsResult = new List<int>();
        DFS(1, dfsResult);
        Console.WriteLine("DFS:");
        Console.WriteLine(string.Join(" ", dfsResult));

        Console.WriteLine("DFS Paths:");
        foreach (var node in graph.Keys.OrderBy(k => k))
        {
            var path = GetPath(node);
            if (path.Count > 0) 
            {
                Console.WriteLine(string.Join(" ", path));
            }
        }

        Array.Clear(visited, 0, visited.Length); 
        var connectedComponents = FindConnectedComponents(n);
        Console.WriteLine("Connected Components:");
        for (int i = 0; i < connectedComponents.Count; i++)
        {
            Console.WriteLine("C" + (i + 1) + ": " + string.Join(" ", connectedComponents[i]));
        }

        Array.Clear(visited, 0, visited.Length); 
        FindArticulationPoints(1);
        Console.WriteLine("Articulation Vertices:");
        Console.WriteLine(string.Join(" ", articulationPoints.Distinct()));
    }

    static List<int> GetPath(int node)
    {
        var path = new List<int>();
        while (node != -1) 
        {
            path.Add(node);
            node = parent[node];
        }
        path.Reverse();
        return path;
    }

    static List<int> GetPath(Dictionary<int, int> parents, int start, int end)
    {
        var path = new List<int>();
        int current = end;
        while (current != start)
        {
            path.Add(current);
            current = parents[current];
        }
        path.Add(start);
        path.Reverse();
        return path;
    }

    static void FindArticulationPoints(int u, int p = -1)
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

    static void DFS(int node, List<int> dfsResult)
    {
        visited[node] = true;
        dfsResult.Add(node);

        foreach (int neighbor in graph[node])
        {
            if (!visited[neighbor])
            {
                parent[neighbor] = node; 
                DFS(neighbor, dfsResult);
            }
        }
    }

    static Dictionary<int, int> BFS(int start)
    {
        var queue = new Queue<int>();
        var parents = new Dictionary<int, int>(); 
        visited[start] = true;
        queue.Enqueue(start);
        parents[start] = -1; 

        while (queue.Count > 0)
        {
            int node = queue.Dequeue();

            foreach (var neighbor in graph[node])
            {
                if (!visited[neighbor])
                {
                    visited[neighbor] = true;
                    queue.Enqueue(neighbor);
                    parents[neighbor] = node; 
                }
            }
        }

        return parents;
    }

    static List<List<int>> FindConnectedComponents(int numberOfNodes)
    {
        var connectedComponents = new List<List<int>>();
        visited = new bool[numberOfNodes + 1]; 

        for (int node = 1; node <= numberOfNodes; node++)
        {
            if (!visited[node])
            {
                var component = new List<int>();
                DFS(node, component); 
                component.Sort(); 
                connectedComponents.Add(component);
            }
        }

        return connectedComponents;
    }
}