using UnityEngine;

public class bunnyGrab : MonoBehaviour
{
    public string bunnyID;

    public void Awake()
    {
        bunnyID = this.name + "-" + transform.position.ToString();
    }
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided Object: " + other.gameObject.name);
        if (other.gameObject.tag == "Player")
        {
            bunnyGot();
        }
    }

    public void bunnyGot()
    {
        GameController.instance.bunnyCollect(bunnyID);
        this.GetComponent<SphereCollider>().enabled = false;

        transform.parent.gameObject.SetActive(false);
        
        //this.gameObject.SetActive(false);
        /*foreach (GameObject child in this.gameObject)
        {
            child.SetActive(false);
            
        }*/

    }
}
