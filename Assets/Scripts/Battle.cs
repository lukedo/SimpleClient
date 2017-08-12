using UnityEngine;

public class Battle : MonoBehaviour
{
    [SerializeField] private Transform heroSpawnPoint;
    [SerializeField] private Transform mainCanvas;

    [SerializeField] private Unit enemy;
    private Hero hero;

    public void Start()
    {
        GameObject heroGameObject = Instantiate(GameManager.Current.HeroPrefabs[GameManager.Current.GetPickedHeroId()]);
        heroGameObject.transform.SetParent(mainCanvas);
        heroGameObject.transform.position = heroSpawnPoint.position;

        hero = heroGameObject.GetComponent<Hero>();

        if (enemy == null)
        {
            Debug.LogError("Set enemy at inspector!");
        }

        GameManager.Current.SetCurrentCanvas(mainCanvas);
        GameManager.Current.SetCurrentBattle(this);
    }

    public void UnitAttacked(string unitType, bool attack)
    {
        if (unitType == "Unit")
        {
            if (attack)
            {
                enemy.GetDamage(10);
            }
            else
            {
                enemy.Die();
            }
        }
        else if (unitType == "Hero")
        {
            if (attack)
            {
                hero.GetDamage(10);
            }
            else
            {
                hero.Die();
            }
        }
    }
}