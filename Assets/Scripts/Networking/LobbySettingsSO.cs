using UnityEngine;

[CreateAssetMenu(fileName = "LobbySettingsSO", menuName = "Scriptable Objects/LobbySettingsSO")]
public class LobbySettingsSO : ScriptableObject
{
    public Size selectedSize;
    public Speed selectedSpeed;
    public AimWeapon selectedWeapon;
    public JumpForce selectedJumpForce;

    public enum Size
    {
        Large,
        Medium,
        Small
    }

    public enum Speed
    {
        Slow,
        Medium,
        Fast
    }

    public enum AimWeapon
    {
        Clicking,
        Tracking
    }

    public enum JumpForce
    {
        Low,
        Medium,
        High
    }
}
