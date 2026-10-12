using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DirtPlot : MonoBehaviour
{
    [Header("Grow states in order")]
    public GameObject[] growStates;
    private int growStateIndex = 0;
    private float intervals = 0;
    private Vector3 growSpace;

    [Header("UI")]
    public Image growthProgressBar;
    public TextMeshProUGUI displayCrop; 

    public SeedTypes crop;
    public bool isOccupied;
    private float growTime;
    private float timeLeft;
    private GameObject finalGrow;
    public bool doneGrowing;

    private GameObject currentStatePrefab;

    public TextMeshProUGUI interactText;
    public bool canInteract = false;
    public Seed willPlantThis;



    void Start()
    {
        //Transform growTransform = gameObject.AddComponent<Transform>();
        Vector3 whereGrow = new Vector3(transform.position.x, (float)(transform.position.y + 0.5), transform.position.z);
        //growTransform.position = whereGrow;
        growSpace = whereGrow;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.childCount == 0 && growthProgressBar.fillAmount != 0)
        {
            ResetEverything();
        }
        if(isOccupied)
        {
            timeLeft -= Time.deltaTime;
            intervals -= Time.deltaTime;
            updateProgressBar();
            displayCrop.text = crop.seedName;
            if (timeLeft <= 0 && doneGrowing == false)
            {
                Debug.Log("Crop is done!");
                Destroy(currentStatePrefab);
                currentStatePrefab = null;
                Instantiate(finalGrow, growSpace, Quaternion.identity, this.gameObject.transform);
                doneGrowing = true;
            }
            if (intervals <= 0)
            {
                manageStates();
                if(growStateIndex <= growStates.Length)
                {
                    manageIntervals();
                }
            }
        }

        if (canInteract)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame) 
            {
                if (isOccupied == false)
                {
                    interactText.text = "";
                    GameController.instance.takeSeeds(1);
                    Debug.Log("You planted a crop");
                    plantThis(willPlantThis);
                    canInteract = false;
                }
                else if(doneGrowing)
                {
                    GameController.instance.addCoinAmt(1);
                    foreach (Transform child in this.gameObject.transform)
                    {
                        Destroy(child.gameObject);
                    }
                    interactText.text = "";
                    canInteract = true;
                }
            }
        }
    
}

    private void ResetEverything()
    {
        crop = null;
        doneGrowing = false;
        isOccupied = false;
        currentStatePrefab = null;
        growthProgressBar.fillAmount = 0;
        displayCrop.text = "";
    }
    private void manageIntervals()
    {
        if(crop.growRate != 0)
        {
            intervals = crop.growRate / (growStates.Length); //NVM //adding plus one, because we manually add the last state
        }
        else
        {
            intervals = 15f / (growStates.Length); //NVM //adding plus one, because we manually add the last state
        }
    }

    private void updateProgressBar()
    {
        growthProgressBar.fillAmount = (1 - (float)(timeLeft/growTime));
    }

    private void manageStates()
    {
        if(growStateIndex < growStates.Length)
        {
            //GameObject destroyThis = GetComponentInChildren<GameObject>();
            Destroy(currentStatePrefab);
            currentStatePrefab = Instantiate(growStates[growStateIndex], growSpace, Quaternion.identity, this.gameObject.transform);
            growStateIndex++;
        }
        else
        {
            //GameObject destroyThis = GetComponentInChildren<GameObject>();
            Destroy(currentStatePrefab);
            //instantiate the final cropstate
            //we need to fix the scaling here
            //currentStatePrefab = Instantiate(crop.grownPrefab, growSpace, Quaternion.identity, this.gameObject.transform);
        }
        //if there is a next state,  
        //sets timer back if this is not a final state
    }

    private void OnTriggerEnter(Collider other)
    {
        if(isOccupied == false && GameController.instance.heldSeeds > 0)
        {
            canInteract = true;
            interactText.text = "Press E to plant";
        }
        else
        {  
            canInteract = false;
            interactText.text = "";
        }

        if(doneGrowing == true)
        {
            canInteract = true;
            interactText.text = "Press E to Harvest";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        interactText.text = "";
        canInteract = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Seed plantSeed = collision.gameObject.GetComponent<Seed>();
        if (plantSeed != null && isOccupied == false)
        {
            plantThis(plantSeed);
            Debug.Log("Called plantThis");
            Destroy(collision.gameObject); 
        }
        else
        {
            Debug.Log("Seed is null, or space is occupied");
        }
    }

    public void plantThis(Seed seed)
    {
            doneGrowing = false;
            //check if the type is null, otherwise we don't destroy it & instantiate prefab
            /*if (seed.type == null)
            {
                Debug.Log("Seed type on this object is null");
                return;
            }*/

            crop = willPlantThis.type;
            Debug.Log("Tried to plant " + crop.seedName);

            if (crop.grownPrefab != null)
            {
                finalGrow = crop.grownPrefab;
                //also checks if the grown prefab exists so we can actually instantiate it later.
                //Destroy(gm.gameObject);
                //instantiates the first crop state and then increases the index
                currentStatePrefab = Instantiate(growStates[0], growSpace, Quaternion.identity, this.gameObject.transform);
                growStateIndex = 1;
                isOccupied = true;
            }
            else { Debug.Log("Final crop state prefab is null!"); return; }
            if (crop.growRate != 0)
            {
                growTime = crop.growRate;
                timeLeft = crop.growRate;
                manageIntervals();
            }
            else
            {
                growTime = 15f;
                timeLeft = 15f;
                manageIntervals();
            }
        
    }
}
