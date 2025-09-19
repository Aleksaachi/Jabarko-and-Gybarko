using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingMenu : MonoBehaviour
{
    public static BuildingMenu instance;
    public GameObject buildOrderPrefab;
    public GameObject blueprintPrefab;
    public BuildOrderTemplate currentTemplate;

    void Awake()
    {
        instance = this;
    }

    public static void SpawnBlueprint(BuildOrderTemplate template)
    {
        if (instance.currentTemplate != null) return;

        instance.currentTemplate = template;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject blueprint = Instantiate(instance.blueprintPrefab, mousePosition, Quaternion.identity);
        blueprint.GetComponent<SpriteRenderer>().sprite = instance.currentTemplate.icon;
    }

    public static void SpawnBuildOrder(Vector3 position)
    {
        BuildingOrder building = Instantiate(instance.buildOrderPrefab, position, Quaternion.identity).GetComponent<BuildingOrder>();
        building.buildOrderTemplate = instance.currentTemplate;
        instance.currentTemplate = null;
        
        EventManager.newBuildingPlaced?.Invoke();
    }


}
