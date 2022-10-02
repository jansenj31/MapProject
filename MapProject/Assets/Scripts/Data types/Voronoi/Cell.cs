using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jonas.Geometry
{
    public class Cell
    {
        Site site;
        List<Border> borderList;
        List<Cell> neighbours;
    }
}