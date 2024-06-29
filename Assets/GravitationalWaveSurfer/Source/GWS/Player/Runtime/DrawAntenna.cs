using UnityEngine;

namespace GWS.Player.Runtime
{
    /// <summary>
    /// Draws the line between the player's bob and the player's body.
    /// </summary>
    public class DrawAntenna: MonoBehaviour
    {
        [SerializeField]
        private GameObject[] joints;

        private LineRenderer line;

        private void Start()
        {
            line = GetComponent<LineRenderer>();
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < joints.Length; i++)
            {
                line.SetPosition(i, joints[i].transform.position);
            }
        }
    }

}