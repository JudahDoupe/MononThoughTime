using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DimensionPicker : MonoBehaviour
{
    public float DimensionDistance = 50;
    public Dimension[] Dimensions;
    public int DimensionCount => Dimensions.Length;
    public int ActiveDimension = 0;

    void Start()
    {
        PickDimension(ActiveDimension);
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
        GetComponent<MotherChunker>().OnDimensionChange(Dimensions[dimensionIndex].gameObject);
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
