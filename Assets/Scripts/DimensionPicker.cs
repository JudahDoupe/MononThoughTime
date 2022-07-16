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

    void Start()
    {
        _instance = this;
        PickDimension(CurrentDimensionIndex);
    }

    void Update()
    {
        Dimensions = transform.GetComponentsInChildren<Dimension>();
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            NextDimension();

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            PreviousDimension();
    }

    public void NextDimension()
    {
        var newDimension = (CurrentDimensionIndex + 1) % DimensionCount;
        PickDimension(newDimension);
    }

    public void PreviousDimension()
    {
        var newDimension = (DimensionCount + (CurrentDimensionIndex - 1)) % DimensionCount;
        PickDimension(newDimension);
    }

    public void PickDimension(int index)
    {
        var currentAudio = CurrentDimension.GetComponent<AudioSource>();
        var currentTime = currentAudio.time;
        currentAudio.Stop();

        CurrentDimensionIndex = index;

        currentAudio = CurrentDimension.GetComponent<AudioSource>();
        currentAudio.Play();
        currentAudio.time = currentTime;

        for (var i = 0; i < DimensionCount; i++)
        {
            var dimensionIndex = (i + CurrentDimensionIndex) % DimensionCount;
            var position = Vector3.right * (i - (DimensionCount / 2)) * DimensionDistance;
            Dimensions[dimensionIndex].transform.position = position;
        }
    }

}
