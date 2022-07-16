using System.Collections.Generic;
using UnityEngine;

public class DimensionPicker : MonoBehaviour
{
    private static DimensionPicker _instance;

    public float SwitchSpeed = 5;
    public float DimensionDistance = 50;
    public Dimension[] Dimensions;

    public static Dimension[] AllDimensions => _instance.Dimensions;
    public static Dimension CurrentDimension => _instance.Dimensions[CurrentDimensionIndex];
    public static int DimensionCount => _instance.Dimensions.Length;
    public static int CurrentDimensionIndex { get; private set; } = 0;

    private static Dictionary<Dimension, Vector3> _targetPositions = new Dictionary<Dimension, Vector3>();

    void Awake()
    {
        _instance = this;
        PositionDimensions(true);
    }

    void Update()
    {
        if (GameController.State != GameController.GameState.Running) 
            return;

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            NextDimension();

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            PreviousDimension();

        foreach (var (dimension, targetPosition) in _targetPositions)
        {
            dimension.transform.localPosition = Vector3.Lerp(dimension.transform.localPosition, targetPosition, Time.deltaTime * SwitchSpeed);
        }
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

        Stats.DimensionChanges++;
    }

    private static void PositionDimensions(bool snap = false)
    {
        for (var i = 0; i < DimensionCount; i++)
        {
            var halfDimensionCount = DimensionCount / 2;
            var offset = (i - CurrentDimensionIndex + DimensionCount + halfDimensionCount) % DimensionCount - halfDimensionCount;
            
            var position = Vector3.right * offset * _instance.DimensionDistance;
            _targetPositions[AllDimensions[i]] = position;

            if (Vector3.Distance(AllDimensions[i].transform.localPosition, position) > _instance.DimensionDistance * 1.5f)
            {
                var shiftDirection = Vector3.Normalize(position - AllDimensions[i].transform.localPosition);
                AllDimensions[i].transform.localPosition = position + shiftDirection * _instance.DimensionDistance;
            }

            if (snap)
            {
                AllDimensions[i].transform.localPosition = position;
            }
        }
    } 
}
