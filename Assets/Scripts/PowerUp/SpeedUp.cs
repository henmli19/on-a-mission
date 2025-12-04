using UnityEngine;

public class SpeedBoost : PowerUp
{
    public float speedMultiplier = 2f;

    protected override void ApplyPowerUp(GameObject player)
    {
        Debug.Log($"Speed Boost! x{speedMultiplier} for {duration} seconds.");

        RobotController robot = player.GetComponent<RobotController>();
        if (robot != null)
        {
            robot.StartCoroutine(robot.ApplySpeedBoost(speedMultiplier, duration));
        }
    }
}