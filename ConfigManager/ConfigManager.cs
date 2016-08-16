using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace ConfigManager
{
    public class ConfigManager
    {
        public ZooKeeper zk = new ZooKeeper("192.168.119.131:2181,192.168.119.132:2181", new TimeSpan(0, 30, 30), null);

        public void CreateNode()
        {
            var stat = zk.Exists("/configuration", false);
            if (stat == null)
            {
                try
                {
                    // 更具体的权限控制
                    //var aclList = new List<ACL> { new ACL { Perms = 31, Id = Ids.ANYONE_ID_UNSAFE } };

                    var data = zk.Create("/configuration", Encoding.UTF8.GetBytes("mydata"), Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
                }
                catch (KeeperException.NodeExistsException ex)
                {
                    // 节点已经存在
                    var exmsg = ex.Message;
                }
                catch (KeeperException.NoNodeException ex)
                {
                    // 创建节点时，其父节点必须要存在，否则会报错
                    throw new Exception("父节点不存在");
                }

                // 其它异常不处理
            }
        }

        public void SetData(string input)
        {
            try
            {
                zk.SetData("/configuration", Encoding.UTF8.GetBytes(input), -1);
            }
            catch (KeeperException.NoNodeException ex)
            {
                throw new Exception("节点不存在");
            }
        }
    }

    public class program
    {
        public static void Main(string[] args)
        {
            //CreateNode();

            var data = string.Empty;
            var manager = new ConfigManager();
            while (true)
            {
                data = Console.ReadLine();
                manager.SetData(data);
            }
        }
    }
}
