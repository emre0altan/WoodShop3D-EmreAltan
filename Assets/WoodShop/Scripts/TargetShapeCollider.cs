using UnityEngine;

public class TargetShapeCollider : MonoBehaviour
{
    public EdgeCollider2D editCollider,targetCollider;
    public LineRenderer lineRenderer;
    public InputHandler inputHandler;

    private Vector2[] tmpPoints;
    private Vector3[] tmp3Points;
    private float maxDist = 0, result = 0;
    private int randShape = 0;


    /// <summary>
    /// Creates a EdgeCollider2D as a shape of target mesh
    /// </summary>
    public void CreateTargetCollider()
    {
        tmpPoints = new Vector2[editCollider.pointCount];
        tmpPoints[0] = editCollider.points[0];        
        tmpPoints[editCollider.pointCount-1] = editCollider.points[editCollider.pointCount-1];
        randShape = Random.Range(0, 3);
        
        if(randShape == 0)
        {
            for (int i = 1; i < editCollider.pointCount - 1; i++)
            {
                tmpPoints[i] = editCollider.points[i] + new Vector2(0, i * 0.0005f + 0.03f);
            }
        }
        else if(randShape == 1)
        {
            for (int i = 1; i < editCollider.pointCount - 1; i++)
            {
                if(editCollider.pointCount / 2 <= i)tmpPoints[i] = editCollider.points[i] + new Vector2(0, (editCollider.pointCount/2-i) * 0.001f + 0.13f);
                else if(editCollider.pointCount / 2 > i)tmpPoints[i] = editCollider.points[i] + new Vector2(0, (i-editCollider.pointCount/2) * 0.001f + 0.13f);
            }
        }
        else if(randShape == 2)
        {
            for (int i = 1; i < editCollider.pointCount - 1; i++)
            {
                tmpPoints[i] = editCollider.points[i] + new Vector2(0, 0.07f);
            }
        }
        Debug.Log(randShape);

        targetCollider.points = tmpPoints;

        lineRenderer.positionCount = editCollider.pointCount*2;
        tmp3Points = new Vector3[editCollider.pointCount*2];
        for(int i = 0; i < editCollider.pointCount*2; i++)
        {
            if (i < editCollider.pointCount) tmp3Points[i] = new Vector3(tmpPoints[i].x * 0.93f, tmpPoints[i].y + 0.09f, -0.2f);
            else if(i < editCollider.pointCount*2-1) tmp3Points[i] = new Vector3(tmpPoints[editCollider.pointCount-i % editCollider.pointCount-1].x * 0.93f, tmpPoints[0].y *2 - tmpPoints[editCollider.pointCount-i % editCollider.pointCount-1].y+0.105f, -0.2f);
            else tmp3Points[i] = new Vector3(tmpPoints[editCollider.pointCount - i % editCollider.pointCount - 1].x * 0.93f, tmpPoints[i%editCollider.pointCount].y+ 0.09f, -0.2f);
        }
        lineRenderer.SetPositions(tmp3Points);
        maxDist = 0;
        for (int i = 0; i < editCollider.pointCount; i++)
        {
            maxDist += (editCollider.points[i] - targetCollider.points[i]).magnitude;
        }
    }

    /// <summary>
    /// Calculates result and show on screen
    /// </summary>
    public void ShowResults()
    {
        result = 0;
        for(int i = 0; i < editCollider.pointCount; i++)
        {
            result += (editCollider.points[i] - targetCollider.points[i]).magnitude;
        }
        if(1 - (result - 5) / (maxDist - 5) > 0.5f)
        {
            inputHandler.Success(1-(result-5)/(maxDist-5));
        }
        else
        {
            inputHandler.Failed(1-(result - 5) / (maxDist - 5));
        }
    }
}
