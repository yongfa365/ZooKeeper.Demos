using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZooKeeperNet;
using System.Threading;
using Org.Apache.Zookeeper.Data;

namespace Locker
{
    //共享锁的实现
    public class LockerCommon : IWatcher
    {
        private static ZooKeeper zk = new ZooKeeper("192.168.119.131:2181", new TimeSpan(0, 0, 30), new LockerCommon());

        public static string root = "/locker";

        public static string ourPath = "";

        public static void GetLock()
        {
            //创建顺序节点
            ourPath = zk.Create($"{root}/seq", "".GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.EphemeralSequential);
            //获取子节点并排序
            List<String> nodes = zk.GetChildren(root, false).ToList();
            nodes.Sort();
            foreach(var item in nodes)
            {
                Console.WriteLine(item);
            }
            //比较排序的第一个节点和自己
            if (ourPath.Equals(root+"/"+nodes[0]))
            {
                var t = DateTime.Now;
                Console.WriteLine($"{ourPath}访问资源,时间为{DateTime.Now}");
                //访问共享资源
                Thread.Sleep(5000);
                //释放锁
                Console.WriteLine($"访问结束{(DateTime.Now - t).TotalMilliseconds}");
                zk.Delete(ourPath, -1);
                Console.WriteLine($"{ourPath}已被删除,时间为{DateTime.Now}");
                Console.ReadLine();
            }
            else
            {
                //等待比自己次小的节点的删除事件
                var myNode = ourPath.Substring(ourPath.LastIndexOf('/') + 1);
                var lower = string.Empty;
                for(int i=0; i<nodes.Count; i++)
                {
                    if (nodes[i].Equals(myNode))
                    {
                        lower = nodes[i - 1];
                    }
                }
                waitForLock(lower,5000);
            }
        }
        public static void waitForLock(String lower,int waitTime)
        {
            //监听比自己次小的节点
            Stat stat = zk.Exists(root + "/" + lower, true);
            if (stat != null)
            {
                Console.WriteLine($"Waiting for {lower}");
                Thread.Sleep(waitTime);
            }
            else
            {
                var t = DateTime.Now;
                Console.WriteLine($"{ourPath}访问资源,时间为{DateTime.Now}");
                //访问共享资源
                Thread.Sleep(5000);
                //释放锁
                Console.WriteLine($"访问结束{(DateTime.Now - t).TotalMilliseconds}");
                try
                {
                    zk.Delete(ourPath, -1);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                Console.WriteLine($"{ourPath}已被删除,时间为{DateTime.Now}");
                Console.ReadLine();
            }
        }

        public void Process(WatchedEvent @event)
        {
            if (@event.Type == EventType.NodeDeleted)
            {
                var t = DateTime.Now;
                //访问共享资源
                Console.WriteLine($"监听到删除事件{@event.Path}");
                Console.WriteLine($"访问资源,时间为{DateTime.Now}");
                //访问共享资源
                Thread.Sleep(5000);
                //释放锁
                Console.WriteLine($"访问结束{(DateTime.Now - t).TotalMilliseconds}");
                zk.Delete(ourPath, -1);
                Console.WriteLine($"{ourPath}已被删除,时间为{DateTime.Now}");
                Console.ReadLine();
            }
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
           LockerCommon.GetLock();
           Console.ReadLine();
        }
    }
}
