using UnityEngine;
using UnityEngine.UI;

public class HeroPicker : MonoBehaviour
{
	[SerializeField]
	public static HeroPicker Current;
    [SerializeField] 
    private Button[] pickerButtons;

    [SerializeField] private Image heroAppearance;
    [SerializeField] private Text winText;
    [SerializeField] private Text loseText;
    
    private bool picked;
    private int currentHero;

    public void Awake()
    {
		Current = this;
        UpdateHeroInformation();
    }
    
    public void ShowPreviousHero()
    {
        if (currentHero != 0)
        {
            currentHero -= 1;
        }
        else
        {
            currentHero = GameManager.Current.HeroPrefabs.Length - 1;
        }
        UpdateHeroInformation();
    }

    public void ShowNextHero()
    {
        if (currentHero < GameManager.Current.HeroPrefabs.Length - 1)
        {
            currentHero += 1;
        }
        else
        {
            currentHero = 0;
        }
        UpdateHeroInformation();
    }

    public void PickHero()
    {
        if (!picked)
        {
            GameManager.Current.SetPickedHeroId(currentHero);
            picked = true;
        }
        foreach (var button in pickerButtons)
        {
            button.interactable = false;
        }
    }

    public void UpdateHeroInformation()
    {
        Hero tmpHero = GameManager.Current.HeroPrefabs[currentHero].GetComponent<Hero>();
		if (heroAppearance != null) 
		{
			heroAppearance.sprite = tmpHero.Appearance;
		}
		if (winText != null) 
		{
			winText.text = tmpHero.WinAmount.ToString ();
		}
		if (loseText != null) 
		{
			loseText.text = tmpHero.LoseAmount.ToString ();
		}
    }
}
