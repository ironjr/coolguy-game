using UnityEngine;

public class BackgroundObject : WeightedPO
{
    public Vector2[] Ranges;

    public float GetRandomPosition()
    {
        if (Ranges != null && Ranges.Length > 0)
        {
            int len = Ranges.Length;
            float[] leftBound = new float[len--];
            leftBound[0] = 0.0f;
            float size = 0.0f;
            float interval;
            for (int i = 0; i < len; ++i)
            {
                interval = Ranges[i].y - Ranges[i].x;
                if (interval < 0)
                {
                    Debug.LogError("Range of a BackgroundObject " + i +
                        "is misspecified.");
                    return 0.0f;
                }
                leftBound[i + 1] = leftBound[i] + interval;
                size += interval;
            }
            interval = Ranges[len].y - Ranges[len].x;
            if (interval < 0)
            {
                Debug.LogError("Range of a BackgroundObject " + len +
                    "is misspecified.");
                return 0.0f;
            }
            size += interval;

            float pos = Random.Range(0, size);

            // Do simple linear search. Since the number of ranges will not
            // exceed 1st order.
            ++len;
            int idx;
            for (idx = 0; idx < len; ++idx)
            {
                if (pos < leftBound[idx]) break;
            }
            --idx;
            return Ranges[idx].x + pos - leftBound[idx];
        }
        else
        {
            Debug.LogError("Range of a BackgroundObject is not specified.");
            return 0.0f;
        }
    }
}
