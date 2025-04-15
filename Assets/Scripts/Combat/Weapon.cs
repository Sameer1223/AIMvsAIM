using Unity.Netcode;
using UnityEngine;

public abstract class Weapon : NetworkBehaviour
{
    public int damage;
    
    public abstract void Fire();
}
