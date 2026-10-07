using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using StarterAssets;
using Yarn;
using Yarn.Unity;

public class GameController : MonoBehaviour
{
    //this instance variable is the static reference to this class
    //it can only be overwritten within this class (the private set determines that)
    public static GameController instance { get; private set; }
    public coinAdd coinUpdate;
    public coinUI coinHUD;
    public int currentCoins = 0;

    //public NPC_Data questGiver; 
    //this is going to be complicated, we can hold onto this npc data,
    //but we will need to change the current phase, which is difficult because
    //we are doing so through the NPC_Interact script.
    //Box interact does something similar, but we have to get it to carry across scenes

    //Will we need to get NPC_interact to grab the data from us? Could work...



    public Transform startingLoc;
    public Vector3 loadLoc;
    public Quaternion loadRot;
    public bool firstSceneloaded = false;
    public GameObject ThirdPersonRig;

    public List<string> collectedCoinIDs;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);

            //this line uses the `sceneLoaded` UnityAction
            //we subscribe the OnSceneLoaded method to this action, which "fires" whenever a new scene is loaded
            SceneManager.sceneLoaded += OnSceneLoaded;
            //Debug.Log("Loaded Scene: " + SceneManager.GetActiveScene());
        }
        if (currentCoins <= 0)
        {
            coinHUD.gameObject.transform.parent.gameObject.SetActive(false);
        }
    }

    //this method receives the `sceneLoaded` UnityAction and requires the Scene and LoadSceneMode arguments
    //we use this method to check what current scene has been loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            if (!firstSceneloaded)
            {
                Debug.Log("Spawned Character at starting Location");
                SpawnCharacter(startingLoc.position, startingLoc.rotation);
                firstSceneloaded = true;
            }
            else
            {
                Debug.Log("Spawned Character at new Location");
                SpawnCharacter(loadLoc, loadRot);
            }
        }
        if (coinHUD == null)
        {
            coinHUD = FindAnyObjectByType<coinUI>();
            Debug.Log("Setting coin UI. It is now " + coinHUD);
            if (currentCoins <= 0)
            {
                coinHUD.gameObject.transform.parent.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.Log("Couldn't find coinUI");
            if (currentCoins > 0)
            {
                coinHUD.gameObject.transform.parent.gameObject.SetActive(true);
            }
        }

        loadCoins();

    }

    public void SpawnCharacter(Vector3 loc, Quaternion rot)
    {
        Debug.Log("SPAWNED THIRD PERSON");
        Instantiate(ThirdPersonRig, loc, rot);
    }

    //the below method is a `setter` method for this singleton
    //it gets called by other GameObjects and classes when we enter/exit "loading zones"
    public void setSpawnPoint(Vector3 loc, Quaternion rot)
    {
        loadLoc = loc;
        loadRot = rot;
    }

    public void coinCollect(string coinID)
    {
        Debug.Log("Coin collect called");
        if (currentCoins == 0)
        {
            coinHUD.gameObject.transform.parent.gameObject.SetActive(true);
        }
        //Debug.Log("Coin ID " + coinID);
        collectedCoinIDs.Add(coinID);
        currentCoins++;
        coinUpdate.AddListener(coinHUD.addCoins);
        coinUpdate.Invoke(currentCoins);
    }

    public void loadCoins()
    {
        coinGrab[] allCoinsInScene = FindObjectsByType<coinGrab>();

        if (collectedCoinIDs != null)
        {
            foreach (coinGrab coin in allCoinsInScene)
            {
                if (collectedCoinIDs.Contains(coin.coinID))
                {
                    Destroy(coin.gameObject);
                }
            }
        }
        coinHUD.addCoins(currentCoins);
    }

    //this is a 'setter' method for the coin HUD variable in this class
    public void setUI(coinUI ui)
    {
        coinHUD = ui;
    }

    public void setPlayerMovement(bool b)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        ThirdPersonController tpc = player.GetComponent<ThirdPersonController>();

        if (b)
        {
            tpc.enabled = false;
        }
        else
        {
            tpc.enabled = true;
        }
    }

    //this method is being called by the NPC_Cam_Control script
    //calling it from there b/c that class has a standardized Transform variable to pass into this method
    public void moveForDialogue(Transform movePos)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = movePos.position;
    }

    [YarnCommand("SubtractQuestAmt")]
    public void getCoinAmt(int amt)
    {
        currentCoins -= amt;
        loadCoins();
    }


}

[System.Serializable]
public class coinAdd : UnityEvent<int>
{

}


