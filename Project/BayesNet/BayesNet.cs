using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Multimap;

namespace BayesNetwork.Classes
{
    public class BayesNet
    {
        public Dictionary<string, BayesNode> Nodes { get; set; }
        public TwoUniqueKeyMap<string, string, BayesEdge> Edges { get; set; } // WEIRD: You can have duplicate edges.
        private Dictionary<string, BayesNode> previousNodes { get; set; }
        public double TotalTimeElapsed {get; set; }                               //I'm pretending the units are days ^^.
        public List<Tuple<BayesNode, double>> EventList { get; set; }
        private List<BayesNode> offBurner { get; set; }

        public BayesNet()
        {
            Nodes = new Dictionary<string, BayesNode>();
            Edges = new TwoUniqueKeyMap<string, string, BayesEdge>();
            offBurner = new List<BayesNode>();
        }

        #region addNode functions
        //standard node
        /// <summary>
        /// Add a node to the Bayesian Network, with it's name and states. Make sure the name is correct, as well as the state names.
        /// </summary>
        /// <param name="variableName">ie. Temperature</param>
        /// <param name="states">The various states of the variable. ie. cold, cool, warm, hot</param>
        public void addNode(string variableName, string[] states)
        {
            Nodes.Add(variableName, new BayesNode() { VariableName = variableName, States = new List<string>(states) });
        }
        public void addNode(string variableName, string[] states, uint observedValue)
        {
            Nodes.Add(variableName, new BayesNode() { VariableName = variableName, States = new List<string>(states), ObservedValue = observedValue });
        }
        #endregion

        #region addEdge Functions
        public BayesEdge addEdge(string variableName1, string variableName2)
        {
            if (!(Nodes.ContainsKey(variableName1) && Nodes.ContainsKey(variableName1)))
                throw new Exception("the nodes you are making an edge with do not exist: " + variableName1 + ", " + variableName2);

            BayesEdge newEdge = new BayesEdge() { NodeA = Nodes[variableName1], NodeB = Nodes[variableName2] };
            Edges.Add(variableName1, variableName2, newEdge);
            return newEdge;
        }
        #endregion

        /*public double infer(string nodeName)
        {
            return 0.0;
        }*/
        /*public double infer(string nodeName)
        {
            /*
             * Inferance is finding out P(X | p(X) where X is the current node and p(X) is all the parents of that node
             * 
             * To simplify inputing information each parents has a weight on how it effects its children.
             * This makes it so that for every parent we do not need to enter 2^|p| entry because
             * they can be calculated using the weighted function
             */
        /*
            BayesNode node = Nodes[nodeName];

            //new case! If the node is "NotInfluenced" as in nothing influences it, 
            //then return either 1 for true or 0 for false, to maintain the current value.
            if (node.ManuallySet)
                if (node.ObservedValue == null)
                    throw new Exception("All nodes that are not influenced, must have an observed value");
                else
                    return (node.ObservedValue == true) ? 1.0 : 0.0;

            if (!Edges.ContainsKey(nodeName))
                return node.Probability;

            HashSet<BayesEdge> edges = Edges[nodeName];

            #region Probability formula Description*/
            /* 
             * Original formula:
             * P = P + (~P*(I-1))
             * Where P = Probability and I = Influence
             * 
             * We need to order of operation to have no effect. i.e. 1.2 then 0.8 needs to give the same result
             * as 0.8 then 1.2.
             * To fix this we let I be the average of all Influences
             * 
             * Now it was possible to come to a value less then 0. i.e. P = 0.2 and I = 0.2
             * To fix this we check if I is less then 1 ( a negative influnce) and we use the below
             * P = P + (P*(I-1)) 
             */
        /*
            #endregion

            double I = new double();
            int countI = new int();
            double P = node.Probability;

            foreach (BayesEdge edge in Edges.GetValues(node.VariableName, true))
            {

                BayesNode Influencer;
                if (edge.PastInfluence)
                    Influencer = PreviousNodes[edge.NodeA];
                else
                    Influencer = Nodes[edge.NodeA];

                bool? InfluencerValue = Influencer.ObservedValue;*/
                /* 
                 * If InfluencerValue is not set, do nothing
                 * If it is set and true add influence to the sum
                 * If it is set and false add the inverse of the influence to the sum 
                 */
        /*
                if (InfluencerValue.HasValue)
                    if (InfluencerValue.Value)
                    {
                        I += edge.Influence;
                        ++countI;
                    }
                    else
                    {
                        I += edge.NegativeInfluence;
                        ++countI;
                    }
            }
            if (I == 0)
                return P;

            I = I / countI;

            double FinalProbability = new double();
            if (I > 1)
                FinalProbability = P + ((1 - P) * (I - 1));
            else
                FinalProbability = P + (P * (I - 1));*/
            /*
             * We now have the probability of X given its parents.
             * We return this as the chance of success.
             */
        /*

            return FinalProbability;
        }*/

        /*public double conditional(string nodeName)
        {
             // Given the node X we can determin properties of a parent of X using 
             // conditional probability formula

            BayesNode node = Nodes[nodeName];
            if (!Edges.ContainsKey(nodeName))
                return node.Probability;

            HashSet<BayesEdge> edges = Edges[nodeName];
            return node.Probability;
        }*/

        /* 
         * To overload the array operator. 
         * Example: You can use "bayesNet['nodeName']" to get a node.
         */
        public BayesNode this[string nodeName]
        {
            get { return Nodes[nodeName]; }
            set { Nodes[nodeName] = value; }
        }

        //I'm storing the influences for much faster access.
        public Dictionary<string, BayesEdge> getNodesInfluences(string nodeName)
        {
            Dictionary<string, BayesEdge> influencesEdges = new Dictionary<string, BayesEdge>();

            // If node doesn't exist, do nothing and return.
            if (Nodes[nodeName] == null)
                return influencesEdges;

            foreach (Dictionary<string, BayesEdge> es in Edges.Values)
                foreach (BayesEdge e in es.Values)
                    if (e.NodeB.VariableName == nodeName)
                        influencesEdges.Add(e.NodeA.VariableName, e);

            return influencesEdges;//Nodes[nodeName].influences =
        }

        //used to see if a variable EVER changes. (right-click -> Find All References)
        public HashSet<BayesNode> getNodesInfluencers(string nodeName)
        {
            HashSet<BayesNode> influencersNodes = new HashSet<BayesNode>();

            // If node doesn't exist, do nothing and return.
            if (Nodes[nodeName] == null)
                return influencersNodes;

            foreach (Dictionary<string, BayesEdge> es in Edges.Values)
                foreach (BayesEdge e in es.Values)
                    if(e.NodeB.VariableName == nodeName)
                        influencersNodes.Add(e.NodeA);

            return influencersNodes;
        }

        public HashSet<BayesNode> getNodesInfluencees(string nodeName)
        {
            HashSet<BayesNode> influencedNodes = new HashSet<BayesNode>();

            // If node doesn't exist, do nothing and return.
            if (Nodes[nodeName] == null)
                return influencedNodes;

            foreach (KeyValuePair<string, Dictionary<string, BayesEdge>> p in Edges)
            {
                if (p.Key == nodeName)
                    foreach (BayesEdge b in p.Value.Values)
                        influencedNodes.Add(b.NodeB);
            }

            return influencedNodes;
        }

        /// <summary>
        /// sets random states for each variable, to start.
        /// These states may not make any sense together, but after a bunch of units of time it'll balance itsself out.
        /// Unless it's in an impossible state. For example if it's monsoon, 
        /// it can never be dry, but then likely something else will give to be more like monsoon, 
        /// so probably not a problem.
        /// </summary>
        public void InitializeRandomStartValues()
        {
            Random rand = new Random();
            List<string> ugh = new List<string>(Nodes.Keys.ToArray());
            foreach (string key in ugh)
            {
                Nodes[key].ObservedValue = (uint)rand.Next(Nodes[key].States.Count - 1);
                Nodes[key] = calculateNextProb(Nodes[key]);
            }
        }

        #region oldupdateFull
        /*
        public Boolean updateFull()
        {
             // This function sets the value of everything in the network to be either true or false
             // to be used by the environment.
            Random randnum = new Random();
            double next = new double();
            double p = new double();

            previousNodes = new Dictionary<string, BayesNode>(Nodes);

            for (int i = 0; i <= 3; i++)
            {
                foreach (string t in ME_nodes.Keys)
                {
                    next = randnum.NextDouble();
                    p = 0.0;
                    //for all nodes in a specific category.
                    bool observedFound = false;
                    foreach (string a in ME_nodes.GetValues(t))
                    {
                        if (!nodeLevels[i].Contains(a))
                            break; //this is lazy... this check should be before the first forloop.

                        p += this.infer(a);
                        if (p < next || observedFound)
                            this[a].ObservedValue = false;
                        else
                            this[a].ObservedValue = observedFound = true;
                    }
                }

                //fill in the details for non-mutually exclusive nodes
                foreach (KeyValuePair<string, BayesNode> b in Nodes)
                {
                    if (b.Value.me_cat == null && nodeLevels[i].Contains(b.Key))
                    {
                        p = this.infer(b.Key);
                        next = randnum.NextDouble();
                        if (p < next)
                            this[b.Key].ObservedValue = false;
                        else
                            this[b.Key].ObservedValue = true;
                    }
                }
            }

            return true;
        }*/
        #endregion

        /// <summary>
        /// prepares the event list and resets the total time elapsed.
        /// </summary>
        public void Initialize()
        {
            TotalTimeElapsed = 0;
            EventList = blindAssumptionUpdate();
        }

        //Problems:
        //1: doesn't account for different scales. one CIM could include [1,3], while another includes [0.1,0.3]. 
        //      Although you could make the arguement that because larger values, mean the state stays longer, 
        //      it is more stable and therefore has a stronger influence.
        /// <summary>
        /// This will go through the process of doing all events for a duration of time specified by timeElapsed
        /// The EventList, and nodes are updated in here.
        /// </summary>
        /// <param name="timeElapsed">The amount of units of time to apply.</param>
        public void updateFull(double timeElapsed)
        {
            //store the total elapsed time for fun.
            TotalTimeElapsed += timeElapsed;
            //If the first event will occur in this time lapse, then loop and try the next event once done.
            while (EventList[0].Value - timeElapsed <= 0)
            {
                //first record the timeElapsed to bring the first event to 0.
                double reducer = EventList[0].Value;

                timeElapsed -= reducer;
                //reduce all events timeLeft by that amount.
                for (int i = 0; i != EventList.Count; i++)
                    EventList[i].Key.timeLeft = EventList[i].Value -= reducer;

                //change the state
                EventList[0].Key = linearlyInfer(EventList[0].Key); //when we change state, we must remember to reset the probabilities, etc. for the node.
                //store the variable that changed
                string changedVar = EventList[0].Key.VariableName;
                //remove the event from the list.
                EventList.RemoveAt(0);

                EventList = recalculateEvents(changedVar, EventList);

                //add in any new nodes that may change.
                for (int i = 0; i != offBurner.Count; ++i )
                {
                    if (isCurrentlyChangeable(offBurner[i]))
                    {
                        EventList.Add(new Tuple<BayesNode, double>(Nodes[offBurner[i].VariableName], 0.0));
                        EventList[EventList.Count - 1].Key = calculateNextProb(EventList[EventList.Count - 1].Key);
                        EventList[EventList.Count - 1].Value = EventList[EventList.Count - 1].Key.timeLeft;
                        offBurner.RemoveAt(i--);
                    }
                }

                //reset the probs on the variable.
                Nodes[changedVar].nextVarProbs = new double[Nodes[changedVar].States.Count];
                Nodes[changedVar].timeLeft = 0;
                if (isCurrentlyChangeable(Nodes[changedVar]))
                {
                    //re-add the changed variable.
                    EventList.Add(new Tuple<BayesNode, double>(Nodes[changedVar], 0.0));
                    EventList[EventList.Count - 1].Key = calculateNextProb(EventList[EventList.Count - 1].Key);
                    EventList[EventList.Count - 1].Value = EventList[EventList.Count - 1].Key.timeLeft;
                }
                else
                {
                    offBurner.Add(Nodes[changedVar]);
                }

                //re-order the list.
                EventList = EventList.OrderBy(kvp => kvp.Value).ToList();
            }

            //reduce all events timeLeft by the remaining amount.
            for (int i = 0; i != EventList.Count; i++)
                EventList[i].Key.timeLeft = EventList[i].Value -= timeElapsed;
        }

        private BayesNode linearlyInfer(BayesNode bayesNode)
        {
            Random r = new Random();

            //Normalized dice roll.             // -1, because we disclude the current state. We are CHANGING states.
            double diceRoll = r.NextDouble() * (bayesNode.nextVarProbs.Sum() - 1);
            double currentSum = 0.0;
            for (uint i = 0; i != bayesNode.nextVarProbs.Length; ++i)
            {
                if(i != bayesNode.ObservedValue) //discluding the time element.
                    if (diceRoll <= (currentSum += bayesNode.nextVarProbs[i]))
                    {
                        bayesNode.ObservedValue = i;    //set the new state.
                        break;
                    }
            }

            return bayesNode;
        }

        /// <summary>
        /// reevaluate the probabilities in the event list. After this function the eventList is likely out of order.
        /// </summary>
        /// <param name="changedVar">The variable that has changed states. We only need to update variables that are affected by that variable.</param>
        /// <param name="eventList">basically EventList.</param>
        /// <returns></returns>
        public List<Tuple<BayesNode, double>> recalculateEvents(string changedVar, List<Tuple<BayesNode, double>> eventList)
        {
            for (int i=0; i!=eventList.Count; ++i)
            {
                if (getNodesInfluences(eventList[i].Key.VariableName).ContainsKey(changedVar))
                {
                    eventList[i].Key = calculateNextProb(eventList[i].Key);
                    eventList[i].Value = eventList[i].Key.timeLeft;
                }
            }

            return eventList;
        }

        /// <summary>
        /// THe most challenging function. This calculates the probability for changing to each state, and the time left before changing.
        /// It takes into account the fact that some states have been half in on state and half in another during it's wait.
        /// </summary>
        /// <param name="bayesNode">The node we are calculating the probability for.</param>
        /// <returns></returns>
        private BayesNode calculateNextProb(BayesNode bayesNode)
        {
            if (!isCurrentlyChangeable(bayesNode))
                return bayesNode;
            if(bayesNode.VariableName == "lightning")
                Console.Write("f");
            //I dunno... something default. Assuming default prob is 1.0/n
            double[] probsSum = new double[bayesNode.States.Count];
            for(uint i=0; i!=probsSum.Length; ++i)
                if(i!=bayesNode.ObservedValue)
                    probsSum[i] = 1/(probsSum.Length-1);
            probsSum[bayesNode.ObservedValue] = 1.0;
            
            int count = 1;
            double[] avgProb = new double[bayesNode.States.Count];

            foreach (BayesEdge be in getNodesInfluences(bayesNode.VariableName).Values)
            {
                //The duration is stored as the negative sum, at the x,x index for state x.
                string influencersState;
                if (be.CIMs.ContainsKey(influencersState = be.NodeA.States[(int)be.NodeA.ObservedValue]))
                {
                    for (int i = 0; i != probsSum.Length; ++i)
                        probsSum[i] += be.CIMs[influencersState][bayesNode.ObservedValue][i];
                    ++count;
                }
            }

            for (int i = 0; i != probsSum.Length; ++i)
                avgProb[i] = probsSum[i]/count;
            double time = avgProb[bayesNode.ObservedValue];

            //this if statement is merely for initialization from what I remember.
            if (bayesNode.timeLeft == 0)
            {
                bayesNode.timeLeft = time;
                bayesNode.nextVarProbs = avgProb;
            }
            else
            {
                //this assumes timeLeft is being updated somewhere else.
                //This is the amount of time left ratio. So the new state only affects the var as much as it should.
                double modifier = bayesNode.timeLeft / (bayesNode.nextVarProbs[bayesNode.ObservedValue]);
                //Apply modifier to probs
                for(int i=0; i!=avgProb.Length; ++i)
                    avgProb[i] *= modifier;
                
                //get new probs. Generally they shouldn't change much. Ie. if only 2 variables effect the node, 
                //then it may change a lot, but if 10 variables effect the node it will change very little (1/10).
                for (int i = 0; i != avgProb.Length; ++i)
                    bayesNode.nextVarProbs[i] = (bayesNode.nextVarProbs[i] + avgProb[i]) / 2;

                bayesNode.nextVarProbs[bayesNode.ObservedValue] = (bayesNode.timeLeft = bayesNode.nextVarProbs.Sum() - bayesNode.nextVarProbs[bayesNode.ObservedValue]);
            }

                return bayesNode;
        }

        //initialization function for probabilities. States are assumed to have been choosen.
        public List<Tuple<BayesNode, double>> blindAssumptionUpdate()
        {
            List<Tuple<BayesNode, double>> eventList = new List<Tuple<BayesNode, double>>();
            int count = 0;

            foreach (BayesNode bn in Nodes.Values)
            {
                //only add nodes if they actually have an influence. (so that they actually change [ie. seasons don't change right now.])
                if (isCurrentlyChangeable(bn))
                {
                    eventList.Add(new Tuple<BayesNode, double>(bn, 0.0));
                    eventList[count].Key = eventList[count].Key;
                    eventList[count].Value = eventList[count].Key.timeLeft;
                    ++count;
                }
                else if (getNodesInfluencers(bn.VariableName).Count != 0)
                    offBurner.Add(bn);
            }
            eventList = eventList.OrderBy(kvp => kvp.Value).ToList();

            return eventList;
        }
        /// <summary>
        /// checks to see if the node can change under the current circumstances.
        /// </summary>
        /// <param name="n">the node in question.</param>
        /// <returns></returns>
        public bool isCurrentlyChangeable(BayesNode n)
        {
            foreach (BayesEdge be in getNodesInfluences(n.VariableName).Values)
                if (be.CIMs.ContainsKey(be.NodeA.States[(int)be.NodeA.ObservedValue]))
                    return true;
            return false;
        }
        /*public void normalizeMEnodes(string me_cat)
        {
            if (!ME_nodes.Keys.Contains(me_cat))
                throw new Exception("You cannot normalize what is not there... Currently that makes no sense.");

            HashSet<string> nodesInCategory = ME_nodes.GetValues(me_cat);

            //get the probability sum.
            double sum = 0;
            foreach (string nodeName in nodesInCategory)
                sum += Nodes[nodeName].Probability;
            //Now normalize!
            foreach (string nodeName in nodesInCategory)
                Nodes[nodeName].Probability /= sum;
        }*/
    }
}
