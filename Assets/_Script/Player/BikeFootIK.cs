using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class BikeFootIK : BaseFootIK
{
    public TwoBoneIKConstraint LeftConstraint;
    public TwoBoneIKConstraint RightConstraint;
    public Transform LeftPedal;
    public Transform RightPedal;
    public Transform LeftTarget;
    public Transform RightTarget;
    public Transform LeftTargetOrigin;
    public Transform RightTargetOrigin;
    public BikeController BikeCtl;

    private bool bindToPedal = false;
    public override void ActiveIk()
    {
        base.ActiveIk();
        LeftConstraint.weight = 1;
        RightConstraint.weight = 1;
        BikeCtl.BikeRenderer.SetActive(true);
        bindToPedal = true;
    }

    public override void DeactiveIk()
    {
        base.DeactiveIk();
        LeftConstraint.weight = 0;
        RightConstraint.weight = 0;
        BikeCtl.BikeRenderer.SetActive(false);
        bindToPedal = false;
        UnbindFromPedal();
    }
    private void UnbindFromPedal()
    {
        LeftTarget.localPosition = LeftTargetOrigin.localPosition;
        RightTarget.localPosition = RightTargetOrigin.localPosition;
    }

    private void LateUpdate()
    {
        if (!bindToPedal) return;
        LeftTarget.position = LeftPedal.position;
        RightTarget.position = RightPedal.position;
    }
}
