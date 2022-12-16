using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public int health = 10;
    [SerializeField, Range(1, 10)] private int damage = 1;
    [SerializeField] private UIPlayerStats ui;

    public UnityEvent onGameOver;

    private void Start()
    {
        ui.UpdateHealth(health);
    }
    public void TakeDamage()
    {
        health -= damage;
        ui.UpdateHealth(health);
        if (health <= 0)
        {
            onGameOver.Invoke();
        }    
    }
}
