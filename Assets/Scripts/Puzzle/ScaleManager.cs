using System.Collections;
using UnityEngine;
using TMPro;

public class ScaleManager : MonoBehaviour
{
    [Header("Components")]
    public TextMeshPro middleText;
    public TextMeshPro leftText;
    public TextMeshPro rightText;
    public DoorHandler doorHandler;
    public ScalePlateHandler leftPlateHandler;
    public ScalePlateHandler rightPlateHandler;
    public Transform leftPlateHolder;
    public Transform rightPlateHolder;
    public Transform middleHandle;

    [Header("Settings")]
    public float weightLimitPerPlate = 20;
    public float moveUnitPerWeight = 0.02f;
    public float rotationUnitPerWeight = 0.25f;
    public float defaultPlateHeight = 1;
    public float moveTime = 0.1f;

    [Header("Plate and Weight properties")]
    public float halfOfPlateThiccnes = .05f;
    public float halfOfWeightThiccnes = .125f;

    [Header("Completed Text")]
    public string completedText = "SCALE COMPLETED!";

    float leftPlateSum = 0;
    float rightPlateSum = 0;

    void Start()
    {
        UpdateTexts();
        leftPlateHandler.WeightChange += OnLeftPlateChange;
        rightPlateHandler.WeightChange += OnRightPlateChange;
    }

    void OnLeftPlateChange(float change)
    {
        leftPlateSum += change;

        UpdateTexts();
        CheckForPuzzleCompletion();
        StartCoroutine(MovePlate());
    }

    void OnRightPlateChange(float change)
    {
        rightPlateSum += change;

        UpdateTexts();
        CheckForPuzzleCompletion();
        StartCoroutine(MovePlate());
    }

    void CheckForPuzzleCompletion()
    {
        if (leftPlateSum == weightLimitPerPlate && rightPlateSum == weightLimitPerPlate)
        {
            doorHandler.SetState(DoorHandler.state.OPEN);
            if (NotificationManager.instance != null)
            {
                NotificationManager.instance.ShowNotification(completedText, 1);
            }
        }
        else
        {
            doorHandler.SetState(DoorHandler.state.CLOSED);
        }

    }

    void UpdateTexts()
    {
        leftText.text = (leftPlateSum - weightLimitPerPlate).ToString();
        rightText.text = (rightPlateSum - weightLimitPerPlate).ToString();

        if (leftPlateSum == rightPlateSum)
        {
            middleText.text = "==";
        }
        else if (leftPlateSum < rightPlateSum)
        {
            middleText.text = "<";
        }
        else
        {
            middleText.text = ">";
        }
    }

    IEnumerator MovePlate()
    {
        float dif = Mathf.Clamp(leftPlateSum - rightPlateSum, -40, 40);//clamped so no funky stuff with overloading the scale alright?
        float moveOffset = Mathf.Abs(moveUnitPerWeight * dif);

        float percent = 0;
        float moveSpeed = 1 / moveTime;
        //moving all the cubes the same amount as the plates

        float leftPlateNewPos = 1;
        float rightPlateNewPos = 1;
        Vector3 middleBarNewRot = new Vector3(0, 0, dif * rotationUnitPerWeight);

        if (dif > 0)
        {
            leftPlateNewPos = 1 - moveOffset;
            rightPlateNewPos = 1 + moveOffset;
        }
        else if (dif < 0)
        {
            leftPlateNewPos = 1 + moveOffset;
            rightPlateNewPos = 1 - moveOffset;
        }

        while (percent < 1)
        {
            percent += Time.deltaTime * moveSpeed;

            leftPlateHolder.position = new Vector3(leftPlateHolder.position.x, Mathf.Lerp(leftPlateHolder.position.y, leftPlateNewPos, percent), leftPlateHolder.position.z);
            leftPlateHandler.AdjustWeightsOnPlate(leftPlateHolder.position.y + halfOfPlateThiccnes + halfOfWeightThiccnes);
            rightPlateHolder.position = new Vector3(rightPlateHolder.position.x, Mathf.Lerp(rightPlateHolder.position.y, rightPlateNewPos, percent), rightPlateHolder.position.z);
            rightPlateHandler.AdjustWeightsOnPlate(rightPlateHolder.position.y + halfOfPlateThiccnes + halfOfWeightThiccnes);
            middleHandle.rotation = Quaternion.Lerp(middleHandle.rotation, Quaternion.Euler(middleBarNewRot), percent);

            yield return null;
        }
    }
}
