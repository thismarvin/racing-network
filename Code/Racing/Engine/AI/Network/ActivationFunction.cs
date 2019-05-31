﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Racing.Engine.AI.Network
{
    class ActivationFunction
    {
        public Functions Function { get; private set; }
        public enum Functions
        {
            Sigmoid, TanH, ReLU, LReLU
        }

        public ActivationFunction(Functions function)
        {
            Function = function;
        }

        public double Compute(double num)
        {
            switch (Function)
            {
                case Functions.Sigmoid:
                    return 1 / (1 + Math.Exp(-num));
                case Functions.TanH:
                    return Math.Tanh(num);
                case Functions.ReLU:
                    if (num > 0)
                        return num;
                    else
                        return 0;
                case Functions.LReLU:
                    if (num > 0)
                        return num;
                    else
                        return 0;
            }
            return 0;
        }

        public double ComputeDerivative(double num)
        {
            switch (Function)
            {
                case Functions.Sigmoid:
                    return num * (1 - num);
                case Functions.TanH:
                    return 1 - num * num;
                case Functions.ReLU:
                    if (num > 0)
                        return 1;
                    else
                        return 0;
                case Functions.LReLU:
                    if (num > 0)
                        return 1;
                    else
                        return 0.01;
            }
            return 0;
        }
    }
}
