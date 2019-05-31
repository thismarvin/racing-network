using System;
using System.Collections.Generic;
using System.Linq;
using Racing.Engine.Entities;
using Racing.Engine.Level;

namespace Racing.Engine.AI.Evolution
{
    static class NE
    {
        public static Player BestPlayer { get; private set; }
        public static int Generations { get; private set; }

        public static void Reset()
        {
            BestPlayer = null;
            Generations = 0;
        }

        private static bool PlayersAreStillAlive(List<Player> players)
        {
            foreach (Player p in players)
            {
                if (!p.Dead)
                    return true;
            }
            return false;
        }

        public static void CalculateFitness(List<Player> players)
        {
            double sum = 0;
            foreach (Player p in players)
            {
                p.Score = Math.Pow(p.DefaultScore, 2);
                sum += p.Score;
            }
            foreach (Player p in players)
            {
                p.Fitness = p.Score / sum;
            }
            BestPlayer = PlayerWithHighestFitness(players);
        }

        private static Player PlayerWithHighestFitness(List<Player> players)
        {
            foreach (Player p in players)
            {
                if (BestPlayer == null)
                {
                    BestPlayer = p;
                }
                if (p.Fitness > BestPlayer.Fitness)
                {
                    BestPlayer = p;
                }
            }
            return BestPlayer;
        }

        private static Player RandomSelection(List<Player> players)
        {
            int index = 0;
            double random = Playfield.RNG.NextDouble();

            while (random > 0)
            {
                random -= players[index].Fitness;
                index++;
            }
            index--;
            return players[index];
        }

        private static void GeneticAlgorithm(List<Player> players)
        {
            int playerIndex = 0;

            // Assign the best player as 20% of the next generations' parent.
            for (int i = 0; i < (int)(players.Count * 0.2); i++)
            {
                players[playerIndex++].SetParent(BestPlayer);
            }
            // Assign a random parent, choosen based on their fitness score, for the remaining 80% of the next generation.
            for (int i = 0; i < (int)(players.Count * 0.8); i++)
            {
                players[playerIndex++].SetParent(RandomSelection(players));
            }

            playerIndex = 0;

            // Slightly mutate the children of the best player 5% of the time.
            for (int i = 0; i < (int)(players.Count * 0.2); i++)
            {
                players[playerIndex++].Breed(0.05, 0.1);
            }
            // Conservatively mutate 70% of the children of a random parent 50% of the time.
            for (int i = 0; i < (int)(players.Count * 0.7); i++)
            {
                players[playerIndex++].Breed(0.50, 1);
            }
            // Drastically mutate 10% of the children of a random parent 75% of the time.
            for (int i = 0; i < (int)(players.Count * 0.1); i++)
            {
                players[playerIndex++].Breed(0.75, 2);
            }

            Generations++;
        }

        public static bool NextGeneration(List<Player> players)
        {
            if (PlayersAreStillAlive(players))
                return false;

            CalculateFitness(players);
            GeneticAlgorithm(players);

            return true;
        }
    }
}
