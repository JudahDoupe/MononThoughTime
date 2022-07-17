using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject StartScreen;
    public GameObject EndScreen;

    public float StartingSpeed;
    public float Acceleration;

    public static GameState State { get; private set; } = GameState.Ready;

    void Start()
    {
        ResetGame();
    }

    void Update()
    {
        if (State == GameState.Ready)
        {
            if (Input.GetKeyDown(KeyCode.A)
                || Input.GetKeyDown(KeyCode.D)
                || Input.GetKeyDown(KeyCode.LeftArrow)
                || Input.GetKeyDown(KeyCode.RightArrow))
            {

                StartGame();
            }
        }
        else if (State == GameState.Dead)
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            {
                ResetGame();
            }
        }
        else if (State == GameState.Running)
        {
            GetComponent<MotherChunker>().ChunkSpeed += Acceleration * Time.deltaTime;

            Stats.Distance += DimensionPicker.CurrentDimension.chunkSpeed * Time.deltaTime;

        }
    }

    public void StartGame()
    {
        EndScreen.SetActive(false);
        StartScreen.SetActive(false);
        State = GameState.Running;

        GetComponent<MotherChunker>().ChunkSpeed = StartingSpeed;
        
        DimensionPicker.PickDimension(DimensionPicker.CurrentDimensionIndex);
    }

    public void EndGame()
    {
        EndScreen.SetActive(true);
        StartScreen.SetActive(false);
        State = GameState.Dead;
        GetComponent<MotherChunker>().ChunkSpeed = 0;
        foreach (var dimension in DimensionPicker.AllDimensions)
        {
            dimension.GetComponent<AudioSource>().Stop();
        }
    }

    public void ResetGame()
    {
        EndScreen.SetActive(false);
        StartScreen.SetActive(true);
        State = GameState.Ready;
        GetComponent<MotherChunker>().ChunkSpeed = 0;
        foreach (var dimension in DimensionPicker.AllDimensions)
        {
            dimension.GetComponent<AudioSource>().Stop();
        }
        FindObjectOfType<Brainard>().ResetPlayer();
        Stats.DimensionChanges = 0;
        Stats.Distance = 0;
    }

    public enum GameState
    {
        Ready,
        Running,
        Dead
    }
}
