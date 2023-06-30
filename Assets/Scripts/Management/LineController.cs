using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer lr;
    private Transform[] endPoints;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    public void SetupLine(GridPoint startGridPoint, GridPoint endGridPoint)
    {
        lr.positionCount = 2;

        Transform[] endPointSet = new Transform[] {startGridPoint.obj.transform, endGridPoint.obj.transform};
        this.endPoints = endPointSet;
    }

    // Update is called once per frame
    private void Update()
    {
        for (int i = 0; i < endPoints.Length; i++)
        {
            lr.SetPosition(i, endPoints[i].position);
        }
    }
}
