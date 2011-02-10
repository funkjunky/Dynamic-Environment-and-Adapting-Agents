using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BayesNetwork.Classes;
using Multimap;
using Extensions;
using System.Windows.Forms;
using System.Design;

namespace Project
{
    public class Program
    {
        public static BayesForm bform;
        static void Main(string[] args)
        {
            //testNetAsGraph();
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            bform = new BayesForm();
        }

        public static void testMultimap()
        {
            Multimap<string, int> testMap = new Multimap<string, int>();
            testMap.Add("even", 2);
            testMap.Add("even", 4);
            testMap.Add("even", 6);
            testMap.Add("prime", 2);
            testMap.Add("prime", 3);

            Console.WriteLine(testMap.ToString());
            Console.ReadKey();
        }

        //testing out creating a BayesNet... isn't that easy? =)
        public static void testNetAsGraph()
        {
            BayesNet bnet = new BayesNet();
            /*bnet.addNode("cloudy", 0.5);
            bnet.addNode("raining", 0.2);
            bnet.addNode("cold", 0.33);
            bnet.addNode("snowing", 0.2);
            bnet.addNode("hailing", 0.1);

            bnet.addEdge("cloudy", "raining", 1.4, 0.2);
            //test inference here...
            bnet.addEdge("cloudy", "snowing", 1.2, 0.2);
            bnet.addEdge("cold", "snowing", 1.6, 0.2);
            bnet.addEdge("raining", "snowing", 0.4, 1.2);
            bnet.addEdge("raining", "hailing", 1.5);
            bnet.addEdge("snowing", "hailing", 1.5);
            //test inference here...
            //with just these influences we should be able to get chance of cloudy if it's snowing or raining.

            Console.WriteLine("BayesNet test:\n");
            Console.WriteLine("Nodes:\n"+bnet.Nodes.toString()); //ya sorry, can't override using method extensions >=(
            Console.WriteLine("Edges:\n" + bnet.Edges.ToString());
            Console.ReadKey();

            // Now lets keep testing things
            // lets do some randomness

            double P = bnet.infer("cloudy");
            double next = new double();
            Random randnum = new Random(System.DateTime.Now.Millisecond);
            next = randnum.NextDouble();
            if (P < next)
                bnet["cloudy"].ObservedValue = false;
            else
                bnet["cloudy"].ObservedValue = true;

            P = bnet.infer("cold");
            next = randnum.NextDouble();
            if (P < next)
                bnet["cold"].ObservedValue = false;
            else
                bnet["cold"].ObservedValue = true;

            Console.WriteLine("BayesNet test:\n");
            Console.WriteLine("Nodes:\n" + bnet.Nodes.toString()); //ya sorry, can't override using method extensions >=(
            Console.WriteLine("Edges:\n" + bnet.Edges.ToString());
            Console.WriteLine("Chances of it raining:" + bnet.infer("raining"));
            Console.WriteLine("Chances of it snowing:" + bnet.infer("snowing"));
            Console.WriteLine("Chances of it hailing:" + bnet.infer("hailing"));
            Console.ReadKey();

            // now lets see if its raining
            P = bnet.infer("raining");
            next = randnum.NextDouble();
            if (P < next)
                bnet["raining"].ObservedValue = false;
            else
                bnet["raining"].ObservedValue = true;

            Console.WriteLine("BayesNet test:\n");
            Console.WriteLine("Nodes:\n" + bnet.Nodes.toString()); //ya sorry, can't override using method extensions >=(
            Console.WriteLine("Edges:\n" + bnet.Edges.ToString());
            Console.WriteLine("Chances of it snowing:" + bnet.infer("snowing"));
            Console.WriteLine("Chances of it hailing:" + bnet.infer("hailing"));
            Console.ReadKey();

            // now we check if its snowing
            P = bnet.infer("snowing");
            next = randnum.NextDouble();
            if (P < next)
                bnet["snowing"].ObservedValue = false;
            else
                bnet["snowing"].ObservedValue = true;

            Console.WriteLine("BayesNet test:\n");
            Console.WriteLine("Nodes:\n" + bnet.Nodes.toString()); //ya sorry, can't override using method extensions >=(
            Console.WriteLine("Edges:\n" + bnet.Edges.ToString());
            Console.WriteLine("Chances of it hailing:" + bnet.infer("hailing"));
            Console.ReadKey();

            // and lastly we check if its hailing
            P = bnet.infer("hailing");
            next = randnum.NextDouble();
            if (P < next)
                bnet["hailing"].ObservedValue = false;
            else
                bnet["hailing"].ObservedValue = true;

            Console.WriteLine("BayesNet test:\n");
            Console.WriteLine("Nodes:\n" + bnet.Nodes.toString()); //ya sorry, can't override using method extensions >=(
            Console.WriteLine("Edges:\n" + bnet.Edges.ToString());
            Console.ReadKey();*/

        }

        public static void Run()
        {
        }
    }
}
