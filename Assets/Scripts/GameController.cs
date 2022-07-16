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
        if (Input.GetKeyDown(KeyCode.A)
            || Input.GetKeyDown(KeyCode.D)
            || Input.GetKeyDown(KeyCode.LeftArrow)
            || Input.GetKeyDown(KeyCode.RightArrow))
        {
            switch (State)
            {
                case GameState.Ready:
                    StartGame();
                    break;
                case GameState.Dead:
                    ResetGame();
                    break;
            }
        }

        if (State == GameState.Running)
        {
            foreach (var dimension in DimensionPicker.AllDimensions)
            {
                dimension.chunkSpeed += Acceleration * Time.deltaTime;
            }
        }
    }

    public void StartGame()
    {
        EndScreen.SetActive(false);
        StartScreen.SetActive(false);
        State = GameState.Running;
        foreach (var dimension in DimensionPicker.AllDimensions)
        {
            dimension.chunkSpeed = StartingSpeed;
        }
        DimensionPicker.PickDimension(DimensionPicker.CurrentDimensionIndex);
    }

    public void EndGame()
    {
        EndScreen.SetActive(true);
        StartScreen.SetActive(false);
        State = GameState.Dead;
        foreach (var dimension in DimensionPicker.AllDimensions)
        {
            dimension.chunkSpeed = 0;
            dimension.GetComponent<AudioSource>().Stop();
        }
    }

    public void ResetGame()
    {
        EndScreen.SetActive(false);
        StartScreen.SetActive(true);
        State = GameState.Ready;
        foreach (var dimension in DimensionPicker.AllDimensions)
        {
            dimension.chunkSpeed = 0;
            dimension.GetComponent<AudioSource>().Stop();
        }
        FindObjectOfType<Brainard>().ResetPlayer();
    }

    public enum GameState
    {
        Ready,
        Running,
        Dead
    }
}
