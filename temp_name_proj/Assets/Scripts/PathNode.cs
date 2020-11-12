using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode //Класс одной клетки\ноды в сетке для алгоритма поиска пути
{
    private Grid<PathNode> grid; //ссылка на сетку, которой принадлежит нода
    //Координаты ноды в сетке
    public int x;
    public int y;
    //Переменные для расчёте стоимости пути
    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkable;

    public PathNode cameFromNode; //Из какой ноды пришли в эту(для, собственно, построения пути)
    public PathNode(Grid<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true;
    }

    public void SetIsWalkable(bool condiiton)
    {
        this.isWalkable = condiiton;
        grid.TriggerGridObjectChanged(x,y);
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
    public override string ToString()
    {
        return x + "," + y + "," + (isWalkable ? "T" : "F");
    }

}
