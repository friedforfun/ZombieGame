using UnityEngine;

public interface IShootable
{
    void shoot(Vector3 targetDirection);

    void reload();
}
