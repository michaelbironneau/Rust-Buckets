using UnityEngine;

public interface ISelectable
{
    void Select();

    void Deselect();

    void MoveTo(Vector3 worldPos);
}
