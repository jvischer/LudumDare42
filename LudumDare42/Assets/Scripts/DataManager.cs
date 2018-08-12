using System;
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
        string scoreJSON = JsonUtility.ToJson(new List<string>());
        PlayerPrefs.SetString(AppConsts.DATA_SCORES_KEY, scoreJSON);
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

    //public static List<string> getRecentScores() {
    //    if (PlayerPrefs.HasKey(AppConsts.DATA_SCORES_KEY)) {
    //        string scoreJSON = PlayerPrefs.GetString(AppConsts.DATA_SCORES_KEY);
    //        return JsonUtility.FromJson<List<string>>(scoreJSON);
    //    }
    //    return new List<string>();
    //}

    //public static void logScore(float time) {
    //    string score = String.Format("{0}s - {1}", time.ToString("#.00"), AppConsts.DIFFICULTY_TO_TEXT[Mathf.Clamp(DataManager.getDifficulty(), 0, AppConsts.DIFFICULTY_TO_TEXT.Length)]);
    //    Debug.Log("Logging score " + score);

    //    List<string> recentScore = getRecentScores();
    //    recentScore.Add(score);
    //    string scoreJSON = JsonUtility.ToJson(recentScore, true);
    //    Debug.Log("storing JSON " + scoreJSON);
    //    PlayerPrefs.SetString(AppConsts.DATA_SCORES_KEY, scoreJSON);
    //}

    public static void save() {
        PlayerPrefs.Save();
    }

}
