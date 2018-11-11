﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuLoadGamePanel : MonoBehaviour {

    public static MenuLoadGamePanel instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        //DontDestroyOnLoad(gameObject);
    }

    public GameObject loadPanelGameSaveElementPrefab;
    public GameObject loadPanelGameSavesElementLayout;
    public List<GameObject> loadPanelGameSaveElements;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayCanvas_NewGameButton(){
        ScenesManager.instance.LaunchNewGame();
    }

    public void BuildLoadGameSaveElements()
    {
        Debug.Log("BuildLoadGameSaveElements");
        for (int i=0; i<SaveManager.instance.savedGameFilesNb; i++)
        {
            GameObject instantiatedLoadGameSaveElement = Instantiate(loadPanelGameSaveElementPrefab, loadPanelGameSaveElementPrefab.transform.position, Quaternion.identity);

            instantiatedLoadGameSaveElement.transform.SetParent(loadPanelGameSavesElementLayout.transform, false);

            instantiatedLoadGameSaveElement.GetComponent<LoadGameSaveElement>().SetInfo(i + 1, SaveManager.instance.globalSavedGameInfoData.saveFilesInfo[i], SaveManager.instance.globalGameSaveData[i]);

            loadPanelGameSaveElements.Add(instantiatedLoadGameSaveElement);
        }
    }

    public void UpdateLoadGameSavePanel()
    {
        Debug.Log("UpdateLoadGameSavePanel");
        foreach (GameObject loadPanelGameSaveElement in loadPanelGameSaveElements)
        {
            UpdateLoadGameSaveElement(loadPanelGameSaveElement);
        }
    }

    public void LoadGameSaveNumberRequest(int saveSlotIndex)
    {
        Debug.Log("LoadGameSaveNumberRequest [" + saveSlotIndex + "]");
        if(SaveManager.instance.globalSavedGameInfoData.saveFilesInfo[saveSlotIndex - 1].isUsed)
        {
            Debug.Log("Loading Game Save [" + saveSlotIndex + "], save exist.");
            if (SaveManager.instance.SetGameSaveToLoadIndex(saveSlotIndex))
            {
                ScenesManager.instance.LaunchSavedGame(SaveManager.instance.gameSaveToLoad);
            }
            else
            {
                Debug.LogError("Can't load game save.");
            }
        }
        else
        {
            Debug.LogError("Can't load save [" + saveSlotIndex + "], save doesn't exist.");
        }
    }


    public void UpdateLoadGameSaveElement(GameObject loadGameSaveElement)
    {
        int saveIndex = loadGameSaveElement.GetComponent<LoadGameSaveElement>().saveIndex;
        Debug.Log("UpdateLoadGameSaveElement [" + saveIndex + "]");

        SaveManager.GameSaveData gameSaveData = SaveManager.instance.globalGameSaveData[saveIndex - 1];
        SaveManager.SavedGameFilesInfoData.SaveFileInfo saveFileInfo = SaveManager.instance.globalSavedGameInfoData.saveFilesInfo[saveIndex - 1];

        loadGameSaveElement.GetComponent<LoadGameSaveElement>().SetInfo(saveIndex, saveFileInfo, gameSaveData);
    }

    public void DeleteGameSaveButtonClicked(int gameSaveIndex)
    {
        SaveManager.instance.SetSavedGameDataFileAsNotUsed(gameSaveIndex);
        SaveManager.instance.ImportGameSavesInfoFile();
        UpdateLoadGameSavePanel();
    }

}
