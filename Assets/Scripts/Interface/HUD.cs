using Unity.Mathematics;
using UnityEngine;

public class HUD : MonoBehaviour
{
    // Singleton
    public static HUD Instance {  get; private set; }
    // Inspector
    [Header("Labels")]
    [SerializeField] private Label gameStateLabel;
    [SerializeField] private Label controlStateLabel;
    [SerializeField] private Label playerHealthLabel;
    [SerializeField] private Label playerMoneyLabel;
    [SerializeField] private Label roundCounterLabel;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        Game.OnGameStateChanged += GameStateChanged;
        Game.OnControlStateChanged += ControlStateChanged;
        Game.OnPlayerHealthChanged += PlayerHealthChanged;
        Game.OnPlayerMoneyChanged += PlayerMoneyChanged;
    }

    private void OnDisable()
    {
        Game.OnGameStateChanged -= GameStateChanged;
        Game.OnControlStateChanged -= ControlStateChanged;
        Game.OnPlayerHealthChanged -= PlayerHealthChanged;
        Game.OnPlayerMoneyChanged -= PlayerMoneyChanged;
    }

    private void Start()
    {
        GameStateChanged(Game.Instance.gState);
        ControlStateChanged(Game.Instance.cState);
    }

    private void GameStateChanged(Game.GameState value)
    {
        gameStateLabel.UpdateText(value.ToString());
        int2 info = Game.Instance.GetRoundInfo();
        roundCounterLabel.UpdateText($"Round {info.x}/{info.y}");
    }

    private void ControlStateChanged(Game.ControlState value)
    {
        controlStateLabel.UpdateText(value.ToString());
    }

    private void PlayerHealthChanged(float newHealth)
    {
        int health = Mathf.CeilToInt(newHealth);
        playerHealthLabel.UpdateText($"{health} HP");
        playerHealthLabel.FlashColor(Color.red, 0.5f);
    }

    private void PlayerMoneyChanged(int newMoney, bool sign)
    {
        playerMoneyLabel.UpdateText($"${newMoney}");
        playerMoneyLabel.FlashColor(sign ? Color.green : Color.red, 0.25f);
    }
}
