using System.Collections.Generic;

public class BFSPathfinder
{
    public static List<int> FindPath(int start, int goal, Dictionary<int, List<int>> graph)
    {
        Queue<int> queue = new Queue<int>();
        Dictionary<int, int> cameFrom = new Dictionary<int, int>();

        queue.Enqueue(start);
        cameFrom[start] = -1;

        while (queue.Count > 0)
        {
            int current = queue.Dequeue();

            if (current == goal)
                break;

            foreach (int neighbor in graph[current])
            {
                if (!cameFrom.ContainsKey(neighbor))
                {
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        List<int> path = new List<int>();

        if (!cameFrom.ContainsKey(goal))
            return path;

        int temp = goal;
        while (temp != -1)
        {
            path.Add(temp);
            temp = cameFrom[temp];
        }

        path.Reverse();
        return path;
    }
}