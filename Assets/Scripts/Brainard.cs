using UnityEngine;

public class Brainard : MonoBehaviour
{
    public int HP = 3;
    public float DamageFlashSpeed = 3;

    private Color _startingColor;
    private float _colorLerp = 0;
    private float _colorLerpT = 0;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Hazard"))
        {
            TakeDamage();
        }
    }

    void Start()
    {
        _startingColor = GetComponent<Renderer>().material.color;
    }

    void Update()
    {
        _colorLerpT += Time.deltaTime * DamageFlashSpeed;
        _colorLerp = HP switch
        {
            3 => 0,
            2 => Mathf.PingPong(_colorLerpT, 1),
            _ => 1,
        };

        GetComponent<Renderer>().material.color = Color.Lerp(_startingColor, Color.red, _colorLerp);
    }

    public void ResetPlayer()
    {
        HP = 3;
    }

    private void TakeDamage()
    {
        HP--;
        _colorLerpT = 0;
        if (HP <= 0)
        {
            FindObjectOfType<GameController>().EndGame();
        }
    }
}