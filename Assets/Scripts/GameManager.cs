using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Photon.MonoBehaviour
{
    public static GameManager Current;
    [SerializeField] private string gameVersion;
    [SerializeField] private string roomName;
    [SerializeField] private Text connetionText;
    [SerializeField] private int pickedHeroId;
    [SerializeField] public GameObject[] HeroPrefabs;
    [SerializeField] private GameObject messageBoxPrefab;
    [SerializeField] private Transform currentCanvas;
    [SerializeField] private string mainSceneName;

    [SerializeField] private string redBattleSceneName;
    [SerializeField] private Button redBattleSceneLoader;
    [SerializeField] private string greenBattleSceneName;
    [SerializeField] private Button greenBattleSceneLoader;
    [SerializeField] private string blueBattleSceneName;
    [SerializeField] private Button blueBattleSceneLoader;
    [SerializeField] private bool battleIsOver = true;
    
    private bool connectedToRoom;

    public void Awake()
    {
		if (Current != null) {
			Current.connetionText = connetionText;
			connetionText.text = "connected";
			Current.currentCanvas = currentCanvas;

		    Current.pickedHeroId = 0;
			Current.redBattleSceneLoader = redBattleSceneLoader;
			Current.greenBattleSceneLoader = greenBattleSceneLoader;
			Current.blueBattleSceneLoader = blueBattleSceneLoader;
			Current.InitSceneLoaders ();

			Destroy (this.gameObject);
		}
		else
		{
			Current = this;
			gameObject.AddComponent<PhotonView> ();
			gameObject.GetComponent<PhotonView> ().viewID = 1;

			DontDestroyOnLoad(this);
		}
    }

    public void Start()
    {
        connetionText.text = "connecting to lobby...";
        PhotonNetwork.ConnectUsingSettings(gameVersion);
    }

    public void InitSceneLoaders()
    {
        redBattleSceneLoader.onClick.AddListener(delegate { LoadBattleScene(redBattleSceneName); });
        greenBattleSceneLoader.onClick.AddListener(delegate { LoadBattleScene(greenBattleSceneName); });
        blueBattleSceneLoader.onClick.AddListener(delegate { LoadBattleScene(blueBattleSceneName); });
    }

    public void OnJoinedLobby()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = false;
        roomOptions.MaxPlayers = 2;

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        connetionText.text = "connecting to room...";
    }

    public void OnJoinedRoom()
    {
        connetionText.text = "connected";
        connectedToRoom = true;
    }

    public void SetCurrentCanvas(Transform canvasTransform)
    {
        currentCanvas = canvasTransform;
    }

    public void LoadBattleScene(string sceneName)
    {
        if (connectedToRoom)
        {
            battleIsOver = false;
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.Log("disconnected");
        }
    }

    public void loadMenuScene()
    {
        SceneManager.LoadScene(mainSceneName);
    }

    public int GetPickedHeroId()
    {
        return pickedHeroId;
    }

    public void SetPickedHeroId(int id)
    {
        pickedHeroId = id;
    }

    public void HeroDied()
    {
        if (!battleIsOver)
        {
            battleIsOver = true;
            HeroPrefabs[pickedHeroId].GetComponent<Hero>().LoseAmount += 1;
        
            MessageBox msgBox = Instantiate(messageBoxPrefab, currentCanvas).GetComponent<MessageBox>();
            msgBox.SetBoxText("Hero is dead!", "ok");
        
            msgBox.Button.onClick.AddListener(delegate { showLoseDialog(); });
        }
    }

    public void checkAttack(int damage, int health, string unitName)
    {
		photonView.RPC("ChatMessage", PhotonTargets.MasterClient, damage, health, unitName);
    }
    
    [PunRPC]
	void ChatMessage(int damage, int health, string unitName)
    {
        Debug.Log("dmg: " + damage + "hlth: " + health);
    }
    
    [PunRPC]
	void PlayerCanDamage(bool canDamage, string unitName)
    {
        if (canDamage)
        {
			GameObject.Find (unitName).GetComponent<Unit> ().GetDamage (10);
        }
        else
        {
			GameObject.Find (unitName).GetComponent<Unit> ().Die();
        }
    }

	[PunRPC]
	void ClearScore()
	{
		Debug.Log ("Clear score");
		foreach (GameObject hero in HeroPrefabs)
		{
			hero.GetComponent<Hero> ().ClearScore();
		}
			
		HeroPicker.Current.UpdateHeroInformation ();
	}

    public void UnitDied()
    {
        if (!battleIsOver)
        {
            battleIsOver = true;
            HeroPrefabs[pickedHeroId].GetComponent<Hero>().WinAmount += 1;

            MessageBox msgBox = Instantiate(messageBoxPrefab, currentCanvas).GetComponent<MessageBox>();
            msgBox.SetBoxText("Enemy is dead!", "ok");

            msgBox.Button.onClick.AddListener(delegate { showWinDialog(); });
        }
    }

    private void showWinDialog()
    {
        MessageBox msgBox = Instantiate(messageBoxPrefab, currentCanvas).GetComponent<MessageBox>();
        msgBox.SetBoxText("You win!", "return to menu");
        
        msgBox.Button.onClick.AddListener(delegate { loadMenuScene(); });
    }

    private void showLoseDialog()
    {
        MessageBox msgBox = Instantiate(messageBoxPrefab, currentCanvas).GetComponent<MessageBox>();
        msgBox.SetBoxText("You lose!", "return to menu");
        
        msgBox.Button.onClick.AddListener(delegate { loadMenuScene(); });
    }
}