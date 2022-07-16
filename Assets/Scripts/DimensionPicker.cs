using System.Collections.Generic;
using UnityEngine;

public class DimensionPicker : MonoBehaviour
{
    private static DimensionPicker _instance;

    public float DimensionDistance = 50;
    public Dimension[] Dimensions;

    public static Dimension[] AllDimensions => _instance.Dimensions;
    public static Dimension CurrentDimension => _instance.Dimensions[CurrentDimensionIndex];
    public static int DimensionCount => _instance.Dimensions.Length;
    public static int CurrentDimensionIndex { get; private set; } = 0;

    void Awake()
    {
        _instance = this;
        PositionDimensions();
    }

    void Update()
    {
        if (GameController.State != GameController.GameState.Running) 
            return;

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            NextDimension();

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            PreviousDimension();
    }

    public static void NextDimension()
    {
        var newDimension = (CurrentDimensionIndex + 1) % DimensionCount;
        PickDimension(newDimension);
    }

    public static void PreviousDimension()
    {
        var newDimension = (DimensionCount + (CurrentDimensionIndex - 1)) % DimensionCount;
        PickDimension(newDimension);
    }

    public static void PickDimension(int index)
    {
        var currentAudio = CurrentDimension.GetComponent<AudioSource>();
        var currentTime = currentAudio.time;
        currentAudio.Stop();

        CurrentDimensionIndex = index;

        currentAudio = CurrentDimension.GetComponent<AudioSource>();
        currentAudio.Play();
        currentAudio.time = currentTime;

        PositionDimensions();
    }

    private static void PositionDimensions()
    {
        for (var i = 0; i < DimensionCount; i++)
        {
            var halfDimensionCount = DimensionCount / 2;
            var offset = (i - CurrentDimensionIndex + DimensionCount + halfDimensionCount) % DimensionCount - halfDimensionCount;
            
            var position = Vector3.right * offset * _instance.DimensionDistance;
            AllDimensions[i].transform.position = position;
        }
    } 
}
