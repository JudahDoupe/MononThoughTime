using System.Collections.Generic;
using UnityEngine;

public class Brainard : MonoBehaviour
{
  // Start is called before the first frame update
  public long test = 111;
  public List<int> yo;

  private void Start()
  {
    yo = new List<int> {1, 1, 1, 1};
  }

  // Update is called once per frame
  private void Update()
  {
  }
}