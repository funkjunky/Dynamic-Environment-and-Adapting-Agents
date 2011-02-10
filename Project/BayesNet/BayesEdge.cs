using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BayesNetwork.Classes
{
    public class BayesEdge
    {
        //TODO: rename nodeA and B
        public BayesNode NodeA { get; set; }
        public BayesNode NodeB { get; set; }

        public Dictionary<string, double[][]> CIMs { get; set; }

        //public float[][] JIM { get; private set; } //readonly = "private set"

        public BayesEdge()
        {
            //this safety code doesn't work, 'cause c#'s order of operations sucks here >=(
            //if (NodeB == null && NodeA == null)
            //    throw new Exception("must fill in the influenced and influencer to construct an edge.");
            CIMs = new Dictionary<string, double[][]>();
        }

        //This is a crummy way to do it, plus it needs to be fixed, as it breaks on build.
        //More user friendly function. You can write the probabilitys in plain english using this.
       /* public BayesEdge addInfluence(
                        string influencedPreviousState,
                        string influencedCurrentState,
                        double relativeProbability)
        {
            //Get the correct node that is being influenced.
            BayesNode influencedNode = NodeB;

            //get the CIM if it exists
            double[][] CIM;
            if (CIMs.ContainsKey(nodeA.States.IndexOf(influencerState)))
                CIM = CIMs[nodeA.States.IndexOf(influencerState)];

            //add the probability to the CIM.
            CIM[NodeB.States.IndexOf(influencedPreviousState)][NodeB.States.IndexOf(influencedCurrentState)]
                = relativeProbability;

            CIMs[influencerState] = CIM;

            return this;
        }*/

        //TODO: make this function not suck... it's easy, but I'm lazy and this function is barely used, and is completly unnecessary.
        //
        //make this generic. So I can use it with calcJIM
        public BayesEdge normalizeIM(string influenced)
        {
            List<string> keys = new List<string>(CIMs.Keys.ToArray());
            foreach (string key in keys)
            {
                double[][] temp = CIMs[key];
                normalizeIM(ref temp);
                CIMs[key] = temp;
            }

            return this;
        }
        public static void normalizeIM(ref double[][] im)
        {
            //using CIM[0] is dangerous, but the matrices are always square and I don't know how to get just the length...
            for (int i = 0; i != im[0].Length; i++)
            {
                double sum = 0;
                for (int k = 0; k != im[0].Length; k++)
                    if (k != i)
                        sum += im[i][k];

                im[i][i] = sum;
            }
        }

        //Use this if a node is set as stable.
        public static void StabilizeProgression(ref double[][] im)
        {
            for (int i = 0; i != im.Length; ++i)
                for (int k = 0; k != im.Length; ++k)
                    if (i != k)
                        // divide by 1.5 to the power of the distance from being the current states neihbor.
                        //This will give (1.5, 2.25, 3.375, 5.0625, etc.) It should become exponentially improbable to jump further.
                        im[i][k] /= Math.Pow(1.5, (Math.Abs(k - i) - 1));

            //for example [-2*][1][1] where * indicates the current state (always negative), this would change to
            //[-2][1][1/1.5] = [-2][1][0.66], this way it is less likely to jump from state 0 to 2. Stabilize ;).
        }

        /// <summary>
        /// This creates a CIM from general probabilities. It also modifies the CIM to change the duration of the state.
        /// </summary>
        /// <param name="influencerState">The edge's influence's node's state that we are modifying.</param>
        /// <param name="prob">The general probability of being in a certain state.</param>
        /// <param name="avgStateDuration">The average duration the state will remain. [not exact average, but at least an approximate middle]</param>
        /// <returns>Hurray Chaining!</returns>
        public BayesEdge addGeneralProbability(string influencerState, double[] prob, double avgStateDuration)
        {
            if (!NodeA.States.Exists(x => x == influencerState))
                throw new Exception("you can't add a CIM to a state that does not exist. (Bayes Edge -> addGeneralProbability)");

            if (prob.Length != NodeB.States.Count)
                throw new Exception("You must provide a probability list that is the same width or height of the state size.");

            int length = prob.Length; //the length of everything. Prob length, CIM lengths, NodeB state length.
            double largestProb = prob.Max();

            //Normalize the probability (/sum), and then factor in the avgStateDuration (*avgStateDuration).
            double sum = prob.Sum();
            for (int i = 0; i != length; ++i)
                prob[i] *= avgStateDuration / sum;

            //this gets the duration modifiers for each row. Higher number, means longer duration.
            double[] rowmod = new double[length];
            for (int i = 0; i != length; ++i)
                rowmod[i] = prob[i] / largestProb;

            double[][] newCIM = new double[length][];
            for (int i = 0; i != length; ++i)
            {
                newCIM[i] = new double[length];
                for (int k = 0; k != length; ++k)
                    if (k != i) //we'll just use normalizeCIM for these entries.
                        newCIM[i][k] = prob[k] * rowmod[i];
            }
            normalizeIM(ref newCIM); //ref function.

            if (NodeB.Stable)
                StabilizeProgression(ref newCIM);

            CIMs[influencerState] = newCIM;

            return this;
        }

        public BayesEdge addCIM(string influencerState, double[][] cim)
        {
            if (!NodeA.States.Exists(x => x == influencerState))
                throw new Exception("you can't add a CIM to a state that does not exist. (Bayes Edge -> addCIM)");
            CIMs[influencerState] = cim;

            return this;
        }

        public BayesEdge makeVertSymmetric(string influencerState) {
            if (!NodeA.States.Exists(x => x == influencerState))
                throw new Exception("you can't add a CIM to a state that does not exist. (Bayes Edge -> addCIM)");

            int length = CIMs[influencerState].Length;
            int halfLength = length/2;

            for (int i = 0; i != CIMs[influencerState].Length / 2; ++i)
                for (int k = 0; k != halfLength; ++k)
                    CIMs[influencerState][length - 1 - i][k] = CIMs[influencerState][i][length-1-k];

            return this;
        }

        //unnecessary for my linear method.
        /*public void calculateJIM()
        {
            //fill it with zeros
            double[][] JIM = new double[][] { 0 };

            int numOfCIMs = CIMs_EffectingA.Count;
            //First the easy almagamation. Basically just put the matrices on the topleft to bottom right diagonal.
            for (int i = 0; i != numOfCIMs; i++)
                for (int k = 0; k != CIMs_EffectingA[i][0].Length; k++) //stupid C#... I want the length of the current dimension!?!?! Not every element counted.
                    for (int j = 0; j != CIMs_EffectingA[i][k].Length; j++)
                        if (j != k)
                            JIM[(i * numOfCIMs) + k][(i * numOfCIMs) + j] = CIMs_EffectingA[i][k][j];

            numOfCIMs = CIMs_EffectingB.Count;
            //First the easy almagamation. Basically just put the matrices on the topleft to bottom right diagonal.
            for (int i = 0; i != numOfCIMs; i++)
                for (int k = 0; k != CIMs_EffectingB[i][0].Length; k++) //stupid C#... I want the length of the current dimension!?!?! Not every element counted.
                    for (int j = 0; j != CIMs_EffectingB[i][k].Length; j++)
                        if (j != k)
                            JIM[(numOfCIMs * k) + 1 + i][(numOfCIMs * j) + 1 + i] = CIMs_EffectingB[i][k][j];
        }*/

        //deprecated
        /*private bool isNodeA(string variableName)
        {
            if (variableName == NodeA.VariableName)
                return true;
            else if (variableName == NodeB.VariableName)
                return false;
            else
                throw new Exception("huh? '" + variableName + "' is not a variable on this edge, sorry...");
        }*/

        // 'A => B' means 'A influences B'
        public override string ToString()
        {
            return "<BayesEdge>{'" + NodeA + "' => '" + NodeB + "', NOT IMPLEMENTED YET}";
        }
    }
}
