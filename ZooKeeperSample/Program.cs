using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZooKeeperSample
{
    class Program
    {
        static void Main(string[] args)
        {
            //ZkNode.CreateEphemeralSequence();
            ZkACL.GetACL();
            //ZkACL.SetOtherACL();
            //ZkACL.SetAuthACL();
        }
    }
}
