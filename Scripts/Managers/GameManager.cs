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
    /// Max wave before the gme is won
    /// </summary>
    [SerializeField]
    int m_maxWave = 1000;
    public int MaxWave { get { return m_maxWave; } }

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
    public bool GameOver { get; set; }

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
    /// This is where the player's click to earn more EXP
    /// </summary>
    [SerializeField]
    GameObject m_clickSurface;

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

    /// <summary>
    /// How much it cost to revive party
    /// </summary>
    int m_sacrificeCost = 1;

    /// <summary>
    /// True while the game is not completed
    /// </summary>
    bool IsGameCompleted { get; set; }

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
        // Hide the game over/game won menus
        m_retryMenu.SetActive(false);
        m_gameCompletedMenu.SetActive(false);
        m_clickSurface.SetActive(false);

        // Show the title screen
        m_titleMenu.SetActive(true);

        // Wait for the player to click start
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
    /// Handles initializing the game for when the game is first loaded
    /// </summary>
    /// <returns></returns>
    void InitGame()
    {
        Wave = 1;
        Round = 1;
        GameOver = false;
        IsGameCompleted = false;
        WaveCompleted = false;
        Time.timeScale = 1;
        PartyManager.instance.ActivateParty();
        m_clickSurface.SetActive(true);
    }

    /// <summary>
    /// Handles all the actions that take place during a play session
    /// </summary>
    /// <returns></returns>
    IEnumerator GameplayLoopRoutine()
    {
        InitGame();

        while (!IsGameCompleted) {
            WaveCompleted = false;

            // Build and run the wave
            WaveManager.instance.BuildWave(Wave, m_enemiesPerWave);
            StartCoroutine(WaveManager.instance.SpawnWaveRoutine());

            // Wait until the enemies are defeated
            // or until the player is dead
            yield return StartCoroutine(WaitForWaveCompletionRoutine());

            if(!GameOver && Wave >= MaxWave) {
                IsGameCompleted = true;
            }

            // To ensure the loop restarts after a game over should the player 
            // sacrifices exp then we wait here
            while (GameOver) {
                yield return null;
            }
        }

        // Show the winning menu
        // This is the end of the game loop as `the win menu
        // prompts them to restart the level which reloads the scene
        if (IsGameCompleted) {
            m_gameCompletedMenu.SetActive(true);
        }
    }

    /// <summary>
    /// Waits until the current wave has been completed
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForWaveCompletionRoutine()
    {
        while (!WaveCompleted && !GameOver) {
            EnemyManager.instance.RunUpdate();
            yield return new WaitForEndOfFrame();
        }

        // Increase wave
        if (!GameOver) {
            Wave++;
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

        if (projectile != null) {
            // Build parent
            if (m_projectilesGO == null) {
                m_projectilesGO = new GameObject("_Projectiles");
            }

            go = Instantiate(projectile.gameObject, m_projectilesGO.transform);
            go.transform.position = spawnPoint.position;
        }

        return go;
    }

    /// <summary>
    /// Triggers the retry game over sequence
    /// </summary>
    public void TriggerGameOver()
    {
        // Already triggered
        if(GameOver) {
            return;
        }

        GameOver = true;
        WaveCompleted = true;
        m_retryMenu.SetActive(true);
        m_clickSurface.SetActive(false);

        PartyManager.instance.DisableParty();
        EnemyManager.instance.DisableEnemies();

        string message = "";
        string cost = "";

        if (EXPManager.instance.EXP > 0) {
            m_sacrificeCost = Mathf.Max(1, Mathf.Min((int)(EXPManager.instance.EXP * m_retryCostMultiplier), EXPManager.instance.EXP));
            cost = m_sacrificeCost.ToString();
            message = "Your knight has been defeated!\nfeed me your EXP to continue\nor lose everything and restart";
        } else {
            message = "Oh, so sad.\nYou don't have enough EXP to retry.\nLooks like is back to the start for you.";
        }

        m_retrySacrificeButton.interactable = !string.IsNullOrEmpty(cost);
        m_retryMessageField.text = message;
        m_retryEXP.text = cost;
    }

    /// <summary>
    /// Starts the game
    /// </summary>
    public void Play()
    {        
        m_titleMenu.SetActive(false);
        StartCoroutine(GameplayLoopRoutine());
    }

    /// <summary>
    /// Offers experience point sacrifice to revive the party
    /// </summary>
    public void Sacrifice()
    {
        if (EXPManager.instance.ConsumeEXP(m_sacrificeCost)) {            
            PartyManager.instance.ReviveParty();            
            WaveManager.instance.DestroyCurrentWave();
            m_retryMenu.SetActive(false);
            m_clickSurface.SetActive(true);
            GameOver = false;
        }
    }

    /// <summary>
    /// Reloads the level to start from the beginning
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }    
}
