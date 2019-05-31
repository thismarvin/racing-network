using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Racing.Engine.Entities.Geometry;
using Racing.Engine.Utilities;

namespace Racing.Engine.AI.Network
{
    class NeuralNetwork
    {
        public List<Layer> Layers { get; private set; }
        public Matrix<double> Output { get; private set; }

        public int TotalHiddenLayers { get; private set; }
        public int TotalInputNodes { get; private set; }
        public int TotalHiddenNodes { get; private set; }
        public int TotalOutputNodes { get; private set; }

        double learningRate;

        Matrix<double> target;
        Matrix<double> gradients;
        Matrix<double> error;
        Matrix<double> deltaWeights;

        List<Circle> nodes;
        List<Shape> nodeInsides;
        List<Line> connections;

        public NeuralNetwork(int totalInputNodes, int totalHiddenLayers, int totalHiddenNodes, int totalOutputNodes, ActivationFunction.Functions function)
        {
            TotalInputNodes = totalInputNodes;
            TotalHiddenLayers = totalHiddenLayers;
            TotalHiddenNodes = totalHiddenNodes;
            TotalOutputNodes = totalOutputNodes;
            learningRate = 0.0075;

            CreateLayers();
            SetActivationFunction(function);            
            VisualSetup();            
        }

        public NeuralNetwork(int totalInputNodes, int totalHiddenLayers, int totalHiddenNodes, int totalOutputNodes) : this(totalInputNodes, totalHiddenLayers, totalHiddenNodes, totalOutputNodes, ActivationFunction.Functions.Sigmoid)
        {                      
            
        }    

        public NeuralNetwork(string name)
        {
            Load(name);

            learningRate = 0.0075;

            VisualSetup();
        }

        public NeuralNetwork(NeuralNetwork neuralNetwork)
        {
            TotalInputNodes = neuralNetwork.TotalInputNodes;
            TotalHiddenLayers = neuralNetwork.TotalHiddenLayers;
            TotalHiddenNodes = neuralNetwork.TotalHiddenNodes;
            TotalOutputNodes = neuralNetwork.TotalOutputNodes;
            learningRate = 0.0075;

            CreateLayers();

            // Copy Weights.
            ((InputLayer)Layers[0]).WeightsOutput = ((InputLayer)neuralNetwork.Layers[0]).WeightsOutput.Clone();

            for (int k = 0; k < TotalHiddenLayers; k++)
            {
                if (k == 0)
                {
                    ((HiddenLayer)Layers[1 + k]).WeightsInput = ((InputLayer)neuralNetwork.Layers[1 + k - 1]).WeightsOutput.Clone();
                }
                else
                {
                    ((HiddenLayer)Layers[1 + k]).WeightsInput = ((HiddenLayer)neuralNetwork.Layers[1 + k - 1]).WeightsOutput.Clone();
                }
                ((HiddenLayer)Layers[1 + k]).WeightsOutput = ((HiddenLayer)neuralNetwork.Layers[1 + k]).WeightsOutput.Clone();
                ((HiddenLayer)Layers[1 + k]).Bias = ((HiddenLayer)neuralNetwork.Layers[1 + k]).Bias.Clone();
            }

            ((OutputLayer)Layers[Layers.Count - 1]).WeightsInput = ((OutputLayer)neuralNetwork.Layers[Layers.Count - 1]).WeightsInput.Clone();
            ((OutputLayer)Layers[Layers.Count - 1]).Bias = ((OutputLayer)neuralNetwork.Layers[Layers.Count - 1]).Bias.Clone();

            UpdateReferences();
            SetActivationFunction(neuralNetwork.Layers[0].ActivationFunction.Function);            
            VisualSetup();
        }

        private void CreateLayers()
        {
            Layers = new List<Layer>();
            Layers.Add(new InputLayer(TotalInputNodes, TotalHiddenNodes));
            if (TotalHiddenLayers == 1)
            {
                Layers.Add(new HiddenLayer(((InputLayer)Layers[0]).WeightsOutput, TotalHiddenNodes, TotalOutputNodes));
            }
            else
            {
                for (int i = 0; i < TotalHiddenLayers; i++)
                {
                    if (i == 0)
                    {
                        Layers.Add(new HiddenLayer(((InputLayer)Layers[0]).WeightsOutput, TotalHiddenNodes, TotalHiddenNodes));
                    }
                    else if (i == TotalHiddenLayers - 1)
                    {
                        Layers.Add(new HiddenLayer(((HiddenLayer)Layers[i]).WeightsOutput, TotalHiddenNodes, TotalOutputNodes));
                    }
                    else
                    {
                        Layers.Add(new HiddenLayer(((HiddenLayer)Layers[i]).WeightsOutput, TotalHiddenNodes, TotalHiddenNodes));
                    }
                }
            }
            Layers.Add(new OutputLayer(((HiddenLayer)Layers.Last()).WeightsOutput, TotalOutputNodes));
        }

        private void SetActivationFunction(ActivationFunction.Functions function)
        {
            foreach (Layer l in Layers)
            {
                l.SetActivationFunction(function);
            }
        }

        private void VisualSetup()
        {
            nodes = new List<Circle>();
            nodeInsides = new List<Shape>();
            connections = new List<Line>();
            
            CreateNodes();
            CreateConnections();
        }

        private void CreateNodes()
        {
            nodes.Clear();

            int radius = 5;
            int spacing = 5;

            int inputHeight = (TotalInputNodes - 1) * radius * 2 + (TotalInputNodes - 1) * spacing / 2;
            int hiddenHeight = (TotalHiddenNodes - 1) * radius * 2 + (TotalHiddenNodes - 1) * spacing / 2;
            int outputHeight = (TotalOutputNodes - 1) * radius * 2 + (TotalOutputNodes - 1) * spacing / 2;
            int offset = 0;

            Vector2 topLeft = new Vector2(Camera.ScreenBounds.Width * 0.1f, (Camera.ScreenBounds.Height - inputHeight) / 2);

            for (int i = 0; i < TotalInputNodes; i++)
            {
                nodes.Add(new Circle(topLeft.X + spacing * 0 * 10, topLeft.Y + i * (radius * 2 + spacing / 2) + offset, radius, 2));
                nodeInsides.Add(new Shape(0, 0, radius + 1, radius + 1, Color.Orange));
                nodeInsides.Last().SetCenter(topLeft.X + spacing * 0 * 10, topLeft.Y + i * (radius * 2 + spacing / 2) + offset);
            }

            offset = (inputHeight - hiddenHeight) / 2;
            for (int j = 0; j < TotalHiddenLayers; j++)
            {
                for (int i = 0; i < TotalHiddenNodes; i++)
                {
                    nodes.Add(new Circle(topLeft.X + spacing * (1 + j) * 10, topLeft.Y + i * (radius * 2 + spacing / 2) + offset, radius, 2));
                    nodeInsides.Add(new Shape(0, 0, radius + 1, radius + 1, Color.Orange));
                    nodeInsides.Last().SetCenter(topLeft.X + spacing * (1 + j) * 10, topLeft.Y + i * (radius * 2 + spacing / 2) + offset);
                }
            }

            offset = (inputHeight - outputHeight) / 2;
            for (int i = 0; i < TotalOutputNodes; i++)
            {
                nodes.Add(new Circle(topLeft.X + spacing * (1 + TotalHiddenLayers) * 10, topLeft.Y + i * (radius * 2 + spacing / 2) + offset, radius, 2));
                nodeInsides.Add(new Shape(0, 0, radius + 1, radius + 1, Color.Orange));
                nodeInsides.Last().SetCenter(topLeft.X + spacing * (1 + TotalHiddenLayers) * 10, topLeft.Y + i * (radius * 2 + spacing / 2) + offset);
            }
        }

        private void CreateConnections()
        {
            connections.Clear();

            Color color = Color.Blue;
            for (int i = 0; i < TotalInputNodes; i++)
            {
                for (int j = 0; j < TotalHiddenNodes; j++)
                {
                    color = ((InputLayer)Layers[0]).WeightsOutput[j, i] > 0 ? Color.Blue : Color.Red;
                    color = new Color(color.R, color.G, color.B, (int)(Math.Abs(((InputLayer)Layers[0]).WeightsOutput[j, i]) * 230) + 25);
                    connections.Add(new Line(nodes[i].Center.X, nodes[i].Center.Y, nodes[TotalInputNodes + j].Center.X, nodes[TotalInputNodes + j].Center.Y, 1, color));
                }
            }

            int max = 0;
            for (int k = 0; k < TotalHiddenLayers; k++)
            {
                max = k != TotalHiddenLayers - 1 ? TotalHiddenNodes : TotalOutputNodes;
                for (int i = 0; i < TotalHiddenNodes; i++)
                {
                    for (int j = 0; j < max; j++)
                    {
                        color = ((HiddenLayer)Layers[1 + k]).WeightsOutput[j, i] > 0 ? Color.Blue : Color.Red;
                        color = new Color(color.R, color.G, color.B, (int)(Math.Abs(((HiddenLayer)Layers[1 + k]).WeightsOutput[j, i]) * 230) + 25);
                        connections.Add(new Line(nodes[TotalInputNodes + i + k * TotalHiddenNodes].Center.X, nodes[TotalInputNodes + i + k * TotalHiddenNodes].Center.Y, nodes[TotalInputNodes + TotalHiddenNodes * (k + 1) + j].Center.X, nodes[TotalInputNodes + TotalHiddenNodes * (k + 1) + j].Center.Y, 1, color));
                    }
                }
            }
        }

        private void UpdateReferences()
        {
            ((HiddenLayer)Layers[1]).UpdateReference(((InputLayer)Layers[0]).WeightsOutput);
            for (int i = 1; i < TotalHiddenLayers; i++)
            {
                ((HiddenLayer)Layers[1 + i]).UpdateReference(((HiddenLayer)Layers[i]).WeightsOutput);
            }
           ((OutputLayer)Layers.Last()).UpdateReference(((HiddenLayer)Layers[Layers.Count - 2]).WeightsOutput);
        }

        private void UpdateVisualization()
        {
            CreateConnections();
        }

        private double[,] ConvertToMultidimensional(double[] array)
        {
            double[,] result = new double[array.Length, 1];
            for (int i = 0; i < array.Length; i++)
                result[i, 0] = array[i];
            return result;
        }
      
        public Matrix<double> Predict(double[] inputArray)
        {
            Layers[0].Nodes = Matrix<double>.Build.DenseOfArray(ConvertToMultidimensional(inputArray));

            for (int i = 1; i <= TotalHiddenLayers; i++)
            {
                Layers[i].Nodes = ((HiddenLayer)Layers[i]).WeightsInput * Layers[i - 1].Nodes;
                ((HiddenLayer)(Layers[i])).AddBias();
                Layers[i].ApplyActivationFunction();
            }

            Layers[Layers.Count - 1].Nodes = ((OutputLayer)Layers[Layers.Count - 1]).WeightsInput * Layers[Layers.Count - 2].Nodes;
            ((OutputLayer)(Layers[Layers.Count - 1])).AddBias();
            Layers.Last().ApplyActivationFunction();

            Output = Layers.Last().Nodes;
            return Output;
        }

        public void Backpropagation(double[] inputArray, double[] targetArray)
        {
            Predict(inputArray);

            target = Matrix<double>.Build.DenseOfArray(ConvertToMultidimensional(targetArray));
            error = target - ((OutputLayer)(Layers[Layers.Count - 1])).Nodes;

            gradients = Layers[Layers.Count - 1].Gradients();
            gradients = gradients.PointwiseMultiply(error) * learningRate;
            deltaWeights = gradients * Layers[Layers.Count - 2].Nodes.Transpose();
            ((HiddenLayer)(Layers[Layers.Count - 2])).TweakWeightsBy(deltaWeights);
            ((OutputLayer)(Layers[Layers.Count - 1])).TweakBiasBy(gradients);
            ((OutputLayer)Layers[Layers.Count - 1]).UpdateReference(((HiddenLayer)Layers[Layers.Count - 2]).WeightsOutput);

            for (int k = 1; k <= TotalHiddenLayers; k++)
            {
                error = ((HiddenLayer)(Layers[Layers.Count - 1 - k])).WeightsOutput.Transpose() * error;

                gradients = Layers[Layers.Count - 1 - k].Gradients();
                gradients = gradients.PointwiseMultiply(error) * learningRate;
                deltaWeights = gradients * Layers[Layers.Count - 2 - k].Nodes.Transpose();

                if (k == TotalHiddenLayers)
                {
                    ((InputLayer)(Layers[Layers.Count - 2 - k])).TweakWeightsBy(deltaWeights);
                    ((HiddenLayer)(Layers[Layers.Count - 1 - k])).TweakBiasBy(gradients);
                    ((HiddenLayer)Layers[Layers.Count - 1 - k]).UpdateReference(((InputLayer)Layers[Layers.Count - 2 - k]).WeightsOutput);
                }
                else
                {
                    ((HiddenLayer)(Layers[Layers.Count - 2 - k])).TweakWeightsBy(deltaWeights);
                    ((HiddenLayer)(Layers[Layers.Count - 1 - k])).TweakBiasBy(gradients);
                    ((HiddenLayer)Layers[Layers.Count - 1 - k]).UpdateReference(((HiddenLayer)Layers[Layers.Count - 2 - k]).WeightsOutput);
                }
            }

            UpdateVisualization();
        }

        public void Mutate(double probability, double standardDeviation)
        {
            foreach (Layer l in Layers)
            {
                l.Mutate(probability, standardDeviation);
            }

            UpdateReferences();
            UpdateVisualization();
        }

        public void Save()
        {
            List<string> data = new List<string>();
            StringBuilder stringBuilder = new StringBuilder();

            data.Add(string.Format("{0} {1} {2} {3}", TotalInputNodes, TotalHiddenLayers, TotalHiddenNodes, TotalOutputNodes));
            data.Add(string.Format("{0} {1}", TotalHiddenNodes, TotalInputNodes));
            for (int i = 0; i < TotalHiddenNodes; i++)
            {
                for (int j = 0; j < TotalInputNodes; j++)
                {
                    stringBuilder.Append(((InputLayer)(Layers[0])).WeightsOutput[i, j] + " ");
                }
                data.Add(stringBuilder.ToString());
                stringBuilder.Clear();
            }
            

            for (int k = 1; k <= TotalHiddenLayers; k++)
            {
                data.Add(string.Format("{0} {1}", k == TotalHiddenLayers ? TotalOutputNodes : TotalHiddenNodes, TotalHiddenNodes));

                for (int i = 0; i < (k == TotalHiddenLayers ? TotalOutputNodes : TotalHiddenNodes); i++)
                {
                    for (int j = 0; j < TotalHiddenNodes; j++)
                    {
                        stringBuilder.Append(((HiddenLayer)Layers[k]).WeightsOutput[i, j] + " ");
                    }
                    data.Add(stringBuilder.ToString());
                    stringBuilder.Clear();
                }
            }

            for (int k = 1; k <= TotalHiddenLayers; k++)
            {
                data.Add(TotalHiddenNodes.ToString());
                for (int i = 0; i < TotalHiddenNodes; i++)
                {
                    data.Add(string.Format(((HiddenLayer)(Layers[k])).Bias[i, 0].ToString()));
                }
            }

            data.Add(string.Format(TotalOutputNodes.ToString()));
            for (int i = 0; i < TotalOutputNodes; i++)
            {
                data.Add(string.Format(((OutputLayer)(Layers.Last())).Bias[i, 0].ToString()));
            }

            data.Add(string.Format(Layers[0].ActivationFunction.Function.ToString()));

            try
            {
                File.WriteAllLines("Neural_Network", data);
            }
            catch (IOException)
            {
                Console.WriteLine("Error, could not save Neural Network.\n");
                return;
            }
            Console.WriteLine("Successfully saved Neural Network.\n");
        }

        public void Load(string name)
        {
            string[] data = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + name);
            string[] line = new string[0];
            int index = 0;

            line = Regex.Split(data[index++], " ");
            TotalInputNodes = int.Parse(line[0]);
            TotalHiddenLayers = int.Parse(line[1]);
            TotalHiddenNodes = int.Parse(line[2]);
            TotalOutputNodes = int.Parse(line[3]);

            CreateLayers();

            line = Regex.Split(data[index++], " ");
            int rows = int.Parse(line[0]);
            int columns = int.Parse(line[1]);

            for (int i = 0; i < rows; i++)
            {
                line = Regex.Split(data[index++], " ");
                for (int j = 0; j < columns; j++)
                {
                    ((InputLayer)(Layers[0])).WeightsOutput[i, j] = double.Parse(line[j]);
                }
            }

            for (int k = 1; k <= TotalHiddenLayers; k++)
            {
                line = Regex.Split(data[index++], " ");
                rows = int.Parse(line[0]);
                columns = int.Parse(line[1]);

                for (int i = 0; i < rows; i++)
                {
                    line = Regex.Split(data[index++], " ");
                    for (int j = 0; j < columns; j++)
                    {
                        ((HiddenLayer)(Layers[k])).WeightsOutput[i, j] = double.Parse(line[j]);
                    }
                }
            }

            for (int k = 1; k <= TotalHiddenLayers; k++)
            {
                line = Regex.Split(data[index++], " ");
                rows = int.Parse(line[0]);
                columns = 1;              

                for (int i = 0; i < rows; i++)
                {
                    line = Regex.Split(data[index++], " ");
                    ((HiddenLayer)(Layers[k])).Bias[i, 0] = double.Parse(line[0]);
                }
            }

            line = Regex.Split(data[index++], " ");
            rows = int.Parse(line[0]);
            columns = 1;

            for (int i = 0; i < rows; i++)
            {
                line = Regex.Split(data[index++], " ");
                ((OutputLayer)(Layers.Last())).Bias[i, 0] = double.Parse(line[0]);
            }

            ParseActivationFunction(Regex.Split(data[index++], " ")[0]);

            ((HiddenLayer)Layers[1]).UpdateReference(((InputLayer)Layers[0]).WeightsOutput);
            for (int i = 1; i < TotalHiddenLayers; i++)
            {
                ((HiddenLayer)Layers[1 + i]).UpdateReference(((HiddenLayer)Layers[i]).WeightsOutput);
            }
            ((OutputLayer)Layers.Last()).UpdateReference(((HiddenLayer)Layers[Layers.Count - 2]).WeightsOutput);
        }

        private void ParseActivationFunction(string function)
        {
            switch (function) {
                case "Sigmoid":
                    SetActivationFunction(ActivationFunction.Functions.Sigmoid);
                    break;
                case "TanH":
                    SetActivationFunction(ActivationFunction.Functions.TanH);
                    break;
                case "ReLU":
                    SetActivationFunction(ActivationFunction.Functions.ReLU);
                    break;
                case "LReLU":
                    SetActivationFunction(ActivationFunction.Functions.LReLU);
                    break;
            }
        }

        public void VisualizeDecisions()
        {
            for (int i = 0; i < TotalInputNodes; i++)
            {
                nodeInsides[i].SetColor(new Color(Palette.LightGray, (float)Layers[0].Nodes[i, 0]));
            }
            for (int j = 0; j < TotalHiddenLayers; j++)
            {
                for (int i = 0; i < TotalHiddenNodes; i++)
                {
                    nodeInsides[TotalInputNodes + i + j * TotalHiddenNodes].SetColor(new Color(Palette.LightGray, (float)Layers[1 + j].Nodes[i, 0]));
                }
            }
            for (int i = 0; i < TotalOutputNodes; i++)
            {
                nodeInsides[TotalInputNodes + TotalHiddenLayers * TotalHiddenNodes + i].SetColor(new Color(Palette.LightGray, (float)Layers[1 + TotalHiddenLayers].Nodes[i, 0]));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, StaticCamera.Transform);
            {
                foreach (Line l in connections)
                {
                    l.Draw(spriteBatch);
                }
                foreach (Shape s in nodeInsides)
                {
                    s.Draw(spriteBatch);
                }
                foreach (Circle c in nodes)
                {
                    c.Draw(spriteBatch);
                }
            }
            spriteBatch.End();
        }
    }
}
