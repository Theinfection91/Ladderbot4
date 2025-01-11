using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ladderbot4.Models;

namespace Ladderbot4.Data
{
    public class StatesAtlasData : Data<StatesAtlas>
    {
        public StatesAtlasData() : base("states_atlas.json", "Databases")
        {

        }

        public void AddState(State newState)
        {
            StatesAtlas stateAtlas = Load();

            if (stateAtlas != null)
            {
                stateAtlas.States.Add(newState);

                Save(stateAtlas);
            }
        }

        public void RemoveState(State state)
        {
            StatesAtlas stateAtlas = Load();

            if (stateAtlas != null)
            {
                // Find correct state to remove
                State? stateToRemove = stateAtlas.States.FirstOrDefault(s => s.LeagueName.Equals(state.LeagueName, StringComparison.OrdinalIgnoreCase));

                if (stateToRemove == null)
                {
                    Console.WriteLine($"{DateTime.Now} StatesManager - The State with the League name of '{state.LeagueName}' was not found.");
                    return;
                }

                stateAtlas.States.Remove(stateToRemove);
                Save(stateAtlas);
            }
        }
    }
}
