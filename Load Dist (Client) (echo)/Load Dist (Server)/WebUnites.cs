using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Load_Dist__Server_
{
    internal class WebUnites
    {
        private Random rnd = new Random(DateTime.Now.Millisecond);
        public WebUnites()
        {
            ID = 0;
            Level = 0;
            ActuallyPower = 0;
            MaxPower = rnd.Next(5,10);
            Depth = rnd.Next(2,4);
            ChildUnits = Create(1);
            ID = 0;
        }
        public WebUnites(int id, int level, int depth)
        {
            ID = id;
            Level = level;
            ActuallyPower = 0;
            MaxPower = rnd.Next(20, 50+5*level);
            Depth = depth;
        }
        public WebUnites (int id, int level, int actPower, int maxPower, int depth)
        {
            ID = id;
            Level = level;
            ChildUnits = null;
            ActuallyPower = actPower;
            MaxPower = maxPower;
            Depth = depth;
        }
        public int ID { get; set; }
        public int Level { get; set; }
        public List<WebUnites> ChildUnits { get; set; } = null;
        public int ActuallyPower { get; set; }
        public int MaxPower { get; set; }
        public int Depth { get; set; }
        private List<WebUnites> Create(int depth)
        {
            if (depth <= Depth)
            {
                List<WebUnites> result = new List<WebUnites>(rnd.Next(1, 3));
                for (int i = 0; i < result.Capacity; i++)
                {
                    ID++;
                    result.Add(new WebUnites(ID, depth, Depth));
                    result[i].ChildUnits = Create(depth + 1);
                }
                return result;
            }
            else return null;
        }
        public WebUnites FindIndex (WebUnites root, int id, int diff)
        {
            WebUnites result;
            if (id == root.ID)
            {
                result = new WebUnites(root.ID, root.Level, root.ActuallyPower + diff, root.MaxPower, root.Depth);
                result.ChildUnits = root.ChildUnits;
            }
            else
            {
                result = new WebUnites(root.ID, root.Level, root.ActuallyPower, root.MaxPower, root.Depth);
                result.ChildUnits = FindInd(root.ChildUnits, id, diff);
            }
            return result;
        }
        private List<WebUnites> FindInd(List<WebUnites> rootChild, int id, int diff)
        {
            //WebUnites result = new WebUnites(root.ID, root.Level, root.ActuallyPower, root.MaxPower, root.Depth);
            List<WebUnites> rslt = new List<WebUnites>(rootChild.Capacity);
            for (int i = 0; i < rslt.Capacity; i++)
            {
                if (rootChild[i].ID == id)
                    rslt.Add(new WebUnites(rootChild[i].ID, rootChild[i].Level, rootChild[i].ActuallyPower+diff, rootChild[i].MaxPower, rootChild[i].Depth));
                else rslt.Add(new WebUnites(rootChild[i].ID, rootChild[i].Level, rootChild[i].ActuallyPower, rootChild[i].MaxPower, rootChild[i].Depth));
                if (rootChild[i].ChildUnits != null) rslt[i].ChildUnits = FindInd(rootChild[i].ChildUnits, id, diff);
            }
            return rslt;
        }
        public WebUnites DeleteLoad(WebUnites root, int id, int diff)
        {
            WebUnites result;
            if (id == root.ID)
            {
                result = new WebUnites(root.ID, root.Level, root.ActuallyPower - diff, root.MaxPower, root.Depth);
                result.ChildUnits = root.ChildUnits;
            }
            else
            {
                result = new WebUnites(root.ID, root.Level, root.ActuallyPower, root.MaxPower, root.Depth);
                result.ChildUnits = DelLoad(root.ChildUnits, id, diff);
            }
            return result;
        }
        private List<WebUnites> DelLoad(List<WebUnites> rootChild, int id, int diff)
        {
            //WebUnites result = new WebUnites(root.ID, root.Level, root.ActuallyPower, root.MaxPower, root.Depth);
            List<WebUnites> rslt = new List<WebUnites>(rootChild.Capacity);
            for (int i = 0; i < rslt.Capacity; i++)
            {
                if (rootChild[i].ID == id)
                    rslt.Add(new WebUnites(rootChild[i].ID, rootChild[i].Level, rootChild[i].ActuallyPower - diff, rootChild[i].MaxPower, rootChild[i].Depth));
                else rslt.Add(new WebUnites(rootChild[i].ID, rootChild[i].Level, rootChild[i].ActuallyPower, rootChild[i].MaxPower, rootChild[i].Depth));
                if (rootChild[i].ChildUnits != null) rslt[i].ChildUnits = DelLoad(rootChild[i].ChildUnits, id, diff);
            }
            return rslt;
        }
        public List<(int,int,int,int)> TreeToList(WebUnites root)
        {
            List<(int, int, int, int)> result = new List<(int, int, int, int)>();
            foreach (var child in root.ChildUnits)
            {
                result.Add((child.ID,child.Level,child.ActuallyPower,child.MaxPower));
                if (child.ChildUnits != null)
                    result.AddRange(TreeToList(child));
            }
            return result;
        }
    }
}
