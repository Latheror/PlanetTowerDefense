﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : MonoBehaviour {

    [Header("Settings")]
    public float originalSize;
    public float size;
    public float lifeTime = 30f;
    public int resourcesToGatherPerUnitOfSize = 10;
    public int resourcesToGather = 0;
    public bool isBeingCollected = false;
    public bool isBeingTargetedByCollector = false;

	// Use this for initialization
	void Start () {
        resourcesToGather = (int)(resourcesToGatherPerUnitOfSize * size);
        InvokeRepeating("Vanish", 30f, 0f);
	}


    public void SetOriginalSize(float size)
    {
        this.originalSize = size;
        this.size = originalSize;
    }

    public void Collect(/*float collectionPower*/)
    {
        ResourcesManager.instance.ProduceResource(ResourcesManager.instance.GetResourceTypeByName("steel"), resourcesToGather);
        ResourcesManager.instance.ProduceResource(ResourcesManager.instance.GetResourceTypeByName("carbon"), Mathf.RoundToInt(resourcesToGather/2));
        Vanish();
    }

    public void Vanish()
    {
        DebrisManager.instance.debrisList.Remove(gameObject);
        Destroy(gameObject);
    }
	
}
