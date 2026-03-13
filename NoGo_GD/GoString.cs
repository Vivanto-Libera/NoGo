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
            return Liberties.Contains(point);
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
    }
}
