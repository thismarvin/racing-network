using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace Racing.Engine.AI.Network
{
    abstract class Layer
    {
        public int TotalInputNodes { get; private set; }
        public int TotalNodes { get; private set; }
        public int TotalOutputNodes { get; private set; }

        public Matrix<double> Nodes { get; set; }

        public ActivationFunction ActivationFunction { get; private set; }

        public Layer(int totalInputNodes, int totalNodes, int totalOutputNodes)
        {
            TotalInputNodes = totalInputNodes;
            TotalNodes = totalNodes;
            TotalOutputNodes = totalOutputNodes;

            Nodes = Matrix<double>.Build.Dense(TotalNodes, 1);

            ActivationFunction = new ActivationFunction(ActivationFunction.Functions.Sigmoid);
        }

        public void SetActivationFunction(ActivationFunction.Functions function)
        {
            ActivationFunction = new ActivationFunction(function);
        }

        public void ApplyActivationFunction()
        {
            for (int y = 0; y < Nodes.RowCount; y++)
            {
                for (int x = 0; x < Nodes.ColumnCount; x++)
                {
                    Nodes[y, x] = ActivationFunction.Compute(Nodes[y, x]);
                }
            }
        }

        public Matrix<double> Gradients()
        {
            return Matrix<double>.Build.Dense(Nodes.RowCount, Nodes.ColumnCount, (i, j) => ActivationFunction.ComputeDerivative(Nodes[i, j]));
        }

        public abstract void Mutate(double probability, double standardDeviation);
    }
}
