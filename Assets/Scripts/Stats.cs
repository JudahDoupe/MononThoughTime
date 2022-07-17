using UnityEngine;

public class Stats : MonoBehaviour
{
    public static float Distance = 0;
    public static int DimensionChanges = 0;

    public static void ResetStats()
    {
        Distance = 0;
        DimensionChanges = 0;
    }

    void Update()
    {
        GetComponent<TMPro.TextMeshProUGUI>().text = 
$@"Distance Traveled: {Distance:n0}m
Dimensions Visited: {DimensionChanges}";
    }
}
