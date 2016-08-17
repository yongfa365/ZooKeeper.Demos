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
    public class Locker : IWatcher
    {
        //创建的节点名
        public string OurPath;
        //父节点路径
        public string Parent;

        public string ConnectionString;

        private TimeSpan sessionTimeout = new TimeSpan(0, 0, 30);
        private ZooKeeper zk;
        public void CreateConnection()
        {
            try
            {
                zk = new ZooKeeper(ConnectionString, sessionTimeout, null);
                zk.Register(new Locker());
                if (zk.Exists(Parent, false) == null)
                {
                    zk.Create(Parent, "".GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
                }
            }
            catch (Exception ex)
            {
            }
        }

        //访问Locker
        public virtual void Invisit()
        {
        }

        /// <summary>
        /// 等待次小节点的删除
        /// </summary>
        /// <param name="lower">比自己的次小节点</param>
        public void WaitForLock(string lower)
        {
            Stat stat = zk.Exists(Parent + "/" + lower, true);
            if (stat == null)
            {
                IsGetLock();
            }
        }

        public void Process(WatchedEvent @event)
        {
            if (@event.Type == EventType.NodeDeleted)
            {
                IsGetLock();
            }
        }

        public bool IsGetLock()
        {
            if(zk==null)
            {
                CreateConnection();
            }
            if (zk.Exists($"{Parent}/{OurPath}", false) == null)
            {
                OurPath = zk.Create($"{Parent}/{OurPath}", "".GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.EphemeralSequential);
            }
            var nodes = zk.GetChildren(Parent, false).ToList();
            nodes.Sort();
            if (OurPath.Equals(Parent + "/" + nodes[0]))
            {
                Invisit();
                UnLock();
                return true;
            }
            else
            {
                var myNode = OurPath.Substring(OurPath.LastIndexOf('/') + 1);
                var lower = string.Empty;
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i].Equals(myNode))
                    {
                        lower = nodes[i - 1];
                    }
                }
                WaitForLock(lower);
                return false;
            }
        }

        /// <summary>
        /// 释放锁
        /// </summary>
        public void UnLock()
        {
           zk.Delete(OurPath, -1);
           zk.Dispose();
        }
    }

}
