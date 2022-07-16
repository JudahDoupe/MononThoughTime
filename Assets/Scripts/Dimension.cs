using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// using System.Random;
using Random = UnityEngine.Random;

public class Dimension : MonoBehaviour
{
  public List<GameObject> chunkPrefab;
  public List<GameObject> dangerChunkPrefab;
  public int maxChunks;
  public float chunkSpeed;
  public Queue<GameObject> activeChunks = new();
  public float despawnThreshold;
  public float chunkSize = 15;

  private Vector3 NextSpawnPosition() => new(0, 0, (activeChunks.LastOrDefault()?.transform.localPosition.z ?? -chunkSize) + chunkSize);

  private void spawnChunk()
  {
    var chunkType = Random.Range(-1, 5);
    
    var chunk = chunkType >= 0
      ? Instantiate(chunkPrefab[0], transform)
      : Instantiate(dangerChunkPrefab[0], transform);
    chunk.transform.localPosition = NextSpawnPosition();
    
    activeChunks.Enqueue(chunk);
  }
  
  private void Update()
  {
    while (activeChunks.Count < maxChunks)
    {
      spawnChunk();
    }
    
    if (activeChunks.Count > maxChunks)
    {
      foreach (var extraChunk in activeChunks.Skip(maxChunks))
      {
        Destroy(extraChunk);
      }
      activeChunks = new Queue<GameObject>(activeChunks.Take(maxChunks));
    }
    
    foreach (var chunk in activeChunks)
    {
      chunk.transform.localPosition = new Vector3(0, 0, chunk.transform.localPosition.z - Time.deltaTime * chunkSpeed);
    }

    if (activeChunks.Peek().transform.localPosition.z <= despawnThreshold)
    {
      Destroy(activeChunks.Dequeue());
    }
  }
}