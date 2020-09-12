using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour,IPointerDownHandler,IDragHandler,IPointerUpHandler
{
    public Transform tool;
	public ReShape reShape;
    public Camera mainCamera;
	public Image toolImage;
	public Sprite[] toolImages;
	public GameObject tapToStart,retryButton,failedPanel,successPanel;
	public Text successText,failText;
	public bool isGameStopped = false;

	private Vector3 screenPoint, offset, tmpToolImagePos,tmpToolPos, cursorPoint, cursorPosition;
	private Animator animator;
	

    private void Awake()
    {
		animator = mainCamera.GetComponent<Animator>();
		tmpToolImagePos = toolImage.rectTransform.position;
		tmpToolPos = tool.position;
	}

    public void OnPointerDown(PointerEventData eventData)
	{
		tool.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
		screenPoint = mainCamera.WorldToScreenPoint(tool.position);
		offset = tool.position - mainCamera.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, screenPoint.z));
        if (tapToStart.activeSelf)
        {
			retryButton.SetActive(true);
			tapToStart.SetActive(false);
        }
	}

    public void OnPointerUp(PointerEventData eventData)
    {
		tool.GetComponent<Rigidbody2D>().constraints =  RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezePositionX  | RigidbodyConstraints2D.FreezeRotation; 
	}

    void IDragHandler.OnDrag(PointerEventData eventData)
	{
        if (!isGameStopped)
        {
			cursorPoint = new Vector3(eventData.position.x, eventData.position.y, screenPoint.z);
			cursorPosition = mainCamera.ScreenToWorldPoint(cursorPoint) + offset;
			tool.position = cursorPosition;
			toolImage.rectTransform.position = mainCamera.WorldToScreenPoint(tool.position) - new Vector3(-3.8f, 83, 0);
			toolImage.rectTransform.rotation = Quaternion.Euler(0, 0, -toolImage.rectTransform.anchoredPosition.x / 50);
		}
	}

	/// <summary>
	/// Changes tool type
	/// </summary>
	public void ChangeTool(int ind)
    {
		reShape.toolType = ind;
		toolImage.sprite = toolImages[ind];
		float yScale;
		if (ind == 0)
		{
			yScale = 0.16f;
			animator.SetBool("SharpTool", false);
		}
		else if (ind == 1)
		{
			yScale = 0.1f;
			animator.SetBool("SharpTool", false);
		}
		else
		{
			yScale = 0.1f;
			animator.SetBool("SharpTool", true);
		}
		ResetToolImage();
		tool.localScale = new Vector3(tool.localScale.x, yScale, tool.localScale.z);
    }

	/// <summary>
	/// Moves tool to initial point
	/// </summary>
	public void ResetToolImage()
    {
		toolImage.rectTransform.position = tmpToolImagePos;
		toolImage.rectTransform.rotation = Quaternion.Euler(0,0,0);
		tool.position = tmpToolPos;
    }

	/// <summary>
	/// Called when failed
	/// </summary>
	public void Failed(float percent)
    {	
		if (percent > 1) percent = 1;
		else if (percent < 0 && percent != -1) percent = 0;
		failedPanel.SetActive(true);
		retryButton.SetActive(false);
		isGameStopped = true;
		if (percent != -1) failText.text = "FAIL!!! - Result: " + (percent*100).ToString("0") + "%";
		else failText.text = "FAIL!!!";
	}

	/// <summary>
	/// Called when successed
	/// </summary>
	public void Success(float percent)
	{
		if (percent > 1) percent = 1;
		else if (percent < 0) percent = 0;
		successPanel.SetActive(true);
		retryButton.SetActive(false);
		isGameStopped = true;
		successText.text = "SUCCESS!!! - Result: " + (percent * 100).ToString("0") + "%";
	}
}
