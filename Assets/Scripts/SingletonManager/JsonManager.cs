using UnityEngine;

public class JsonManager : Singleton<JsonManager>
{
    public string Filename = "gamedata.json";
    public uint PersonalHighScore = 0u;
    public int StartThemeID = 0;
    public uint Level = 0u;
    public string Username = "The Cool Guy";

    private string _path;

	void Awake()
    {
        _path = Application.persistentDataPath + "/" + Filename;
	}

    public void SaveData()
    {
        JsonWrapper wrapper = new JsonWrapper();
        wrapper.PersonalHighScore = PersonalHighScore;
        wrapper.StartThemeID = StartThemeID;
        wrapper.Level = Level;
        wrapper.Username = Username;

        string contents = JsonUtility.ToJson(wrapper, true); // TODO Set this to false for optimization.
        System.IO.File.WriteAllText(_path, contents);
        Debug.Log("Data successfully stored at " + Time.time);
    }

    public void LoadData()
    {
        try
        {
            if (System.IO.File.Exists(_path))
            {
                string contents = System.IO.File.ReadAllText(_path);
                JsonWrapper wrapper = JsonUtility.FromJson<JsonWrapper>(contents);

                PersonalHighScore = wrapper.PersonalHighScore;
                StartThemeID = wrapper.StartThemeID;
                Level = wrapper.Level;
                Username = wrapper.Username;
                Debug.Log("Data successfully loaded at " + Time.time);
            }
            else
            {
                Debug.Log("Unable to read the save data. File " + Filename + " does not exist.");
                PersonalHighScore = 0u;
                StartThemeID = 0;
                Level = 0u;
                Username = "The Cool Guy";
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}
