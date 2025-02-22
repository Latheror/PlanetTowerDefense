﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSlot : MonoBehaviour {

    [Header("Info")]
    public int id;  // Ground: "1xx", Satellite: "2xx"
    public float angleRad;
    public bool hasBuilding = false;
    public BuildingManager.BuildingType.BuildingLocationType locationType;

    [Header("Attached Building")]
    public Building attachedBuilding = null;

    [Header("UI")]
    public Color defaultColor;
    public Color selectionColor;

    public void ChangeColor(Color color)
    {
        gameObject.GetComponent<Renderer>().material.color = color;
    }

    public void SetDefaultColor()
    {
        ChangeColor(defaultColor);
    }

    public void SetSelectionColor()
    {
        //Debug.Log("SetSelectionColor");
        ChangeColor(selectionColor);
    }

    public void SetAngle(float angle)
    {
        this.angleRad = angle;
    }

    public void SetBuilding(Building building)
    {
        if(building != null)
        {
            hasBuilding = true;
            attachedBuilding = building;
            GetComponent<SphereCollider>().enabled = false;
        }
    }

    public bool CanBuildHere()
    {
        return (hasBuilding == false && attachedBuilding == null);
    }

    public void RemoveBuilding()
    {
        attachedBuilding = null;
        hasBuilding = false;
        GetComponent<SphereCollider>().enabled = true;
    }

    public void OnTouch()
    {
        Debug.Log("BuildingSlot | OnTouch");
        BuildingManager.instance.BuildingSlotTouched(this);
    }

}
