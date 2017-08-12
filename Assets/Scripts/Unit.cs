using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Unit : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] protected Text HealthBar;
    protected int HealthCurrent = 100;

    public void OnPointerClick(PointerEventData data)
    {
        UnitAttacked();
    }

    protected virtual void UnitAttacked()
    {
        GameManager.Current.CheckAttack(10, HealthCurrent, "Unit");
    }

    public void GetDamage(int amount)
    {
        HealthCurrent -= amount;
        HealthBar.text = HealthCurrent.ToString();
    }

    public virtual void Die()
    {
        HealthCurrent = 0;
        HealthBar.text = HealthCurrent.ToString();
        GameManager.Current.UnitDied();
    }
}