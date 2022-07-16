using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotherChunker : MonoBehaviour
{
    public GameObject ActiveDimension;

    void Start()
    {
        OnDimensionChange(ActiveDimension);

    }

    void Update()
    {
    }

    public void OnDimensionChange(GameObject newDimension)
    {
        var currentTime = ActiveDimension.GetComponent<AudioSource>().time;
        ActiveDimension.GetComponent<AudioSource>().Stop();
        ActiveDimension = newDimension;
        ActiveDimension.GetComponent<AudioSource>().Play();
        ActiveDimension.GetComponent<AudioSource>().time = currentTime;
    }
}