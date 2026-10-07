using UnityEngine;
using Yarn.Unity;

public class Gate : MonoBehaviour
{
    public Transform gate1;
    public int gate1Rot;
    public Transform gate2;
    public int gate2Rot;

    public void Open()
    {
        gate1.transform.Rotate(gate1.transform.rotation.x, gate1Rot, gate1.transform.rotation.z);
        gate2.transform.Rotate(gate2.transform.rotation.x, gate2Rot, gate2.transform.rotation.z);
    }
}
