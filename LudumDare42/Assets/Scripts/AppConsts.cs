using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class AppConsts {

    public static int[] RECYCLE_BIN_LIMIT_BY_DIFFICULTY = new int[] { 25, 20, 15 };

    public static string[] DIFFICULTY_TO_TEXT = new string[] { "Easy", "Medium", "1337" };
    public const string ADMIN_FMT = "Administrator - {0}";

    public const string DATA_SCORES_KEY = "Scores";
    public const string DATA_DIFFICULTY_KEY = "Diff";
    public const string DATA_AUDIO_KEY = "Audio";

    public const float RECYCLE_BIN_WIDTH_TOLERANCE = 0.75F;

    public const float PERCENT_OF_FILES_AS_ANTIVIRUS_TO_KILL_THE_VIRUS = 0.05F;

    public const float DESKTOP_MOVEMENT_NULLABLE_SQR_DISTANCE = 40.0F;

    public const int MISSING_FILE_ID = -1;
    public const int DEFAULT_FILE_ID = -2;

    public const float CLIPPY_INTRO_CONVERSATION_DELAY = 6.0F;
    public const float CLIPPY_ZIP_BOMB_CONVERSATION_DELAY = 2.5F;
    public const float CLIPPY_POST_ZIP_BOMB_HELP_CONVERSATION_DELAY = 45.0F;
    public const float CLIPPY_SCORE_CONVERSATION_DELAY = 1.0F;
    public const float CLIPPY_DESTROYED_RECYCLE_BIN_CONVERSATION_DELAY = 0.5F;

    public const float FILE_UNZIP_DURATION = 1.5F;

    public const string FILE_NAME_VIRUS_FMT = "lib {0}.zip";
    public const string FILE_NAME_ANTIVIRUS_FMT = "api-win-core l1-{0}.dll";

    /// <summary>
    /// Yes/No
    /// </summary>
    public const string CLIPPY_TEXT_INTRO = "Hi! I am Clippy, your office assistant. Would you like some assistance today?";

    /// <summary>
    /// Okay
    /// </summary>
    public const string CLIPPY_TEXT_INTRO_RESPONSE_YES = "I'm sorry, it seems I haven't had my morning coffee. I'll be back soon.";

    /// <summary>
    /// Yes/No
    /// </summary>
    public const string CLIPPY_TEXT_UNZIPPED = "It looks like you have opened a zip bomb! Would you like some assistance?";

    /// <summary>
    /// Okay
    /// </summary>
    public const string CLIPPY_TEXT_UNZIPPED_RESPONSE_YES = "Drag and drop files into the recycle bin to clear space on your drive.";

    /// <summary>
    /// Okay
    /// </summary>
    public const string CLIPPY_TEXT_UNZIPPED_RESPONSE_YES_2 = "Click and drag let's you delete multiple files at once. You can also right click to delete files!";

    /// <summary>
    /// Okay
    /// </summary>
    public const string CLIPPY_TEXT_UNZIPPED_RESPONSE_YES_3 = "The recycle bin will need to be cleared regularly. Don't let it or your drive fill up!";

    /// <summary>
    /// Okay
    /// </summary>
    public const string CLIPPY_TEXT_UNZIPPED_RESPONSE_YES_4 = "Try not to delete antivirus files as they are created, they will slow the virus down!";

    /// <summary>
    /// Okay
    /// </summary>
    public const string CLIPPY_TEXT_UNZIPPED_RESPONSE_YES_5 = "If half of your drive is made up of antiviruses then you will kill the virus!";

    /// <summary>
    /// Okay
    /// </summary>
    public const string CLIPPY_TEXT_HELP = "Please stop the virus! It's hurting me!";

    /// <summary>
    /// Okay
    /// </summary>
    public const string CLIPPY_TEXT_SCORE = "The virus is gone! It ran for {0} seconds.";

    /// <summary>
    /// Okay
    /// </summary>
    public const string CLIPPY_TEXT_SCORE_2 = "You fully deleted {0} virus files and {1} antivirus files.";

    /// <summary>
    /// Okay
    /// </summary>
    public const string CLIPPY_TEXT_DESTROYED_RECYCLE_BIN = "Oh no! You deleted the recycle bin! We're all doomed! Doomed!!!";

    public const string CLIPPY_TEXT_RESPONSE_NO = "Okay. Have a nice day!";

    public const string NOTIFICATION_ON_ANTIVIRUS_EXECUTED_HEADER = "Virus detected!";
    public const string NOTIFICATION_ON_ANTIVIRUS_EXECUTED_BODY = "Starting all defense processes.";
    public const string NOTIFICATION_ON_ZIP_BOMB_KILLED_HEADER = "Virus killed!";
    public const string NOTIFICATION_ON_ZIP_BOMB_KILLED_BODY = "All virus processes have been killed.";
    public const string NOTIFICATION_ON_RECYCLE_BIN_FILLED_HEADER = "Recycle bin is full!";
    public const string NOTIFICATION_ON_RECYCLE_BIN_FILLED_BODY = "You must empty it before you continue.";

}

[Serializable]
public class DifficultyData {

    [SerializeField] private float _spawnRampUpDuration;
    [SerializeField] private float _initialMinSpawnDelay;
    [SerializeField] private float _initialMaxSpawnDelay;
    [SerializeField] private float _endMinSpawnDelay;
    [SerializeField] private float _endMaxSpawnDelay;

    public float getWaitTime(float time) {
        float progress = Mathf.Clamp01(time / _spawnRampUpDuration);
        float minSpawnDelayDelta = _endMinSpawnDelay - _initialMinSpawnDelay;
        float maxSpawnDelayDelta = _endMaxSpawnDelay - _initialMaxSpawnDelay;
        return UnityEngine.Random.Range(_initialMinSpawnDelay + progress * minSpawnDelayDelta, _initialMaxSpawnDelay + progress * maxSpawnDelayDelta);
    }

}
