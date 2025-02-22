﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class BuildingManager : MonoBehaviour {

    public static BuildingManager instance;
    void Awake() {
        if (instance != null) { Debug.LogError("More than one BuildingManager in scene !"); return; } instance = this;
    }

    public enum BuildingState { Default, BuildingSelected, LocationSelected, BuildingAndLocationSelected, Building }
    public enum BuildingCategory { Attack, Defense, Production }

    [Header("World")]
    public GameObject mainPlanet;

    [Header("Building Stage")]
    public BuildingState buildingState;
    public GameObject buildingPreviewIndicator;
    public GameObject chosenBuildingSlot;
    public BuildingType selectedBuilding = null;

    [Header("Buildings")]
    public List<BuildingType> availableBuildings = new List<BuildingType>();
    public List<GameObject> buildingList = new List<GameObject>();

    [Header("Building Prefabs")]
    public GameObject laserTurretPrefab;
    public GameObject bulletTurretPrefab;
    public GameObject freezingTurretPrefab;
    public GameObject powerPlantPrefab;
    public GameObject mineBuildingPrefab;
    public GameObject laserSatellitePrefab;
    public GameObject shockSatellitePrefab;
    public GameObject debrisCollectorStationPrefab;
    public GameObject satelliteSolarStationPrefab;
    public GameObject healingTurretPrefab;
    public GameObject spaceportPrefab;
    public GameObject stormSatellitePrefab;
    public GameObject meteorCrusherPrefab;

    [Header("Other Prefabs")]
    public GameObject powerMissingIndicatorPrefab;

    [Header("UI")]
    public GameObject buildButton;
    public GameObject buildButtonForeground;
    public Sprite buildButtonCanPaySprite;
    public Sprite buildButtonCantPaySprite;
    public TextMeshProUGUI buildButtonText;
    public Color canPayColor;
    public Color cantPayColor;

    [Header("Settings")]
    public float powerMissingIndicatorPlacementDistance = 20f;

    const int mineFacilityIndex = 5;


    public void Initialize()
    {
        buildingState = BuildingState.Default;
        mainPlanet = GameManager.instance.mainPlanet;

        ShowBuildButton(false);
    }

    public void SetAvailableBuildings()
    {
        availableBuildings.Add(new BuildingType(1, "Laser Turret", laserTurretPrefab, BuildingCategory.Attack, 25f,
                new List<ResourcesManager.ResourceAmount>(){
                    new ResourcesManager.ResourceAmount("steel", 75),
                    new ResourcesManager.ResourceAmount("carbon", 35)
                },
                BuildingType.BuildingLocationType.Planet, "LaserTurret", 3, 0,
                "Powerful turret firing a laser beam at incoming enemies.",
                new List<ResourcesManager.UpgradeCost>(){
                    new ResourcesManager.UpgradeCost(2, new List<ResourcesManager.ResourceAmount>(){
                        new ResourcesManager.ResourceAmount("steel", 100),
                        new ResourcesManager.ResourceAmount("carbon", 75)

                    }),
                    new ResourcesManager.UpgradeCost(3, new List<ResourcesManager.ResourceAmount>(){
                       new ResourcesManager.ResourceAmount("composite", 80),
                       new ResourcesManager.ResourceAmount("electronics", 50)
                    })
                },
                new List<SpecializedUpgrade>()  // Needs to contain 3 elements, two stat upgrade, one side upgrade
                {
                    new SpecializedUpgrade(SpecializedUpgrade.UpgradeType.statUpgrade, new List<ResourcesManager.ResourceAmount>(){
                       new ResourcesManager.ResourceAmount("composite", 50),
                       new ResourcesManager.ResourceAmount("electronics", 100)
                    }, null, 100),
                    new SpecializedUpgrade(SpecializedUpgrade.UpgradeType.sideUpgrade, new List<ResourcesManager.ResourceAmount>(){
                       new ResourcesManager.ResourceAmount("composite", 200),
                       new ResourcesManager.ResourceAmount("electronics", 50)
                    }, null, 100),
                    new SpecializedUpgrade(SpecializedUpgrade.UpgradeType.statUpgrade, new List<ResourcesManager.ResourceAmount>(){
                       new ResourcesManager.ResourceAmount("composite", 100),
                       new ResourcesManager.ResourceAmount("electronics", 100)
                    }, null, 100)
                },
                true, false,
                new List<Building.BuildingStat>()
                {
                    new Building.BuildingStat(Building.BuildingStat.StatType.damagePower)
                },
                false
                ));

        availableBuildings.Add(new BuildingType(2, "Missile Turret", bulletTurretPrefab, BuildingCategory.Attack, 20f, new List<ResourcesManager.ResourceAmount>(){
                new ResourcesManager.ResourceAmount("steel", 50),
                new ResourcesManager.ResourceAmount("carbon", 50)
                },
                BuildingType.BuildingLocationType.Planet, "MissileTurret", 3, 1,
                "Shoots missiles at incoming enemies.",
                new List<ResourcesManager.UpgradeCost>() {
                    new ResourcesManager.UpgradeCost(2, new List<ResourcesManager.ResourceAmount>(){
                        new ResourcesManager.ResourceAmount("steel", 150),
                        new ResourcesManager.ResourceAmount("carbon", 100)

                    }),
                    new ResourcesManager.UpgradeCost(3, new List<ResourcesManager.ResourceAmount>(){
                       new ResourcesManager.ResourceAmount("composite", 75),
                       new ResourcesManager.ResourceAmount("electronics", 50)
                    })},
                new List<SpecializedUpgrade>()
                {

                },
                true, false,
                new List<Building.BuildingStat>()
                {
                    new Building.BuildingStat(Building.BuildingStat.StatType.damagePower)
                },
                false
                ));

        availableBuildings.Add(new BuildingType(3, "Freezing Turret", freezingTurretPrefab, BuildingCategory.Defense, 10f,
                new List<ResourcesManager.ResourceAmount>(){
                    new ResourcesManager.ResourceAmount("steel", 200),
                    new ResourcesManager.ResourceAmount("carbon", 120)
                },
                BuildingType.BuildingLocationType.Planet, "FreezingTurret", 3, 2,
                "Freezes nearby enemies and slow them down.",
                new List<ResourcesManager.UpgradeCost>() {
                    new ResourcesManager.UpgradeCost(2, new List<ResourcesManager.ResourceAmount>(){
                        new ResourcesManager.ResourceAmount("steel", 400),
                        new ResourcesManager.ResourceAmount("carbon", 300)

                    }),
                    new ResourcesManager.UpgradeCost(3, new List<ResourcesManager.ResourceAmount>(){
                       new ResourcesManager.ResourceAmount("composite", 50),
                       new ResourcesManager.ResourceAmount("electronics", 100)
                    })                },
                new List<SpecializedUpgrade>()
                {

                },
                true, false,
                new List<Building.BuildingStat>()
                {
                    new Building.BuildingStat(Building.BuildingStat.StatType.freezingPower)
                },
                false
                ));

        availableBuildings.Add(new BuildingType(4, "Power Plant", powerPlantPrefab, BuildingCategory.Production, 0f,
                new List<ResourcesManager.ResourceAmount>(){
                    new ResourcesManager.ResourceAmount("steel", 80),
                    new ResourcesManager.ResourceAmount("carbon", 20)
                 },
                BuildingType.BuildingLocationType.Planet, "PowerPlant", 3, 0,
                "Provides energy to your infrastructures.",
                new List<ResourcesManager.UpgradeCost>() {
                    new ResourcesManager.UpgradeCost(2, new List<ResourcesManager.ResourceAmount>(){
                        new ResourcesManager.ResourceAmount("steel", 100),
                        new ResourcesManager.ResourceAmount("carbon", 50)

                    }),
                    new ResourcesManager.UpgradeCost(3, new List<ResourcesManager.ResourceAmount>(){
                        new ResourcesManager.ResourceAmount("composite", 50),
                        new ResourcesManager.ResourceAmount("electronics", 50)
                    })                },
                new List<SpecializedUpgrade>()
                {

                },
                false, true,
                new List<Building.BuildingStat>()
                {
                },
                false
                ));

        availableBuildings.Add(new BuildingType(mineFacilityIndex, "Mining Facility", mineBuildingPrefab, BuildingCategory.Production, 10f,
                new List<ResourcesManager.ResourceAmount>(){
                    new ResourcesManager.ResourceAmount("steel", 40),
                    new ResourcesManager.ResourceAmount("carbon", 25)
                },
                BuildingType.BuildingLocationType.Planet, "MiningFacility", 3, 0,
                "Gather resources needed to build infrastructures.",
                new List<ResourcesManager.UpgradeCost>() {
                    new ResourcesManager.UpgradeCost(2, new List<ResourcesManager.ResourceAmount>(){
                        new ResourcesManager.ResourceAmount("steel", 120),
                        new ResourcesManager.ResourceAmount("carbon", 50)

                    }),
                    new ResourcesManager.UpgradeCost(3, new List<ResourcesManager.ResourceAmount>(){
                       new ResourcesManager.ResourceAmount("composite", 50),
                       new ResourcesManager.ResourceAmount("electronics", 50)
                    })                },
                new List<SpecializedUpgrade>()
                {

                },
                false, false,
                new List<Building.BuildingStat>()
                {
                    new Building.BuildingStat(Building.BuildingStat.StatType.miningSpeed)
                },
                false
                ));

        availableBuildings.Add(new BuildingType(6, "Shock Satellite", shockSatellitePrefab, BuildingCategory.Attack, 10f,
                new List<ResourcesManager.ResourceAmount>(){
                    new ResourcesManager.ResourceAmount("steel", 120),
                    new ResourcesManager.ResourceAmount("carbon", 80)
                },
                BuildingType.BuildingLocationType.Disks, "shock_satellite", 3, 3,
                "Satellite building dealing damage salves in a circle around it.",
                new List<ResourcesManager.UpgradeCost>() {
                    new ResourcesManager.UpgradeCost(2, new List<ResourcesManager.ResourceAmount>(){
                        new ResourcesManager.ResourceAmount("steel", 300),
                        new ResourcesManager.ResourceAmount("carbon", 180)

                    }),
                    new ResourcesManager.UpgradeCost(3, new List<ResourcesManager.ResourceAmount>(){
                       new ResourcesManager.ResourceAmount("composite", 300),
                       new ResourcesManager.ResourceAmount("electronics", 250)
                    })                },
                new List<SpecializedUpgrade>()
                {

                },
                true, false,
                new List<Building.BuildingStat>()
                {
                    new Building.BuildingStat(Building.BuildingStat.StatType.damagePower)
                },
                false
                ));

        availableBuildings.Add(new BuildingType(7, "Recycling Station", debrisCollectorStationPrefab, BuildingCategory.Production, 10f,
                new List<ResourcesManager.ResourceAmount>(){
                    new ResourcesManager.ResourceAmount("steel", 40),
                    new ResourcesManager.ResourceAmount("carbon", 25)
                },
                BuildingType.BuildingLocationType.Disks, "recycling_station", 3, 4,
                "Satellite base of recycling shuttles, able to recycle meteor debris and enemy spaceship wrecks.",
                new List<ResourcesManager.UpgradeCost>() {
                    new ResourcesManager.UpgradeCost(2, new List<ResourcesManager.ResourceAmount>(){
                        new ResourcesManager.ResourceAmount("steel", 240),
                        new ResourcesManager.ResourceAmount("carbon", 100)

                    }),
                    new ResourcesManager.UpgradeCost(3, new List<ResourcesManager.ResourceAmount>(){
                       new ResourcesManager.ResourceAmount("composite", 150),
                       new ResourcesManager.ResourceAmount("electronics", 50)
                    })                },
                new List<SpecializedUpgrade>()
                {

                },
                true, false,
                new List<Building.BuildingStat>()
                {
                },
                false
        ));

        availableBuildings.Add(new BuildingType(8, "Solar Station", satelliteSolarStationPrefab, BuildingCategory.Production, 0f,
                new List<ResourcesManager.ResourceAmount>(){
                    new ResourcesManager.ResourceAmount("steel", 80),
                    new ResourcesManager.ResourceAmount("carbon", 80)
                },
                BuildingType.BuildingLocationType.Disks, "solar_station", 3, 5,
                "A satellite covered by solar panels, providing energy to your infrastructures.",
                new List<ResourcesManager.UpgradeCost>() {
                    new ResourcesManager.UpgradeCost(2, new List<ResourcesManager.ResourceAmount>(){
                        new ResourcesManager.ResourceAmount("steel", 200),
                        new ResourcesManager.ResourceAmount("carbon", 120)

                    }),
                    new ResourcesManager.UpgradeCost(3, new List<ResourcesManager.ResourceAmount>(){
                       new ResourcesManager.ResourceAmount("composite", 180),
                       new ResourcesManager.ResourceAmount("electronics", 220)
                    })                },
                new List<SpecializedUpgrade>()
                {

                },
                false, true,
                new List<Building.BuildingStat>()
                {
                },
                false
                ));

        availableBuildings.Add(new BuildingType(9, "Healing Turret", healingTurretPrefab, BuildingCategory.Defense, 15f,
                new List<ResourcesManager.ResourceAmount>(){
                    new ResourcesManager.ResourceAmount("steel", 400),
                    new ResourcesManager.ResourceAmount("carbon", 150)
                },
                BuildingType.BuildingLocationType.Planet, "healing_turret", 3, 6,
                "Turret able to restore your spaceships health.",
                new List<ResourcesManager.UpgradeCost>() {
                    new ResourcesManager.UpgradeCost(2, new List<ResourcesManager.ResourceAmount>(){
                        new ResourcesManager.ResourceAmount("steel", 600),
                        new ResourcesManager.ResourceAmount("carbon", 200)

                    }),
                    new ResourcesManager.UpgradeCost(3, new List<ResourcesManager.ResourceAmount>(){
                               new ResourcesManager.ResourceAmount("composite", 250),
                               new ResourcesManager.ResourceAmount("electronics", 200)
                    })                },
                new List<SpecializedUpgrade>()
                {

                },
                true, false,
                new List<Building.BuildingStat>()
                {
                    new Building.BuildingStat(Building.BuildingStat.StatType.healingPower)
                },
                false
                ));

        availableBuildings.Add(new BuildingType(10, "Spaceport", spaceportPrefab, BuildingCategory.Defense, 15f,
                new List<ResourcesManager.ResourceAmount>(){
                    new ResourcesManager.ResourceAmount("steel", 800),
                    new ResourcesManager.ResourceAmount("carbon", 600)
                },
                BuildingType.BuildingLocationType.Disks, "spaceport", 3, 7,
                "Build new spaceships and recruit pilots in the spaceport.",
                new List<ResourcesManager.UpgradeCost>() {
                            new ResourcesManager.UpgradeCost(2, new List<ResourcesManager.ResourceAmount>(){
                                new ResourcesManager.ResourceAmount("steel", 1200),
                                new ResourcesManager.ResourceAmount("composite", 500)
                            }),
                            new ResourcesManager.UpgradeCost(3, new List<ResourcesManager.ResourceAmount>(){
                                new ResourcesManager.ResourceAmount("steel", 3200),
                                new ResourcesManager.ResourceAmount("electronics", 600)
                            })        },
                new List<SpecializedUpgrade>()
                {

                },
                false, false,
                new List<Building.BuildingStat>()
                {
                },
                true
        ));

        availableBuildings.Add(new BuildingType(11, "Storm Satellite", stormSatellitePrefab, BuildingCategory.Attack, 30f,
                new List<ResourcesManager.ResourceAmount>(){
                    new ResourcesManager.ResourceAmount("steel", 80),
                    new ResourcesManager.ResourceAmount("carbon", 40)
                },
                BuildingType.BuildingLocationType.Disks, "Satellites/storm_satellite_t", 3, 8,
                "A satellite able to transfer damages between nearby enemies.",
                new List<ResourcesManager.UpgradeCost>() {
                            new ResourcesManager.UpgradeCost(2, new List<ResourcesManager.ResourceAmount>(){
                                new ResourcesManager.ResourceAmount("steel", 120),
                                new ResourcesManager.ResourceAmount("carbon", 100)
                            }),
                            new ResourcesManager.UpgradeCost(3, new List<ResourcesManager.ResourceAmount>(){
                                new ResourcesManager.ResourceAmount("composite", 200),
                                new ResourcesManager.ResourceAmount("electronics", 200)
                            })        },
                new List<SpecializedUpgrade>()
                {

                },
                true, false,
                new List<Building.BuildingStat>()
                {
                    new Building.BuildingStat(Building.BuildingStat.StatType.damagePower)
                },
                false
        ));

        availableBuildings.Add(new BuildingType(12, "Meteor Crusher", meteorCrusherPrefab, BuildingCategory.Attack, 30f,
                new List<ResourcesManager.ResourceAmount>(){
                    new ResourcesManager.ResourceAmount("steel", 650),
                    new ResourcesManager.ResourceAmount("carbon", 400)
                },
                BuildingType.BuildingLocationType.Planet, "MeteorCrusher", 3, 9,
                "A turret targeting the biggest meteors and crushing them into each other.",
                new List<ResourcesManager.UpgradeCost>() {
                            new ResourcesManager.UpgradeCost(2, new List<ResourcesManager.ResourceAmount>(){
                                new ResourcesManager.ResourceAmount("steel", 1000),
                                new ResourcesManager.ResourceAmount("carbon", 650)
                            }),
                            new ResourcesManager.UpgradeCost(3, new List<ResourcesManager.ResourceAmount>(){
                                new ResourcesManager.ResourceAmount("composite", 300),
                                new ResourcesManager.ResourceAmount("electronics", 350)
                            })
                },
                new List<SpecializedUpgrade>()
                {

                },
                true, false,
                new List<Building.BuildingStat>()
                {
                    new Building.BuildingStat(Building.BuildingStat.StatType.damagePower)
                },
                false
        ));
    }

    public void SelectBuilding(BuildingType bType)
    {
        GameManager.instance.ChangeSelectionState(GameManager.SelectionState.ShopItemSelected);

        if (buildingState == BuildingState.Default || buildingState == BuildingState.BuildingSelected || buildingState == BuildingState.LocationSelected || buildingState == BuildingState.BuildingAndLocationSelected)
        {
            selectedBuilding = bType;

            UpdateBuildingState(2);
        }
    }

    public void DeselectBuilding()
    {
        selectedBuilding = null;
    }

    public void CancelButton()
    {
        UpdateBuildingState(3);
    }

    public void BuildButton()
    {
        // Tutorial indicator //
        TutorialManager.instance.DisplayIndicator(TutorialManager.IndicatorID.click_on_build, false);
        TutorialManager.instance.DisplayIndicatorIfNotDisplayedYet(TutorialManager.IndicatorID.touch_building);
        // ------------------ //

        if (buildingState == BuildingState.LocationSelected || buildingState == BuildingState.BuildingAndLocationSelected)
        {
            if (chosenBuildingSlot.GetComponent<BuildingSlot>().CanBuildHere())
            {
                if (ResourcesManager.instance.CanPayConstruction(selectedBuilding))
                {
                    ResourcesManager.instance.PayConstruction(selectedBuilding);

                    BuildBuilding();

                    buildingState = BuildingState.Default;
                    ShowBuildButton(false);
                    BuildingSlotManager.instance.ResetAllBuildingSlotsColor();
                    SurroundingAreasManager.instance.ResetAllSatelliteBuildingSlotsColor();
                    //ShopPanel.instance.ResetLastShopItemSelected();               // TO REDO
                    //Debug.Log("Building Placed | Leaving Building State.");

                    // If building is Unique, disable corresponding ShopItem
                    if(selectedBuilding.isUnique)
                    {
                        //ShopPanel.instance.GetShopItemAssociatedWithBuildingType(selectedBuilding).SetActive(false);  // TO REDO
                    }
                }
            }
        }

        GameManager.instance.ChangeSelectionState(GameManager.SelectionState.Default);
    }

    //public void ShowCancelButton(bool display) { cancelButton.SetActive(display); }
    public void ShowBuildButton(bool display) { buildButton.SetActive(display); }

    public void DisplayBuildingPreview() {
        chosenBuildingSlot.GetComponent<BuildingSlot>().SetSelectionColor();
    }

    public Vector3 GetBuildingLocationFromTouchPoint()
    {
        Vector3 lastTouchPos = TouchManager.instance.lastTouch;
        float touchX = lastTouchPos.x;
        float touchY = lastTouchPos.y;
        Vector2 normalizedTouchXY = new Vector2(touchX, touchY).normalized;

        // TODO : Change
        Vector3 pos = new Vector3(touchX - Screen.width / 2, touchY - Screen.height / 2, GameManager.instance.mainPlanet.transform.position.z);

        Vector2 posV2 = new Vector2(touchX - Screen.width / 2, touchY - Screen.height / 2);
        Vector2 normalizedPos2 = posV2.normalized;
        Vector2 scaledPos2 = normalizedPos2 * GameManager.instance.mainPlanet.transform.localScale.x / 2;

        return new Vector3(scaledPos2.x, scaledPos2.y, pos.z);
    }

    public Vector3 GetLocationFromTouchPointOnPlanetPlane(Vector3 touchPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPos);
        Plane planetPlane = new Plane(Vector3.forward, mainPlanet.transform.position);
        float distance = 0;
        Vector3 intersectPointPos = new Vector3(0f, 0f, 0f);

        if (planetPlane.Raycast(ray, out distance)) {
            intersectPointPos = ray.GetPoint(distance);
        }

        return intersectPointPos;
    }

    public void SelectBuildingLocation(BuildingSlot buildingSpot = null)
    {
        // Tutorial indicator //
        TutorialManager.instance.DisplayIndicator(TutorialManager.IndicatorID.select_building_spot, false);
        //TutorialManager.instance.DisplayIndicator(TutorialManager.IndicatorID.protect_people, false);
        //TutorialManager.instance.DisplayIndicator(TutorialManager.IndicatorID.protect_people, false);
        TutorialManager.instance.DisplayIndicatorIfNotDisplayedYet(TutorialManager.IndicatorID.select_building);
        // ------------------ //

        // Chose Building Spot
        //chosenBuildingSlot = SelectBuildingSpotFromTouch(); // OLD

        if (buildingSpot != null)
        {
            chosenBuildingSlot = buildingSpot.gameObject;
        }

        if (chosenBuildingSlot != null)
        {
            BuildingSlotManager.instance.ResetAllBuildingSlotsColor();
            SurroundingAreasManager.instance.ResetAllSatelliteBuildingSlotsColor();

            buildingSpot.SetSelectionColor();

            // Display Build button if we can pay for the building
            UpdateBuildingState(1);            
        }
        else { Debug.Log("SelectBuildingLocation | chosenBuildingSlot null"); }
    }

    public void UpdateBuildingState(int param)  // 1: Set location, 2: Set building, 3: Reset
    {
        bool canPay = false;
        if (selectedBuilding != null)
            canPay = ResourcesManager.instance.CanPayConstruction(selectedBuilding);

        switch (param)
        {
            case 1:     // Set building spot
            {
                if(buildingState == BuildingState.Default)
                {
                    buildingState = BuildingState.LocationSelected;
                }
                else if (buildingState == BuildingState.BuildingSelected)
                {
                    buildingState = BuildingState.BuildingAndLocationSelected;
                    DisplayBuildingPreview();

                    ShowBuildButton(true);
                    AdaptBuildButton(canPay);
                }
                break;
            }
            case 2:     // Set building
            {
                if (buildingState == BuildingState.Default)
                {
                    buildingState = BuildingState.BuildingSelected;
                }
                else if (buildingState == BuildingState.LocationSelected)
                {
                    buildingState = BuildingState.BuildingAndLocationSelected;
                    DisplayBuildingPreview();

                    ShowBuildButton(true);
                    AdaptBuildButton(canPay);
                }
                break;
            }
            case 3:     // Reset
            {
                buildingState = BuildingState.Default;
                //ShopPanel.instance.ResetLastShopItemSelected();       // TO REDO
                BuildingShopManager.instance.CloseAllPanels();
                DeselectBuilding();
                BuildingSlotManager.instance.ResetAllBuildingSlotsColor();
                ShowBuildButton(false);
                break;
            }
            default:
            {
                break;
            }
        }
    }

    public GameObject SelectBuildingSpotFromTouch()
    {
        GameObject buildingSpot = null;
        Vector3 touchPos = TouchManager.instance.lastTouch;
        Vector3 planeLoc = GetLocationFromTouchPointOnPlanetPlane(touchPos);

        BuildingSlotManager.instance.ResetAllBuildingSlotsColor();
        SurroundingAreasManager.instance.ResetAllSatelliteBuildingSlotsColor();

        if (selectedBuilding.buildingLocationType == BuildingType.BuildingLocationType.Planet)
        {
            buildingSpot = BuildingSlotManager.instance.FindGroundClosestBuildingSlot(planeLoc);
        }
        else if (selectedBuilding.buildingLocationType == BuildingType.BuildingLocationType.Disks)
        {
            buildingSpot = SurroundingAreasManager.instance.FindClosestBuildingSlotInUnlockedDisks(planeLoc);
        }

        if(buildingSpot != null)
        {
            buildingSpot.GetComponent<BuildingSlot>().SetSelectionColor();
        }

        return buildingSpot;
    }

    public void SelectBuildingSpot(BuildingSlot buildingSpot)
    {
        SelectBuildingLocation(buildingSpot);
    }

    public void BuildBuilding()
    {
        if (selectedBuilding != null)
        {
            float buildingSpotAngle_rad = chosenBuildingSlot.GetComponent<BuildingSlot>().angleRad;
            float buildingSpotAngle_deg = GeometryManager.RadiansToDegrees(buildingSpotAngle_rad);

            Vector3 instantiationPosition = chosenBuildingSlot.transform.position;
            // Instantiate satellite slighly in front of building slot
            if (selectedBuilding.buildingLocationType == BuildingType.BuildingLocationType.Disks)
            {
                instantiationPosition += new Vector3(0f, 0f, -10f);
            }
            GameObject instantiatedBuilding = Instantiate(selectedBuilding.prefab, instantiationPosition, Quaternion.Euler(0f, 0f, buildingSpotAngle_deg));
            buildingList.Add(instantiatedBuilding);
            Building b = instantiatedBuilding.GetComponent<Building>();
            b.buildingType = selectedBuilding;
            b.buildingSpotAngleRad = buildingSpotAngle_rad;
            b.buildingSpotAngleDeg = buildingSpotAngle_deg;
            b.currentTier = 1;
            //instantiatedBuilding.GetComponent<Building>().energyConsumption = selectedBuilding.energyConsumption;
            b.buildingSpot = chosenBuildingSlot;
            chosenBuildingSlot.GetComponent<BuildingSlot>().SetBuilding(laserTurretPrefab.GetComponent<LaserTurret>());
            instantiatedBuilding.transform.SetParent(chosenBuildingSlot.transform);
            //Debug.Log("New building instantiated !");

            // Building type lists
            if (selectedBuilding.name == "Recycling Station")
            {
                InfrastructureManager.instance.recyclingStationsList.Add(instantiatedBuilding);
            }
            else if (selectedBuilding.name == "Spaceport")
            {
                InfrastructureManager.instance.SetSpaceport(instantiatedBuilding);
            }

            // Add Info Canvas on building
            AddInfoCanvasOnBuilding(instantiatedBuilding);

            // Distribute the available energy across all buildings
            EnergyPanel.instance.UpdateEnergyProductionAndConsumption();

            // Refresh population effects
            PopulationManager.instance.ApplyPopulationEffects();

            // Transmit info to BuildingShopManager
            BuildingShopManager.instance.OnBuildingBuilt();
        }
    }

    public void AddInfoCanvasOnBuilding(GameObject buildingGO)
    {
        Building b = buildingGO.GetComponent<Building>();
        float constructionAngle = b.buildingSpotAngleRad/* + 45*/;
        Debug.Log("AddInfoCanvasOnBuilding | ConstructionAngle [" + constructionAngle + "]");
        Vector3 pos = buildingGO.transform.position + new Vector3(Mathf.Cos(constructionAngle) * powerMissingIndicatorPlacementDistance, Mathf.Sin(constructionAngle) * powerMissingIndicatorPlacementDistance, 0);

        GameObject instantiatedInfoCanvas = Instantiate(powerMissingIndicatorPrefab, pos, Quaternion.identity);
        instantiatedInfoCanvas.transform.SetParent(buildingGO.transform, true);

        buildingGO.GetComponent<Building>().powerMissingCanvas = instantiatedInfoCanvas;
    }

    public void DeselectSelectedBuildingSlot()
    {
        if (chosenBuildingSlot.GetComponent<BuildingSlot>().locationType == BuildingType.BuildingLocationType.Planet)    // Planet slot
        {
            BuildingSlotManager.instance.ResetAllBuildingSlotsColor();
        }
        else    // Satellite slot
        {
            SurroundingAreasManager.instance.ResetAllSatelliteBuildingSlotsColor();
        }

        chosenBuildingSlot = null;

        if (buildingState == BuildingState.BuildingAndLocationSelected)
        {
            buildingState = BuildingState.BuildingSelected;
        } else if (buildingState == BuildingState.BuildingSelected)
        {
            buildingState = BuildingState.Default;
        }

    }

    public void UnlockBuildingType(BuildingType bType)
    {
        if (!bType.isUnlocked)
        {
            //ShopPanel.instance.AddBuildingShopItem(bType);  // OLD
            bType.isUnlocked = true;
            //Debug.Log("Building \"" + bType.name + "\" unlocked.");
        }
        else
        {
            //Debug.Log("Building : " + bType.name + " is already unlocked !");
        }
    }

    public BuildingType GetBuildingTypeByID(int id)
    {
        BuildingType bType = null;
        foreach (BuildingType b in availableBuildings)
        {
            if (b.id == id)
            {
                bType = b;
                break;
            }
        }
        return bType;
    }

    public void BuildBuildingOnSlotAtTier(BuildingType buildingType, GameObject buildingSlot, int tier)
    {
        float buildingSpotAngle_rad = buildingSlot.GetComponent<BuildingSlot>().angleRad;
        float buildingSpotAngle_deg = GeometryManager.RadiansToDegrees(buildingSpotAngle_rad);

        Vector3 instantiationPosition = buildingSlot.transform.position;

        // Instantiate satellite slighly in front of building slot
        if (buildingType.buildingLocationType == BuildingType.BuildingLocationType.Disks)
        {
            instantiationPosition += new Vector3(0f, 0f, -10f);
        }

        GameObject instantiatedBuilding = Instantiate(buildingType.prefab, instantiationPosition, Quaternion.Euler(0f, 0f, buildingSpotAngle_deg));
        buildingList.Add(instantiatedBuilding);
        Building b = instantiatedBuilding.GetComponent<Building>();
        b.buildingType = buildingType;
        b.buildingSpotAngleRad = buildingSpotAngle_rad;
        b.buildingSpotAngleDeg = buildingSpotAngle_deg;
        b.currentTier = 1;
        b.energyConsumption = buildingType.energyConsumption;
        b.buildingSpot = buildingSlot;

        buildingSlot.GetComponent<BuildingSlot>().SetBuilding(laserTurretPrefab.GetComponent<LaserTurret>());
        instantiatedBuilding.transform.SetParent(buildingSlot.transform);
        //Debug.Log("New building instantiated !");

        // Distribute the available energy across all buildings
        EnergyPanel.instance.UpdateEnergyProductionAndConsumption();

        // Special needs for some buildings
        if (b.buildingType.name == "Recycling Station")
        {
            InfrastructureManager.instance.recyclingStationsList.Add(instantiatedBuilding);
        }
        else if (b.buildingType.name == "Spaceport")
        {
            InfrastructureManager.instance.SetSpaceport(instantiatedBuilding);
        }

        if (tier > 1)
        {
            instantiatedBuilding.GetComponent<Building>().UpgradeToTier(tier);
        }
    }

    public UnlockedBuildingData[] BuildUnlockedBuildingsData()
    {
        UnlockedBuildingData[] unlockedBuildingsData = new UnlockedBuildingData[availableBuildings.Count];
        for (int i = 0; i < availableBuildings.Count; i++)
        {
            unlockedBuildingsData[i] = new UnlockedBuildingData(availableBuildings[i].id, availableBuildings[i].isUnlocked);
        }
        return unlockedBuildingsData;
    }

    public void ApplyUnlockedBuildingsData(UnlockedBuildingData[] unlockedBuildingsData)
    {
        foreach (UnlockedBuildingData unlockedBuildingData in unlockedBuildingsData)
        {
            if (unlockedBuildingData.isUnlocked)
            {
                Debug.Log("ApplyUnlockedBuildingsData | Building [" + unlockedBuildingData.buildingIndex + "] was previously unlocked.");
                UnlockBuildingType(GetBuildingTypeByID(unlockedBuildingData.buildingIndex));
            }
        }
    }

    public bool IsBuildingTypeAtLeastPlacedOnce(BuildingType buildingType)
    {
        bool isPlaced = false;
        foreach (GameObject building in buildingList)
        {
            if(building.GetComponent<Building>().buildingType == buildingType)
            {
                isPlaced = true;
                break;
            }
        }
        return isPlaced;
    }

    public void BuildingSlotTouched(BuildingSlot buildingSlot)     // NEW
    {
        BuildingShopManager.instance.BuildingSlotTouched(buildingSlot);

        SelectBuildingSpot(buildingSlot);
    }

    public void BuildingShopItemSelected(BuildingType buildingType)     // NEW
    {
        // Tutorial indicator //                              
        TutorialManager.instance.DisplayIndicator(TutorialManager.IndicatorID.select_building, false);
        TutorialManager.instance.DisplayIndicatorIfNotDisplayedYet(TutorialManager.IndicatorID.click_on_build);
        // ------------------ //

        //Debug.Log("Building Shop Item Clicked !");
        //ShopPanel.instance.ResetLastShopItemSelected();           // TO REDO
        SelectBuilding(buildingType);

        /*if (ResourcesManager.instance.CanPayConstruction(buildingType))
        {
            SetBackGroundColor(ShopPanel.instance.buildingShopItemSelectedCanPayBackgroundColor);
        }
        else
        {
            SetBackGroundColor(ShopPanel.instance.buildingShopItemSelectedCantPayBackgroundColor);
        }*/
    }

    public void ResetBuildingOperations()
    {
        UpdateBuildingState(3);
    }

    public void AdaptBuildButton(bool canPay)
    {
        //buildButton.GetComponent<Image>().color = (canPay) ? canPayColor : cantPayColor;
        //buildButtonForeground.GetComponent<Image>().sprite = (canPay) ? buildButtonCanPaySprite : buildButtonCantPaySprite;
        buildButtonText.text = (canPay) ? "Build" : "No resources !";
    }

    public void UpdateCurrentLevelFinished(bool finished)
    {
        foreach (GameObject building in buildingList)
        {
            if (building.GetComponent<Building>().buildingType.id == mineFacilityIndex)
            {
                building.GetComponent<MineBuilding>().StartStopMiningAnimation();
            }
        }
    }

    // TODO : Only testing purpose
    public void TestBuildButton()
    {
        //Debug.Log("Test Build Button.");
        if (buildingState == BuildingState.Default)
        {
            buildingState = BuildingState.Building;
            //Debug.Log("Entering Building State.");
        }
    }

    public class SpecializedUpgrade
    {
        public enum UpgradeType { statUpgrade, sideUpgrade }

        public Building.BuildingStat upgradedStat;
        public float newValue;
        public List<ResourcesManager.ResourceAmount> upgradeCosts;
        public UpgradeType upgradeType;

        public SpecializedUpgrade(UpgradeType upgradeType, List<ResourcesManager.ResourceAmount> upgradeCosts, Building.BuildingStat upgradedStat, float newValue)
        {
            this.upgradeType = upgradeType;
            this.upgradedStat = upgradedStat;
            this.upgradeCosts = upgradeCosts;
            this.newValue = newValue;
        }
    }

    [System.Serializable]
    public class BuildingType {

        public int id;
        public string name;
        public GameObject prefab;
        public List<ResourcesManager.ResourceAmount> resourceCosts;
        public float energyConsumption = 0;
        public enum BuildingLocationType { Planet, Disks };
        public BuildingLocationType buildingLocationType;
        public Sprite buildingImage;
        public int maxTier = 3;
        public bool isUnlocked = true;
        public int unlockedAtLevelNb = 0;
        public string description;
        public List<ResourcesManager.UpgradeCost> upgradeCosts;
        public List<SpecializedUpgrade> specializedUpgrades;
        public bool hasRange;
        public bool producesEnergy;
        public List<Building.BuildingStat> specificStats;
        public bool isUnique;
        public BuildingCategory buildingCategory;

        public BuildingType(int id, string name, GameObject prefab, BuildingCategory buildingCategory, float energyConsumption, List<ResourcesManager.ResourceAmount> cost, BuildingLocationType buildingLocationType, string imageName, int maxTier, int unlockedAtLevelNb, string description, List<ResourcesManager.UpgradeCost> upgradeCosts, List<SpecializedUpgrade> specializedUpgrades, bool hasRange, bool producesEnergy, List<Building.BuildingStat> specificStats, bool isUnique)
        {
            this.id = id;
            this.name = name;
            this.prefab = prefab;
            this.energyConsumption = energyConsumption;
            this.resourceCosts = cost;
            this.buildingLocationType = buildingLocationType;
            this.buildingImage = Resources.Load<Sprite>("Images/Buildings/" + imageName);           // Place the building image with the correct name in this folder
            this.maxTier = maxTier;
            this.isUnlocked = (unlockedAtLevelNb == 0) ? true : false;
            this.unlockedAtLevelNb = unlockedAtLevelNb;
            this.description = description;
            this.upgradeCosts = upgradeCosts;
            this.specializedUpgrades = specializedUpgrades;
            this.hasRange = hasRange;
            this.producesEnergy = producesEnergy;
            this.specificStats = specificStats;
            this.isUnique = isUnique;
            this.buildingCategory = buildingCategory;
        }

        public List<ResourcesManager.ResourceAmount> GetUpgradeCostsForTierNb(int tierNb)
        {
            //Debug.Log("GetUpgradeCostsForTierNb [" + tierNb + "]");
            List <ResourcesManager.ResourceAmount> costs = new List<ResourcesManager.ResourceAmount>();
            foreach (ResourcesManager.UpgradeCost upgradeCost in upgradeCosts)
            {
                //Debug.Log("Comparing upgrade cost [" + upgradeCost.tierIndex + "]");
                if (upgradeCost.tierIndex == tierNb) // Matching tier nb
                {
                    //Debug.Log("Found matching UpgradeCostList");
                    costs = upgradeCost.resourceCosts;
                    break;
                }
            }
            return costs;
        }
    }

    [Serializable]
    public class UnlockedBuildingData
    {
        public int buildingIndex;
        public bool isUnlocked;

        public UnlockedBuildingData(int buildingIndex, bool isUnlocked)
        {
            this.buildingIndex = buildingIndex;
            this.isUnlocked = isUnlocked;
        }
    }

}
