using Org.Apache.Zookeeper.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace ZooKeeperSample
{
    public class ZkWatch
    {
       // public static ZooKeeper zk = new ZooKeeper("192.168.119.131:2181,192.168.119.132:2181", new TimeSpan(0, 0, 30), new WatchCommon2());

        public static ZooKeeper zk = new ZooKeeper("192.168.119.131:2181,192.168.119.132:2181", new TimeSpan(0, 0, 30), null);

        public static void Watch()
        {
            // 这里注册的 watch 实例会覆盖 new Zookeeper() 时指定的 watch 实例
            // 如果注册多个，则会把最后注册的覆盖之前注册的
            zk.Register(new WatchCommon(zk));
            zk.Register(new WatchCommon3(zk));

            // 3 种添加 watch 的方式 
            Stat stat = new Stat();
            // 1. 这种方式增加 watch ,只能watch 此节点的数据更新和删除事件
            zk.GetData("/test/t1", true, stat); // /test/t1 要事先存在 
            zk.GetData("/test/t2", new WatchCommon(zk), stat); // 单独指定实例，不使用 注册时的实例   /test/t2 要事先存在 

            // 2. 这种方式增加 watch ,可以watch 此节点的数据更新和删除事件，以及节点创建事件
            zk.Exists("/test/t3", true);   //  /test/t3 可事先不存在 

            // 3. 这种方式增加的 watch, 可以 watch 此节点的删除事件，以及子节点的创建和删除事件
            zk.GetChildren("/test/t3", true); //  /test/t3 要事先存在 

            /*
             上面 语句 zk.Exists("/test/t3", true);  和 zk.GetChildren("/test/t3", true); watch 中都包含 删除事件，
             那如果把 /test/t3 删除，是否会收到两次通知呢？
             测试结果是只收到一次通知，因为 watch 实例是一样的，
             如果 后面的语句改成 zk.GetChildren("/test/t3", new WatchCommon2()); 即单独的 watch 实例，
             则把 /test/t3 删除，每个实例都会收到通知
             */

            /* 注意点：
             1.watch是一次性触发的，如果获取一个watch事件并希望得到新变化的通知，需要重新设置watch
             2.watch是一次性触发的并且在获取watch事件和设置新watch事件之间有延迟，所以不能可靠的观察到节点的每一次变化。要认识到这一点。
             */

            var childWatchList = zk.ChildWatches;
            var dataWatchList = zk.DataWatches;
            var existWatchList = zk.ExistWatches;
        }
    }
    public class WatchCommon : IWatcher
    {
        private ZooKeeper zk = null;

        public WatchCommon(ZooKeeper zookeeper)
        {
            zk = zookeeper;
        }

        public void Process(WatchedEvent @event)
        {
            // 全部重新 watch 
            zk.GetData(@event.Path, this, null);
            zk.Exists(@event.Path, this);
            zk.GetChildren(@event.Path, this);

            Console.WriteLine("receive watch notify:");
            Console.WriteLine("  path is : {0}", @event.Path);
            Console.WriteLine("  state is : {0}", @event.State);
            Console.WriteLine("  type is : {0}", @event.Type);
        }
    }

    public class WatchCommon2 : IWatcher
    {
        public void Process(WatchedEvent @event)
        {
            Console.WriteLine("22receive watch notify:");
            Console.WriteLine("  path is : {0}", @event.Path);
            Console.WriteLine("  state is : {0}", @event.State);
            Console.WriteLine("  type is : {0}", @event.Type);
        }
    }

    public class WatchCommon3 : IWatcher
    {
        private ZooKeeper zk = null;

        public WatchCommon3(ZooKeeper zookeeper)
        {
            zk = zookeeper;
        }

        public void Process(WatchedEvent @event)
        {
            // 全部重新 watch 
            zk.GetData(@event.Path, this, null);
            zk.Exists(@event.Path, this);
            zk.GetChildren(@event.Path, this);

            Console.WriteLine("33receive watch notify:");
            Console.WriteLine("  path is : {0}", @event.Path);
            Console.WriteLine("  state is : {0}", @event.State);
            Console.WriteLine("  type is : {0}", @event.Type);
        }
    }
}
