using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperInstallModel
{
    class Program
    {
        static void Main(string[] args)
        {

            string revMsg = "Initial Success";
            Model.SuperInstallModel spModel = new Model.SuperInstallModel();
            if (!spModel.Initialize())
            {
                revMsg = "Initial Failed";
            }
            Console.WriteLine(revMsg);
            Console.ReadLine();
        }
    }
}
