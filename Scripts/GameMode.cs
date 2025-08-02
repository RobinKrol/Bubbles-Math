using UnityEngine;

[System.Serializable]
public class OperationRange
{
    public int min;
    public int max;
}
public enum GameModeType { Easy, Normal, Hard, Expert, Master }

[System.Serializable]
public class GameModeParams
{
    public OperationRange plusRange;
    public OperationRange minusRange;
    public OperationRange multiplyRange;
    public OperationRange divideRange;
    public int digitMin;
    public int digitMax;
    public float bubbleSpawnInterval;
    public float scoreMultiplier;
    public float autoDestroyTime;
}

public class GameMode : MonoBehaviour
{
    public GameModeParams easyParams;
    public GameModeParams normalParams;
    public GameModeParams hardParams;
    public GameModeParams expertParams;
public GameModeParams masterParams;
    // В будущем можно добавить другие параметры: скорость пузырьков, количество одновременно и т.д.
} 