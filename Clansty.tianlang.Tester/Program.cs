﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clansty.tianlang.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Cmds.gcmds["help"].Func);

            Console.ReadLine();
        }
    }
}