using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BayesNetwork.Classes
{
    public class BayesNode
    {
        public string VariableName { get; set; }
        public List<string> States { get; set; }
        public bool Stable { get; set; }    //if this is set, then when a general probability is given, 
                                            //it will be stabalized to not change as radically.

        //These variables are for state tracking:
        public Dictionary<string, BayesEdge> influences { get; set; }   //for effeciency.
        public double timeLeft { get; set; }    //stores the time left till the variable is set to change. Don't depend on this being up to date.
        public double[] nextVarProbs { get; set; }  //stores the probability for the next variables. This will be updated as necessary.
        private uint _observedValue; //Zero based!
        public uint ObservedValue
        {
            get { return _observedValue; }
            set
            {
                if (value >= States.Count)
                    throw new Exception("You can't set the variable to a state that doesn't exist.");

                _observedValue = value;

                
            }
        }
        public void setObservedValue(string state)
        { ObservedValue = (uint)States.IndexOf(state); }

        public override string ToString()
        {
            return "<BayesNode>{\"" + VariableName + "\", " + ObservedValue.ToString() + "}";
        }
    }
}
