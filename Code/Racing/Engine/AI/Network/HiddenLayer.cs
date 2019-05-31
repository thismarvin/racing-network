using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Racing.Engine.Level;

namespace Racing.Engine.AI.Network
{
    class HiddenLayer : Layer
    {
        public Matrix<double> WeightsInput { get; set; }
        public Matrix<double> WeightsOutput { get; set; }
        public Matrix<double> Bias { get; set; }

        public HiddenLayer(Matrix<double> weightsInput, int totalNodes, int totalOutputNodes) : base(weightsInput.RowCount, totalNodes, totalOutputNodes)
        {
            WeightsInput = weightsInput;
            WeightsOutput = Matrix<double>.Build.Dense(TotalOutputNodes, TotalNodes, (i, j) => RandomHelper.Range(-1, 1));

            Bias = Matrix<double>.Build.Dense(TotalNodes, 1, (i, j) => RandomHelper.Range(-1, 1));
        }

        public void UpdateReference(Matrix<double> weightsInput)
        {
            WeightsInput = weightsInput;
        }

        public override void Mutate(double probability, double standardDeviation)
        {
            if (Playfield.RNG.NextDouble() <= probability)
                WeightsOutput = WeightsOutput.Add(RandomHelper.Gaussian(0, standardDeviation));
            if (Playfield.RNG.NextDouble() <= probability)
                Bias = Bias.Add(RandomHelper.Gaussian(0, standardDeviation));
        }

        public void AddBias()
        {
            Nodes = Nodes.Add(Bias);
        }

        public void TweakWeightsBy(Matrix<double> delta)
        {
            WeightsOutput = WeightsOutput.Add(delta);
        }

        public void TweakBiasBy(Matrix<double> gradients)
        {
            Bias = Bias.Add(gradients);
        }
    }
}
