using System;

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
            spModel.SetStartInstall();
            //Console.WriteLine(revMsg);
            Console.ReadLine();
        }
    }
}
