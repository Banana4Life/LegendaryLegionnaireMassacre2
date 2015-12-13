﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class RiverPopulator : GridPopulator
{
    private static readonly Random random = new Random();
    public GameObject riverPrefab;

    private int getBendcase(Tile c, Tile p, Tile n)
    {
        if (p != null && n != null)
        {
            if (p.Y == n.Y)
            {
                return 1;
            }
            if (p.X == n.X)
            {
                return 2;
            }
            if ((p.X + n.X)/2 == c.X && (p.Y + n.Y)/2 == c.Y)
            {
                return 3;
            }
            if (p.Y == c.Y && c.X == n.X)
            {
                return 4;
            }
            if (p.X == c.X && c.X - 1 == n.X && c.Y + 1 == n.X)
            {
                return 5;
            }
            if (p.Y == c.Y && c.X + 1 == n.X && c.Y - 1 == n.Y)
            {
                return 6;
            }
            if (p.X == c.X && c.Y == n.Y)
            {
                return 7;
            }
            if (c.X + 1 == p.X && c.Y - 1 == p.Y && c.X == n.X)
            {
                return 8;
            }
            if (p.Y == n.Y && c.X - 1 == n.X && c.Y + 1 == n.Y)
            {
                return 9;
            }
        }
        return 0;
    }

    public
        override void populate(GameObject[,] gameObjects)
    {
        var borderTiles = new List<GameObject>();

        for (var x = 0; x < gameObjects.GetLength(0); x++)
        {
            for (var y = 0; y < gameObjects.GetLength(1); y++)
            {
                if (x == 0 || y == 0 || x == gameObjects.GetLength(0) - 1 || y == gameObjects.GetLength(1) - 1)
                {
                    borderTiles.Add(gameObjects[x, y]);
                    //Debug.Log("added " + x + "," + y);
                }
            }
        }

        var path = new List<GameObject>();
        path.Add(borderTiles[random.Next(0, borderTiles.Count)]);
        Debug.Log("rivered " + path[0].GetComponent<TileBehaviour>().tile);

        Debug.Log(path.Contains(path[0]));
        var blockedNeighbours = new List<GameObject>();


        //activate after first tile and exit when hitting next border tile
        do
        {
            var neighbours = path.Last().GetComponent<TileBehaviour>().tile.Neighbours.Select(n => n.GameObject);

            var allowedNeighbours =
                neighbours.Where(go => !path.Contains(go) && !blockedNeighbours.Contains(go)).ToList();
            if (path.Count == 1)
            {
                allowedNeighbours = allowedNeighbours.Where(go => !borderTiles.Contains(go)).ToList();
            }
            //lastNeighbours.Clear();
            blockedNeighbours.AddRange(neighbours);

            if (!allowedNeighbours.Any())
            {
                Debug.Log("lake please");
                break;
            }

            var nextTile = allowedNeighbours[random.Next(0, allowedNeighbours.Count)];


            path.Add(nextTile);
            nextTile.AddComponent<River>();

            var hexriver = Instantiate(riverPrefab);
            hexriver.transform.parent = nextTile.transform;
            hexriver.transform.localPosition = Vector3.back;

            Debug.Log("rivered " + nextTile.GetComponent<TileBehaviour>().tile);
        } while (!borderTiles.Contains(path[path.Count - 1]));

        HexGrid.drawPath(path, Color.blue, gameObject => gameObject.transform.position);
        for (var i = 0; i < path.Count; i++)
        {
            var c = path[i].GetComponent<Tile>();
            var p = i > 0 ? path[i - 1].GetComponent<Tile>() : null;
            var n = i < path.Count - 1 ? path[i + 1].GetComponent<Tile>() : null;

            var bendcase = getBendcase(c, p, n);
            if (bendcase == 0) bendcase = getBendcase(c, n, p);

            int r;
            switch (bendcase)
            {
                case 0:
                    r = 0;
                    Debug.LogWarning("no bendcase");
                    break;
                case 1:
                    r = 0;
                    break;
                case 2:
                    r = 60;
                    break;
                case 3:
                    r = 120;
                    break;
                case 4:
                    r = 0;
                    break;
                case 5:
                    r = 60;
                    break;
                case 6:
                    r = 120;
                    break;
                case 7:
                    r = 0;
                    break;
                case 8:
                    r = 60;
                    break;
                case 9:
                    r = 120;
                    break;
            }
        }


        /*        if (random.Next(probability) <= 100)
                {
                    var village = Instantiate(villagePrefab);
                    var hex = gameObjects[x, y];
                    village.transform.parent = hex.transform;
                    village.transform.localPosition = Vector3.zero;

                }*/
    }

    public void populateLake(GameObject[,] gameObjects, int xStart, int yStart)
    {
    }
}