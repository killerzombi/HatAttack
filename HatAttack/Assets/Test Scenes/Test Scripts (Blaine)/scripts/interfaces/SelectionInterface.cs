using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SelectionInterface {
    void selected(Color C);
    void deselected();
    Vector2Int getPosition();
}
