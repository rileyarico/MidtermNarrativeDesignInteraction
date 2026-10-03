using UnityEngine;

public class moveAway : MonoBehaviour
{
    public Transform originalTransform;
    public Transform playerTransform;
    public float speed = 1f;
    public Rigidbody rb;

    public void Awake()
    {
        originalTransform = transform;
        //playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Vector3 faceThisWay = transform.position - other.gameObject.transform.position;
            faceThisWay.y = 0;
            Quaternion newQ = new Quaternion(faceThisWay.x, faceThisWay.y, faceThisWay.z, 180);
            Debug.Log("NewQ is " +  newQ);
            transform.rotation = newQ;
            rb.linearVelocity = faceThisWay * speed;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        rb.linearVelocity = Vector3.zero;
    }


}
