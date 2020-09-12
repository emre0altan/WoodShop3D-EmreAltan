using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReShape : MonoBehaviour
{
    public MegaShapeLine myLine;
    public MegaShapeLathe lathe;
    public WoodCollider woodCollider;
    public InputHandler inputHandler;
    public ParticleSystem cutEffect;
    public GameObject tapToStartPanel;
    public int toolType;


    private ParticleSystem tmpPar;
    private Vector3 tmpVcc, newPos;
    private int contPoint, vertexDetail = 200, vertexBound = 50;
    private float sculptSpeed = 0.001f, distConst = -0.03f, updateCounter = 1f;    
    private bool woodThrowed = false;


    private void Start()
    {
        tmpVcc = new Vector3();
        tmpPar = GameObject.Instantiate(cutEffect);

        ResetWood();
    }

    private void Update()
    {
        updateCounter -= Time.deltaTime;
        if(updateCounter <= 0)
        {
            lathe.update = false;
        }
    }

    /// <summary>
    /// Deforms wood relative to tool type and update necessary parts
    /// </summary>
    public void CutWood()
    {
        contPoint = Mathf.FloorToInt((myLine.splines[0].knots.Count-2) * (myLine.GetKnotPos(0, 0).z - transform.position.x) / (myLine.GetKnotPos(0,0).z-myLine.GetKnotPos(0,myLine.splines[0].knots.Count-1).z)) +1;
        if (contPoint < 1) contPoint = 1;
        else if (contPoint > myLine.splines[0].knots.Count - 1) contPoint = myLine.splines[0].knots.Count - 1;

        if (toolType == 0) {
            distConst = -0.025f;
            vertexBound = 30;
        }
        else if (toolType == 1) {
            distConst = -0.04f;
            vertexBound = 30;
        }
        else
        {
            distConst = -0.04f;
            vertexBound = 60;
        }

        if (myLine.GetKnotPos(0, contPoint).x - transform.position.y > distConst) return;


        #region Tool Edits
        
        sculptSpeed = 0.001f;

        //Main sculpt deformation
        if (myLine.GetKnotPos(0, contPoint).x <= 0.09f) myLine.MoveKnot(0, contPoint, myLine.GetKnotPos(0, contPoint) + new Vector3(sculptSpeed, 0, 0));

        //If knot move too far to up, then restore it
        if (myLine.GetKnotPos(0, contPoint).x > 0.085f) {
            tmpVcc.x = 0.09f;
            tmpVcc.y = myLine.GetKnotPos(0, contPoint).y;
            tmpVcc.z = myLine.GetKnotPos(0, contPoint).z;
            myLine.MoveKnot(0, contPoint,tmpVcc);

            ThrowWood(contPoint,vertexDetail/vertexBound);
            woodThrowed = true;
        }



        if (!woodThrowed)
        {
            //Deform right adjacent vertices relative to tool type
            for (int i = 0; i < vertexDetail / vertexBound; i++)
            {
                if (contPoint - 1 - i >= 0)
                {
                    if (toolType == 0 && myLine.GetKnotPos(0, contPoint - 1 - i).x <= myLine.GetKnotPos(0, contPoint).x)
                    {
                        myLine.MoveKnot(0, contPoint - 1 - i, new Vector3(myLine.GetKnotPos(0, contPoint).x, 0, myLine.GetKnotPos(0, contPoint - 1 - i).z));
                    }
                    else if (toolType == 1 && myLine.GetKnotPos(0, contPoint - 1 - i).x <= myLine.GetKnotPos(0, contPoint).x)
                    {
                        if(myLine.GetKnotPos(0,contPoint - 1 -i).x < myLine.GetKnotPos(0, contPoint).x - i * 0.0006f)
                            myLine.MoveKnot(0, contPoint - 1 - i, new Vector3(myLine.GetKnotPos(0, contPoint).x - i * 0.0006f, 0, myLine.GetKnotPos(0, contPoint - 1 - i).z));
                    }
                    else if (toolType == 2 && myLine.GetKnotPos(0, contPoint - 1 - i).x <= myLine.GetKnotPos(0, contPoint).x)
                    {
                        if (myLine.GetKnotPos(0, contPoint - 1 - i).x < myLine.GetKnotPos(0, contPoint).x - i * 0.004f)
                            myLine.MoveKnot(0, contPoint - 1 - i, new Vector3(myLine.GetKnotPos(0, contPoint).x - i * 0.004f, 0, myLine.GetKnotPos(0, contPoint - 1 - i).z));
                    }
                }
            }
           
            //Deform left adjacent vertices relative to tool type
            for (int i = 0; i < vertexDetail / vertexBound; i++)
            {
                if (contPoint + 1 + i < myLine.splines[0].knots.Count)
                {
                    if (toolType == 0 && myLine.GetKnotPos(0, contPoint + 1 + i).x <= myLine.GetKnotPos(0, contPoint).x)
                    {
                        //Right knot
                        myLine.MoveKnot(0, contPoint + 1 + i, new Vector3(myLine.GetKnotPos(0, contPoint).x, 0, myLine.GetKnotPos(0, contPoint + 1 + i).z));
                    }
                    else if (toolType == 1 && myLine.GetKnotPos(0, contPoint + 1 + i).x <= myLine.GetKnotPos(0, contPoint).x)
                    {
                        if (myLine.GetKnotPos(0, contPoint + 1 + i).x < myLine.GetKnotPos(0, contPoint).x - i * 0.0006f)
                            myLine.MoveKnot(0, contPoint + 1 + i, new Vector3(myLine.GetKnotPos(0, contPoint).x - i * 0.0006f, 0, myLine.GetKnotPos(0, contPoint + 1 + i).z));
                    }
                    else if (toolType == 2 && myLine.GetKnotPos(0, contPoint + 1 + i).x <= myLine.GetKnotPos(0, contPoint).x)
                    {
                        if (myLine.GetKnotPos(0, contPoint + 1 + i).x < myLine.GetKnotPos(0, contPoint).x - i * 0.004f)
                            myLine.MoveKnot(0, contPoint + 1 + i, new Vector3(myLine.GetKnotPos(0, contPoint).x - i * 0.004f, 0, myLine.GetKnotPos(0, contPoint + 1 + i).z));
                    }
                }
            }
        }

        woodThrowed = false;

        #endregion

        tmpPar.transform.position = transform.position + new Vector3(0, 0.08f, -0.5f);
        tmpPar.Play();

        lathe.update = true;
        updateCounter = 0.1f;
        woodCollider.UpdateCollider(myLine);
    }

    /// <summary>
    /// Removes divided wood part from main part
    /// </summary>
    public void ThrowWood(int contactPoint, int range)
    {
        int direction;
        if (contactPoint < myLine.splines[0].knots.Count / 2) direction = 1;
        else direction = -1;

        if (direction == -1)
        {
            for (int i= myLine.splines[0].knots.Count-1; i >contactPoint-range; i--)
            {
                //if (i < 0) break;
                myLine.splines[0].knots.RemoveAt(i);
                woodCollider.edgeCollider.points[i] = woodCollider.edgeCollider.points[contactPoint];
            }
            myLine.SetKnotPos(0, contactPoint - range, new Vector3(0.09f, 0, myLine.GetKnotPos(0, contactPoint - range - 1).z));
        }
        else
        {
            for (int i = contactPoint + range - 1; i >= 0; i--)
            {
                //if (i > myLine.splines[0].knots.Count - 1) continue;
                myLine.splines[0].knots.RemoveAt(i);
                woodCollider.edgeCollider.points[i] = woodCollider.edgeCollider.points[contactPoint];
            }
            myLine.SetKnotPos(0, 0, new Vector3(0.09f, 0, myLine.GetKnotPos(0, 1).z));
        }

        if(myLine.splines[0].knots.Count < 5)
        {
            lathe.gameObject.SetActive(false);
            woodCollider.gameObject.SetActive(false);
        }

    }

    /// <summary>
    /// Reset wood shape to initial state
    /// </summary>
    public void ResetWood()
    {
        int knotCount = myLine.splines[0].knots.Count;

        for (int i = 0; i < vertexDetail + 2; i++)
        {
            if (i == 0)
            {
                newPos = new Vector3(0.09f, 0, 0.2f);
            }
            else if (i < vertexDetail + 1)
            {
                newPos = new Vector3(-0.04446144f, 0, 0.2f - 0.005f * (i - 1) * 80 / vertexDetail);
            }
            else
            {
                newPos = new Vector3(0.09f, 0, 0.2f - 0.005f * (i - 2) * 80 / vertexDetail);
            }


            if (i > knotCount - 1)
            {
                myLine.splines[0].AddKnot(newPos, newPos, newPos);
            }
            else
            {
                myLine.SetKnotPos(0, i, newPos);
            }
        }
        if (!lathe.gameObject.activeSelf)
        {
            lathe.gameObject.SetActive(true);
            woodCollider.gameObject.SetActive(true);
        }

        myLine.smoothness = 0f;
        myLine.AutoCurve();

        lathe.update = true;
        updateCounter = 0.1f;

        tapToStartPanel.SetActive(true);
        inputHandler.ResetToolImage();
        inputHandler.isGameStopped = false;

        woodCollider.UpdateCollider(myLine);
        woodCollider.targetShape.CreateTargetCollider();
    }

}
