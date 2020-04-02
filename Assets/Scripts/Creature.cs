using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GoalType
{
    Food,
    Water,
    Fight
}

public enum DirectionType
{
    Top = 0,
    TopRight,
    Right,
    BottomRight,
    Bottom,
    BottomLeft,
    Left,
    TopLeft
}

public class Creature
{
    public int Id;

    private int X;
    private int Y;

    public float Health = 1;

    public float Hunger = 0;
    public float Thirst = 0;

    public int VisionRange = 10;

    public  GoalType GoalType;

    public float Intelligence = UnityEngine.Random.value;

    public GameObject Skin;

    public GameObject[] Vision = new GameObject[1];

    public GameObject Goal;
    public GameObject Path;

    public Creature(Vector2 Coords)
    {
        SetX((int)Coords.x);
        SetY((int)Coords.y);
    }

    public Creature(int x, int y)
    {
        SetX(x);
        SetY(y);
    }

    public void SetX(int value)
    {
        X = value;
    }

    public void SetY(int value)
    {
        Y = value;
    }

    public int GetX()
    {
        return X;
    }

    public int GetY()
    {
        return Y;
    }

    public DirectionType ChooseDirection()
    {
        return (DirectionType) UnityEngine.Random.Range(0, 8);
    }

    public void SetGoal()
    {
        if (Hunger < 0.0f && Thirst < 0.0f)
            GoalType = GoalType.Fight;
        else if (Hunger > Thirst)
            GoalType = GoalType.Food;
        else
            GoalType = GoalType.Water;
    }
}