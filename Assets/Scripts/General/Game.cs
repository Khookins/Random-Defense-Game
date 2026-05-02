using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[SerializeField]
public class Game : MonoBehaviour
{
    // Singleton
    public static Game Instance
    {
        get;
        private set;
    }
    // Inspector
    [SerializeField] private RoundData roundData;
    [SerializeField] private GameObject winLossScreen;
    [SerializeField] private string[] WinMessages;
    [SerializeField] private string[] LossMessages;
    // Events
    public static event Action<GameState> OnGameStateChanged;
    public static event Action<ControlState> OnControlStateChanged;
    public static event Action<Tower> OnTowerPlacementStarted;
    public static event Action OnTowerPlacementEnded;
    public static event Action OnRoundStarted;
    public static event Action OnRoundEnded;
    public static event Action<float> OnPlayerHealthChanged;
    public static event Action<int,bool> OnPlayerMoneyChanged;
	public enum GameState
    {
        Preparation,
        Engagement
    }
    public enum ControlState
    {
        Normal,
        Modifying,
        Placing
    }
    // Generic Variables
    private GameState _gState = GameState.Preparation;
    public GameState gState
    {
        get { return _gState; }
        private set
        {
            _gState = value;
            OnGameStateChanged.Invoke(_gState);
        }
    }
    private ControlState _cState = ControlState.Normal;
    public ControlState cState
    {
        get { return _cState; }
        private set
        {
            _cState = value;
            OnControlStateChanged.Invoke(_cState);
        }
    }

    private int CurrentRound = 0;
    private int MaxRound = 0;
    private float PlayerHealth = 100f;
    private int PlayerMoney = 0;

    private void Awake()
    {
        Instance = this;
        CurrentRound = 0;
        PlayerHealth = 100f;
        PlayerMoney = 0;
        MaxRound = roundData.waves.Count;
    }

    // Reloads the scene.
    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Deals damage to the player base.
    public void TakePlayerDamage(float amount)
    {
        PlayerHealth -= amount;
        OnPlayerHealthChanged.Invoke(PlayerHealth);
        if (PlayerHealth <= 0 && !winLossScreen.activeSelf)
        {
            WinLossScreen(false);
        }
    }

    // Updates the money the player has, and checks if they lost or gained. (Deemed unecessary for the MVP)
    public void ChangePlayerMoney(int amount)
    {
        int oldPlayerMoney = PlayerMoney;
        PlayerMoney += amount;
        OnPlayerMoneyChanged.Invoke(PlayerMoney,PlayerMoney > oldPlayerMoney);
    }

    // Emits the tower placement started event so that other objects can update accordingly. Also enters the Placing Control State.
    public void EnterTowerPlacement(Tower tower)
    {
        if (gState != GameState.Preparation) { Debug.LogWarning("Unable to go into tower placement. Reason: Wrong game State"); return; }
        cState = ControlState.Placing;
        OnTowerPlacementStarted.Invoke(tower);
    }

    // Emits the tower placement ended event so that other objects can update accordingly. Also exits the Placing Control State.
    public void ExitTowerPlacement()
    {
        OnTowerPlacementEnded.Invoke();
        cState = ControlState.Normal;
    }

    // Emits the round started event so that other objects can update accordingly. Also enters the Engagement Game State.
    public void StartRound()
    {
        CurrentRound++;
        gState = GameState.Engagement;
        OnRoundStarted.Invoke();
    }

    // Emits the round ended event so that other objects can update accordingly. Also enters the Preperation Game State. Also handles triggering the win/lose screen function.
    public void EndRound()
    {
        if (CurrentRound >= MaxRound)
        {
            WinLossScreen(true);
        }
        else
        {
            gState = GameState.Preparation;
            OnRoundEnded.Invoke();
        }
    }

    // Handles the win/lose screen.
    public void WinLossScreen(bool win)
    {
        string[] messages = win ? WinMessages : LossMessages;
        int i = 0;

        winLossScreen.SetActive(true);

        foreach (TMP_Text text in winLossScreen.GetComponentsInChildren<TMP_Text>())
        {
            if (i >= messages.Length) break;
            text.text = messages[i];
            i++;
        }
    }

    // Gets the current round info.
    public int2 GetRoundInfo()
    {
        return new int2(CurrentRound, MaxRound);
    }
}


