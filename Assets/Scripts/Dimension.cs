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
  public int spawnZ => (maxChunks - 1)*15;
  public float chunkSpeed;
  public Queue<GameObject> activeChunks = new();
  public float despawnThreshold;

  public void Start()
  {
    for (int i = 0; i < maxChunks; i++)
    {
      spawnChunk();
    }

    var spaces = 0;

    foreach (var chunk in activeChunks)
    {
      chunk.transform.localPosition = new Vector3(0, 0, spaces);
      spaces += 15;
    }
  }

  private GameObject spawnChunk()
  {


    var chunkType = Random.Range(-1, 1);
    print(chunkType.ToString());
    activeChunks.Enqueue(chunkType == 0
      ? Instantiate(chunkPrefab[0], transform)
      : Instantiate(dangerChunkPrefab[0], transform));

    return activeChunks.Last();
  }
  
  private void Update()
  {
    if (activeChunks.Count < maxChunks && activeChunks.Last().transform.localPosition.z <= spawnZ)
    {
      var chunk_1 = spawnChunk();
      chunk_1.transform.localPosition = new Vector3(0, 0, spawnZ);
    }


    foreach (var chunk in activeChunks)
    {
      chunk.transform.localPosition = new Vector3(0, 0, chunk.transform.position.z - Time.deltaTime * chunkSpeed);
    }

    if (activeChunks.First().transform.localPosition.z <= despawnThreshold)
    {
      Destroy(activeChunks.Dequeue());
    }
  }


}