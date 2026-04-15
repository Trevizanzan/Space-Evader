using System;

[Serializable]
public class LevelAttempt
{
    public string timestamp;       // quando è avvenuto il tentativo
    public string levelName;       // nome del livello (es. "Level_03_Fighters")
    public int levelIndex;         // indice numerico del livello
    public float timeAlive;        // quanto ha sopravvissuto IN quel livello
    public int score;              // score al momento del game over / completamento
    public int enemiesKilled;
    public int bossesKilled;
    public int asteroidsDestroyed;
    public int shotsFired;
    public int damageTaken;
    public bool completedLevel;    // ha completato il livello o è morto?
    //public int loopCount;          // in quale loop si trovava
}

[Serializable]
public class AllStats
{
    public System.Collections.Generic.List<LevelAttempt> attempts = new();
}