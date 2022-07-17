using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Brainard : MonoBehaviour
{
    [Header("Health")]
    public int HP = 3;

    [Header("Health Visualization")]
    public List<Image> Hearts;
    public Sprite FullHeart;
    public Sprite EmptyHeart;

    [Header("Stumble")]
    public float StumbleSpeed = 1;
    public float StumbleExaggerate = 10;
    public AudioSource StumbleSound;

    [Header("Fall")]
    public float FallSpeed = 1;
    public AudioSource FallSound;


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Hazard") 
            && GameController.State == GameController.GameState.Running)
        {
            TakeDamage();
        }
    }

    public void ResetPlayer()
    {
        StopAllCoroutines();
        HP = Hearts.Count;
        UpdateVisualization();
        transform.localEulerAngles = Vector3.zero;
    }

    private void TakeDamage()
    {
        HP--;
        UpdateVisualization();
        if (HP <= 0)
        {
            FindObjectOfType<GameController>().EndGame();
            StopAllCoroutines();
            StartCoroutine(Fall());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(Stumble());
        }
    }

    private void UpdateVisualization()
    {
        for (var i = 0; i < Hearts.Count; i++)
        {
            Hearts[i].sprite = i < HP
                ? FullHeart
                : EmptyHeart;
        }
    }


    private IEnumerator Stumble()
    {
        StumbleSound.Play();
        var t = 0f;
        while (t < 1)
        {
            transform.localEulerAngles = new Vector3(EaseInElastic(t) * StumbleExaggerate, 0, EaseInElastic(t+ 0.2f) * StumbleExaggerate);
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime * StumbleSpeed;
        }

        transform.localEulerAngles = Vector3.zero;
    }

    private IEnumerator Fall()
    {
        FallSound.Play();
        var t = 0f;
        while (t < 1)
        {
            transform.localEulerAngles = new Vector3(EaseOutElastic(t) * 90, 0, EaseOutElastic(t) * 30);
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime * FallSpeed;
        }
        transform.localEulerAngles = new Vector3(90,0,30);
    }

    private float EaseInElastic(float t)
    {
        return 1 - EaseOutElastic(t);
    }
    private float EaseOutElastic(float t)
    {
        var c4 = (2f * math.PI) / 3f;
        return t == 0
            ? 0f
            : Math.Abs(t - 1) < 0.0001f
                ? 1
                : math.pow(2f, -10f * t) * math.sin((t * 10f - 0.75f) * c4) + 1;
    }
}