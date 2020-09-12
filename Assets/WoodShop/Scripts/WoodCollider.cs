using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodCollider : MonoBehaviour
{
    public EdgeCollider2D edgeCollider;
    public MegaShapeLine shapeLine;
    public TargetShapeCollider targetShape;

    private Vector2[] points;
    private bool targetCreated = false;

    /// <summary>
    /// Updates collider according to mesh line
    /// </summary>
    public void UpdateCollider(MegaShapeLine line)
    {
        points = new Vector2[line.splines[0].knots.Count];

        for(int i = 0; i < line.splines[0].knots.Count; i++)
        {
            points[i].x = line.GetKnotPos(0, i).z;
            points[i].y = line.GetKnotPos(0, i).x;
        }
        edgeCollider.points = points;

        if (!targetCreated)
        {
            targetShape.CreateTargetCollider();
            targetCreated = true;
        }
    }

}
