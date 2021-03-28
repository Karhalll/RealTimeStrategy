using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private Building[] buildings = new Building[0];

    private List<Unit> myUnits = new List<Unit>();
    private List<Building> myBuildings = new List<Building>();

    public List<Unit> GetMyUnits()
    {
        return myUnits;
    }

    public List<Building> GetMyBuildings()
    {
        return myBuildings;
    }

    #region Server

    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawn += ServerHandleUnitSpawn;
        Unit.ServerOnUnitDespawn += ServerHandleUnitDespawn;
        Building.ServerOnBuildingSpawn += ServerHandleBuildingSpawn;
        Building.ServerOnBuildingDespawn += ServerHandleBuildingDespawn;
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawn -= ServerHandleUnitSpawn;
        Unit.ServerOnUnitDespawn -= ServerHandleUnitDespawn;
        Building.ServerOnBuildingSpawn -= ServerHandleBuildingSpawn;
        Building.ServerOnBuildingDespawn -= ServerHandleBuildingDespawn;
    }

    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 point)
    {
        Building buildingToPlace = null;

        foreach (Building building in buildings)
        {
            if (building.GetId() == buildingId)
            {
                buildingToPlace = building;
                break;
            }
        }

        if (buildingToPlace == null) { return; }

        GameObject buildingInstance = 
            Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);

        NetworkServer.Spawn(buildingInstance, connectionToClient);
    }

    private void ServerHandleUnitSpawn(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myUnits.Add(unit);
    }

    private void ServerHandleUnitDespawn(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myUnits.Remove(unit);
    }

    private void ServerHandleBuildingSpawn(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myBuildings.Add(building);
    }

    private void ServerHandleBuildingDespawn(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myBuildings.Remove(building);
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        if (NetworkServer.active) { return; }

        Unit.AuthorityOnUnitSpawn += AuthorityHandleUnitSpawn;
        Unit.AuthorityOnUnitDespawn += AuthorityHandleUnitDespawn;

        Building.AuthorityOnBuildingSpawn += AuthorityHandleBuildingSpawn;
        Building.AuthorityOnBuildingDespawn += AuthorityHandleBuildingDespawn;
    } 

    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority) { return; }

        Unit.AuthorityOnUnitSpawn -= AuthorityHandleUnitSpawn;
        Unit.AuthorityOnUnitDespawn -= AuthorityHandleUnitDespawn;

        Building.AuthorityOnBuildingSpawn -= AuthorityHandleBuildingSpawn;
        Building.AuthorityOnBuildingDespawn -= AuthorityHandleBuildingDespawn;
    }

    private void AuthorityHandleUnitSpawn(Unit unit)
    {
        myUnits.Add(unit);
    }

    private void AuthorityHandleUnitDespawn(Unit unit)
    {
        myUnits.Remove(unit);
    }

    private void AuthorityHandleBuildingSpawn(Building building)
    {
        myBuildings.Add(building);
    }

    private void AuthorityHandleBuildingDespawn(Building building)
    {
        myBuildings.Remove(building);
    }

    #endregion
}
