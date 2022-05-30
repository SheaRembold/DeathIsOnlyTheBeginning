using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable
{
    Transform HitTarget { get; }
    void SetHighlighted(bool selected);
    void SetSelected(bool selected);

}
