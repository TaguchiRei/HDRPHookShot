using UnityEngine;

public class PlayerAbilityManager : MonoBehaviour
{
    [SerializeField] PlayerMove _playerMove;
    public void AbilityEffect(int abilityNumber, int useEnergy = 1)
    {
        if (abilityNumber == 0)
        {
            _playerMove.GaugeChanger(30);
        }
    }
}
