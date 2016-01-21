﻿using O2DESNet.Warehouse.Statics;
using System;
using System.Linq;

namespace O2DESNet.Warehouse
{
    class Program
    {
        static void Main(string[] args)
        {
            //WarehouseSim whsim_base = new WarehouseSim("ZA");
            //var byteArray = Serializer.ObjectToByteArray(whsim_base);
            //WarehouseSim whsim = (WarehouseSim) Serializer.ByteArrayToObject(byteArray);
            //whsim.GeneratePicklist(strategy);

            //ExperimentRunAllStrategies();

            ExperimentSelectStrategy();

            Console.WriteLine("\n:: Experiment End ::");
            Console.ReadKey();
        }

        private static void ExperimentRunAllStrategies()
        {
            WarehouseSim whsim = null;

            foreach (PicklistGenerator.Strategy strategy in Enum.GetValues(typeof(PicklistGenerator.Strategy)))
            {
                whsim = new WarehouseSim("ZA", strategy);
                whsim.Run(24);
                whsim.PrintStatistics();
                whsim = null;
            }
        }

        private static void ExperimentSelectStrategy()
        {
            WarehouseSim whsim;

            Console.Write("Strategy to implement? (A/B/C/D) ");
            var strat = Console.ReadLine();

            if (strat[0] == 'A' || strat[0] == 'a') whsim = new WarehouseSim("ZA", PicklistGenerator.Strategy.A);
            else if (strat[0] == 'B' || strat[0] == 'b') whsim = new WarehouseSim("ZA", PicklistGenerator.Strategy.B);
            else if (strat[0] == 'C' || strat[0] == 'c') whsim = new WarehouseSim("ZA", PicklistGenerator.Strategy.C);
            else if (strat[0] == 'D' || strat[0] == 'd') whsim = new WarehouseSim("ZA", PicklistGenerator.Strategy.D);
            else throw new Exception("No such strategy!");

            whsim.Run(24);
            whsim.PrintStatistics();
        }

        /// <summary>
        /// For debugging purposes
        /// </summary>
        /// <param name="pm"></param>

        static void DisplayRouteTable(Scenario pm)
        {
            foreach (var cp in pm.ControlPoints)
            {
                Console.WriteLine("Route Table at CP_{0}:", cp.Id);
                foreach (var item in cp.RoutingTable)
                    Console.WriteLine("{0}:{1}", item.Key.Id, item.Value.Id);
                Console.WriteLine();
            }
        }

        static void DisplayPathingTable(Scenario pm)
        {
            foreach (var cp in pm.ControlPoints)
            {
                Console.WriteLine("Pathing Table at CP_{0}:", cp.Id);
                foreach (var item in cp.PathingTable)
                    Console.WriteLine("{0}:{1}", item.Key.Id, item.Value.Id);
                Console.WriteLine();
            }
        }
    }
}
