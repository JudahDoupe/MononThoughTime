using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DimensionPicker : MonoBehaviour
{
    public List<GameObject> Dimensions;
    public int DimensionCount => Dimensions.Count;
    public int ActiveDimension = 0;
    public float DimensionDistance = 50;

    void Start()
    {
        PickDimension(ActiveDimension);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            NextDimension();

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            PreviousDimension();
    }

    public void NextDimension()
    {
        var newDimension = (ActiveDimension + 1) % DimensionCount;
        PickDimension(newDimension);
    }

    public void PreviousDimension()
    {
        var newDimension = (DimensionCount + (ActiveDimension - 1)) % DimensionCount;
        PickDimension(newDimension);
    }

    public void PickDimension(int dimensionIndex)
    {
        ActiveDimension = dimensionIndex;
        PositionDimensions();
    }

    private void PositionDimensions()
    {
        for (var i = 0; i < DimensionCount; i++)
        {
            var dimensionIndex = (i + ActiveDimension) % DimensionCount;
            var position = Vector3.right * (i - (DimensionCount / 2)) * DimensionDistance;
            Dimensions[dimensionIndex].transform.position = position;
        }
    }
}
