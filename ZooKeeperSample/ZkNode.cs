using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace ZooKeeperSample
{

    public class ZkNode
    {
        public static ZooKeeper zk = new ZooKeeper("192.168.119.131:2181,192.168.119.132:2181", new TimeSpan(0, 30, 30), null);

        //Ephemeral节点，会话结束自动删除
        //Sequence节点，自动在节点名后加上编号
        public static void CreateEphemeralSequence()
        {
            for(var i=0;i<5;i++)
            {
                zk.Create("/sequence", $"hy{i}".GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.EphemeralSequential);
            }
            zk.Create("/sequence", $"hy".GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Ephemeral); ;
        }

    }
}
