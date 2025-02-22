﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class SettingsImportManager : MonoBehaviour {

    public TextAsset xmlBuildingsFile;

    // NOT USED
    /*public void ImportBuildingsSettings()
    {
        Debug.Log("Importing Buildings Settings.");

        // XML Document
        XmlDocument xmlBuildingsDocument = new XmlDocument();
        xmlBuildingsDocument.Load("Assets/XML/buildings.xml");

        // Base buildings node
        XmlNode buildingsNode = xmlBuildingsDocument.GetElementsByTagName("buildings")[0];

        // Turrets
        XmlNode turretsNode = buildingsNode.SelectNodes("turrets")[0];
     
        foreach (XmlNode buildingNode in turretsNode.ChildNodes) {
            Debug.Log("Turret Building: " + buildingNode.Attributes["name"].Value);

            switch(buildingNode.Attributes["name"].Value)
            {
                case "laser_turret" : {
                    XmlNode tiersNode = buildingNode.SelectSingleNode("tiers");
                    XmlNode tierNode = tiersNode.SelectSingleNode("tier");
                    XmlNode traitsNode = tierNode.SelectSingleNode("traits");

                    LaserTurret laserTurret = BuildingManager.instance.laserTurretPrefab.GetComponent<LaserTurret>();

                    // Fill Laser Turret prefab fields
                    XmlNode powerNode = traitsNode.SelectSingleNode("power");
                    Debug.Log("Power: " + powerNode.InnerText);
                    if(powerNode != null)
                    {
                            laserTurret.power = float.Parse(powerNode.InnerText);
                    }else{
                        Debug.Log("No power node found.");
                    }

                    XmlNode angleRangeNode = traitsNode.SelectSingleNode("angle_range");
                    Debug.Log("Angle_range: " + angleRangeNode.InnerText);
                    if(angleRangeNode != null)
                    {
                            laserTurret.angleRange = float.Parse(angleRangeNode.InnerText);
                    }else{
                        Debug.Log("No angle range node found.");
                    }

                    break;

                }
                case "missile_turret": {
                    break;
                }
                case "freezing_turret": {
                    break;
                }
                default : {
                    break;
                }
            }




        }

        // Production Buildings
        XmlNode productionBuildingsNode = buildingsNode.SelectNodes("production")[0];

        foreach (XmlNode buildingNode in productionBuildingsNode.ChildNodes) {
            Debug.Log("Production Building: " + buildingNode.Attributes["name"].Value);

            switch (buildingNode.Attributes["name"].Value)
            {
                case "power_plant": {
                        Debug.Log("Getting PowerPlant settings");
                        PowerPlant powerPlant = BuildingManager.instance.powerPlantPrefab.GetComponent<PowerPlant>();
                        XmlNode tiersNode = GetChildNode_Tiers(buildingNode);
                        XmlNode tier_1_Node = GetChildNode_Tier(tiersNode);
                        XmlNode traitsNode = GetChildNode_Traits(tier_1_Node);
                        XmlNode productionPowerNode = traitsNode.SelectSingleNode("powerProduction");
                        if (productionPowerNode != null){
                            powerPlant.energyProduction = float.Parse(productionPowerNode.InnerText);
                        } else {
                            Debug.Log("No productionPowerNode node found.");
                        }
                        break;
                }
                case "recycling_station": {
                    break;
                }
            }
        }
    }*/

    public XmlNode GetChildNode_Tiers(XmlNode parent){
        return parent.SelectSingleNode("tiers");
    }

    public XmlNode GetChildNode_Tier(XmlNode parent){
        return parent.SelectSingleNode("tier");
    }

    public XmlNode GetChildNode_Traits(XmlNode parent){
        return parent.SelectSingleNode("traits");
    }




}
