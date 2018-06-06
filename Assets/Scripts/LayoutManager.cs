using System.Collections.Generic;
using UnityEngine;

public class LayoutManager : MonoBehaviour
{
    public GameObject[] DigitObjects;
    public float DigitDistance = 0.6f;
    public Transform ScoreContainerTransform;

    public void DisplayScore(int score)
    {
        // Remove current digits from the panel.
        foreach (Transform child in ScoreContainerTransform)
        {
            Destroy(child.gameObject);
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
            GameObject digit = Instantiate(DigitObjects[digits[index]], ScoreContainerTransform);
            digit.transform.localPosition = new Vector3(xOffset, 0);
            xOffset -= DigitDistance;
            ++index;
        }
    }
}
