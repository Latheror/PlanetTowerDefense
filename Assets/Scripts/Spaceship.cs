﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Spaceship : MonoBehaviour {

    [Header("Main")]
    public bool isActivated;
    public bool isInAutomaticMode;
    public bool selected = false;
    public bool isAlly = false;
    public float maxHealthPoints = 100f;
    public float healthPoints = 100f;
    public bool hasShield;
    public float shieldPoints = 100f;
    public float maxShieldPoints = 100f;
    public float shieldRegenerationDelay = 3f;
    public float shieldRegenerationAmount = 5f;
    public GameObject homeSpaceport = null;
    public SpaceshipManager.SpaceshipType spaceshipType;

    [Header("Movement")]
    public float movementSpeed = 100f;
    public float rotationSpeed = 50f;
    public float idleRotationSpeed = 5f;
    public Vector3 manualDestination;
    public bool manualDestinationReached = false;
    public float manualDestinationDelta = 20f;
    public Vector3 tempDestination;

    [Header("Parts")]
    public GameObject[] shootingPoints;
    public GameObject shieldHolder;

    [Header("Attack")]
    public GameObject target;
    public GameObject previousTarget;
    public float attackDistance = 20;
    public bool pulseFinished = true;
    public float pulsePeriod = 1f;
    public float firePeriod = 1f;
    public float laserSpatialLength = 10f;
    public float damagePower = 20;
    public bool isUnderAttack = false;
    public float isUnderAttackTimer = 0f;
    public float isUnderAttackCoolDown = 3f;

    [Header("UI")]
    //public GameObject infoPanel;
    //public TextMeshProUGUI infoPanelModeText;
    //public GameObject infoPanelModeButton;
    public GameObject healthBarPanel;
    public GameObject healthPointsBar;
    public GameObject healthPointsBarBack;
    public GameObject shieldBarPanel;
    public GameObject shieldPointsBar;
    public GameObject shieldPointsBarBack;
    //public Color infoPanelAutoModeColor = Color.green;
    //public Color infoPanelManualModeColor = Color.red;

    // Use this for initialization
    void Start () {
        target = null;
        isActivated = true;
        healthPoints = maxHealthPoints;
        manualDestination = transform.position;
        manualDestinationReached = true;
        isInAutomaticMode = true;
        //infoPanel.SetActive(false);
        SetStartingMode();
        DisableShootingPoints();
    }

    protected virtual void UpdateTarget(){ }
    protected virtual void HandleMovements() { }
    protected virtual void AttackTarget() { }

    public bool IsTargetInRange()
    {
        bool isInRange = false;
        if(target != null)
        {
            isInRange = ((transform.position - target.transform.position).sqrMagnitude <= attackDistance * attackDistance);
        }
        return isInRange; 
    }

    public bool IsTargetInRangeWithDelta()
    {
        return ((transform.position - target.transform.position).sqrMagnitude <= attackDistance * attackDistance * 2);
    }

    public void RotateTowardsTarget()
    {
        if(target != null)
        {
            Vector3 targetDir = target.transform.position - transform.position;
            float rotationStep = rotationSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, rotationStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    public void RotateTowardsTempDest()
    {
        Vector3 targetDir = tempDestination - transform.position;
        float rotationStep = rotationSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, rotationStep, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);
    }

    protected void RotateTowardsManualDestination()
    {
        Vector3 destDir = manualDestination - transform.position;
        float rotationStep = rotationSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, destDir, rotationStep, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);
    }

    protected void RotateAroundPlanet()
    {
        float positionAngle = GeometryManager.GetRadAngleFromGameObject(gameObject);
        float degreePositionAngle = GeometryManager.RadiansToDegrees(positionAngle);
        //Debug.Log("RotateAroundPlanet | PositionAngleRad [" + positionAngle + "] | DegreePositionAngle [" + degreePositionAngle + "]");

        transform.rotation = Quaternion.Euler(-90 - degreePositionAngle, 90, -90);
        transform.RotateAround(GameManager.instance.mainPlanet.transform.position, Vector3.forward, Time.deltaTime * idleRotationSpeed);
    }

    protected IEnumerator FireLasersRoutine()
    {
        //Debug.Log("Firing !");

        bool laserReachedTarget = false;
        pulseFinished = false;
        float elapsedTime = 0f;
        float distanceRatio = 0f;

        while (!laserReachedTarget && target != null)
        {
            // Start firing
            elapsedTime += Time.deltaTime;
            distanceRatio = elapsedTime / pulsePeriod;

            //Debug.Log("Distance ratio: " + distanceRatio);

            foreach (GameObject shootingPoint in shootingPoints)
            {
                Vector3 shootingPos = shootingPoint.transform.position;
                Vector3 posDiff = target.transform.position - shootingPos;
                Vector3 middleLaserPos = (shootingPos + (posDiff * distanceRatio));

                Vector3 leftLaserPos = (shootingPos + (posDiff * distanceRatio * 0.9f));
                Vector3 rightLaserPos = Vector3.zero;

                if ((Vector3.Distance(shootingPos, shootingPos + (posDiff * distanceRatio * 1.1f))) < (Vector3.Distance(shootingPos, target.transform.position)))
                {
                    rightLaserPos = (shootingPos + (posDiff * distanceRatio * 1.1f));
                }
                else
                {
                    rightLaserPos = target.transform.position;
                }

                LineRenderer lr = shootingPoint.GetComponent<LineRenderer>();
                lr.SetPosition(0, leftLaserPos);
                lr.SetPosition(1, rightLaserPos);

            }

            EnableLasers(true);

            // Wait and stop firing
            yield return new WaitForSeconds(pulsePeriod /5);
            EnableLasers(false);

            if (elapsedTime >= pulsePeriod)
            {
                laserReachedTarget = true;
            }
        }

        // Deal damage to target
        DealDamageToTarget();

        yield return new WaitForSeconds(firePeriod);

        pulseFinished = true;
    }

    protected void DealDamageToTarget()
    {
        if (target != null)
        {
            if(target.CompareTag("meteor"))   // Target is a meteor
            {
                if(target.GetComponent<Meteor>().TakeDamage(damagePower))
                {
                    // Meteor destroyed
                    if(isAlly)
                    {
                        GetComponent<AllySpaceship>().RewardExperiencePointsFromMeteor(target.GetComponent<Meteor>());
                    }
                }
            }
            else if(target.CompareTag("spaceship") || target.CompareTag("enemy"))
            {
                if(target.GetComponent<Spaceship>().TakeDamage(damagePower))
                {
                    // Meteor destroyed
                    if (isAlly)
                    {
                        GetComponent<AllySpaceship>().RewardExperiencePointsFromSpaceship(target.GetComponent<Spaceship>());
                    }
                }
            }
        }
    }

    // Returns true if spaceship is destroyed after taking damage
    public bool TakeDamage(float damage)
    {
        bool destroyed = false;
        if(hasShield)
        {
            float damageTakenByShield = AbsorbDamageInShield(damage);
            if (damage > damageTakenByShield)    // Shield down to zero, decrease healthpoints
            {
                float remainingDamage = damage - damageTakenByShield;
                DecreaseHealthPoints(remainingDamage);
            }
        }
        else
        {
            DecreaseHealthPoints(damage);
        }

        if(healthPoints <= 0)
        {
            destroyed = true;
            DestroySpaceship();
        }
        else
        {
            StopCoroutine(UnderAttackTimerCoroutine(0f));
            StartCoroutine(UnderAttackTimerCoroutine(isUnderAttackCoolDown));
        }

        return destroyed;
    }

    public void Heal(float healingPower)
    {
        healthPoints = Mathf.Min(maxHealthPoints, healthPoints + healingPower);
        UpdateHealthBar();
        SpaceshipInfoPanel.instance.UpdateInfo();
    }

    protected void UpdateHealthBar()
    {
        if(healthBarPanel != null && healthPointsBar != null)
        {
            float healthBarBackPanelWidth = healthPointsBarBack.GetComponent<RectTransform>().rect.width;
            //Debug.Log("Spaceship | healthBarBackPanelWidth [" + healthBarBackPanelWidth + "]");

            float healthRatio = healthPoints / maxHealthPoints;

            RectTransform healthPointsBarRectTransform = healthPointsBar.GetComponent<RectTransform>();
            healthPointsBarRectTransform.sizeDelta = new Vector2(healthBarBackPanelWidth * healthRatio, healthPointsBarRectTransform.sizeDelta.y);
            //Debug.Log("Spaceship | sizeDelta [" + healthPointsBarRectTransform.sizeDelta + "]");
        }
    }

    protected void UpdateShieldBar()
    {
        if (shieldBarPanel != null && shieldPointsBar != null)
        {
            float shieldBarBackPanelWidth = shieldPointsBarBack.GetComponent<RectTransform>().rect.width;
            //Debug.Log("Spaceship | shieldBarBackPanelWidth [" + shieldBarBackPanelWidth + "]");

            float shieldRatio = shieldPoints / maxShieldPoints;

            //Debug.Log("Shield Ratio: " + shieldRatio);

            RectTransform shieldPointsBarRectTransform = shieldPointsBar.GetComponent<RectTransform>();
            shieldPointsBarRectTransform.sizeDelta = new Vector2(shieldBarBackPanelWidth * shieldRatio, shieldPointsBarRectTransform.sizeDelta.y);
            //Debug.Log("Spaceship | sizeDelta [" + shieldPointsBarRectTransform.sizeDelta + "]");
        }
    }

    public virtual void DestroySpaceship() { }

    // Not used anymore
    private void SetLasersPositions(Vector3 pos11, Vector3 pos12, Vector3 pos21, Vector3 pos22)
    {
        //foreach (GameObject shootingPoint in shootingPoints)
        //{
        LineRenderer lr1 = shootingPoints[0].GetComponent<LineRenderer>();
        lr1.SetPosition(0, pos11);
        lr1.SetPosition(1, pos12);

        LineRenderer lr2 = shootingPoints[1].GetComponent<LineRenderer>();
        lr2.SetPosition(0, pos21);
        lr2.SetPosition(1, pos22);
        //}
    }


    protected void EnableLasers(bool enable)
    {
        foreach (GameObject shootingPoint in shootingPoints)
        {
            LineRenderer lr = shootingPoint.GetComponent<LineRenderer>();
            if (enable) {
                lr.enabled = true;
            }
            else {
                lr.enabled = false;
            }
        }
    }

    public void Select(bool select)
    {
        // Tutorial indicator //
        TutorialManager.instance.DisplayIndicator(TutorialManager.IndicatorID.select_spaceship, false);
        // ------------------ //

        this.selected = select;
        if(select)
        {
            SpaceshipManager.instance.SelectSpaceship(this.gameObject);
        }
        else
        {
            SpaceshipManager.instance.DeselectSpaceship();
        }
    }

    public void Highlight()
    {

    }

    public void SwitchMode()
    {
        isInAutomaticMode = ! isInAutomaticMode;
        if(!isInAutomaticMode)
        {
            manualDestination = transform.position;           
        }
        SpaceshipInfoPanel.instance.UpdateModeDisplay();
        Debug.Log("Switching Mode !");
    }

    public void SetManualDestination(Vector3 dest)
    {
        manualDestination = dest;
    }

    public bool IsCloseEnoughToDestination()
    {
        return ((transform.position - manualDestination).sqrMagnitude < manualDestinationDelta * manualDestinationDelta);
    }

    public void InfoPanelCloseButtonActions()
    {
        SpaceshipManager.instance.DeselectSpaceship();
    }

    public void SetStartingMode()
    {
        isInAutomaticMode = true;
        SpaceshipInfoPanel.instance.UpdateModeDisplay();
    }

    public void UpdateInfoPanels()
    {
        // Allied spaceship
        if(isAlly)
        {
            SpaceshipInfoPanel.instance.UpdateInfo();
        }
        else  // Enemy Spaceship
        {
            if(selected)
            {
                EnemyInfoPanel.instance.UpdateInfo();
            }
        }
    }

    public void DecreaseHealthPoints(float amount)
    {
        healthPoints = Mathf.Max(0f, healthPoints - amount);
        UpdateHealthBar();
        UpdateInfoPanels();
    }

    public void IncreaseHealthPoints(float amount)
    {
        healthPoints = Mathf.Min(maxHealthPoints, healthPoints + amount);
        UpdateHealthBar();
        UpdateInfoPanels();
    }

    public void DecreaseShieldPoints(float amount)
    {
        shieldPoints = Mathf.Max(0f, shieldPoints - amount);
        UpdateShieldBar();
        UpdateInfoPanels();
        UpdateShieldDisplay();
    }

    public void IncreaseShieldPoints(float amount)
    {
        shieldPoints = Mathf.Min(maxShieldPoints, shieldPoints + amount);
        UpdateShieldBar();
        UpdateInfoPanels();
        UpdateShieldDisplay();
    }

    public void RegenerateShield()
    {
        //Debug.Log("RegenerateShield");
        if(!isUnderAttack && shieldPoints < maxShieldPoints)
        {
            IncreaseShieldPoints(shieldRegenerationAmount);
        }
    }

    public float AbsorbDamageInShield(float amount)
    {
        float shieldPointsBefore = shieldPoints;
        DecreaseShieldPoints(amount);
        float shieldPointsAfter = shieldPoints;
        return (shieldPointsBefore - shieldPointsAfter);
    }

    public IEnumerator UnderAttackTimerCoroutine(float coolDownTime)
    {
        isUnderAttack = true;
        isUnderAttackTimer = coolDownTime;
        while(isUnderAttackTimer > 0f)
        {
            isUnderAttackTimer -= Time.deltaTime;
            yield return null;
        }
        isUnderAttack = false;
        yield return null;
    }

    public void DisableShootingPoints()
    {
        foreach (GameObject shootingPoint in shootingPoints)
        {
            shootingPoint.GetComponent<LineRenderer>().enabled = false;
        }
    }

    public void DisplayShield(bool display)
    {
        if(hasShield && shieldHolder != null)
        {
            shieldHolder.SetActive(display);
        }
    }

    public void UpdateShieldDisplay()
    {
        DisplayShield(shieldPoints > 0);
    }

    public void SetSpaceshipType(SpaceshipManager.SpaceshipType sType)
    {
        spaceshipType = sType;
    }
}
