using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Interface base class for player and enemy units
/// A unit has stats, graphics, weapons, and common functionality 
/// </summary>
[RequireComponent(typeof(Animator))]
public abstract class Unit : MonoBehaviour
{
    /// <summary>
    /// The name of the unit
    /// </summary>
    protected string m_name;
    public string UnitName { get { return m_name; } }

    /// <summary>
    /// The action method to trigger after waiting routine it done
    /// </summary>
    protected delegate void ActionDelegate();

    /// <summary>
    /// A reference to the animator controller for this unit
    /// </summary>
    protected Animator m_animator;

    /// <summary>
    /// The game sprite
    /// </summary>
    protected Sprite m_sprite;

    /// <summary>
    /// UI sprite
    /// </summary>
    protected Sprite m_icon;

    /// <summary>
    /// The layer mask that this unit targets for attacks
    /// </summary>
    protected LayerMask m_targetLayer;
    public LayerMask TargetLayer { get { return m_targetLayer; } }

    /// <summary>
    /// Unit's current stats
    /// </summary>
    [SerializeField]
    StatData m_stats;
    public virtual StatData Stats
    {
        get {
            if (!m_stats.IsInitialized) {
                m_stats.Init();
            }

            return m_stats;
        }
        set {
            m_stats = value;
            OnStatsChanged();
        }
    }

    /// <summary>
    /// Experience growth rate
    /// </summary>
    [SerializeField, Tooltip("Base experience growth rate")]
    protected GrowthRate m_experienceRate;
    int m_baseExperience = 0;

    /// <summary>
    /// The base experience to use for this unit's growth
    /// </summary>
    public int BaseExperience
    {
        get {
            if (m_baseExperience == 0) {
                Dice dice = Growth.GetDiceForRate(m_experienceRate);
                m_baseExperience = dice.Roll();
            }
            return m_baseExperience;
        }
    }

    /// <summary>
    /// This unit's weapon range
    /// </summary>
    [SerializeField]
    protected WeaponRange m_range;
    public WeaponRange Range { get { return m_range; } }

    /// <summary>
    /// The projectile this unit can fire
    /// </summary>
    [SerializeField]
    protected Projectile m_projectile;

    /// <summary>
    /// Where to spawn the projectile at
    /// </summary>
    [SerializeField]
    protected Transform m_projectileSpawnPoint;

    /// <summary>
    /// The visual effect for display hp gained or lost
    /// </summary>
    [SerializeField]
    protected GameObject m_hpDisplayPrefab;

    /// <summary>
    /// Where to spawn the hp display at
    /// </summary>
    [SerializeField]
    protected Transform m_hpDisplaySpawnPoint;

    /// <summary>
    /// How much to set the flash effect to 
    /// </summary>
    [SerializeField, Range(0f, 1f)]
    protected float m_flashAmount = .25f;

    /// <summary>
    /// What color to set the flash effect for
    /// </summary>
    [SerializeField]
    protected Color m_flashColor = Color.white;

    /// <summary>
    /// How long to keep the flash effect active
    /// </summary>
    [SerializeField]
    protected float m_flashDuration = .10f;

    /// <summary>
    /// A reference to the sprite renderer
    /// </summary>
    [SerializeField]
    protected SpriteRenderer m_renderer;

    /// <summary>
    /// A reference to the cooldown timer used to trigger attacks
    /// </summary>
    [SerializeField]
    protected CoolDownTimer m_attackTimer;
    public CoolDownTimer AttackTimer { get { return m_attackTimer; } }

    /// <summary>
    /// A reference to the cooldown timer used to trigger specials
    /// </summary>
    [SerializeField]
    protected CoolDownTimer m_specialTimer;
    public CoolDownTimer SpecialTimer { get { return m_specialTimer; } }

    /// <summary>
    /// Returns the damage this unit can inflict
    /// </summary>
    public int Damage
    {
        get {
            return Stats.GetStat(StatsId.Attack);
        }
    }

    /// <summary>
    /// While true the enemy logic runs
    /// </summary>
    bool m_isActive = false;
    public virtual bool IsActive
    {
        get { return m_isActive; }
        set {
            m_isActive = value;

            if (!m_isActive) {
                StopAll();
            } else {
                AIManager.instance.ResumeTimer(m_attackTimer);
                AIManager.instance.ResumeTimer(m_specialTimer);
            }
        }
    }

    /// <summary>
    /// True when the animator notifies the unit to fire project
    /// </summary>
    protected bool m_fireProjectile = false;

    /// <summary>
    /// Prevents flashing to happen while it is stil flashing
    /// </summary>
    protected bool m_isFlashing = false;

    /// <summary>
    /// Contains active animator triggers to prevent playing them again
    /// </summary>
    List<string> m_triggers = new List<string>();

    /// <summary>
    /// Initialization
    /// </summary>
    abstract public void Init();

    /// <summary>
    /// Triggered when the stats of this unit change
    /// </summary>
    abstract protected void OnStatsChanged();

    /// <summary>
    /// Handles how the unit attacks
    /// </summary>
    protected abstract void Attack();

    /// <summary>
    /// Handles how the unit invokes specials
    /// </summary>
    protected abstract void Special();

    /// <summary>
    /// Using the dice defined for each stats growth returns Stats with 
    /// the results of the rolls for each growth dice
    /// </summary>
    /// <returns></returns>
    StatData GetStatsGrowth()
    {
        StatData stats = new StatData();

        foreach (KeyValuePair<StatsId, Dice> growth in Stats.Growths) {
            StatsId id = growth.Key;
            Dice dice = growth.Value;
            stats[id] = dice.Roll();
        }

        return stats;
    }

    /// <summary>
    /// Allows the Awake() to be overriten
    /// We have to copy the stats to avoid override the originals as they are 
    /// scriptable objects and have permanent changes. Meaning changes in play mode
    /// carry over to edit mode
    /// </summary>
    protected virtual void Awake()
    {
        m_animator = GetComponent<Animator>();

        OnStatsChanged();
        SetupTimers();

        if(m_renderer == null) {
            m_renderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    /// <summary>
    /// Calculates the EXP required for the next level because at this point
    /// the EXPManager has been assigned
    /// Triggers initialization
    /// </summary>
    protected virtual void Start()
    {
        // Ensures the unit's current HP is its max
        Stats[StatsId.HP_Cur] = Stats[StatsId.HP_Max];
        Stats.NextLevelExp = EXPManager.instance.NextLevelEXP(Stats.Level, BaseExperience);
        Init();
    }

    /// <summary>
    /// Creates and binds the cooldown timers
    /// Does not start them as this is controlled by the unit
    /// </summary>
    protected virtual void SetupTimers()
    {
        // Creates default timers
        m_attackTimer = new CoolDownTimer(Stats[RateId.Attack]);
        m_specialTimer = new CoolDownTimer(Stats[RateId.Special]);

        m_attackTimer.OnTimerCompletedAction = AttackAction;
        m_specialTimer.OnTimerCompletedAction = SpecialAction;
    }

    /// <summary>
    /// Triggerd when the cooldown complete
    /// Triggers the routine to wait until a target is available to attack
    /// </summary>
    public void AttackAction()
    {
        if (IsActive) {
            StartCoroutine(WaitToInvokeTrigger("Attack", "Special", Attack));
        }
    }

    /// <summary>
    /// Trigger by the cool down when the timer reaches its target
    /// </summary>
    public virtual void SpecialAction()
    {
        if (IsActive) {
            StartCoroutine(WaitToInvokeTrigger("Special", "Attack", Special));
        }
    }

    /// <summary>
    /// Waits until the unit has a target before attacking
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitToInvokeTrigger(string trigger, string waitForTrigger, ActionDelegate Action)
    {
        // If special is being used ... wait
        while (m_triggers.Contains(waitForTrigger) && IsActive) {
            yield return null;
        }

        // Wait for a target to become available
        if (IsActive) {
            List<Unit> targets = AIManager.instance.GetTargetsInRange(this);
            while (IsActive && targets.Count < 1 && IsActive) {
                targets = AIManager.instance.GetTargetsInRange(this);
                if (targets.Count < 1) {
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        // Trigger the action
        if (IsActive) {
            Action();
        }
    }

    /// <summary>
    /// Sets the given trigger in the animator
    /// Prevents the re-trigger while the trigger is still active
    /// </summary>
    /// <param name="name"></param>
    protected void SetAnimatorTrigger(string name)
    {
        if (!m_triggers.Contains(name)) {
            if (m_animator != null) {
                m_triggers.Add(name);
                m_animator.SetTrigger(name);
            }
        }
    }

    /// <summary>
    /// Removes the given trigger from the active trigger list
    /// </summary>
    /// <param name="name"></param>
    public void TriggerCompleted(string name)
    {
        if (m_triggers.Contains(name)) {
            m_triggers.Remove(name);
        }
    }

    /// <summary>
    /// Spawns and fires the unit's projectile when the animation tells it to
    /// Sets the targe to the given type
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    protected virtual IEnumerator FireProjectileRoutine(Vector3 direction, Type targetType)
    {
        if (IsActive) {
            // Wait to spawn/fire projectile
            m_fireProjectile = false;

            // Play the animation and wait for it to be done
            SetAnimatorTrigger("Attack");
            while (!m_fireProjectile && IsActive) {
                yield return new WaitForEndOfFrame();
            }
        }

        if (IsActive) {
            GameObject go = GameManager.instance.SpawnProjectile(m_projectile, m_projectileSpawnPoint);

            if (go != null && go.GetComponent<Projectile>() != null) {
                Projectile projectile = go.GetComponent<Projectile>();
                projectile.Fire(direction, Stats.GetStat(StatsId.Attack), targetType);
            }

            ResetTimer(m_attackTimer);
        }
    }

    /// <summary>
    /// Called by the animator to trigger the projectile to spawn/move
    /// </summary>
    public void FireProjectile()
    {
        m_fireProjectile = true;
    }

    /// <summary>
    /// Triggered when the unit is attacked
    /// </summary>
    /// <param name="damage"></param>
    public virtual void HurtAction(int damage)
    {
        // Done this way to trigger the OnChange until we can come up with something better
        StatData stats = Stats;
        stats[StatsId.HP_Cur] = Mathf.Max(0, stats[StatsId.HP_Cur] - damage);
        Stats = stats;
        
        DisplayDamage(damage);
        StartCoroutine(FlashSpriteRoutine());
    }

    /// <summary>
    /// Triggers the spawning of the HP change display with the text and color for damage
    /// </summary>
    /// <param name="damage"></param>
    public void DisplayDamage(int damage)
    {
        DisplayHPChange(damage, Color.red);
    }

    /// <summary>
    /// Triggers the spawning of the HP change display with the text and color for healing
    /// </summary>
    /// <param name="amount"></param>
    public void DisplayHealing(int amount)
    {
        DisplayHPChange(amount, Color.green);
    }

    /// <summary>
    /// Spawns the effects that shows HP gain or lost
    /// </summary>
    /// <param name="amount"></param>
    void DisplayHPChange(int amount, Color color)
    {
        if (m_hpDisplayPrefab != null && m_hpDisplaySpawnPoint != null) {
            GameObject go = Instantiate(m_hpDisplayPrefab, transform, true);
            go.transform.position = m_hpDisplaySpawnPoint.position;

            HPChangeDisplay display = go.GetComponentInChildren<HPChangeDisplay>();
            display.SetText(amount.ToString());
            display.SetColor(color);
        }
    }

    /// <summary>
    /// Triggers the white flashing effect when the enemy is hurt
    /// </summary>
    /// <returns></returns>
    IEnumerator FlashSpriteRoutine()
    {
        if (!m_isFlashing && IsActive) {
            m_isFlashing = true;

            m_renderer.material.color = m_flashColor;
            m_renderer.material.SetFloat("_FlashAmount", m_flashAmount);

            yield return new WaitForSeconds(m_flashDuration);
            ResetFlashingEffect();
        }
    }

    /// <summary>
    /// Ensures the unit is reset back to a normal state
    /// </summary>
    public void ResetFlashingEffect()
    {
        m_renderer.material.SetFloat("_FlashAmount", 0f);
        m_renderer.material.color = Color.white;
        m_isFlashing = false;
    }

    /// <summary>
    /// Returns true if the given parameter is one listed in the animator controller
    /// </summary>
    /// <param name="paramName"></param>
    /// <returns></returns>
    bool HasParameter(string paramName)
    {
        bool has = false;

        foreach (AnimatorControllerParameter paramete in m_animator.parameters) {
            if(paramete.name == paramName) {
                has = true;
            }
            break;
        }

        return has;
    }

    /// <summary>
    /// Ensures the timer routine is stopped and then resets it
    /// Timer gets stated by the AIManager or the unit that called it
    /// </summary>
    /// <param name="timer"></param>
    protected virtual void ResetTimer(CoolDownTimer timer)
    {
        if (timer.IsRunning) {
            AIManager.instance.PauseTimer(timer);
        }

        timer.ResetTimer();
        AIManager.instance.ResumeTimer(timer);
    }

    /// <summary>
    /// Increase the unit's current experience by the given number
    /// Tirgers a level up while the current stats is greater than the exp required for the next level
    /// </summary>
    /// <param name="exp"></param>
    public void AddExp(int exp)
    {
        Stats.Exp += exp;

        while (Stats.Exp >= Stats.NextLevelExp) {
            TriggerLevelUp();
        }
    }

    /// <summary>
    /// Increases the current level by a single level
    /// </summary>
    public void TriggerLevelUp()
    {
        LevelUpMetada data = CreateLevelUp(1);
        LevelUp(data);
    }

    /// <summary>
    /// Returns the information that represents the stats changes for adding
    /// the given level number to the current level.
    /// </summary>
    /// <param name="level">How many levels to increase the unit by</param>
    /// <returns></returns>
    public LevelUpMetada CreateLevelUp(int level)
    {
        StatData stats = new StatData();

        // Increase the stats for each level
        for (int i = 0; i < level; i++) {
            foreach (KeyValuePair<StatsId, Dice> growth in Stats.Growths) {
                StatsId id = growth.Key;
                Dice dice = growth.Value;
                stats[id] = dice.Roll();
            }
        }

        LevelUpMetada data = new LevelUpMetada(level, stats);

        return data;
    }

    /// <summary>
    /// Applies the level and stats changes presented in the level up metadata
    /// Increases the total experience points required to level up for the next level
    /// The increment of the current experience point happens prior to this call
    /// </summary>
    /// <param name="data"></param>
    public void LevelUp(LevelUpMetada data)
    {
        Stats.Level += data.level;
        Stats.Exp = Stats.NextLevelExp;
        Stats.NextLevelExp = EXPManager.instance.NextLevelEXP(Stats.Level, BaseExperience);

        foreach (KeyValuePair<StatsId, int> item in data.stats) {
            StatsId id = item.Key;
            int value = item.Value;
            Stats[id] += value;
        }

        OnStatsChanged();
    }
    
    /// <summary>
    /// Helps with garbage collection forcing all coroutines to stop when the unit is not 
    /// active which usually means the unit is dead
    /// </summary>
    protected void StopAll()
    {
        StopAllCoroutines();

        if (m_attackTimer != null) {
            m_attackTimer.StopTimer();
        }

        if (m_specialTimer != null) {
            m_specialTimer.StopTimer();
        }
    }
}
