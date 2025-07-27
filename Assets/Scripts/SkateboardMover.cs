using UnityEngine;

public class SkateboardMover : MonoBehaviour
{
    [SerializeField] Rigidbody2D frontWheel;
    [SerializeField] Rigidbody2D rearWheel;
    [SerializeField] float torque;

    int input;

    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            input = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            input = 1;
        }
        else if (!Input.GetKeyDown(KeyCode.A) && !Input.GetKeyDown(KeyCode.D))
        {
            input = 0;
        }
    }

    private void FixedUpdate()
    {
        if (input != 0)
        {
            Accelerate(input, torque);
        }
    }

    void Accelerate(int direction, float torque)
    {
        //(front wheel was locking up and getting dragged along; rear-axle drive got rid of the issue)
        torque *= -direction;
        if (torque < 0) //forward movement
        {
            rearWheel.AddTorque(torque);
        }
        else //backward movement
        {
            frontWheel.AddTorque(torque);
        }
    }
}
