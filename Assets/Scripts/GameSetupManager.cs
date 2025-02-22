﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetupManager : MonoBehaviour {

    public static GameSetupManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public GameSetupParameters gameSetupParameters;

    public void SetGameSetupParameters(GameSetupParameters gameSetupParameters)
    {
        this.gameSetupParameters = gameSetupParameters;
        Debug.Log("SetGameSetupParameters | isNewGame: " + gameSetupParameters.isNewGame);
    }

    public void SetupGame()
    {
        Debug.Log("SetupGame");

        TutorialManager.instance.DefineAvailableTutorialIndicators();
        SurroundingAreasManager.instance.DefineDisks();

        TechTreeManager.instance.InitializeTechnologies();

        if (gameSetupParameters.isNewGame)
        {
            // New game
            Debug.Log("Setup new game...");
            BuildingSlotManager.instance.BuildGroundBuildingSlots();
            LevelManager.instance.NewGameSetup();

            // Tutorial manager
            TutorialManager.instance.DisplayStartIndicators();

            // Spaceship manager 
            SpaceshipManager.instance.NewGameSetupActions();

            // Timer
            TimeManager.instance.SetTimerEnabled(OptionsManager.instance.IsTimerOptionEnabled());
        }
        else
        {
            // Saved game
            Debug.Log("Setup saved game...");
            BuildingSlotManager.instance.BuildGroundBuildingSlots();
            TutorialManager.instance.HideIndicators();
            SetupSavedData();
        }

        GameSetupFinished();
    }

    public void GameSetupFinished()
    {
        EventsManager.instance.GameSetupFinished();
    }

    public void SetupSavedData()
    {
        Debug.Log("SetupSavedData");
        Building.BuildingData[] buildingsData = gameSetupParameters.gameSaveData.buildingsData;
        GameManager.GeneralGameData generalGameData = gameSetupParameters.gameSaveData.generalGameData;
        SpaceshipManager.SpaceshipData[] spaceshipsData = gameSetupParameters.gameSaveData.spaceshipsData;
        Level.LevelData reachedLevelData = gameSetupParameters.gameSaveData.reachedLevelData;
        ResourcesManager.ResourceData[] resourcesData = gameSetupParameters.gameSaveData.resourcesData;
        BuildingManager.UnlockedBuildingData[] unlockedBuildingsData = gameSetupParameters.gameSaveData.unlockedBuildingsData;
        TechTreeManager.TechnologyData[] technologiesData = gameSetupParameters.gameSaveData.technologiesData;
        MegaStructureManager.MegaStructuresData megaStructuresData = gameSetupParameters.gameSaveData.megaStructuresData;
        PopulationManager.PopulationData populationData = gameSetupParameters.gameSaveData.populationData;

        if(buildingsData != null){
            SetupSavedBuildings(buildingsData);
        }

        if (generalGameData != null)
        {
            SetupGeneralParameters(generalGameData);
        }

        if(reachedLevelData != null)
        {
            SetupReachedLevelParameters(reachedLevelData);
        }

        if (resourcesData != null)
        {
            SetupResourcesData(resourcesData);
        }

        if (technologiesData != null)
        {
            SetupTechnologies(technologiesData);
        }

        if (spaceshipsData != null)
        {
            SetupSpaceships(spaceshipsData);
        }

        if (megaStructuresData != null)
        {
            SetupMegaStructures(megaStructuresData);
        }

        if (populationData != null)
        {
            SetupPopulationData(populationData);
        }
    }

    public void SetupSavedBuildings(Building.BuildingData[] buildingsData)
    {
        Debug.Log("SetupSavedBuildings");
        Debug.Log("Buildings Nb: " + buildingsData.Length);
        for(int i=0; i<buildingsData.Length; i++)
        {
            Building.BuildingData bData = buildingsData[i];
            BuildingManager.instance.BuildBuildingOnSlotAtTier(BuildingManager.instance.GetBuildingTypeByID(bData.buildingTypeID), BuildingSlotManager.instance.GetBuildingSlotByID(bData.buildingSlotID), bData.tier);
        }
    }

    public void SetupGeneralParameters(GameManager.GeneralGameData generalGameData)
    {
        int score = generalGameData.score;
        int hits = generalGameData.hits;
        int planetLife = generalGameData.planetLife;
        int unlockedDisksNb = generalGameData.unlockedDisks;
        int experiencePoints = generalGameData.experiencePoints;
        int artifactsNb = generalGameData.artifactsNb;
        bool timerEnabled = generalGameData.timerEnabled;
        bool gameOverOccured = generalGameData.hasGameOverOccured;
        bool infiniteModeEnabled = generalGameData.infiniteModeEnabled;

        //Debug.Log("SetupGeneralParameters | Score [" + score + "] | Hits [" + hits + "] | UnlockedDisks [" + unlockedDisksNb + "]");

        ScoreManager.instance.SetScore(score);
        ScoreManager.instance.SetExperiencePointsAndArtifactsNb(experiencePoints, artifactsNb);
        ScoreManager.instance.SetPlanetLife(planetLife);
        InfoManager.instance.SetMeteorCollisionsValue(hits);
        SurroundingAreasManager.instance.SetUnlockedDisksNb(unlockedDisksNb);
        TimeManager.instance.SetTimerEnabled(timerEnabled);
        GameManager.instance.SetInfiniteMode(infiniteModeEnabled);
        ScoreManager.instance.SetGameOverHappened(gameOverOccured);
    }

    public void SetupReachedLevelParameters(Level.LevelData reachedLevelData)
    {
        int reachedLevelIndex = reachedLevelData.levelIndex;
        Debug.Log("SetupReachedLevelParameters [" + reachedLevelIndex + "]");
        LevelManager.instance.SetupGameAtLevelIndex(reachedLevelIndex);
    }

    public void SetupResourcesData(ResourcesManager.ResourceData[] resourcesData)
    {
        Debug.Log("SetupResourcesData");
        Debug.Log("ResourceType Nb: " + resourcesData.Length);
        ResourcesManager.instance.SetResourcesAmounts(resourcesData);
    }

    public void SetupUnlockedBuildings(BuildingManager.UnlockedBuildingData[] unlockedBuildingsData)
    {
        Debug.Log("SetupUnlockedBuildings | BuildingTypeNb [" + unlockedBuildingsData.Length +"]");
        BuildingManager.instance.ApplyUnlockedBuildingsData(unlockedBuildingsData);
    }

    public void SetupSpaceships(SpaceshipManager.SpaceshipData[] spaceshipsData)
    {
        Debug.Log("SetupSpaceships | SpaceshipsNb [" + spaceshipsData.Length + "]");
        SpaceshipManager.instance.SetupSavedSpaceships(spaceshipsData);
    }

    public void SetupTechnologies(TechTreeManager.TechnologyData[] technologiesData)
    {
        Debug.Log("SetupTechnologies | TechnologiesNb [" + technologiesData.Length + "]");
        TechTreeManager.instance.SetupSavedTechnologies(technologiesData);
    }

    public void SetupMegaStructures(MegaStructureManager.MegaStructuresData megaStructuresData)
    {
        Debug.Log("SetupMegaStructures");
        MegaStructureManager.instance.SetupSavedMegaStructuresData(megaStructuresData);
    }

    public void SetupPopulationData(PopulationManager.PopulationData populationData)
    {
        Debug.Log("SetupPopulationData");
        PopulationManager.instance.SetupSavedPopulationData(populationData);
    }

    public class GameSetupParameters
    {
        public bool isNewGame;
        public SaveManager.GameSaveData gameSaveData;

        public GameSetupParameters(bool isNewGame, SaveManager.GameSaveData gameSaveData)
        {
            this.isNewGame = isNewGame;
            if(!isNewGame){
                this.gameSaveData = gameSaveData;
            }
            else{
                this.gameSaveData = null;
            }
        }
    }
}
