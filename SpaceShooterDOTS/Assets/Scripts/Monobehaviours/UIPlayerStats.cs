using UnityEngine;
using TMPro;

public class UIPlayerStats : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI healthText;

    private void Start()
    {
        scoreText.SetText("Total score: " + 0);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void UpdateScore(int totalScore)
    {
        scoreText.SetText("Total score: " + totalScore);
    }

    public void UpdateHealth(int health)
    {
        healthText.SetText("Player health: " + health);
    }
}
