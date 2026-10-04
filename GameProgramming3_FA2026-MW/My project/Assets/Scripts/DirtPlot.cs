using TMPro;
using Unity.VisualScripting;
using UnityEngine;
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

    private SeedTypes crop;
    private bool isOccupied;
    private float growTime;
    private float timeLeft;
    private GameObject finalGrow;
    private bool doneGrowing;

    private GameObject currentStatePrefab;



    void Start()
    {
        //Transform growTransform = gameObject.AddComponent<Transform>();
        Vector3 whereGrow = new Vector3(transform.position.x, (float)(transform.position.y + 0.5), transform.position.z);
        //growTransform.position = whereGrow;
        growSpace = whereGrow;
        doneGrowing = false;
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
    }

    private void ResetEverything()
    {
        crop = null;
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

    private void OnCollisionEnter(Collision collision)
    {
        //returns if space is already occupied
        if(isOccupied == true)
        {
            Debug.Log("Space is already occupied!");
            return;
        }
        //else
        Seed seed = collision.gameObject.GetComponent<Seed>();
        if(seed != null)
        {
            doneGrowing = false;
            //check if the type is null, otherwise we don't destroy it & instantiate prefab
            if(seed.type == null)
            {
                Debug.Log("Seed type on this object is null");
                return;
            }

            crop = seed.type;
            Debug.Log("Tried to plant " + crop.seedName);

            if (crop.grownPrefab != null)
            {
                finalGrow = crop.grownPrefab;
                //also checks if the grown prefab exists so we can actually instantiate it later.
                Destroy(collision.gameObject);
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
}
