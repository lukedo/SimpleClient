using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Unit : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] protected Text healthBar;
    protected int healthCurrent = 100; 
    
    public void OnPointerClick(PointerEventData data)
    {
		GameManager.Current.checkAttack(10, healthCurrent, gameObject.name);
    }

    public void GetDamage(int amount)
    {
		healthCurrent -= amount; 
		healthBar.text = healthCurrent.ToString();
    }

    public virtual void Die()
    {
		healthCurrent = 0; 
		healthBar.text = healthCurrent.ToString();
        GameManager.Current.UnitDied();
    }
}
