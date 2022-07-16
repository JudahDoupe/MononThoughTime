using UnityEngine;

public class Brainard : MonoBehaviour
{
    public int HP = 3;


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Hazard"))
        {
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        HP--;
        if (HP <= 0)
        {
            FindObjectOfType<GameController>().EndGame();
        }
    }
}