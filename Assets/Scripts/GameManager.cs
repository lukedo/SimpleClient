using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviour,  IPhotonPeerListener
{
    public static GameManager Current;

    #region photon

    [Header("Photon")]
    [SerializeField] private string serverAddress = "127.0.0.1:4530";
    [SerializeField] private string applicationName = "SimpleGameServer";
    [SerializeField] private float updateRate = 0.05f;
    [SerializeField] private Text connetionText;
    private PhotonPeer peer;

    #endregion

    [Header("Scenes")]
    [SerializeField] private string mainSceneName;
    [SerializeField] private string redBattleSceneName;
    [SerializeField] private Button redBattleSceneLoader;
    [SerializeField] private string greenBattleSceneName;
    [SerializeField] private Button greenBattleSceneLoader;
    [SerializeField] private string blueBattleSceneName;
    [SerializeField] private Button blueBattleSceneLoader;
    
    [Header("Prefabs")]
    [SerializeField] public GameObject[] HeroPrefabs;
    [SerializeField] private GameObject messageBoxPrefab;
    
    [Header("Other")]
    [SerializeField] private Transform currentCanvas;
    [SerializeField] private bool battleIsOver = true;
    
    private Battle currentBattle;
    private int pickedHeroId;
    private bool connectedToServer;

    public void Awake()
    {
		if (Current != null) {
			Current.connetionText = connetionText;
			connetionText.text = connetionText.text = "connected";
			Current.currentCanvas = currentCanvas;

		    Current.pickedHeroId = 0;
			Current.redBattleSceneLoader = redBattleSceneLoader;
			Current.greenBattleSceneLoader = greenBattleSceneLoader;
			Current.blueBattleSceneLoader = blueBattleSceneLoader;
			Current.InitSceneLoaders ();

			Destroy (gameObject);
		}
		else
		{
			Current = this;
		    
		    Application.runInBackground = true;
		    ConnectToPhoton();
			DontDestroyOnLoad(this);
		}
    }

    #region  photon

    public void ConnectToPhoton()
    {
        peer = new PhotonPeer(this, ConnectionProtocol.Tcp);
        connetionText.text = "connecting...";
        peer.Connect(serverAddress, applicationName);
        StartCoroutine(UpdatePeer());
    }
    
    private IEnumerator UpdatePeer()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateRate);
            peer.Service();
        }
    }

    public void OnOperationResponse(OperationResponse operationResponse)
    {
        if ((OperationCode) operationResponse.OperationCode == OperationCode.Attack)
        {
            currentBattle.UnitAttacked((string) operationResponse.Parameters[0], 
                (bool) operationResponse.Parameters[1]);
        }
    }
    
    public void CheckAttack(int damage, int health, string unitName)
    {
        OperationRequest request = new OperationRequest
        {
            OperationCode = (byte) OperationCode.Attack,
            Parameters = new Dictionary<byte, object> {{0, unitName}, {1, damage}, {2, health}}
        };

        peer.OpCustom(request, true, 0, false);
    }
    
    private void OnApplicationQuit()
    {
        if (peer != null)
        {
            peer.Disconnect();
        }
    }

    public void OnStatusChanged(StatusCode statusCode)
    {
        if (statusCode == StatusCode.Connect)
        {
            connetionText.text = "connected";
            connectedToServer = true;
        }
        else
        {
            connectedToServer = false;
            if (connetionText != null)
            {
                connetionText.text = statusCode.ToString().ToLower();
            }
        }
    }

    public void OnEvent(EventData eventData)
    {
        if (eventData.Parameters[0].Equals("ClearScore"))
        {
            Debug.Log ("Clear score");
            foreach (GameObject hero in HeroPrefabs)
            {
                hero.GetComponent<Hero> ().ClearScore();
            }
			
            HeroPicker.Current.UpdateHeroInformation ();
        }
    }

    public void OnMessage(object messages)
    {
    }
	
    public void DebugReturn(DebugLevel level, string message)
    {
    }
    #endregion

    public void InitSceneLoaders()
    {
        redBattleSceneLoader.onClick.AddListener(delegate { LoadBattleScene(redBattleSceneName); });
        greenBattleSceneLoader.onClick.AddListener(delegate { LoadBattleScene(greenBattleSceneName); });
        blueBattleSceneLoader.onClick.AddListener(delegate { LoadBattleScene(blueBattleSceneName); });
    }

    public void LoadBattleScene(string sceneName)
    {
        if (connectedToServer)
        {
            battleIsOver = false;
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.Log("disconnected");
        }
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene(mainSceneName);
    }
    
    public void SetCurrentCanvas(Transform canvasTransform)
    {
        currentCanvas = canvasTransform;
    }

    public int GetPickedHeroId()
    {
        return pickedHeroId;
    }

    public void SetPickedHeroId(int id)
    {
        pickedHeroId = id;
    }

    public void SetCurrentBattle(Battle battle)
    {
        currentBattle = battle;
    }

    public void HeroDied()
    {
        if (battleIsOver)
        {
            return;
        }
        
        battleIsOver = true;
        HeroPrefabs[pickedHeroId].GetComponent<Hero>().LoseAmount += 1;

        MessageBox.CreateMessageBox(messageBoxPrefab, currentCanvas, "Hero is dead!", "Ok",
            delegate
            {
                MessageBox.CreateMessageBox(messageBoxPrefab, currentCanvas, "You lose", "return to menu",
                    LoadMenuScene);
            });
    }
    
    public void UnitDied()
    {
        if (battleIsOver)
        {
            return;
        }
        
        battleIsOver = true;
        HeroPrefabs[pickedHeroId].GetComponent<Hero>().WinAmount += 1;

        MessageBox.CreateMessageBox(messageBoxPrefab, currentCanvas, "Enemy is dead!", "Ok",
            delegate
            {
                MessageBox.CreateMessageBox(messageBoxPrefab, currentCanvas, "You win", "return to menu",
                    LoadMenuScene);
            });
    }
}