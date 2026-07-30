using System;

namespace BannoyasGames.CargoExit.Core
{
    public readonly struct CargoParcelPlan
    {
        public CargoParcelPlan(string id, CargoDestination destination)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException(
                    "A parcel needs a stable identifier.",
                    nameof(id));
            }

            Id = id;
            Destination = destination;
        }

        public string Id { get; }

        public CargoDestination Destination { get; }
    }
}
