using UnityEngine;

public class HotbarButton : MonoBehaviour
{
    [SerializeField] private Tower tower;

    public void OnClick()
    {
        Game.Instance.EnterTowerPlacement(tower);
    }
}
