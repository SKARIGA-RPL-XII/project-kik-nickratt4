using UnityEngine;

public interface ITarget
{
    Transform GetTransform();
    void Select();
    void Deselect();
}
