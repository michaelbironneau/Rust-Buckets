using UnityEngine;

public interface ISelectable
{
    void Select();

    void Deselect();

    void RelatedAction(GameObject obj);

    void MoveTo(Vector3 worldPos);
}
