using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class coinGrab : MonoBehaviour
{
    public string coinID;
    public bool canInteract;
    public TextMeshProUGUI interactText;

    public void Awake()
    {
        coinID = this.name + "-" + transform.position.ToString();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player entered box trigger");
            setText("Press E to Harvest");
            canInteract = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            setText("");
            canInteract = false;
        }
    }
    public void setText(string text)
    {
        interactText.text = text;
    }

    public void Update()
    {
        if (canInteract)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                coinGot();
                canInteract = false;
            }
        }
    }

    public void coinGot()
    {
        setText("");
        GameController.instance.coinCollect(coinID);
        this.GetComponent<MeshRenderer>().enabled = false;
        //this.GetComponent<CapsuleCollider>().enabled = false;
        //this.GetComponent<SphereCollider>().enabled = false;
        foreach (Collider collide in this.gameObject.GetComponentsInChildren<Collider>())
        {
            collide.enabled = false;
        }

    }
}
