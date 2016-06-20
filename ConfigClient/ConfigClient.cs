using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace ConfigClient
{
    //配置管理
    //客户端wacth节点，发生变化时收到通知更新数据
    public class Watch
    {
        public static ZooKeeper zk = new ZooKeeper("192.168.119.131:2181,192.168.119.132:2181", new TimeSpan(0, 30, 30), null);

        public static string Configuration = System.Text.Encoding.Default.GetString(zk.GetData("/configuration", false, null));

        public void watch()
        {
            zk.Register(new Watcher(zk));
            System.Text.Encoding.Default.GetString(zk.GetData("/configuration", true, null));
        }

        public void GetConfNew()
        {
            Console.WriteLine(Configuration);
        }
    }

    public class Watcher : IWatcher
    {
        private ZooKeeper zk = null;

        public Watcher(ZooKeeper zookeeper)
        {
            zk = zookeeper;
        }

        public void Process(WatchedEvent @event)
        {
            // 重新 watch 并赋值
            Watch.Configuration = System.Text.Encoding.Default.GetString(zk.GetData(@event.Path, true, null));
        }
    }

    public class ConfigClient
    {
        
        static void Main(string[] args)
        {
            new Watch().watch();
            Console.WriteLine(Watch.Configuration);
            //在ConfigManager修改节点数据
            Thread.Sleep(5000);
            new Watch().GetConfNew();
            //在ConfigManager修改节点数据
            Thread.Sleep(5000);
            new Watch().GetConfNew();
            Thread.Sleep(5000);
        }
    }
}
