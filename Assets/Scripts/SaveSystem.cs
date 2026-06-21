using UnityEngine;

public static class SaveSystem
{
    public static int currentSaveSlot = 1;

    public static void SaveRating(int slotNumber, int ratingValue)
    {
        PlayerPrefs.SetInt($"Slot_{slotNumber}_Rating", ratingValue);
        PlayerPrefs.SetInt($"Slot_{slotNumber}_Exists", 1);
        PlayerPrefs.Save();
        Debug.Log($"[SaveSystem] slot number:{slotNumber} saved. Rank: {ratingValue} PTS");
    }

    public static int LoadRating(int slotNumber)
    {
        return PlayerPrefs.GetInt($"Slot_{slotNumber}_Rating", 100);
    }

    public static bool DoesSlotExist(int slotNumber)
    {
        return PlayerPrefs.GetInt($"Slot_{slotNumber}_Exists", 0) == 1;
    }

    public static void DeleteSlot(int slotNumber)
    {
        PlayerPrefs.DeleteKey($"Slot_{slotNumber}_Rating");
        PlayerPrefs.DeleteKey($"Slot_{slotNumber}_Exists");
        PlayerPrefs.Save();
        Debug.Log($"[SaveSystem] slot number:{slotNumber} full cleaned.");
    }
}
