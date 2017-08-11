using UnityEngine;

public class Battle : MonoBehaviour
{
    [SerializeField] private Transform heroSpawnPoint;
    [SerializeField] private Transform mainCanvas;

    public void Start()
    {
        GameObject hero = Instantiate(GameManager.Current.HeroPrefabs[GameManager.Current.GetPickedHeroId()]);
        hero.transform.SetParent(mainCanvas);
        hero.transform.position = heroSpawnPoint.position;
        
        GameManager.Current.SetCurrentCanvas(mainCanvas);
    }
}