using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dimension : MonoBehaviour
{
  public List<GameObject> chunkPrefab;
  public List<GameObject> dangerChunkPrefab;

  private float ChunkSize => GetComponentInParent<MotherChunker>().ChunkSize;
  public Queue<GameObject> ActiveChunks = new();
  
  private Vector3 NextSpawnPosition() => new(0, 0, (ActiveChunks.LastOrDefault()?.transform.localPosition.z ?? -ChunkSize) + ChunkSize);

  public void SpawnChunk(bool isSafe)
  {
    var chunk = isSafe
      ? Instantiate(chunkPrefab[Convert.ToInt32(UnityEngine.Random.Range(0,chunkPrefab.Count))], transform)
      : Instantiate(dangerChunkPrefab[Convert.ToInt32(UnityEngine.Random.Range(0, dangerChunkPrefab.Count))], transform);
    chunk.transform.localPosition = NextSpawnPosition();
    chunk.GetComponent<Chunk>().isSafe = isSafe;
    
    ActiveChunks.Enqueue(chunk);
  }
}