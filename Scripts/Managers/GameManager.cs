using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the game session
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton
    /// </summary>
    static public GameManager instance;

    /// <summary>
    /// How many enemies to build per wave
    /// </summary>
    [SerializeField]
    int m_enemiesPerWave = 10;

    /// <summary>
    /// The current round number
    /// </summary>
    public int Round { get; set; }

    /// <summary>
    /// The current wave number
    /// </summary>
    public int Wave { get; set; }

    /// <summary>
    /// True when all the enemies for the current wave are defeated
    /// </summary>
    public bool WaveCompleted { get; set; }
    
    /// <summary>
    /// A container for all projectile prefabs
    /// </summary>
    GameObject m_projectilesGO;

    /// <summary>
    /// True when game over is triggered
    /// </summary>
    public bool GameOver = false;

    /// <summary>
    /// Total EXP in percentage required to continue
    /// </summary>
    [SerializeField]
    float m_retryCostMultiplier = 0.25f;

    /// <summary>
    /// Title and play button
    /// </summary>
    [SerializeField]
    GameObject m_titleMenu;

    /// <summary>
    /// Menu for game over
    /// </summary>
    [SerializeField]
    GameObject m_retryMenu;

    /// <summary>
    /// Menu for game over
    /// </summary>
    [SerializeField]
    GameObject m_gameCompletedMenu;

    /// <summary>
    /// The text to display retry cost
    /// </summary>
    [SerializeField]
    Text m_retryEXP;

    /// <summary>
    /// What to say on the retry option
    /// </summary>
    [SerializeField]
    Text m_retryMessageField;

    /// <summary>
    /// The sacrifice button
    /// </summary>
    [SerializeField]
    Button m_retrySacrificeButton;

    bool m_waitingToStart = true;
    int m_sacrificeCost = 0;

    /// <summary>
    /// Sets up reference and prevents more than once instance of the GM at a time
    /// </summary>
    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Starts the game routine
    /// </summary>
    void Start()
    {
        m_retryMenu.SetActive(false);
        m_gameCompletedMenu.SetActive(false);
        m_titleMenu.SetActive(true);
        StartCoroutine(GameRoutine());
    }

    /// <summary>
    /// Closes the application when ESC is pressed
    /// </summary>
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    /// <summary>
    /// Spawns and returns the given projectile gameobject at the given point
    /// Projectiles are contained in the parent <see cref="m_projectilesGO"/> gameobject
    /// </summary>
    /// <param name="projectile"></param>
    /// <param name="spawnPoint"></param>
    /// <returns></returns>
    public GameObject SpawnProjectile(Projectile projectile, Transform spawnPoint)
    {
        GameObject go = null;

        if(projectile != null) {
            // Build parent
            if(m_projectilesGO == null) {
                m_projectilesGO = new GameObject("_Projectiles");
            }

            go = Instantiate(projectile.gameObject, m_projectilesGO.transform);
            go.transform.position = spawnPoint.position;
        }

        return go;
    }

    /// <summary>
    /// Triggers all the routines, in order, to run the game, waiting for each
    /// routine to complete
    /// </summary>
    /// <returns></returns>
    IEnumerator GameRoutine()
    {
        while (m_waitingToStart) {
            Time.timeScale = 0;
            yield return null;
        }

        Time.timeScale = 1;

        yield return StartCoroutine(InitializeGameRoutine());
        yield return StartCoroutine(GameplayLoopRoutine());
    }

    /// <summary>
    /// Handles initializing the game for when the game is first loaded
    /// </summary>
    /// <returns></returns>
    IEnumerator InitializeGameRoutine()
    {
        Wave = 1;
        Round = 1;
        yield return null;
    }

    /// <summary>
    /// Handles all the actions that take place during a play session
    /// </summary>
    /// <returns></returns>
    IEnumerator GameplayLoopRoutine()
    {
        while (true) {
            WaveCompleted = false;
            yield return StartCoroutine(BuildWaveRoutine());
            yield return StartCoroutine(WaveManager.instance.SpawnWaveRoutine());
            yield return StartCoroutine(WaitForWaveCompletionRoutine());

            if(Wave > 10) {
                break;
            }
        }

        m_gameCompletedMenu.SetActive(true);
    }

    /// <summary>
    /// Requests that the current wave of enemies be built
    /// </summary>
    /// <returns></returns>
    IEnumerator BuildWaveRoutine()
    {
        WaveManager.instance.BuildWave(Wave, m_enemiesPerWave);
        yield return null;
    }

    /// <summary>
    /// Waits until the current wave has been completed
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForWaveCompletionRoutine()
    {
        while (!WaveCompleted && !GameOver) {
            yield return null;
        }

        // Increase wave
        if (!GameOver) {
            Wave++;
        }
    }

    /// <summary>
    /// Triggers the retry game over sequence
    /// </summary>
    public void TriggerGameOver()
    {
        if(GameOver) {
            return;
        }

        Time.timeScale = 0;
        m_retryMenu.SetActive(true);

        if (EXPManager.instance.EXP > 0) {
            m_retrySacrificeButton.interactable = true;
            m_sacrificeCost = Mathf.Max(1, Mathf.Min((int)(EXPManager.instance.EXP * m_retryCostMultiplier), EXPManager.instance.EXP));
            m_retryMessageField.text = "Your knight has been defeated!\nfeed me your EXP to continue\nor lose everything and restart";
            m_retryEXP.text = m_sacrificeCost.ToString();
        } else {
            m_retrySacrificeButton.interactable = false;
            m_retryEXP.text = "n/a";
            m_retryMessageField.text = "Oh, so sad.\nYou don't have enough EXP to retry.\nLooks like is back to the start for you.";
        }
    }

    public void Play()
    {        
        m_titleMenu.SetActive(false);
        m_waitingToStart = false;
    }

    public void Sacrifice()
    {
        if (EXPManager.instance.ConsumeEXP(m_sacrificeCost)) {
            m_retryMenu.SetActive(false);
            PartyManager.instance.HealParty(1000000);
            Time.timeScale = 1;
            GameOver = false;
            WaveManager.instance.DestroyCurrentWave();
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    
}
