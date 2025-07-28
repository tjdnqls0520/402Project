using UnityEngine;

namespace GDS.Common.Scripts {
    /// <summary>
    /// Adds character WASD movement
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour {
        public float speed = 10f;
        Rigidbody rb;

        void Awake() => rb = GetComponent<Rigidbody>();

        void FixedUpdate() {
            Vector3 dir = new(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (dir.sqrMagnitude > 0.01f) {
                Vector3 move = dir.normalized * speed * Time.fixedDeltaTime;
                rb.MovePosition(rb.position + move);
                rb.MoveRotation(Quaternion.LookRotation(dir));
            }
        }
    }
}