﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GeometryManager : MonoBehaviour {

    public static GeometryManager instance;
    void Awake(){ 
        if (instance != null){ Debug.LogError("More than one MeteorsManager in scene !"); return; } instance = this;
    }

    [Header("Settings")]
    public float circleFactor = 100f;
    public float planetRadius = 40f;

    [Header("World")]
    public GameObject mainPlanet;

    // Angle in radians
    public static float GetRadAngleFromXY(float x, float y)
    {
        float angle = 0f;
        if(x >= 0)
        {
            angle = Mathf.Atan(y / x);
        }
        else
        {
            if(y >= 0)
            {
                angle = Mathf.PI + Mathf.Atan(y / x);
            }
            else
            {
                angle = Mathf.Atan(y / x) - Mathf.PI;
            }
        }
        return angle;
    }

    public static float GetRadAngleFromGameObject(GameObject obj)
    {
        float angle = 0f;
        float x = obj.transform.position.x;
        float y = obj.transform.position.y;
        if (x >= 0)
        {
            angle = Mathf.Atan(y / x);
        }
        else
        {
            if (y >= 0)
            {
                angle = Mathf.PI + Mathf.Atan(y / x);
            }
            else
            {
                angle = Mathf.Atan(y / x) - Mathf.PI;
            }
        }
        return angle;
    }

    public float GetDistanceFromPlanetCenter(Vector3 pos)
    {
        return Vector3.Distance(pos, mainPlanet.transform.position);
    }

    public static float RadiansToDegrees(float radian)
    {
        return radian * 180 / (Mathf.PI);
    }



    public float NormalizeRadianAngle(float radianAngle)
    {
        if (radianAngle >= Mathf.PI)
        {
            radianAngle -= 2 * Mathf.PI;
        }
        else 
        if (radianAngle <= - Mathf.PI)
        {
            radianAngle += 2 * Mathf.PI;
        }
        return radianAngle;
    }

    // Angles need to be in degree
    public bool IsAngleInRange(float centerAngle, float rangeAngle, float angleToCompare)
    {
        Debug.Log("IsAngleInRange RAD | CenterAngle: " + centerAngle + " | RangeAngle: " + rangeAngle + " | AngleToCompare: " + angleToCompare);

        centerAngle = RadiansToDegrees(centerAngle);
        rangeAngle = RadiansToDegrees(rangeAngle);
        angleToCompare = RadiansToDegrees(angleToCompare);

        Debug.Log("IsAngleInRange DEG | CenterAngle: " + centerAngle +" | RangeAngle: " + rangeAngle + " | AngleToCompare: " + angleToCompare);
        if(rangeAngle >= 0)
        {
            float deltaAngle = Mathf.Abs(Mathf.DeltaAngle(centerAngle, angleToCompare));
           
            bool angleInRange = (deltaAngle <= rangeAngle / 2);

            Debug.Log("IsAngleInRange DEG | deltaAngle: " + deltaAngle + "] | AngleInRange [" + angleInRange + "]");
            return angleInRange;
        }
        else
        {   
            Debug.Log("ERROR | IsAngleInRange | rangeAngle is negative !");
            return false;
        }
    }

    public Vector3 GetLocationFromTouchPointOnPlanetPlane(Vector3 touchPos)
    {
        //Debug.Log("GetLocationFromTouchPointOnPlanetPlane | TouchPos: " + touchPos);

        Ray ray = Camera.main.ScreenPointToRay(touchPos);

        Plane planetPlane = new Plane(Vector3.forward, mainPlanet.transform.position);
        float distance = 0;
        Vector3 intersectPointPos = new Vector3(0f, 0f, 0f);

        if (planetPlane.Raycast(ray, out distance))
        {
            intersectPointPos = ray.GetPoint(distance);
        }

        //Debug.Log("GetLocationFromTouchPointOnPlanetPlane | IntersectPointPos: " + intersectPointPos);
        intersectPointPos.z = 0;

        return intersectPointPos;
    }

    public Vector3 GetLocationFromTouchPointOnPlanetPlaneWithOffset(Vector3 touchPos)
    {
        //Debug.Log("GetLocationFromTouchPointOnPlanetPlane | TouchPos: " + touchPos);

        Ray ray = Camera.main.ScreenPointToRay(touchPos);

        Plane planetPlane = new Plane(Vector3.forward, mainPlanet.transform.position);
        float distance = 0;
        Vector3 intersectPointPos = new Vector3(0f, 0f, 0f);

        if (planetPlane.Raycast(ray, out distance))
        {
            intersectPointPos = ray.GetPoint(distance);
        }

        //Debug.Log("GetLocationFromTouchPointOnPlanetPlane | IntersectPointPos: " + intersectPointPos);
        intersectPointPos = new Vector3(intersectPointPos.x, intersectPointPos.y, intersectPointPos.z - 50);

        return intersectPointPos;
    }

    /*public bool IsTouchWithinSpaceshipInfoPanelArea(Vector3 touchPos)
    {
        float touchPosX = touchPos.x;
        float touchPosY = touchPos.y;
        float margin = 15;
        GameObject infoPanel = SpaceshipManager.instance.currentSelectedSpaceshipInfoPanel;
        float infoPanelLeftBorder = infoPanel.GetComponent<RectTransform>().position.x - infoPanel.GetComponent<RectTransform>().sizeDelta.x / 2;
        float infoPanelRightBorder = infoPanel.GetComponent<RectTransform>().position.x + infoPanel.GetComponent<RectTransform>().sizeDelta.x / 2;
        float infoPanelTopBorder = infoPanel.GetComponent<RectTransform>().position.y + infoPanel.GetComponent<RectTransform>().sizeDelta.y / 2;
        float infoPanelBottomBorder = infoPanel.GetComponent<RectTransform>().position.y - infoPanel.GetComponent<RectTransform>().sizeDelta.y / 2;

        //Debug.Log("IsTouchWithinSpaceshipInfoPanelArea | Left: " + infoPanelLeftBorder + " | Right: " + infoPanelRightBorder + " | Top: " + infoPanelTopBorder + " | Bottom: " + infoPanelBottomBorder);


        return ((touchPosX >= (infoPanelLeftBorder - margin)) && (touchPosX <= (infoPanelRightBorder + margin)) && (touchPosY >= (infoPanelBottomBorder - margin)) && (touchPosY <= (infoPanelTopBorder + margin)));
    }*/

    public Vector3 RandomSpawnPosition()
    {
        Vector2 randomCirclePos = UnityEngine.Random.insideUnitCircle.normalized;
        Vector3 pos = new Vector3(randomCirclePos.x * circleFactor, randomCirclePos.y * circleFactor, GameManager.instance.objectsDepthOffset);

        return pos;
    }

    public Vector3 RandomSpawnPositionAtRadius(float radius)
    {
        Vector2 randomCirclePos = UnityEngine.Random.insideUnitCircle.normalized;
        Vector3 pos = new Vector3(randomCirclePos.x * radius, randomCirclePos.y * radius, GameManager.instance.objectsDepthOffset);

        return pos;
    }

    public bool AreObjectsInRange(GameObject obj1, GameObject obj2, float range)
    {
        return ((obj1.transform.position - obj2.transform.position).sqrMagnitude <= range*range);
    }

    public bool SegmentIntersectWithPlanet(Vector3 pos1, Vector3 pos2)
    {
        bool doesIntersect = false;

        if (!PosAtSameAngleWithDelta(pos1, pos2, 0.2f))
        {
            //Debug.Log("SegmentIntersectWithPlanet | Pos1: " + pos1.ToString() + " | Pos2: " + pos2.ToString());

            float cx = mainPlanet.transform.position.x;
            float cy = mainPlanet.transform.position.y;
            float r = planetRadius + 10; // Margin, not to be too close to the planet

            float dx = pos2.x - pos1.x;
            float dy = pos2.y - pos1.y;

            float lcx = cx - pos1.x;
            float lcy = cy - pos1.y;

            //Debug.Log("dx: " + dx + " | dy: " + dy + " | lcx: " + lcx + " | lcy: " + lcy + " | r: " + r);

            float a = Mathf.Pow(dx, 2) + Mathf.Pow(dy, 2);
            float b = 2 * (dx * (pos1.x - cx) + dy * (pos1.y - cy));
            //float c = (pos1.x - cx) * (pos1.x - cx) + (pos1.y - cy) * (pos1.y - cy) - Mathf.Pow(r, 2);
            float c = Mathf.Pow(lcx, 2) + Mathf.Pow(lcy, 2) - Mathf.Pow(r, 2);

            float delta = Mathf.Pow(b, 2) - 4 * a * c;

            doesIntersect = (delta >= 0) ? true : false;

            //Debug.Log("Delta: " + delta);
        }

        //Debug.Log("SegmentIntersectWithPlanet | DoesIntersect: " + doesIntersect);

        return doesIntersect;
    }

    public static float GetMeanAngle(float angle1, float angle2)
    {
        float meanAngle = 0f;
        //Debug.Log("GetMeanAngle | Angle1: " + angle1 + ", Angle2: " + angle2);

        if((angle1 >= 0 && angle2 >= 0) || (angle1 <= 0 && angle2 <= 0)) // Angles have the same sign
        {
            meanAngle = (angle1 + angle2) / 2;
        }
        else // Angles have different signs
        {
            meanAngle = Mathf.Atan2(Mathf.Sin(angle1)+ Mathf.Sin(angle2), Mathf.Cos(angle1) + Mathf.Cos(angle2));
        }

        //Debug.Log("MeanAngle: " + meanAngle);
        return meanAngle;
    }

    public static float NormalizeRadAngle(float radAngle)
    {
        float normalizedAngle = radAngle;
        if (radAngle > Mathf.PI)
        {
            normalizedAngle = radAngle - 2*Mathf.PI;
        }
        else if(radAngle < - Mathf.PI)
        {
            normalizedAngle = radAngle + 2 * Mathf.PI;
        }

        return normalizedAngle;
    }

    public static bool PosWithinPlanetArea(Vector3 pos)
    {
        return ((instance.mainPlanet.transform.position - pos).sqrMagnitude < instance.planetRadius*instance.planetRadius);
    }
    
    public static bool PosAtSameAngleWithDelta(Vector3 pos1, Vector3 pos2, float delta)
    {
        bool sameAngle = (Mathf.Abs(GetRadAngleFromXY(pos1.x, pos1.y) - GetRadAngleFromXY(pos2.x, pos2.y)) <= delta);

        return sameAngle;
    }

    public static bool IsObjectLeftToOther(GameObject target_1, GameObject target_2)
    {
        float angle1 = GetRadAngleFromGameObject(target_1);
        float angle2 = GetRadAngleFromGameObject(target_2);

        Debug.Log("Angle1 [" + angle1 + "] | Angle2 [" + angle2 + "]");

        float difference = angle2 - angle1;

        if (difference < -Mathf.PI)
            difference += Mathf.PI * 2;
        if (difference > Mathf.PI)
            difference -= Mathf.PI * 2;

        return (difference < 0.0f);
    }

    [Serializable]
    public class Position
    {
        public float x, y, z;

        public Position(float x, float y, float z)
        {
            this.x = x; this.y = y; this.z = z;
        }

        public Position(Vector3 pos)
        {
            this.x = pos.x; this.y = pos.y; this.z = pos.z;
        }
    }
}
