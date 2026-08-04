using System;
using UnityEngine;

public class OrangeBoxGhost : MonoBehaviour
{
    [SerializeField] private SnapTarget snapTarget;
    private bool countFlag;

    private void Awake()
    {
        snapTarget = transform.GetComponent<SnapTarget>();
    }

    private void Update()
    {
        if (snapTarget.isOccupied == false && countFlag == false)
        {
            MFGDashboard.Instance.UpdateProductsCompleted(1);
            countFlag = true;
        }

    }
}
