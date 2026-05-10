using Unity.Cinemachine;
using UnityEngine;

namespace Cameras
{
    public class GetCameraPriority : MonoBehaviour
    {
        public CinemachineCamera Vcam;
        public int CameraPriority = 10;
        public float InitialDelay = 0.2f;

        private void Start()
        {
            StartCoroutine(ApplyPriorityAfterDelay());
        }

        private System.Collections.IEnumerator ApplyPriorityAfterDelay()
        {
            yield return new WaitForSeconds(InitialDelay);

            if (Vcam != null)
            {
                Vcam.Priority = CameraPriority;
            }
            else
            {
                Debug.LogWarning("No Cinemachine Camera assigned.");
            }
        }
    }
}