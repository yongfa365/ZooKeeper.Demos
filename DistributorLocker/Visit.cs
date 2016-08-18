using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace DistributorLocker
{
    public class Program
    {
        public static void Main(string[] args)
        {            
            var connectionString = "192.168.119.131:2181";
            var sessionTimeout = new TimeSpan(0, 0, 30);
            var ourPath = "/locker/seq";
            using (var zk = new ZooKeeper(connectionString, sessionTimeout, null))
            {
                using (var locker = new Locker(zk, ourPath))
                {
                    locker.GetLock();
                    if(!Locker.HasGetLock)
                    {
                        Locker.monitor.WaitOne();
                    }
                    if(Locker.HasGetLock)
                    {
                        //访问资源
                        Thread.Sleep(5000);
                    }
                }
            }   
            Console.ReadLine();
        }
    }
}
