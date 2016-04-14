﻿using O2DESNet.Warehouse.Statics;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

            var experiment = new ExperimentFramework();
            experiment.ExperimentRunAllStrategies("ZA_Orders.csv");

            //ExperimentSelectStrategy();

            Console.WriteLine("\n:: Experiment End ::");
            Console.WriteLine("\nEnter any key to exit...");
            Console.ReadKey();
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