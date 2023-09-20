using UnityEngine;

namespace CatInMaze
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target; // The target the camera should follow
        public float smoothing = 5.0f; // Determines how smooth the camera movement will be. Higher values mean slower movement.

        private Vector3 offset; // The initial offset between the camera and the target

        private void Start()
        {
            // Calculate the initial offset between the camera and the target
            offset = transform.position - target.position;
        }

        private void FixedUpdate()
        {
            if (target)
            {
                // Calculate the desired position for the camera based on the target's position and the offset
                Vector3 desiredPosition = target.position + offset;

                // Lerp between the current camera position and the desired position to create a smooth movement
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothing * Time.fixedDeltaTime);

                // Set the camera's position to the smoothed position
                transform.position = smoothedPosition;
            }
        }
    }
}