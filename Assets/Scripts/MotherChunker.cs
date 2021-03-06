using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Priority_Queue;
using Random = UnityEngine.Random;


public class State : FastPriorityQueueNode
{
  public int2 position;
}


public class MotherChunker : MonoBehaviour
{
  public int MaxChunks = 6;
  public float ChunkSpeed = 15;
  public float DespawnThreshold = -15;
  public float ChunkSize = 15;
  public float DeathRatio = 0.4f;
  public int MinimumValidPaths = 2;
  public int MinimumVerticalSegments = 3;
  public int MaxWhileLoopIters = 50;


  List<int2> GetNeighbors(int2 point, Dictionary<int2, bool> chunkMap)
  {
    var top = new int2(point.x, point.y + 1);
    var bottom = new int2(point.x, point.y - 1);
    var left = new int2(point.x - 1, point.y);
    var right = new int2(point.x + 1, point.y);

    if (left.x < 0)
    {
      left.x = DimensionPicker.DimensionCount - 1;
    }

    if (right.x > DimensionPicker.DimensionCount - 1)
    {
      right.x = 0;
    }


    if (bottom.y < 0)
    {
      bottom.y = 0;
    }

    if (top.y >= MaxChunks)
    {
      top.y = MaxChunks - 1;
    }


    var safeNeighbors = new List<int2>();
    if (chunkMap[top])
    {
      safeNeighbors.Add(top);
    }

    if (chunkMap[bottom])
    {
      safeNeighbors.Add(bottom);
    }

    if (chunkMap[left])
    {
      safeNeighbors.Add(left);
    }

    if (chunkMap[right])
    {
      safeNeighbors.Add(right);
    }


    return safeNeighbors;
  }

  int heuristic(int2 endPoint, int2 neighbor)
  {
    return Math.Abs(endPoint.x - neighbor.x) + Math.Abs(endPoint.y - neighbor.y);
  }

  List<int2> GetPath(int2 player_loc, int2 goal, Dictionary<int2, bool> chunkMap)
  {
    var start = new State
    {
      position = player_loc
    };

    var frontier = new FastPriorityQueue<State>(MaxChunks * DimensionPicker.AllDimensions.Length + 5);
    var cameFrom = new Dictionary<int2, int2>();
    var costSoFar = new Dictionary<int2, int>();

    frontier.Enqueue(start, 0);
    cameFrom.Add(start.position, new int2(-1, -1));
    costSoFar.Add(start.position, 0);


    State current_chunk = null;
    var inter = 0;
    while (frontier.Count > 0)
    {
      inter++;
      if (inter > MaxWhileLoopIters)
      {
        print("loop 3");
        break;
      }

      ;
      current_chunk = frontier.Dequeue();

      if (current_chunk.position.Equals(goal))
      {
        break;
      }

      var neighbors = GetNeighbors(current_chunk.position, chunkMap);
      foreach (var neighbor in neighbors)
      {
        var new_cost = 1;
        if (costSoFar.ContainsKey(neighbor))
        {
          new_cost += costSoFar[neighbor];
        }

        if (!costSoFar.ContainsKey(neighbor) || new_cost < costSoFar[neighbor])
        {
          costSoFar[neighbor] = new_cost;
          var priority = new_cost + heuristic(goal, neighbor);
          frontier.Enqueue(new State
          {
            position = neighbor
          }, priority);
          cameFrom[neighbor] = current_chunk.position;
        }
      }
    }


    var path = new List<int2>();
    if (!current_chunk.position.Equals(goal))
    {
      return path;
    }

    var current = goal;
    inter = 0;
    while (!current.Equals(new int2(-1, -1)) || !current.Equals(start.position))
    {
      inter++;
      if (inter > MaxWhileLoopIters)
      {
        print("loop 4");
        break;
      }

      if (current.x == -1 && current.y == -1)
      {
        break;
      }

      path.Add(current);
      current = cameFrom[current];
    }

    return path;
  }

  public void ResetChunks()
  {
    foreach (var dimension in DimensionPicker.AllDimensions)
    {
      foreach (var chunk in dimension.ActiveChunks)
      {
        Destroy(chunk);
      }
      dimension.ActiveChunks.Clear();
    }
    BuildInitialMap();
  }
  
  void BuildInitialMap()
  {
    foreach (var dimension in DimensionPicker.AllDimensions)
    {
      for (int y = 0; y < MaxChunks; y++)
      {
        dimension.SpawnChunk(true);
      }
    }
  }

  List<int2> GetEndLocations(Dictionary<int2, bool> chunkMap)
  {
    var endLocations = new List<int2>();
    for (int x = 0; x < DimensionPicker.AllDimensions.Length; x++)
    {
      var coord = new int2(x, MaxChunks - 1);
      if (chunkMap[coord])
      {
        endLocations.Add(coord);
      }
    }

    return endLocations;
  }


  void Start()
  {
    BuildInitialMap();
    if (DeathRatio >= 1)
      DeathRatio = 0.4f;
  }

  private List<bool> Shuffle(List<bool> ts)
  {
    var count = ts.Count;
    var last = count - 1;
    for (var i = 0; i < last; ++i)
    {
      var r = UnityEngine.Random.Range(i, count);
      var tmp = ts[i];
      ts[i] = ts[r];
      ts[r] = tmp;
    }

    return ts;
  }

  List<bool> GenerateRow()
  {
    var deathNum = Convert.ToInt32(Math.Round(DeathRatio * DimensionPicker.DimensionCount));
    if (deathNum == 0)
    {
      deathNum = 1;
    }
    var row = new List<bool>();
    for (int i = 0; i < deathNum; i++)
    {
      row.Add(false);
    }

    var inter = 0;
    while (row.Count < DimensionPicker.DimensionCount)
    {
      inter++;
      if (inter > MaxWhileLoopIters)
      {
        print("loop 1");
        break;
      }

      row.Add(true);
    }

    return Shuffle(row);
  }


  List<bool> AppendChunksToDimensions()
  {
    var chunkMap = BuildChunkMap();
    var isValidRow = false;

    var validRow = new List<bool>();
    var inter = 0;
    while (!isValidRow)
    {
      inter++;
      if (inter > MaxWhileLoopIters)
      {
        for (int _ = 0; _ < DimensionPicker.AllDimensions.Length; _++)
        {
          isValidRow = true;
          validRow.Add(true);
        }

        break;
        print("loop 2");
      }

      var newRow = GenerateRow();
      for (int i = 0; i < newRow.Count; i++)
      {
        chunkMap[new int2(i, MaxChunks - 1)] = newRow[i];
      }

      var endLocations = GetEndLocations(chunkMap);
      var validPaths = new List<List<int2>>();


      var startLoc = new int2(DimensionPicker.CurrentDimensionIndex, 0);

      foreach (var endLocation in endLocations)
      {
        var path = GetPath(startLoc, endLocation, chunkMap);
        if (path.Count > 0)
        {
          validPaths.Add(path);
        }
      }

      if (validPaths.Count == 0)
      {
        var backupRow = new List<bool>();
        foreach (var dimension in DimensionPicker.AllDimensions)
        {
          var chunk = dimension.ActiveChunks.ToList()[MaxChunks - 2];
          backupRow.Add(chunk);
        }

        validRow = backupRow;
        break;
      }




      var failedVerticalCheck = false;
      foreach (var path in validPaths)
      {
        if (failedVerticalCheck)
        {
          break;
        }
        var totalVerticalPoints = 0;
        var currentVerticalAxis = -1;
        
        foreach (var point in path)
        {
          if (currentVerticalAxis == -1)
          {
            currentVerticalAxis = point.x;
            totalVerticalPoints = 1;
            continue;
          }
          
          if (currentVerticalAxis == point.x)
          {
            totalVerticalPoints++;
            continue;
          }

          if (currentVerticalAxis != point.x)
          {
            if (totalVerticalPoints == 1)
            {
              currentVerticalAxis = point.x;
              continue;
            }

            if (totalVerticalPoints >= 2 && totalVerticalPoints > MinimumVerticalSegments)
            {
              totalVerticalPoints = 1;
              currentVerticalAxis = point.x;
              continue;
            }
            
            if ( totalVerticalPoints>=2 && totalVerticalPoints < MinimumVerticalSegments)
            {
              failedVerticalCheck = true;
              break;
            }
            
            
          }
        }
      }

      if (failedVerticalCheck)
      {
        continue;
      }

      var hasStraightPaths =  validPaths.Any(x => x.Count == MaxChunks);
      if (hasStraightPaths)
      {
        continue;
      }
      
      
      if (validPaths.Count < MinimumValidPaths)
      {
        continue;
      }
      
      //check that the paths have minimum vertical segments
      //check for minimum hoortizontal segments
      //diagnal segements threshold
      validRow = newRow;
      isValidRow = true;
    }

    return validRow;
  }


  Dictionary<int2, bool> BuildChunkMap()
  {
    var chunkMap = new Dictionary<int2, bool>();
    for (int x = 0; x < DimensionPicker.AllDimensions.Length; x++)
    {
      var dimension = DimensionPicker.AllDimensions[x];
      var chunks = dimension.ActiveChunks.ToList();
      for (int y = 0; y < MaxChunks - 1; y++)
      {
        chunkMap[new int2(x, y)] = chunks[y].GetComponent<Chunk>().isSafe;
      }
    }

    return chunkMap;
  }

  void SpawnChunkInDimensions(List<bool> newChunks)
  {
    for (int i = 0; i < newChunks.Count; i++)
    {
      DimensionPicker.AllDimensions[i].SpawnChunk(newChunks[i]);
    }
  }

  void RemoveExtraChunks(Dimension dimension)
  {
    if (dimension.ActiveChunks.Count > MaxChunks)
    {
      foreach (var extraChunk in dimension.ActiveChunks.Skip(MaxChunks))
      {
        Destroy(extraChunk);
      }

      dimension.ActiveChunks = new Queue<GameObject>(dimension.ActiveChunks.Take(MaxChunks));
    }
  }

  void SlideChunks(Dimension dimension)
  {
    foreach (var chunk in dimension.ActiveChunks)
    { 
      var distance = Time.deltaTime * ChunkSpeed * GameController.GameSpeed;
      chunk.transform.localPosition -= new Vector3(0, 0, distance);
      Stats.Distance += distance;
    }
  }

  void DespawnChunks(Dimension dimension)
  {
    if (dimension.ActiveChunks.Peek().transform.localPosition.z <= DespawnThreshold)
    {
      Destroy(dimension.ActiveChunks.Dequeue());
    }
  }

  bool RowDeleteCheck()
  {
    var allRowsMissingChunk = true;
    foreach (var dimension in DimensionPicker.AllDimensions)
    {
      if (dimension.ActiveChunks.Count < MaxChunks - 1)
      {
        allRowsMissingChunk = false;
        break;
      }
    }

    return allRowsMissingChunk;
  }

  void UpdateChunks()
  {
    if (RowDeleteCheck())
    {
      var new_chunks = AppendChunksToDimensions();
      SpawnChunkInDimensions(new_chunks);
    }

    foreach (var dimension in DimensionPicker.AllDimensions)
    {
      RemoveExtraChunks(dimension);
      SlideChunks(dimension);
      DespawnChunks(dimension);
    }
  }

  void Update()
  {
    UpdateChunks();
  }
}