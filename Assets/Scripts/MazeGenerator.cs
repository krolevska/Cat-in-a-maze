using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CatInMaze
{
    public class MazeGenerator : MonoBehaviour
    {
        // Dimensions of the maze
        public int width, height;

        // Reference to the Tilemap component where the maze walls will be drawn
        public Tilemap wallsTilemap;

        // Tile that will be used for the maze walls
        public Tile wallTile;

        // 2D array to track which cells of the maze have been visited
        private bool[,] visited;

        // Array of possible directions in which the generator can move
        private Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 2),
            new Vector2Int(2, 0),
            new Vector2Int(0, -2),
            new Vector2Int(-2, 0)
        };

        private Vector2Int finishPoint; // To store the finish point coordinates

        // Called when the script instance is being loaded
        private void Start()
        {
            InitializeMaze();
            GenerateMaze();
            CarveFinish();  // Carve out the finish point after generating the maze
        }

        // Set the initial state of the maze
        void InitializeMaze()
        {
            visited = new bool[width, height];

            // Set start position on the bottom wall
            int startX = Random.Range(1, width - 1);

            // Set finish position on the top wall here
            finishPoint = new Vector2Int(Random.Range(1, width - 1), height - 1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // For the bottom wall (y == 0), skip setting a wall tile only at the start position
                    if (y <= 1 && x == startX)
                        continue;

                    wallsTilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
                }
            }
        }

        // Carve out the finish point at the top of the maze
        void CarveFinish()
        {
            wallsTilemap.SetTile(new Vector3Int(finishPoint.x, finishPoint.y, 0), null);
        }

        // Implement the maze generation using depth-first search
        void GenerateMaze()
        {
            Stack<Vector2Int> stack = new Stack<Vector2Int>();
            Vector2Int startPos = new Vector2Int(Random.Range(1, (width - 2) / 2) * 2, 0);

            CarveCell(startPos);
            stack.Push(startPos);

            while (stack.Count > 0)
            {
                Vector2Int current = stack.Peek();
                List<Vector2Int> unvisitedNeighbors = GetUnvisitedNeighbors(current);

                if (unvisitedNeighbors.Count > 0)
                {
                    Vector2Int chosenOne = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)];
                    RemoveWall(current, chosenOne);
                    CarveCell(chosenOne);
                    stack.Push(chosenOne);
                }
                else
                {
                    stack.Pop();
                }
            }
        }

        // Marks a cell as visited and removes its wall tile
        void CarveCell(Vector2Int pos)
        {
            visited[pos.x, pos.y] = true;
            wallsTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), null);
        }

        // Get a list of unvisited neighbors for a given cell
        List<Vector2Int> GetUnvisitedNeighbors(Vector2Int current)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();

            foreach (Vector2Int dir in directions)
            {
                Vector2Int nextPos = current + dir;
                if (IsInside(nextPos) && !visited[nextPos.x, nextPos.y])
                    neighbors.Add(nextPos);
            }

            return neighbors;
        }

        // Remove a wall between two adjacent cells
        void RemoveWall(Vector2Int current, Vector2Int next)
        {
            Vector2Int wallPos = current + (next - current) / 2;
            wallsTilemap.SetTile(new Vector3Int(wallPos.x, wallPos.y, 0), null);
        }

        // Check if a given position is inside the maze boundaries
        bool IsInside(Vector2Int pos)
        {
            return pos.x > 0 && pos.y > 0 && pos.x < width - 1 && pos.y < height - 1;
        }
    }
}
