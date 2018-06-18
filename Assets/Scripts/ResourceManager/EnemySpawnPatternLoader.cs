using System.Collections.Generic;
using UnityEngine;

public static class EnemySpawnPatternLoader
{
    public static void LoadData(int themeID, ref EnemySpawnPatternData spawnPattern)
    {
        TextAsset data = Resources.Load<TextAsset>("EnemySpawnPattern" + themeID);
        string[] lines = data.text.Split(new char[] { '\n' });
        int lineNo = 0;

        char[] comma = new char[] { ',' };

        // Receive current enemy dictionary.
        int numEntry;
        int.TryParse(lines[lineNo++], out numEntry);
        ++lineNo; // Skip the field descriptions.
        Dictionary<int, string> dictionary = spawnPattern.EnemyDictionary = new Dictionary<int, string>();
        for (int i = 0; i < numEntry; ++i)
        {
            int enemyID;
            string[] entries = lines[lineNo++].Split(comma);
            int.TryParse(entries[0], out enemyID);
            dictionary.Add(enemyID, entries[1]);
        }

        // Receive pattern informations.
        while (lines[lineNo] != null)
        {
            string[] patternInfo = lines[lineNo++].Split(comma);
            if (!string.Equals(patternInfo[0].Trim(), "pattern")) return;
            ++lineNo;

            int patternLength;
            int.TryParse(patternInfo[2], out patternLength);
            EnemySpawnPattern pattern = new EnemySpawnPattern();
            pattern.Pattern = new List<EnemySpawnPatternEntry>();
            for (int i = 0; i < patternLength; ++i)
            {
                string[] entryString = lines[lineNo++].Split(comma);
                EnemySpawnPatternEntry entry = new EnemySpawnPatternEntry();
                int.TryParse(entryString[0], out entry.EnemyID);
                float.TryParse(entryString[1], out entry.SpawnX);
                float.TryParse(entryString[2], out entry.SpawnY);
                bool.TryParse(entryString[3], out entry.WaitForNextEntry);
                if (entry.WaitForNextEntry)
                {
                    float.TryParse(entryString[4], out entry.WaitAmount);
                }
                else
                {
                    entry.WaitAmount = 0.0f;
                }
                pattern.Pattern.Add(entry);
            }
        }
    }
}
