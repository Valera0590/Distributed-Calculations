using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Load_Dist__Server_
{
    internal class WebUnites
    {
        private Random rnd = new Random(DateTime.Now.Millisecond);
        public WebUnites()
        {
            ID = 0;
            Level = 1;
            ActuallyPower = 0;
            MaxPower = rnd.Next(20, 55);
            Inc.Add(ID);
            //Depth = rnd.Next(2,4);
            List<WebUnites> Children = new List<WebUnites>();
            Children.Add(new WebUnites(3,1));
            ChildUnits = new List<WebUnites>(Children);
            Children.Clear();

            Children.Add(new WebUnites(2,1));
            ChildUnits[0].ChildUnits = new List<WebUnites>(Children);
            Children.Clear();

            Children.Add(this);
            Children.Add(new WebUnites(1,1));
            ChildUnits[0].ChildUnits[0].ChildUnits = new List<WebUnites> (Children);
            Children.Clear();
            Children.Add(this);
            Children.Add(ChildUnits[0]);
            ChildUnits[0].ChildUnits[0].ChildUnits[1].ChildUnits = new List<WebUnites>(Children);
            Children.Clear();
            Children = null;

        }
        public WebUnites(int id, int level)
        {
            ID = id;
            Level = level;
            ActuallyPower = 0;
            MaxPower = rnd.Next(20, 55);
            Inc.Add(id);
            NInc = null;
        }
        public WebUnites (int id, int level, int actPower, int maxPower)
        {
            ID = id;
            Level = level;
            ChildUnits = null;
            ActuallyPower = actPower;
            MaxPower = maxPower;
            Inc.Clear();
            Inc.Add(id);
            NInc = null;

        }
        public int ID { get; set; }
        public int Level { get; set; }
        public List<WebUnites> ChildUnits { get; set; } = null;
        public List<int> Inc { get; set; } = new List<int>();
        public List<int> NInc { get; set; } = new List<int>();
        public int ActuallyPower { get; set; }
        public int MaxPower { get; set; }
        public bool FlagEnd { get; set; } = false;
        private List<(int, int, int, int)> result { get; set; } = null;

        public WebUnites RestoreIncs(WebUnites root)
        {
            FlagEnd = false;
            WebUnites rootNew = new WebUnites(root.ID, root.Level, root.ActuallyPower, root.MaxPower);  // 0
            WebUnites temp = root.ChildUnits[0];
            List<WebUnites> Children = new List<WebUnites>();
            Children.Add(new WebUnites(temp.ID, temp.Level, temp.ActuallyPower, temp.MaxPower));  // 3
            rootNew.ChildUnits = new List <WebUnites> (Children);
            Children.Clear();
            temp = temp.ChildUnits[0];
            Children.Add(new WebUnites(temp.ID, temp.Level, temp.ActuallyPower, temp.MaxPower));  // 2
            rootNew.ChildUnits[0].ChildUnits = new List<WebUnites>(Children);
            Children.Clear();
            temp = temp.ChildUnits[1];
            Children.Add(rootNew);      // 0
            Children.Add(new WebUnites(temp.ID, temp.Level, temp.ActuallyPower, temp.MaxPower)); // 1
            rootNew.ChildUnits[0].ChildUnits[0].ChildUnits = new List<WebUnites>(Children);
            Children.Clear();
            temp = temp.ChildUnits[1];
            Children.Add(rootNew);
            Children.Add(new WebUnites(temp.ID, temp.Level, temp.ActuallyPower, temp.MaxPower));
            rootNew.ChildUnits[0].ChildUnits[0].ChildUnits[1].ChildUnits = new List<WebUnites>(Children);
            Children.Clear();
            Children = null;
            temp = null;
            return rootNew;
        }
        public WebUnites FindIndexFinn (WebUnites root, int id, int diff)
        {
            WebUnites result;
            bool[] Graph = new bool[4];
            for (int i = 0; i < 4; i++)
                Graph[i] = false;
            Graph[root.ID] = true;
            if (id == root.ID)
            {
                result = new WebUnites(root.ID, root.Level, root.ActuallyPower + diff, root.MaxPower);
                result.ChildUnits = root.ChildUnits;
            }
            else
            {
                result = new WebUnites(root.ID, root.Level, root.ActuallyPower, root.MaxPower);
                result.ChildUnits = FindIndFinn(root.ChildUnits, id, diff, Graph);
            }
            for (int i = 0; i < 4; i++)
                Graph[i] = false;
            return result;
        }
        private List<WebUnites> FindIndFinn(List<WebUnites> rootChild, int id, int diff, bool[] graph)
        {
            List<WebUnites> rslt = new List<WebUnites>(rootChild.Capacity);
            
            for (int i = 0; i < rslt.Capacity; i++)
            {
                if (graph[rootChild[i].ID] == true)     // если вершина уже рассматривалась
                {
                    rslt.Add(rootChild[i]);
                    continue;
                }
                graph[rootChild[i].ID] = true;
                if (rootChild[i].ID == id)
                    rslt.Add(new WebUnites(rootChild[i].ID, rootChild[i].Level, rootChild[i].ActuallyPower+diff, rootChild[i].MaxPower));
                else rslt.Add(new WebUnites(rootChild[i].ID, rootChild[i].Level, rootChild[i].ActuallyPower, rootChild[i].MaxPower));
                rslt[i].ChildUnits = FindIndFinn(rootChild[i].ChildUnits, id, diff, graph);
            }
            return rslt;
        }
        public WebUnites DeleteLoadFinn(WebUnites root, int id, int diff)
        {
            WebUnites result;
            bool[] Graph = new bool[4];
            for (int i = 0; i < 4; i++)
                Graph[i] = false;
            Graph[root.ID] = true;
            if (id == root.ID)
            {
                result = new WebUnites(root.ID, root.Level, root.ActuallyPower - diff, root.MaxPower);
                result.ChildUnits = root.ChildUnits;
            }
            else
            {
                result = new WebUnites(root.ID, root.Level, root.ActuallyPower, root.MaxPower);
                result.ChildUnits = DelLoadFinn(root.ChildUnits, id, diff, Graph);
            }
            for (int i = 0; i < 4; i++)
                Graph[i] = false;
            return result;
        }
        private List<WebUnites> DelLoadFinn(List<WebUnites> rootChild, int id, int diff, bool[] graph)
        {
            //WebUnites result = new WebUnites(root.ID, root.Level, root.ActuallyPower, root.MaxPower, root.Depth);
            List<WebUnites> rslt = new List<WebUnites>(rootChild.Capacity);
            for (int i = 0; i < rslt.Capacity; i++)
            {
                if (graph[rootChild[i].ID] == true)     // если вершина уже рассматривалась
                {
                    rslt.Add(rootChild[i]);
                    continue;
                }
                graph[rootChild[i].ID] = true;
                if (rootChild[i].ID == id)
                    rslt.Add(new WebUnites(rootChild[i].ID, rootChild[i].Level, rootChild[i].ActuallyPower - diff, rootChild[i].MaxPower));
                else rslt.Add(new WebUnites(rootChild[i].ID, rootChild[i].Level, rootChild[i].ActuallyPower, rootChild[i].MaxPower));
                rslt[i].ChildUnits = DelLoadFinn(rootChild[i].ChildUnits, id, diff, graph);
            }
            return rslt;
        }
        public List<(int, int, int, int)> TreeToListFinn(WebUnites root)
        {
            List<int> inc = new List<int>(4);
            List<int> ninc = new List<int>(4);
            result = new List<(int, int, int, int)>();
            result = TTLF(root, inc, ninc);
            List<(int, int, int, int)> res = new List<(int, int, int, int)> (result);
            result = null;
            return res;
        }
        private List<(int, int, int, int)> TTLF(WebUnites root, List<int> inc, List<int> ninc)
        {
            result = AddingToListUnic(result, root);
            if (root.NInc != null && root.NInc.Count == 4) FlagEnd = true;
            if (inc != null) root.Inc = Concatenation(root.Inc, inc);
            if (ninc != null) root.NInc = Concatenation(root.NInc, ninc);
            if (!FlagEnd)
            {
                Task[] tasks = new Task[root.ChildUnits.Count];
                for (int i = 0; i < root.ChildUnits.Count; i++)
                {
                    //int j = i;
                    WebUnites web = root.ChildUnits[i];
                    List<int> inclist = new List<int> (root.Inc);
                    List<int> nInclist = new List<int>(root.NInc);
                    if (root.Inc.Contains(root.ChildUnits[i].ID))
                        if (!root.NInc.Contains(root.ID))
                            root.NInc.Add(root.ID);
                    tasks[i] = Task.Run(() =>
                    {
                        TTLF(web, inclist, nInclist);
                    });
                    //tasks[i].Start();
                    Thread.Sleep(100);
                }
                Task.WaitAll(tasks);
                //Thread.Sleep(6000);
            }
            return result;
        }
        private List<(int, int, int, int)> AddingToListUnic(List<(int, int, int, int)> src, WebUnites child)
        {
            List<(int, int, int, int)> result = new List<(int, int, int, int)>();
            if (src.Count == 0) result.Add((child.ID, child.Level, child.ActuallyPower, child.MaxPower));
            for (int i = 0; i < src.Count; i++)
            {
                if (src[i].Item1 == child.ID)
                    if (src[i] == (child.ID, child.Level, child.ActuallyPower, child.MaxPower))
                        result.Add(src[i]);
                    else result.Add((child.ID, child.Level, child.ActuallyPower, child.MaxPower));
                else result.Add(src[i]);
            }
            return result;
        }
        private List<int> Concatenation(List<int> l1, List<int> l2)
        {
            List<int> result = new List<int>();
            if (l1 == null) result.AddRange(l2);
            else
            {
                result.AddRange(l1);
                for (int i = 0; i < l2.Count; i++)
                {
                    if (!result.Contains(l2[i])) result.Add(l2[i]);
                }
            }
            return result;
        }
        
    }
}
