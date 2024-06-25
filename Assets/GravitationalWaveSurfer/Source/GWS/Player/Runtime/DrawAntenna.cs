using UnityEngine;

public class DrawAntenna : MonoBehaviour
{
    [SerializeField]
    private GameObject[] joints;

    private LineRenderer line;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        for (int i = 0; i < joints.Length; i++)
        {
            line.SetPosition(i, joints[i].transform.position);
        }
    }
}
