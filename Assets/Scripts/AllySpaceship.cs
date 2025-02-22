﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllySpaceship : Spaceship {

    [Header("UI")]
    public Image starImage;

    [Header("Operation")]
    public int id = -1;
    public int experiencePoints = 0;
    public int experiencePointsPerMeteorSizeUnit = 10;
    public int experiencePointsPerSpaceshipDestroyed = 100;
    public int level = 1;

    public void Initialize()
    {
        isAlly = true;
        target = null;
        isActivated = true;
        manualDestination = transform.position;
        manualDestinationReached = true;
        isInAutomaticMode = true;
        SetStartingMode();
        level = 1;
        experiencePoints = 0;
        SetStarImage();
        InvokeRepeating("RegenerateShield", 0f, shieldRegenerationDelay);
    }

    // Update is called once per frame
    void Update () {
        if (GameManager.instance.gameState == GameManager.GameState.Default)
        {
            UpdateTarget();
            HandleMovements();
            AttackTarget();
            AvoidOtherAllies();
        }
    }

    protected override void UpdateTarget()
    {
        // Spaceships only work if they are activated
        if (isActivated)
        {
            //Debug.Log("Laser Turret | Update target");
            List<GameObject> meteors = MeteorsManager.instance.meteorsList;
            List<GameObject> enemies = EnemiesManager.instance.enemies;
            float shortestDistance = Mathf.Infinity;
            int maxPriorityFound = 0;
            GameObject nearestEnemy = null;

            // Search for meteors
            if (meteors.Count > 0)
            {
                int priority = EnemiesManager.instance.meteorPriority;
                if(priority > maxPriorityFound)
                {
                    shortestDistance = Mathf.Infinity;
                    nearestEnemy = null;
                    maxPriorityFound = priority;
                    foreach (GameObject meteor in meteors)      
                    {
                        float distanceToEnemy_squared = (transform.position - meteor.transform.position).sqrMagnitude;
                        //Debug.Log("Meteor found - Distance is : " + distanceToEnemy);
                        if (distanceToEnemy_squared < shortestDistance)
                        {
                            shortestDistance = distanceToEnemy_squared;
                            nearestEnemy = meteor;
                        }
                    }
                }
            }

            // Search for enemies (spaceships, ...)
            //Debug.Log("Number of enemy spaceships: " + EnemiesManager.instance.enemies.Count);
            if (EnemiesManager.instance.enemies.Count > 0)
            {
                int priority = EnemiesManager.instance.spaceshipsPriority;
                if (priority > maxPriorityFound)
                {
                    shortestDistance = Mathf.Infinity;
                    nearestEnemy = null;
                    foreach (GameObject enemy in enemies)       
                    {
                        float distanceToEnemy_squared = (transform.position - enemy.transform.position).sqrMagnitude;
                        if (distanceToEnemy_squared < shortestDistance /* Temporary */ && enemy.GetComponent<Spaceship>().isActivated)
                        {
                            shortestDistance = distanceToEnemy_squared;
                            nearestEnemy = enemy;
                        }
                    }
                }
            }

            if (nearestEnemy != null)
            {
                target = nearestEnemy;
                if (target != previousTarget)   // This is a new target
                {
                    if (previousTarget != null && previousTarget.CompareTag("meteor"))
                    {
                        previousTarget.GetComponent<Meteor>().ResetMeteorSettings();
                    }
                    previousTarget = target;
                }
                //Debug.Log("New meteor target set: " + target + " - Distance is: " + shortestDistance);
            }
            else
            {
                if (previousTarget != null && previousTarget.CompareTag("meteor"))
                {
                    previousTarget.GetComponent<Meteor>().ResetMeteorSettings();
                }
                target = null;
                previousTarget = target;
            }
        }
        else
        {
            //Debug.Log("Spaceship isn't activated !");
        }
    }

    protected override void HandleMovements()
    {
        if(isActivated)
        {
            // Set manual destination to target
            if(isInAutomaticMode && target != null)
            {
                SetManualDestination(target.transform.position);
            }

            if (manualDestination != null)
            {
                // Check if path towards destination intersects with planet
                if (!IsCloseEnoughToDestination() && (GeometryManager.instance.SegmentIntersectWithPlanet(gameObject.transform.position, manualDestination)))
                {
                    //Debug.Log("HandleMovements | Setting a temp dest");
                    float spaceshipPosAngle = GeometryManager.GetRadAngleFromXY(transform.position.x, transform.position.y);
                    float spaceshipPosDistance = GeometryManager.instance.GetDistanceFromPlanetCenter(transform.position);
                    //Debug.Log("spaceshipPosAngle: " + spaceshipPosAngle + " | spaceshipPosDistance: " + spaceshipPosDistance);
                    float manualDestPosAngle = GeometryManager.GetRadAngleFromXY(manualDestination.x, manualDestination.y);
                    float manualDestPosDistance = GeometryManager.instance.GetDistanceFromPlanetCenter(manualDestination);
                    //Debug.Log("manualDestPosAngle: " + manualDestPosAngle + " | manualDestPosDistance: " + manualDestPosDistance);

                    // Mean angle
                    float meanAngle = GeometryManager.GetMeanAngle(spaceshipPosAngle, manualDestPosAngle);
                    float meanDistance = Mathf.Max((manualDestPosAngle + manualDestPosDistance) / 2, 10);
                    //Debug.Log("meanAngle: " + meanAngle + " | meanDistance: " + meanDistance);

                    tempDestination = new Vector3(meanDistance * Mathf.Cos(meanAngle), meanDistance * Mathf.Sin(meanAngle), manualDestination.z);
                }
                else
                {
                    // Set temp dest to manual dest
                    tempDestination = manualDestination;
                }
            }

            if (!isInAutomaticMode)  // Manual Mode
            {
                if (!IsCloseEnoughToDestination())
                {
                    // Move towards destination
                    transform.position = Vector3.MoveTowards(transform.position, tempDestination, Time.deltaTime * movementSpeed);
                    RotateTowardsTempDest();
                }
                else
                {
                    if (target != null && IsTargetInRange())
                    {
                        RotateTowardsTarget();
                    }
                }
            }
            else  // Automatic mode
            {
                if (target != null)
                {
                    if (!IsTargetInRange())
                    {
                        // Go closer to target
                        transform.position = Vector3.MoveTowards(transform.position, tempDestination, Time.deltaTime * movementSpeed);
                    }
                    RotateTowardsTempDest();
                }
            }
        }
    }

    protected override void AttackTarget()
    {
        if (isActivated && target != null)
        {
            if (isInAutomaticMode) // Automatic Mode
            {
                if (IsTargetInRange() && pulseFinished)
                {
                    //Debug.Log("Start firing coroutine");
                    StartCoroutine(FireLasersRoutine());
                }
            }
            else  // Manual Mode
            {
                //Debug.Log("AttackTarget, manual mode | IsCloseEnoughToDestination [" + IsCloseEnoughToDestination() + "] | IsTargetInRange [" + IsTargetInRange() + "]");
                if (IsCloseEnoughToDestination() && IsTargetInRange() && pulseFinished)
                {
                    StartCoroutine(FireLasersRoutine());
                }
            }
        }
    }

    public override void DestroySpaceship()
    {
        //Debug.Log("Allied Spaceship has been destroyed !");
        // temporary
        isActivated = false;
        SpaceshipManager.instance.allySpaceships.Remove(gameObject);
        SpaceshipManager.instance.UpdateFleetPointsInfo();

        // Remove from potential Spaceport
        if(homeSpaceport != null)
        {
            homeSpaceport.GetComponent<Spaceport>().RemoveSpaceship(gameObject);
        }

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision col)
    {
        //Debug.Log("Spaceship collided with : " + col.gameObject.name);
        if (col.gameObject.CompareTag("gatherable"))
        {
            //Debug.Log("Touched gatherable object !");
            Gatherable g = col.gameObject.GetComponent<Gatherable>();
            g.ActOnSpaceship(this);       
        }
    }

    // Avoid spaceships being at the same position when they don't have any target
    public void AvoidOtherAllies()
    {
        if(target == null)
        {
            GameObject otherAlly = SpaceshipManager.instance.IsOtherAllyInRange(gameObject);
            //Debug.Log("AvoidOtherAllies |" + otherAlly);
            if (otherAlly != null)
            {
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, otherAlly.transform.position, -movementSpeed * Time.deltaTime);

                Vector3 alliedDir = otherAlly.transform.position - transform.position;
                float rotationStep = rotationSpeed * Time.deltaTime;
                Vector3 newDir = Vector3.RotateTowards(- transform.forward, alliedDir, rotationStep, 0.0f);
                transform.rotation = Quaternion.LookRotation(newDir);
            }
        }
    }

    public void IncreaseExperiencePoints(int delta)
    {
        experiencePoints += delta;
        //Debug.Log("IncreaseExperiencePoints [" + experiencePoints + "]");

        if(level < 3)   // TODO: use a variable (max level)
        {
            if(experiencePoints > spaceshipType.levelExperiencePointLimits[level - 1])
            {
                Debug.Log("AllySpaceship | Level increased !");
                IncreaseLevel();
            }
        }
        else
        {
            experiencePoints = spaceshipType.levelExperiencePointLimits[level - 1];
        }
    }

    public void IncreaseLevel()
    {
        level++;
        ApplyLevelUpStatsModifications();
        RetrieveFullStats();
        SetStarImage();
    }

    public void ApplyLevelUpStatsModifications()
    {
        float bonusFactor = spaceshipType.levelUpStatIncreaseFactor;

        damagePower *= bonusFactor;
        maxHealthPoints *= bonusFactor;
        maxShieldPoints *= bonusFactor;
    }

    public void RetrieveFullStats()
    {
        healthPoints = maxHealthPoints;
        shieldPoints = maxShieldPoints;
    }

    public void RewardExperiencePointsFromMeteor(Meteor meteor)
    {
        IncreaseExperiencePoints(Mathf.FloorToInt(meteor.originalSize * experiencePointsPerMeteorSizeUnit));
    }

    public void RewardExperiencePointsFromSpaceship(Spaceship spaceship)
    {
        // TODO : Take spaceship parameters into account
        IncreaseExperiencePoints(Mathf.FloorToInt(experiencePointsPerSpaceshipDestroyed));
    }

    public void SetLevel(int level_)
    {
        level = level_;
        SetStarImage();
    }

    public void SetStarImage()
    {
        Sprite starSprite = SpaceshipManager.instance.oneStarSprite;
        switch(level)
        {
            case 1:
            {
                starSprite = SpaceshipManager.instance.oneStarSprite;
                break;
            }
            case 2:
            {
                starSprite = SpaceshipManager.instance.twoStarsSprite;
                break;
            }
            case 3:
            {
                starSprite = SpaceshipManager.instance.threeStarsSprite;
                break;
            }
            default:
            {
                Debug.Log("Spaceship Level out of range [" + level + "]");
                break;
            }
        }

        starImage.sprite = starSprite;
    }

}
