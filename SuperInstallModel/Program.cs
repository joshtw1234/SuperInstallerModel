﻿using System;
using System.IO;

namespace SuperInstallModel
{
    class Program
    {
        static void Main(string[] args)
        {
            string revMsg = "Initial Success";
            Model.SuperInstallModel spModel = new Model.SuperInstallModel();
            if (spModel.Initialize())
            {
                spModel.SetStartInstall();
            }
            //Console.WriteLine(revMsg);
            Console.ReadLine();
        }
    }
}
