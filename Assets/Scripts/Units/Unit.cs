using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;

    public static event Action<Unit> ServerOnUnitSpawn;
    public static event Action<Unit> ServerOnUnitDespawn;

    public static event Action<Unit> AuthorityOnUnitSpawn;
    public static event Action<Unit> AuthorityOnUnitDespawn;

    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }

    public Targeter GetTargeter()
    {
        return targeter;
    }

    #region Server

    public override void OnStartServer()
    {
        ServerOnUnitSpawn?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnUnitDespawn?.Invoke(this);
    }

    #endregion

    #region Client

    public override void OnStartClient()
    {
        if (!isClientOnly || !hasAuthority) { return; }

        AuthorityOnUnitSpawn?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority) { return; }

        AuthorityOnUnitDespawn?.Invoke(this);
    }

    [Client]
    public void Select() 
    {
        if (!hasAuthority) { return; }

        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority) { return; }

        onDeselected?.Invoke();
    }

    #endregion
}
