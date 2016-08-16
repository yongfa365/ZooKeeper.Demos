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
    public class Watch : IWatcher
    {
        public static ZooKeeper zk = new ZooKeeper("192.168.119.131:2181", new TimeSpan(0, 0, 30), null);

        public static string Configuration = string.Empty;

        public void Process(WatchedEvent @event)
        {
            // 重新 watch 并赋值
            Configuration = System.Text.Encoding.Default.GetString(zk.GetData(@event.Path, true, null));
        }

        public void watch(string path)
        {
            Configuration = System.Text.Encoding.Default.GetString(zk.GetData(path, false, null));
            zk.Register(new Watch());
            System.Text.Encoding.Default.GetString(zk.GetData(path, true, null));
        }
    }


    public class ConfigClient
    {
        static void Main(string[] args)
        {
            new Watch().watch("/configuration");
            Console.WriteLine(Watch.Configuration);
            //在ConfigManager修改节点数据
            Thread.Sleep(5000);
            Console.WriteLine(Watch.Configuration);
            //在ConfigManager修改节点数据
            Thread.Sleep(5000);
            Console.WriteLine(Watch.Configuration);
            Thread.Sleep(5000);
        }
    }
}
