using UnityEngine;

public static class RandomFlip2D
{
    public static Quaternion NextFlipX()
    {
        return Quaternion.Euler(Random.Range(0, 2) * 180.0f, 0, 0);
    }

    public static Quaternion NextFlipY()
    {
        return Quaternion.Euler(0, Random.Range(0, 2) * 180.0f, 0);
    }

    public static Quaternion NextRotsqZ()
    {
        return Quaternion.Euler(0, 0, Random.Range(0, 4) * 90.0f);
    }

    public static Quaternion NextFlipXY()
    {
        int random = Random.Range(0, 4);
        int flipX = random & 0x1;
        int flipY = random & 0x2;
        return Quaternion.Euler(flipX * 180.0f, flipY * 180.0f, 0);
    }

    public static Quaternion NextFlipXYRotsqZ()
    {
        int random = Random.Range(0, 16);
        int flipX = random & 0x1;
        int flipY = random & 0x2;
        int rotate = random >> 2;
        return Quaternion.Euler(flipX * 180.0f, flipY * 180.0f, rotate * 90.0f);
    }
}
