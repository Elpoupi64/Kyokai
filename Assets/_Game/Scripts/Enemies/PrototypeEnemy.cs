using UnityEngine;

public class PrototypeEnemy : MonoBehaviour
{
    [SerializeField] private int health = 3;

    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log($"{gameObject.name} subit {amount} dégâts. PV restants : {health}");

        if (health <= 0)
            Destroy(gameObject);
    }
}
