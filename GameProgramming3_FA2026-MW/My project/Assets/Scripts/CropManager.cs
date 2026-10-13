using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CropManager : MonoBehaviour
{
    public static CropManager instance { get; private set; }
    private bool[] needsPlanting = new bool[5];
    public Seed cornSeed;
    public static bool hasCompletedFirstScene = false;

    private void Awake()
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

        if(hasCompletedFirstScene == false)
        {
            Debug.Log("Planeted Starter");
            StarterPlants();
            hasCompletedFirstScene = true;
        }
    }

    private void Update()
    {
        DirtPlot[] dirtPlots = FindObjectsByType<DirtPlot>();
        Array.Sort(dirtPlots, (a, b) => -StringComparer.OrdinalIgnoreCase.Compare(a.transform.parent.name, b.transform.parent.name));
        if (dirtPlots != null)
        {
            //Debug.Log("Dirt plots does not equal null");
            for (int i = 0; i < dirtPlots.Length; i++)
            {
                if (dirtPlots[i].isOccupied)
                {
                    needsPlanting[i] = true;
                }
                else
                {
                    needsPlanting[i] = false;
                }
            }
            //Debug.Log("List of plots that are occupied = " + needsPlanting);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == 0)
        {
            DirtPlot[] allPlotsInScene = FindObjectsByType<DirtPlot>();
            Array.Sort(allPlotsInScene, (a, b) => -StringComparer.OrdinalIgnoreCase.Compare(a.transform.parent.name, b.transform.parent.name));
            LoadCrops(allPlotsInScene);
        }
        else
        {
            hasCompletedFirstScene = true;
        }
    }    

    public void LoadCrops(DirtPlot[] list)
    {
        if(list.Length != needsPlanting.Length)
        {
            Debug.Log("Error. Plots in scene is not equal to needPlanting length");
        }
        for (int i = 0;i < needsPlanting.Length;i++)
        {
            if (needsPlanting[i] == true)
            {
                Debug.Log("Planting corn at index " + i);
                list[i].loadCrop(cornSeed);
            }
        }
    }
    
    public void StarterPlants()
    {
        DirtPlot[] plotsInScene = FindObjectsByType<DirtPlot>();
        Array.Sort(plotsInScene, (a, b) => -StringComparer.OrdinalIgnoreCase.Compare(a.transform.parent.name, b.transform.parent.name));
        plotsInScene[plotsInScene.Length - 2].loadCrop(cornSeed);
        plotsInScene[plotsInScene.Length - 1].loadCrop(cornSeed);
    }
}
