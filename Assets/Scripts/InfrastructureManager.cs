﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfrastructureManager : MonoBehaviour {

    public static InfrastructureManager instance;
    void Awake()
    {
        if (instance != null) { Debug.LogError("More than one InfrastructureManager in scene !"); return; }
        instance = this;
    }

    public GameObject selectedBuilding;
    public GameObject previouslySelectedBuilding;

    public List<GameObject> recyclingStationsList = new List<GameObject>();
    public GameObject spaceport = null; // unique

    public void BuildingTouched(GameObject building)
    {
        selectedBuilding = building;
        //Debug.Log(selectedBuilding.GetComponent<Building>().buildingType.name + " selected.");

        GameManager.instance.ChangeSelectionState(GameManager.SelectionState.BuildingSelected);

        if (previouslySelectedBuilding != null)
        {
            previouslySelectedBuilding.GetComponent<Building>().BuildingDeselected();
        }

        // Transmit info to concerned building script
        selectedBuilding.GetComponent<Building>().BuildingTouched();

        // Transmit info to BuildingInfoPanel
        BuildingInfoPanel.instance.BuildingTouched(selectedBuilding);

        // Take actions based on building "Tags" list
        BuildingTagsActions(selectedBuilding);

        previouslySelectedBuilding = selectedBuilding;
    }

    public bool UpgradeBuildingRequest(GameObject building)
    {
        bool requestAccepted = false;
        //Debug.Log("UpgradeBuildingRequest");
        if (ResourcesManager.instance.CanPayUpgradeCosts(building.GetComponent<Building>()))
        {
            if(ResourcesManager.instance.PayUpgradeCosts(building.GetComponent<Building>()))
            {
                //Debug.Log("UpgradeBuilding : Can pay upgrade");
                building.GetComponent<Building>().UpgradeToNextTier();
                requestAccepted = true;
            }
        }
        else
        {
            //Debug.Log("UpgradeBuilding : Can't pay upgrade !");
        }
        return requestAccepted;
    }

    public void DestroyBuilding(GameObject building)
    {
        Building b = building.GetComponent<Building>();       
        b.DestroyBuildingSpecificActions();
        b.buildingSpot.GetComponent<BuildingSlot>().RemoveBuilding();

        BuildingManager.instance.buildingList.Remove(building);

        // Building type lists
        if(b.buildingType.name == "Recycling Station")
        {
            recyclingStationsList.Remove(building);
        }

        if(b.buildingType.name == "Spaceport")
        {
            Spaceport sp = building.GetComponent<Spaceport>();
            foreach (GameObject spaceship in sp.attachedSpaceships)
            {
                spaceship.GetComponent<Spaceship>().DestroySpaceship();
            }
            spaceport = null;   // Reset associated spaceport
        }

        // Unique Buildings
        if (b.buildingType.isUnique)
        {
            //ShopPanel.instance.DisplayBuildingShopItemBack(b.buildingType);
        }

        EnergyPanel.instance.UpdateEnergyProductionAndConsumption();

        GameManager.instance.ChangeSelectionState(GameManager.SelectionState.Default);

        Destroy(building);
    }

    public void BuildingTagsActions(GameObject building)
    {
        //Debug.Log("BuildingTagsActions");
        if(building.GetComponent<Building>().HasTag("spaceport"))
        {
            Debug.Log("Spaceport Touched !");

            SpaceportInfoPanel.instance.SpaceportTouched(building);
        }
        else
        {
            SpaceportInfoPanel.instance.DisplayPanel(false);
        }
    }

    public void ClearBuildings()
    {
        foreach (GameObject building in BuildingManager.instance.buildingList.ToArray())
        {
            DestroyBuilding(building);
        }
    }

    public void SetSpaceport(GameObject newSpaceport)
    {
        spaceport = newSpaceport;
    }
}
