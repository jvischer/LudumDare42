using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataManager {

    public static void verifyData() {
        if (!PlayerPrefs.HasKey(AppConsts.DATA_AUDIO_KEY)) {
            initializeData();
        }
    }

    private static void initializeData() {
        PlayerPrefs.SetInt(AppConsts.DATA_DIFFICULTY_KEY, 0);
        PlayerPrefs.SetInt(AppConsts.DATA_AUDIO_KEY, 1);

        PlayerPrefs.Save();
    }

	public static bool tryGetBool(string key, out bool result) {
        if (PlayerPrefs.HasKey(key)) {
            result = PlayerPrefs.GetInt(key) != 0;
            return true;
        }
        result = false;
        return false;
    }

    public static void setBool(string key, bool value) {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }

    public static int getDifficulty() {
        if (PlayerPrefs.HasKey(AppConsts.DATA_DIFFICULTY_KEY)) {
            return PlayerPrefs.GetInt(AppConsts.DATA_DIFFICULTY_KEY);
        }
        return 0;
    }

    public static void setDifficulty(int difficultyRating) {
        PlayerPrefs.SetInt(AppConsts.DATA_DIFFICULTY_KEY, difficultyRating);

        save();
    }

    public static void save() {
        PlayerPrefs.Save();
    }

}
