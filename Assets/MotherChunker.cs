using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotherChunker : MonoBehaviour
{
    public float CurrentAudioTime;
    private float ClipLength = 19.492f;
    public GameObject ActiveDimension;

    void Start()
    {
        OnDimensionChange(ActiveDimension);

    }

    void Update()
    {
        CurrentAudioTime = Time.deltaTime / ClipLength;
    }

    void OnDimensionChange(GameObject newDimension)
    {
        ActiveDimension.GetComponent<AudioSource>().Stop();
        ActiveDimension = newDimension;
        ActiveDimension.GetComponent<AudioSource>().time = CurrentAudioTime;
        ActiveDimension.GetComponent<AudioSource>().Play();
    }
}