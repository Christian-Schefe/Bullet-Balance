using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    [SerializeField] private DashedLine line;

    public void Set(Node from, Node to, float spacing)
    {
        var dir = (to.transform.position - from.transform.position).normalized;
        Vector2 fromPos = from.transform.position + dir * spacing;
        Vector2 toPos = to.transform.position - dir * spacing;

        line.SetPositions(fromPos, toPos);
    }
}
