using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Racing.Engine.Level;

namespace Racing.Engine.AI.Network
{
    class InputLayer : Layer
    {
        public Matrix<double> WeightsOutput { get; set; }

        public InputLayer(int totalNodes, int totalOutputNodes) : base(0, totalNodes, totalOutputNodes)
        {
            WeightsOutput = Matrix<double>.Build.Dense(TotalOutputNodes, TotalNodes, (i, j) => RandomHelper.Range(-1, 1));
        }

        public override void Mutate(double probability, double standardDeviation)
        {
            if (Playfield.RNG.NextDouble() <= probability)
                WeightsOutput = WeightsOutput.Add(RandomHelper.Gaussian(0, standardDeviation));
        }

        public void TweakWeightsBy(Matrix<double> delta)
        {
            WeightsOutput = WeightsOutput.Add(delta);
        }

        public void SetWeightsOutput(Matrix<double> weightsOutput)
        {
            WeightsOutput = Matrix<double>.Build.Dense(TotalOutputNodes, TotalNodes, (i, j) => weightsOutput[i,j]);
        }
    }
}
