using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UnitStats", order = 1)]
public class UnitStats : ScriptableObject
{
    /// <summary>
    /// Hit Points
    /// </summary>
    public int hp;

    /// <summary>
    /// Total Hit Points
    /// </summary>
    public int maxHP;

    /// <summary>
    /// Unit's base attack power
    /// </summary>
    public int attackPower;

    /// <summary>
    /// How long between intervals before the unit can attack
    /// </summary>
    public float attackFrequency;

    /// <summary>
    /// Unit's base special power
    /// </summary>
    public int specialPower;

    /// <summary>
    /// How long between intervals before the unit can attack
    /// </summary>
    public float specialFrequency;

    /// <summary>
    /// Unit's current level
    /// </summary>
    public int level;

    /// <summary>
    /// Required experience points for next level 
    /// </summary>
    public int nextLevelExp;

    /// <summary>
    /// How much to increase stats by on level up
    /// </summary>
    public float levelUpMultiplier;


    /// <summary>
    /// How much to increase stats by on level up
    /// </summary>
    public float frequencyMultiplier = 0.98f;

    /// <summary>
    /// How fast the unit moves
    /// </summary>
    public float movementSpeed;
}
