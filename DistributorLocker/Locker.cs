using Org.Apache.Zookeeper.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace DistributorLocker
{
    public class Locker : IWatcher,IDisposable
    {
        //创建的节点名
        private string OurPath;

        private ZooKeeper ZK;

        public static bool HasGetLock;

        public static AutoResetEvent monitor = new AutoResetEvent(false);

        public Locker(ZooKeeper zk,string ourPath)
        {
            ZK = zk;
            OurPath = ourPath;
            var parent = OurPath.Substring(0, OurPath.LastIndexOf('/'));
            if (ZK.Exists(parent, false) == null)
            {
                ZK.Create(parent, "".GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
            }
            if (ZK.Exists(OurPath, false) == null)
            {
                OurPath = ZK.Create(OurPath, "".GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.EphemeralSequential);
            }
        }

        public void WaitForLock(string lower)
        {
            Stat stat = ZK.Exists(lower, true);
            //watch lower之前,lower已經被刪了
            if (stat == null)
            {
                GetLock();
            }
        }

        public void Process(WatchedEvent @event)
        {
            if (@event.Type == EventType.NodeDeleted)
            {
                GetLock();
            }
        }

        public void GetLock()
        {
            var parent = OurPath.Substring(0, OurPath.LastIndexOf('/'));
            var nodes = ZK.GetChildren(parent, false).ToList();
            nodes.Sort();
            if (OurPath.Equals(parent + "/" + nodes[0]))
            {
                HasGetLock = true;
                monitor.Set();
            }
            else
            {
                //注册watch
                if (ZK.Exists(OurPath, false) != null)
                {
                    ZK.Register(new Locker(ZK, OurPath));
                }
                var myNode = OurPath.Substring(OurPath.LastIndexOf('/') + 1);
                var lower = string.Empty;
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i].Equals(myNode))
                    {
                        lower = nodes[i - 1];
                    }
                }
                WaitForLock(parent+"/"+lower);
                HasGetLock = false;
            }
        }

        public void Dispose()
        {
            ZK.Delete(OurPath, -1);
        }
    }
}
