﻿using UnityEngine;

public class Hero : Unit
{
    [SerializeField] private Sprite appearance;
    [SerializeField] private int winAmount;
    [SerializeField] private int loseAmount;
    
    public int WinAmount
    {
        get { return winAmount; }
        set { winAmount = value; }
    }

    public int LoseAmount
    {
        get { return loseAmount; }
        set { loseAmount = value; }
    }

    public Sprite Appearance
    {
        get { return appearance; }
        set { appearance = value; }
    }

	public void ClearScore() 
	{
		winAmount = 0;
		loseAmount = 0;
	}

    public override void Die()
    {
		healthCurrent = 0; 
		healthBar.text = healthCurrent.ToString();
        GameManager.Current.HeroDied();
    }
}