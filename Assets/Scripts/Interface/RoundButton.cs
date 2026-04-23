using System;
using UnityEngine;
using UnityEngine.UI;

public class RoundButton : MonoBehaviour
{
    private Button button;
    private Action visible;
    private Action hidden;

    private void Awake()
    {
        button = GetComponent<Button>();
        hidden = () => UpdateVisibility(false);
        visible = () => UpdateVisibility(true);
    }

    private void OnEnable()
    {

        Game.OnRoundStarted += hidden;
        Game.OnRoundEnded += visible;
    }

    private void OnDisable()
    {

        Game.OnRoundStarted -= hidden;
        Game.OnRoundEnded -= visible;
    }

    private void UpdateVisibility(bool visible)
    {
        button.interactable = visible;
    }

    public void OnClick()
    {
        Game.Instance.StartRound();
    }
}
