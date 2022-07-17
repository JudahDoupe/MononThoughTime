using System;
using Unity.Mathematics;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject StartScreen;
    public GameObject PlayScreen;
    public GameObject EndScreen;

    [Range(0.0001f, 0.1f)]
    public float Acceleration;
    [Range(1, 10)]
    public float MaxSpeed = 10;
    public float MinAudioSpeed = 0.5f;
    public float MaxAudioSpeed = 2;

    public static float GameSpeed;

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
            GameSpeed = math.clamp(GameSpeed + (Acceleration * Time.deltaTime), 1, MaxSpeed);
            var audioSource = DimensionPicker.CurrentDimension.GetComponent<AudioSource>();

            audioSource.pitch = math.lerp(MinAudioSpeed, MaxAudioSpeed, GameSpeed / MaxSpeed);
        }

    }

    public void StartGame()
    {
        EndScreen.SetActive(false);
        StartScreen.SetActive(false);
        PlayScreen.SetActive(true);
        State = GameState.Running;
        GameSpeed = 1;

        DimensionPicker.PickDimension(DimensionPicker.CurrentDimensionIndex);
    }

    public void EndGame()
    {
        EndScreen.SetActive(true);
        StartScreen.SetActive(false);
        PlayScreen.SetActive(true);
        State = GameState.Dead;
        GameSpeed = 0;
        foreach (var dimension in DimensionPicker.AllDimensions)
        {
            dimension.GetComponent<AudioSource>().Stop();
        }
    }

    public void ResetGame()
    {
        EndScreen.SetActive(false);
        StartScreen.SetActive(true);
        PlayScreen.SetActive(false);
        State = GameState.Ready;
        GameSpeed = 0;
        foreach (var dimension in DimensionPicker.AllDimensions)
        {
            dimension.GetComponent<AudioSource>().Stop();
        }
        FindObjectOfType<Brainard>().ResetPlayer();
        GetComponent<MotherChunker>().ResetChunks();
        Stats.ResetStats();
    }

    public enum GameState
    {
        Ready,
        Running,
        Dead
    }
}
