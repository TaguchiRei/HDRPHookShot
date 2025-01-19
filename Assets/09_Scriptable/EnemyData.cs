using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public List<EnemyAndNumber> enemyAndNumbers;
}
[Serializable]
public class EnemyAndNumber
{
    public List<EnemyDataFormat> enemy;
}
[Serializable]
public class EnemyDataFormat
{
    public GameObject enemy;
    public int number;
}