using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BayesNetwork.Classes;
using Project.Classes;
using Multimap;
using Extensions;
using System.Windows.Forms;
using System.Design;

namespace Project
{
    public class Timer
    {
        public static BayesForm bform;
        public BayesNet bnet;
        int sizeX;
        int sizeY;
        public Dictionary<string, int> counter = new Dictionary<string, int>();
        double chanceForWater = 0.08;
        // fish can only spawn in water
        double chanceForFish = 0.7;
        double chanceForShark = 0.4;

        double chanceForMountain = 0.1;
        //caves are only in mountains
        double chanceForCave = 0.2;
        double chanceForTree = 0.3;
        double chanceForRabbit = 0.8;
        double chanceForJackal = 0.2;
        // wolf can only spawn on mountains
        double chanceForWolf = 0.9;
        double chanceForAgent = 0.5;

        public Timer(BayesForm bForm)
        {
            bform = bForm;
            setUpBnet();
            // seting up the environment with standard objects
            sizeX = EnvironmentMap.sizeX;
            sizeY = EnvironmentMap.sizeY;
            int X = 0;
            int Y = 0;
            Random rand = new Random(System.DateTime.Now.Millisecond);
            double water;
            double mountain;
            double plant;
            double animal;
            counter.Add("water", 0);
            counter.Add("fish", 0);
            counter.Add("shark", 0);
            counter.Add("ground", 0);
            counter.Add("mountain", 0);
            counter.Add("tree", 0);
            counter.Add("rabbit", 0);
            counter.Add("jackal", 0);
            counter.Add("wolf", 0);
            counter.Add("Cave", 0);
            counter.Add("Agent", 0);
            counter.Add("House", 0);
            counter.Add("garden", 0);
            counter.Add("woodmill", 0);
            counter.Add("dam", 0);


            while (X < sizeX)
            {
                while (Y < sizeY)
                {
                    water = rand.NextDouble();
                    if (water < chanceForWater)
                    {
                        EnvironmentMap.add(new water(bnet), X, Y);
                        animal = rand.NextDouble();
                        if (animal < chanceForFish)
                            EnvironmentMap.add(new fish(bnet), X, Y);
                        animal = rand.NextDouble();
                        if (animal < chanceForShark)
                            EnvironmentMap.add(new shark(bnet), X, Y);
                    }
                    else
                    {
                        mountain = rand.NextDouble();
                        if (mountain < chanceForMountain)
                        {
                            EnvironmentMap.add(new mountain(bnet), X, Y);
                            animal = rand.NextDouble();
                            if (animal < chanceForWolf)
                                EnvironmentMap.add(new wolf(bnet), X, Y);
                            mountain = rand.NextDouble();
                            if (mountain < chanceForCave)
                            {
                                animal = rand.NextDouble();
                                Cave cave = new Cave(bnet);
                                if (animal < chanceForAgent)
                                {
                                    EnvironmentMap.add(new Agent(bnet, cave), X, Y);
                                    EnvironmentMap.add(new Agent(bnet, cave), X, Y);
                                    EnvironmentMap.add(new Agent(bnet, cave), X, Y);
                                }
                                EnvironmentMap.add(cave, X, Y);
                            }
                        }
                        else
                        {
                            EnvironmentMap.add(new ground(bnet), X, Y);
                            plant = rand.NextDouble();
                            if (plant < chanceForTree)
                            {
                                animal = rand.NextDouble();
                                if (animal < chanceForRabbit)
                                    EnvironmentMap.add(new rabbit(bnet), X, Y);
                                EnvironmentMap.add(new tree(bnet), X, Y);
                            }
                            else
                            {
                                animal = rand.NextDouble();
                                if (animal < chanceForJackal)
                                    EnvironmentMap.add(new jackal(bnet), X, Y);
                            }
                        }
                    }
                    ++Y;
                }
                Y = 0;
                ++X;
            }

            EnvironmentMap.update(counter);
            EnvironmentMap.update(counter);
            /*
            //starting infinite loop

            int turncounter = 50;
            int show = 10;
            while (true)
            {
                ++year;
                bnet.updateFull();
                clearCounter(counter);

                EnvironmentMap.update(counter);
                if (turncounter >= show)
                {
                    turncounter = 0;
                    bform.Refresh();
                    /*
                    Console.WriteLine("\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
                    Console.WriteLine("Counter :");
                    foreach (KeyValuePair<string, int> C in counter)
                    {
                        if (C.Key.Equals("Cave"))
                            Console.WriteLine("\nAgent Related Information\n");
                        Console.WriteLine(C.Key + " = " + C.Value);
                    }
                    Console.WriteLine("\n");
                    Console.WriteLine("Year : " + year);
                    Console.WriteLine("BayesNet :\n");
                    //Console.WriteLine("Nodes:\n" + bnet.Nodes.toString());
                    EnvironmentMap.toString();
                    *
                }
                ++turncounter;
                //Console.ReadKey();
            }
             */
        }

        public void clearCounter(Dictionary<string, int> counter)
        {
            counter["water"] = 0;
            counter["fish"] = 0;
            counter["shark"] = 0;
            counter["ground"] = 0;
            counter["mountain"] = 0;
            counter["Cave"] = 0;
            counter["tree"] = 0;
            counter["rabbit"] = 0;
            counter["jackal"] = 0;
            counter["wolf"] = 0;
            counter["Agent"] = 0;
            counter["House"] = 0;
            counter["garden"] = 0;
            counter["woodmill"] = 0;
            counter["dam"] = 0;
        }

        public void setUpBnet()
        {
            bnet = new BayesNet();
            EnvironmentMap.setup();

            bnet.addNode("Season", new[] { "Spring", "Summer", "Fall", "Winter" });
            bnet.addNode("Monsoon", new[] { "false", "true" });

            bnet.addNode("temperature", new[] { "cool", "cold", "warm", "hot" });
            bnet.addNode("humidity", new[] { "dry", "halfHumid", "humid" });
            bnet.addNode("wind", new[] { "calmWinds", "windy", "strongWinds" });
            bnet.addNode("clouds", new[] { "clearSky", "partiallyCloudy", "cloudy" });

            bnet.addNode("precipitation", new[] { "noPrecepitation", "raining", "snowing", "hailing" });

            bnet.addNode("lightning", new[] { "false", "true" });

            //--


            //CURRENT effects:

            //The seasons!

            //There are a lot of ways to do Intensity Matrices.

            bnet.addEdge("Season", "temperature")
                .addGeneralProbability("Spring", new[] { 0.2, 0.30, 0.4, 0.1 }, 3)
            //summer is hot
                .addGeneralProbability("Summer", new[] { 0.01, 0.25, 0.75, 0.5 }, 3)
            //fall is slightly cool
                .addGeneralProbability("Fall", new[] { 0.1, 0.4, 0.3, 0.2 }, 3)
            //winter is cold
                .addGeneralProbability("Winter", new[] { 0.5, 0.75, 0.25, 0.01 }, 3);

            bnet.addEdge("Season", "humidity")
                .addGeneralProbability("Spring", new[] { 0.2, 0.4, 0.4 }, 5);

            bnet.addEdge("Season", "wind")
                .addGeneralProbability("Spring", new[] {0.2, 0.4, 0.6}, 1);

            bnet.addEdge("Monsoon", "humidity")    //no chance of no humidity, low chance of halfhumid, high chance of very humid.
                .addGeneralProbability("true", new[] { 0, 0.1, 0.9 }, 10);
            bnet.addEdge("Monsoon", "precipitation") 
                .addGeneralProbability("true", new[] { 0.1, 0.6, 0.1, 0.2 }, 10);

            //more clouds, and humid it is, the higher chance of raining.
            bnet.addEdge("clouds", "precipitation")
                .addGeneralProbability("clearSky", new[] { 0.85, 0.05, 0.05, 0.05 }, 5)
                .addGeneralProbability("partiallyCloudy", new[] { 0.4, 0.2, 0.2, 0.2 }, 3)
                .addGeneralProbability("cloudy", new[] { 0.1, 0.3, 0.3, 0.3 }, 1);
            //TODO: make precipitation lower cloud distribution, and lack of precipitation increase cloud distribution.
            //bnet.addEdge("noPrecepitation",
            //    new double[] { 0.4, 0.3, 0.25 }, true, true);

            bnet.addEdge("humidity", "precipitation")
                .addGeneralProbability("humid", new[] { 0.2, 0.25, 0.25, 0.3 }, 3)
                .addGeneralProbability("halfHumid", new[] { 0.5, 0.2, 0.2, 0.1 }, 2);

            bnet.addEdge("temperature", "precipitation")
                .addGeneralProbability("cold", new[] { 0.45, 0.05, 0.4, 0.1 }, 8)   //when it snows, it keeps snowing for a long time?
                .addGeneralProbability("cool", new[] { 0.25, 0.25, 0.3, 0.2 }, 3)
                .addGeneralProbability("warm", new[] { 0.5, 0.45, 0.01, 0.04 }, 3)
                .addGeneralProbability("hot", new[] { 0.55, 0.43, 0.01, 0.01 }, 2);

            //tornedo is out of service, until I think of a better way to do it. Right now it's just hail... no tornedo =\
            //bnet.addEdge("strongWinds", //I'm thinking tornados.
            //    new string[] { "noPrecepitation", "raining", "snowing", "hailing" },
            //    new double[] { 0.3, 0.2, 0.1, 0.4 });

            bnet.addEdge("precipitation", "lightning") //raining = 1.2, 0.1);
                .addGeneralProbability("raining", new[] { 1.5, 1.0 }, 1);
            //storm is out of commission until I can do it better.
            //bnet.addEdge("wind", "lightning") //strongWinds = 1.3, 0.1); //I guess this implies a storm?
            //    .addGeneralProbability("strongWinds", new[] { 1.3, 0.1 });


            //PREVIOUS
            //from my understanding, humidity builds up clouds. If it is really humid a number of days in summer, you expect a large storm.
            bnet.addEdge("humidity", "clouds")
                .addGeneralProbability("humid", new[] { 0.15, 0.35, 0.5 }, 5)
                .addGeneralProbability("dry", new[] { 0.5, 0.3, 0.2 }, 3);

            //if it's windy the clouds may have passed.
            bnet.addEdge("wind", "clouds")
                .addGeneralProbability("strongWinds", new[] { 0.5, 0.25, 0.25 }, 1);

            bnet.InitializeRandomStartValues();
            bnet.Initialize();

            #region oldEdgeCode
            /*
             * bnet.addEdge("Spring",
                new string[] { "cold", "cool", "warm", "hot" },
                new double[] { 0.1, 0.35, 0.35, 0.2 });
            bnet.addEdge("Spring",
                new string[] { "humid", "halfHumid", "dry" },
                new double[] { 0.45, 0.35, 0.2 });

            //summer is hot
            bnet.addEdge("Summer",
                new string[] { "cold", "cool", "warm", "hot" },
                new double[] { 0.05, 0.2, 0.35, 0.4 });

            //fall is slightly cool, and windy
            bnet.addEdge("Fall",
                new string[] { "cold", "cool", "warm", "hot" },
                new double[] { 0.2, 0.35, 0.35, 0.1 });
            bnet.addEdge("Spring",
                new string[] { "strongWinds", "windy", "calmWinds" },
                new double[] { 0.45, 0.35, 0.2 });

            //winter is cold
            bnet.addEdge("Winter",
                new string[] { "cold", "cool", "warm", "hot" },
                new double[] { 0.4, 0.35, 0.2, 0.05 });

            bnet.addEdge("Monsoon",
                new string[] { "humid", "halfHumid", "dry" },
                new double[] { 0.9, 0.1, 0.0 });
            bnet.addEdge("Monsoon",
                new string[] { "noPrecepitation", "raining", "snowing", "hailing" },
                new double[] { 0.1, 0.6, 0.1, 0.2 });

            //more clouds, and humid it is, the higher chance of raining.
            bnet.addEdge("cloudy",
                new string[] { "noPrecepitation", "raining", "snowing", "hailing" },
                new double[] { 0.1, 0.3, 0.3, 0.3 });
            bnet.addEdge("partiallyCloudy",
                new string[] { "noPrecepitation", "raining", "snowing", "hailing" },
                new double[] { 0.4, 0.2, 0.2, 0.2 });
            bnet.addEdge("clearSky",
                new string[] { "noPrecepitation", "raining", "snowing", "hailing" },
                new double[] { 0.85, 0.05, 0.05, 0.05 });

            bnet.addEdge("humid",
                new string[] { "noPrecepitation", "raining", "snowing", "hailing" },
                new double[] { 0.2, 0.25, 0.25, 0.3 });
            bnet.addEdge("dry",
                new string[] { "noPrecepitation", "raining", "snowing", "hailing" },
                new double[] { 0.5, 0.2, 0.2, 0.1 });

            bnet.addEdge("cold",
                new string[] { "noPrecepitation", "raining", "snowing", "hailing" },
                new double[] { 0.45, 0.05, 0.4, 0.1 });
            bnet.addEdge("cool",
                new string[] { "noPrecepitation", "raining", "snowing", "hailing" },
                new double[] { 0.25, 0.25, 0.3, 0.2 });
            bnet.addEdge("warm",
                new string[] { "noPrecepitation", "raining", "snowing", "hailing" },
                new double[] { 0.5, 0.45, 0.01, 0.04 });
            bnet.addEdge("hot",
                new string[] { "noPrecepitation", "raining", "snowing", "hailing" },
                new double[] { 0.55, 0.43, 0.01, 0.01 });
            bnet.addEdge("strongWinds", //I'm thinking tornados.
                new string[] { "noPrecepitation", "raining", "snowing", "hailing" },
                new double[] { 0.3, 0.2, 0.1, 0.4 });

            bnet.addEdge("raining", "lightning", 1.2, 0.1);
            bnet.addEdge("strongWinds", "lightning", 1.3, 0.1);



            //PREVIOUS
            //from my understanding, humidity builds up clouds. If it is really humid a number of days in summer, you expect a large storm.
            bnet.addEdge("humid",
                new string[] { "clearSky", "partiallyCloudy", "cloudy" },
                new double[] { 0.15, 0.35, 0.5 },
                true);
            bnet.addEdge("dry",
                new string[] { "clearSky", "partiallyCloudy", "cloudy" },
                new double[] { 0.5, 0.3, 0.2 },
                true);

            //temperature usually stays moderately consistant. This will help keep it from jumping from hot to cold so easily.
            bnet.addEdge("cold",
                new string[] { "cold", "cool", "warm", "hot" },
                new double[] { 0.35, 0.3, 0.2, 0.15 }, true);
            bnet.addEdge("cool",
                new string[] { "cold", "cool", "warm", "hot" },
                new double[] { 0.25, 0.3, 0.25, 0.2 }, true);
            bnet.addEdge("warm",
                new string[] { "cold", "cool", "warm", "hot" },
                new double[] { 0.2, 0.25, 0.3, 0.25 }, true);
            bnet.addEdge("hot",
                new string[] { "cold", "cool", "warm", "hot" },
                new double[] { 0.15, 0.2, 0.3, 0.35 }, true);

            //if their was no raining, then the clouds should generally still exist. Unless ofcourse they passed.
            bnet.addEdge("noPrecepitation",
                new string[] { "cloudy", "partiallyCloudy", "clearSky" },
                new double[] { 0.4, 0.3, 0.25 },true, true);

            //if it's windy the clouds may have passed.
            bnet.addEdge("windy",
                new string[] { "cloudy", "partiallyCloudy", "clearSky" },
                new double[] { 0.25, 0.25, 0.5 }, true);
             * 
             */
            #endregion
        }
    }
}
