using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    [SerializeField, Range(1, 10)] private int score = 1;
    [SerializeField] private UIPlayerStats ui;

    private int _totalScore;
    public void IncrementTotalScore()
    {
        _totalScore += score;
        ui.UpdateScore(_totalScore);
    }
}
