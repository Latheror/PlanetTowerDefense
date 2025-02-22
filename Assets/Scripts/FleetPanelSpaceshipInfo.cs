﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FleetPanelSpaceshipInfo : MonoBehaviour {

    [Header("UI")]
    public TextMeshProUGUI spaceshipTypeText;
    public TextMeshProUGUI healthPointsText;
    public TextMeshProUGUI maxHealthPointsText;
    public TextMeshProUGUI shieldPointsText;
    public TextMeshProUGUI maxShieldPointsText;
    public TextMeshProUGUI experiencePointsText;
    public TextMeshProUGUI nextLevelexperiencePointsText;
    public TextMeshProUGUI fleetPointsUsedText;

    public Image spaceshipImage;

    public GameObject healthBarBackground;
    public GameObject healthBar;

    public GameObject shieldBarBackground;
    public GameObject shieldBar;

    public GameObject experienceBarBackground;
    public GameObject experienceBar;

    public GameObject starsImagePanel;

    [Header("Operation")]
    public GameObject spaceship;

    public void SetSpaceship(GameObject alliedSpaceship)
    {
        spaceship = alliedSpaceship;
    }

    public void UpdateInfo()
    {
        if(spaceship.GetComponent<AllySpaceship>() != null)
        {
            AllySpaceship alliedSpaceship = spaceship.GetComponent<AllySpaceship>();

            spaceshipTypeText.text = spaceship.GetComponent<AllySpaceship>().spaceshipType.typeName.ToString();
            healthPointsText.text = alliedSpaceship.healthPoints.ToString();
            maxHealthPointsText.text = alliedSpaceship.maxHealthPoints.ToString();
            shieldPointsText.text = alliedSpaceship.shieldPoints.ToString();
            maxShieldPointsText.text = alliedSpaceship.maxShieldPoints.ToString();
            experiencePointsText.text = alliedSpaceship.experiencePoints.ToString();

            spaceshipImage.sprite = alliedSpaceship.spaceshipType.sprite;

            nextLevelexperiencePointsText.text = (alliedSpaceship.level < 3 /* USE A VARIABLE HERE */) ? (alliedSpaceship.spaceshipType.levelExperiencePointLimits[alliedSpaceship.level - 1].ToString()) : " - ";
            fleetPointsUsedText.text = alliedSpaceship.spaceshipType.fleetPointsNeeded.ToString();

            UpdateShieldBar();
            UpdateHealthBar();
            UpdateExperienceBar();
            UpdateStarsImage();
        }
    }

    public void UpdateShieldBar()
    {
        if (shieldBarBackground != null && shieldBar != null)
        {
            float shieldBarBackPanelWidth = shieldBarBackground.GetComponent<RectTransform>().rect.width;
            //Debug.Log("ShieldBarBackPanelWidth [" + shieldBarBackPanelWidth + "]");

            float maxShieldPoints = spaceship.GetComponent<AllySpaceship>().maxShieldPoints;
            float shieldPoints = spaceship.GetComponent<AllySpaceship>().shieldPoints;
            float shieldRatio = shieldPoints / maxShieldPoints;
            //Debug.Log("ShieldRatio [" + shieldRatio + "]");

            RectTransform shieldPointsBarRectTransform = shieldBar.GetComponent<RectTransform>();
            //Debug.Log("shieldPointsBarRectTransform | SizeDeltaX [" + shieldPointsBarRectTransform.sizeDelta.x + "] | SizeDeltaY [" + shieldPointsBarRectTransform.sizeDelta.y + "]");
            shieldPointsBarRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 130 * shieldRatio);

        }
    }

    public void UpdateHealthBar()
    {
        if (healthBarBackground != null && healthBar != null)
        {
            float maxHealthPoints = spaceship.GetComponent<AllySpaceship>().maxHealthPoints;
            float healthPoints = spaceship.GetComponent<AllySpaceship>().healthPoints;
            float healthRatio = healthPoints / maxHealthPoints;

            RectTransform healthPointsBarRectTransform = healthBar.GetComponent<RectTransform>();
            healthPointsBarRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 130 * healthRatio);
        }
    }

    public void UpdateExperienceBar()
    {
        if (experienceBarBackground != null && experienceBar != null)
        {
            AllySpaceship allyS = spaceship.GetComponent<AllySpaceship>();
            float nextLevelExperiencePoints = (allyS.level < 3) ? allyS.spaceshipType.levelExperiencePointLimits[allyS.level - 1] : allyS.experiencePoints;
            float experiencePoints = spaceship.GetComponent<AllySpaceship>().experiencePoints;
            float experienceRatio = experiencePoints / nextLevelExperiencePoints;

            RectTransform experiencePointsBarRectTransform = experienceBar.GetComponent<RectTransform>();
            experiencePointsBarRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 130 * experienceRatio);
        }
    }

    public void UpdateStarsImage()
    {
        Sprite starsSprite = SpaceshipManager.instance.oneStarSprite;
        switch (spaceship.GetComponent<AllySpaceship>().level)
        {          
            case 1:
            {
                starsSprite = SpaceshipManager.instance.oneStarSprite;
                break;
            }
            case 2:
            {
                starsSprite = SpaceshipManager.instance.twoStarsSprite;
                break;
            }
            case 3:
            {
                starsSprite = SpaceshipManager.instance.threeStarsSprite;
                break;
            }
            default:
            {
                starsSprite = SpaceshipManager.instance.threeStarsSprite;
                break;
            }
        }
        starsImagePanel.GetComponent<Image>().sprite = starsSprite;
    }
}
