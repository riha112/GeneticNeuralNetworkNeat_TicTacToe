﻿using System;
using System.Collections.Generic;
using System.Text;
using DataManager.Models;

namespace GNNNeatLibrary.Controllers
{
    class BreedingController
    {
        private readonly INetNeatController _netNeatController;
        private IInnovationController _innovationController;
        private readonly INetController _netController;

        public BreedingController(
            INetNeatController netNeatController,
            IInnovationController innovationController,
            INetController netController)
        {
            _netNeatController = netNeatController;
            _innovationController = innovationController;
            _netController = netController;
        }

        public NetModel Breed(NetModel mother, NetModel father)
        {
            var rnd = new Random();

            // Mother should always be fitter then father
            if (mother.FitnessScore < father.FitnessScore)
                return Breed(father, mother);

            // Quick rules for how offspring is generated
            // If both parents have the same innovation then take a random one
            // If fittest parent have innovation then take it.
            // Otherwise skip.

            // As we only care for fittest parents innovation range
            // we can ignore the range of weaker one.
            // - In first check we have ensured that mother is always the fittest one.
            var range = _netNeatController.GetInnovationRange(mother);
            var parents = new NetModel[2] { mother, father };

            // Each connection has an innovation id, thus we can
            // Create array where each connection is stored in cell
            // based on its innovations id.
            // We store connections id in table.
            var table = new int[2, range.max - range.min + 1];
            for (int p = 0; p < 2; p++)
            {
                for (var c = 0; c < parents[p].Connections.Count; c++)
                {
                    var column = parents[p].Connections[c].InnovationId - range.min;
                    // Father can go outside range.
                    if (column < 0 || column >= table.GetLength(2))
                        continue;

                    table[p, column] = c;
                }
            }

            var child = _netController.New();

            // Populates child with connections
            for (var c = 0; c < range.max - range.max + 1; c++)
            {
                // Disjoint on mother, thus we don't care about the connection 
                if (table[0, c] == 0)
                    continue;

                if (table[0, c] != 0 && table[1, c] != 0)
                {
                    // Adds random connection to child
                    int parentId = rnd.Next(0, 2);
                    _netController.AddConnection(ref child, parents[parentId].Connections[table[parentId, c]]);
                    continue;
                }

                // TODO: Make connection ICloneable
                // If disjoint on father, then add mother connection
                _netController.AddConnection(ref child, mother.Connections[table[0,c]]);
            }

            // Populates child with nodes
            // TODO: Populate child with nodes

            return child;
        }
    }
}
