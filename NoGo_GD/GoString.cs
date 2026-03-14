using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using static NoGo.StoneColor;
namespace NoGo
{
    public class GoString(StoneColor color, HashSet<Point> stones, HashSet<Point> liberties)
    {
        public StoneColor StringColor
        {
            get;
        } = color;
        public HashSet<Point> Stones
        {
            get;
        } = stones;
        public HashSet<Point> Liberties
        {
            get;
        } = liberties;
        public void RemoveLiberty(Point liberty) 
        {
            Liberties.Remove(liberty);
        }
        public bool HasPoint(Point point) 
        {
            return Stones.Contains(point);
        }
        public int CountLiberties() 
        {
            return Liberties.Count;
        }
        public GoString Merge(GoString goString) 
        {
            HashSet<Point> newStones = [.. Stones];
            newStones.UnionWith(goString.Stones);
            HashSet<Point> newLiberties = [.. Liberties];
            newLiberties.UnionWith(goString.Liberties);
            foreach (Point p in newStones)
            {
                newLiberties.Remove(p);
            }
            return new GoString(StringColor, newStones, newLiberties);
        }
        public GoString Clone() 
        {
            return new GoString(StringColor, [.. Stones], [.. Liberties]);
        }
        public override bool Equals(object obj)
        {
            if (obj is not GoString other) 
            {
                return false;
            }
            return StringColor == other.StringColor && Stones.SetEquals(other.Stones) && Liberties.SetEquals(other.Liberties);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(StringColor, Stones, Liberties);
        }
    }
}
