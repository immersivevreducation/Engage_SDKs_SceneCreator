using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handler class for OutfitZones, attach script to GameObject in scene,
/// assign a Collider from each zone to the script, and assign an associated override value
/// to each collider.
/// </summary>
public class OutfitZoneHandler : MonoBehaviour
{
    //Public variables
    [Header("Colliders for each zone")]
    public List<GameObject> zones;
    [Header("Override value for each zone in order")]
    [Tooltip("-1 for no outfit, 0 for spacesuit")]
    public List<int> overrideValues;
    [Header("Override value for if player isn't in a zone")]
    public int defaultZoneOverride;
    //private variables
    Dictionary<Collider, int> zoneMap;
    bool outfitSet = false;
    bool goTriggered = false;
    bool goTriggeredSummon = false;
    int startZoneOverride;

    /// <summary>
    /// Initialise lists and add values to dictionary
    /// </summary>

}