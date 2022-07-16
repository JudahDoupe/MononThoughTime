using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dimension : MonoBehaviour
{
  public List<GameObject> chunkPrefab;
  public int maxChunks;
  public int spawnZ => (maxChunks - 1)*15;
  public float chunkSpeed;
  public Queue<GameObject> activeChunks = new();
  public float despawnThreshold;

  public void Start()
  {
    for (int i = 0; i < maxChunks; i++)
    {
      activeChunks.Enqueue(Instantiate(chunkPrefab[0]));

    }

    var spaces = 0;

    foreach (var chunk in activeChunks)
    {
      chunk.transform.position = new Vector3(0, 0, spaces);
      spaces += 15;
    }
  }

  private void Update()
  {
    if (activeChunks.Count < maxChunks && activeChunks.Last().transform.position.z <= spawnZ)
    {
      var chunk_1 = Instantiate(chunkPrefab[0]);
      chunk_1.transform.position = new Vector3(0, 0, spawnZ);
      activeChunks.Enqueue(chunk_1);
    }


    foreach (var chunk in activeChunks)
    {
      chunk.transform.position = new Vector3(0, 0, chunk.transform.position.z - Time.deltaTime * chunkSpeed);
    }

    if (activeChunks.First().transform.position.z <= despawnThreshold)
    {
      Destroy(activeChunks.Dequeue());
    }
  }
}