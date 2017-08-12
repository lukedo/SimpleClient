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

    [SerializeField] private string serverAddress = "127.0.0.1:4530";
    [SerializeField] private string applicationName = "SimpleGameServer";
    [SerializeField] private float updateRate = 0.05f;
    private PhotonPeer peer;

    #endregion
    
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
        StartCoroutine(updatePeer());
    }
    
    private IEnumerator updatePeer()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateRate);
            peer.Service();
        }
    }

    public void OnOperationResponse(OperationResponse operationResponse)
    {
        switch ((OperationCode) operationResponse.OperationCode)
        {
            case OperationCode.Test:
                Debug.Log(operationResponse.Parameters[1]);
                Debug.Log(operationResponse.Parameters[2]);
                break;
            case OperationCode.Attack:
                if ((bool) operationResponse.Parameters[1])
                {
                    GameObject.Find ((string)operationResponse[0]).GetComponent<Unit> ().GetDamage (10);
                }
                else
                {
                    GameObject.Find ((string)operationResponse[0]).GetComponent<Unit> ().Die();
                }
                break;
            default:
                Debug.Log("Unknown operation code");
                break;
        }
    }
    
    public void checkAttack(int damage, int health, string unitName)
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
            connetionText.text = statusCode.ToString().ToLower();
        }
    }

    public void OnEvent(EventData eventData)
    {
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

    public void SetCurrentCanvas(Transform canvasTransform)
    {
        currentCanvas = canvasTransform;
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