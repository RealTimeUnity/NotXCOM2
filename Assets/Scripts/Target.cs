using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Target
{
    public enum TargetType { None, Location, Enemy, Friendly, Self };

    private TargetType type = TargetType.None;
    private Character character = null;
    private Vector3 location = Vector3.zero;

    public Target()
    {

    }

    public Target(TargetType type, Character character)
    {
        this.type = type;
        this.character = character;
    }

    public Target(Vector3 location)
    {
        this.type = TargetType.Location;
        this.location = location;
    }

    public TargetType GetTargetType()
    {
        return this.type;
    }

    public void SetTargetType(TargetType type)
    {
        this.type = type;
    }

    public Character GetCharacterTarget()
    {
        if (this.type == TargetType.Enemy ||
            this.type == TargetType.Friendly ||
            this.type == TargetType.Self)
        {
            return this.character;
        }

        return null;
    }

    public void SetCharacterTarget(Character character)
    {
        if (this.type == TargetType.Enemy ||
            this.type == TargetType.Friendly ||
            this.type == TargetType.Self)
        {
            this.character = character;
        }
    }

    public Vector3 GetLocationTarget()
    {
        if (this.type == TargetType.Location)
        {
            return this.location;
        }

        return Vector3.zero;
    }

    public void SetLocationTarget(Vector3 location)
    {
        if (this.type == TargetType.Location)
        {
            this.location = location;
        }
    }
}
