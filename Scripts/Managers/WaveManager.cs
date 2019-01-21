using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Creates, updates, and manages the current wave
/// Handles re-loading the wave on player death keeping track on units
/// defeated to reduce reward given.
/// </summary>
public class WaveManager : MonoBehaviour
{
    /// <summary>
    /// Singleton
    /// </summary>
    public static WaveManager instance;

    /// <summary>
    /// Where to spawn the unit
    /// </summary>
    [SerializeField]
    Transform m_spawnPoint;

    /// <summary>
    /// How many seconds to wait before spawning the next unit
    /// </summary>
    [SerializeField, Tooltip("In seconds, how long to wait to spawn the next unit")]
    float m_waveFrequency = 1f;

    /// <summary>
    /// A reference to the text component that displays the current wave
    /// </summary>
    [SerializeField]
    Text m_waveCounter;

    /// <summary>
    /// A collection of all the units that we can create
    /// </summary>
    [SerializeField]
    List<EnemyUnit> m_baseUnits = new List<EnemyUnit>();

    /// <summary>
    /// A list of the bosses
    /// </summary>
    [SerializeField]
    List<EnemyUnit> m_bossUnits = new List<EnemyUnit>();

    /// <summary>
    /// Keeps track of the next unit type to load
    /// </summary>
    Queue<EnemyUnit> m_bossQueue = new Queue<EnemyUnit>();

    /// <summary>
    /// The current active enemies for this wave
    /// </summary>
    Queue<EnemyUnit> m_waveQueue;

    /// <summary>
    /// How much to reduce the total EXP an enemy give each time is defeated
    /// </summary>
    float m_expDecrement = .95f;

    /// <summary>
    /// Keeps the master instances of all the unique unit types
    /// Since all enemies are copies of these we simply clone the 
    /// unit we want to load and save ourselves some memory 
    /// </summary>
    Dictionary<string, GameObject> m_units = new Dictionary<string, GameObject>();

    /// <summary>
    /// A container for the current wave of enemies gameobject
    /// </summary>
    GameObject m_waveGO;

    /// <summary>
    /// Keeps track of all enemies spawned for the current wave
    /// that have not been deated. Once this hit zero, it triggers
    /// The next wave creation
    /// </summary>
    List<EnemyUnit> m_activeUnits;

    /// <summary>
    /// Keeps track of the next unit type to load
    /// </summary>
    List<Type> m_nextType;

    /// <summary>
    /// Sets references
    /// </summary>
    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Initialize
    /// Builds the list of units we can clone
    /// </summary>
    void Start()
    {
        m_nextType = new List<Type>();
        m_waveGO = new GameObject("_WaveUnits");

        foreach (EnemyUnit unit in m_baseUnits) {
            m_nextType.Add(unit.GetType());
            m_units.Add(unit.GetType().Name, unit.gameObject);
        }
    }

    /// <summary>
    /// Builds the given wave with the total units given
    /// These are store in the wave queue and active units list
    /// </summary>
    /// <param name="wave"></param>
    /// <param name="totalEnemies"></param>
    public void BuildWave(int wave, int totalEnemies)
    {
        m_waveQueue = new Queue<EnemyUnit>();

        // We will use the average level of the party
        // as the level of the enemy
        int level = PartyManager.instance.AverageLevel * wave;

        // We are still showing the individual units
        if (wave <= m_nextType.Count) {
            DequeueBaseUnit(wave, level, totalEnemies);
        } else {
            CreateRandomWave(level, totalEnemies);
        }

        m_activeUnits = new List<EnemyUnit>(m_waveQueue);
        UpdateWaveCounter(wave);
        EXPManager.instance.UpdateEnemyExpForWave(wave);
    }

    /// <summary>
    /// Creates the next unit waiting to be shown for the first time
    /// </summary>
    /// <param name="wave"></param>
    /// <param name="totalEnemies"></param>
    void DequeueBaseUnit(int wave, int level, int totalEnemies)
    {
        Type nextType = m_nextType[wave - 1];

        Dictionary<string, LevelUpMetada> levelData = new Dictionary<string, LevelUpMetada>();

        for (int i = 0; i < totalEnemies; i++) {
            EnemyUnit unit = BuildUnit(nextType.Name);

            if (unit != null) {

                // Because stats are initialized on unit creation with random base values
                // we want to zero them out so that each unit of the same type has the same values
                unit.Stats.SetStatsToZero();

                if (!levelData.ContainsKey(unit.name)) {
                    levelData[unit.name] = unit.CreateLevelUp(level);
                }

                LevelUpMetada data = levelData[unit.name];

                unit.LevelUp(data);

                // Ensure we are using the max health from the level up
                // as well as setting the starting HP to the max HP
                unit.Stats[StatsId.HP_Max] = data.stats[StatsId.HP_Max];
                unit.Stats[StatsId.HP_Cur] = unit.Stats[StatsId.HP_Max];

                // Trigger stats change to update the health bars
                unit.Stats = unit.Stats;

                m_waveQueue.Enqueue(unit);
            }
        }
    }

    /// <summary>
    /// Creates random units for the wave
    /// </summary>
    /// <param name="wave"></param>
    /// <param name="totalEnemies"></param>
    void CreateRandomWave(int level, int totalEnemies)
    {
        // Keeps track of stats built for a specific unit type so that 
        // all units of that types share the same stats
        Dictionary<string, LevelUpMetada> levelData = new Dictionary<string, LevelUpMetada>();

        for (int i = 0; i < totalEnemies; i++) {
            // Select a random unit to create
            int index = UnityEngine.Random.Range(0, m_baseUnits.Count);
            EnemyUnit unit = BuildUnit(m_baseUnits[index].GetType().Name);

            if (unit != null) {
                // Because stats are initialized on unit creation with random base values
                // we want to zero them out so that each unit of the same type has the same values
                unit.Stats.SetStatsToZero();

                if (!levelData.ContainsKey(unit.name)) {
                    levelData[unit.name] = unit.CreateLevelUp(level);
                }

                LevelUpMetada data = levelData[unit.name];
                unit.LevelUp(data);

                m_waveQueue.Enqueue(unit);
            }
        }
    }

    /// <summary>
    /// While there are enemies enqueue, spawns them at internals
    /// </summary>
    /// <returns></returns>
    public IEnumerator SpawnWaveRoutine()
    {
        while (m_waveQueue.Count > 0 && !GameManager.instance.GameOver) {
            EnemyUnit unit = m_waveQueue.Dequeue();
            EnemyManager.instance.AddUnit(unit);

            // Because stats calculation may still be happening
            // We will wait for one frame before triggering a stats change
            // to ensure that the UI for the enemy's HP updates at the right time
            yield return new WaitForEndOfFrame();
            unit.Stats = unit.Stats;

            // Wait to spawn the next unit
            yield return new WaitForSeconds(m_waveFrequency);
        }
    }

    /// <summary>
    /// Updates the text of the wave counter with the current wave number
    /// </summary>
    /// <param name="wave"></param>
    void UpdateWaveCounter(int wave)
    {
        if(m_waveCounter != null) {
            m_waveCounter.text = string.Format("Wave {0}", wave);
        }
    }

    /// <summary>
    /// Instantiates the given unit type its recognized
    /// </summary>
    /// <param name="unitType"></param>
    /// <returns></returns>
    EnemyUnit BuildUnit(string unitType)
    {
        EnemyUnit unit = default(EnemyUnit);

        if (m_units.ContainsKey(unitType)) {
            GameObject go = Instantiate(m_units[unitType], m_waveGO.transform);
            go.transform.position = m_spawnPoint.position;
            unit = go.GetComponent<EnemyUnit>();
        }

        return unit;
    }

    /// <summary>
    /// Removes the given unit from the active list
    /// If this is the last unit on the list then a new wave is to be triggered
    /// </summary>
    /// <param name="unit"></param>
    public void EnemyDefeated(EnemyUnit unit)
    {
        EnemyManager.instance.RemoveUnit(unit);

        if (m_activeUnits.Contains(unit)) {
            m_activeUnits.Remove(unit);   

            if (m_activeUnits.Count < 1) {
                GameManager.instance.WaveCompleted = true;
            }
        }
    }

    /// <summary>
    /// Destroy all active units
    /// </summary>
    public void DestroyCurrentWave()
    {
        foreach (EnemyUnit unit in FindObjectsOfType<EnemyUnit>()) {
            unit.Stats[StatsId.HP_Cur] = 0;
            EnemyManager.instance.RemoveUnit(unit);
            Destroy(unit.gameObject);
        }
    }
}