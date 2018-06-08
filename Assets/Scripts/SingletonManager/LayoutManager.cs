using System.Collections.Generic;
using UnityEngine;

public class LayoutManager : Singleton<LayoutManager>
{
    public GameObject GameOverUIObject;
    public PooledObject[] DigitObjects;
    public float DigitDistance = 0.6f;
    public Transform ScoreContainerTransform;
    public Transform HighscoreBoardTransform;
    
    public void DisplayScore(int score)
    {
        DisplayScore(score, ScoreContainerTransform);
    }

    public void DisplayScore(int score, Transform container)
    {
        // Remove current digits from the panel.
        foreach (Transform child in container)
        {
            PooledObject po = child.GetComponent<PooledObject>();
            po.ReturnToPool();
        }

        // Process the current score.
        List<int> digits = new List<int>();
        do
        {
            digits.Add(score % 10);
            score /= 10;
        }
        while (score != 0);

        // Display the current score to the panel.
        int numDigits = digits.Count;
        int index = 0;
        float xOffset = DigitDistance * (numDigits - 1) / 2.0f;
        while (index < numDigits)
        {
            PooledObject digit = DigitObjects[digits[index]].GetObject();
            Transform digitTransform = digit.transform;
            digitTransform.SetParent(container);
            digitTransform.localPosition = new Vector3(xOffset, 0);
            xOffset -= DigitDistance;
            ++index;
        }
    }

    public void DisplayGameOver(uint score, uint highScore)
    {
        GameOverUIObject.SetActive(true);
        ScoreContainerTransform.position = new Vector3(0, 2);
    }
}
