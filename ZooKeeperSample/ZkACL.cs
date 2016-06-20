using Org.Apache.Zookeeper.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace ZooKeeperSample
{
    public class ZkACL
    {
        private static ZooKeeper zk = new ZooKeeper("192.168.119.131:2181,192.168.119.132:2181", new TimeSpan(0, 0, 30), null);

        public static void GetACL()
        {
            Stat stat = new Stat();
            var data = zk.GetACL("/test", stat); // 第二个参数不允许为空，为空时会报错
            // ip 和 world 不需要也不能添加 AddAuthInfo 
            zk.AddAuthInfo("digest", "user1:12345".GetBytes());
            var data1 = System.Text.Encoding.Default.GetString(zk.GetData("/test", false, stat));
        }

        public static void SetAuthACL()
        {
            //auth
            zk.AddAuthInfo("digest", "user1:12345".GetBytes());
            var aclList = new List<ACL>
            {
                new ACL { Perms = 31, Id = new ZKId { Id = "", Scheme = "auth" } },
            };
            zk.SetACL("/test", aclList, -1);
        }
        public static void SetOtherACL()
        {
            var aclList = new List<ACL>
            {
                //world
                new ACL { Perms = Perms.ALL, Id = new ZKId { Id = "anyone", Scheme = "world" } },
                //digest
                //密码必须是密文,先使用SHA1加密，然后base64编码
                new ACL { Perms = Perms.READ, Id = new ZKId { Id = "user1:+owfoSBn/am19roBPzR1/MfCblE=", Scheme = "digest" } },
                //ip
                new ACL { Perms = Perms.ALL, Id = new ZKId { Id = "172.18.23.56", Scheme = "ip" } },
                //以172.18开头的ip地址
                new ACL { Perms = Perms.ALL, Id = new ZKId { Id = "172.18.0.0/16", Scheme = "ip" } },
            };
            //创建节点时指定ACL
            zk.Create("/test/app", "hy".GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
            //单独设置ACL
            zk.SetACL("/test", aclList, -1);
        }
}
}
