using System;
using System.Collections.Generic;

namespace BannoyasGames.CargoExit.Core
{
    public sealed class CargoSortSession
    {
        private readonly Dictionary<CargoDestination, int> required = new();
        private readonly Dictionary<CargoDestination, int> sorted = new();
        private readonly Dictionary<string, CargoDestination> planned = new();
        private readonly HashSet<string> sortedParcelIds = new();
        private int sortedParcels;

        public CargoSortSession(IEnumerable<CargoParcelPlan> plannedParcels)
        {
            if (plannedParcels == null)
            {
                throw new ArgumentNullException(nameof(plannedParcels));
            }

            foreach (var parcel in plannedParcels)
            {
                if (!planned.TryAdd(parcel.Id, parcel.Destination))
                {
                    throw new ArgumentException(
                        $"Parcel id '{parcel.Id}' is duplicated.",
                        nameof(plannedParcels));
                }

                required.TryGetValue(parcel.Destination, out var count);
                required[parcel.Destination] = count + 1;
                sorted.TryAdd(parcel.Destination, 0);
            }

            if (planned.Count == 0)
            {
                throw new ArgumentException(
                    "A sorting session needs at least one parcel.",
                    nameof(plannedParcels));
            }
        }

        public int TotalParcels => planned.Count;

        public int SortedParcels => sortedParcels;

        public bool IsComplete => sortedParcels == TotalParcels;

        public int GetRequiredCount(CargoDestination destination)
        {
            return required.TryGetValue(destination, out var count) ? count : 0;
        }

        public int GetSortedCount(CargoDestination destination)
        {
            return sorted.TryGetValue(destination, out var count) ? count : 0;
        }

        public SortPlacementResult TryPlace(
            string parcelId,
            CargoDestination palletDestination)
        {
            var requiredCount = GetRequiredCount(palletDestination);
            var currentCount = GetSortedCount(palletDestination);

            if (!planned.TryGetValue(parcelId, out var parcelDestination))
            {
                return new SortPlacementResult(
                    SortPlacementStatus.UnknownParcel,
                    currentCount,
                    requiredCount,
                    IsComplete);
            }

            if (sortedParcelIds.Contains(parcelId))
            {
                return new SortPlacementResult(
                    SortPlacementStatus.AlreadySorted,
                    currentCount,
                    requiredCount,
                    IsComplete);
            }

            if (parcelDestination != palletDestination)
            {
                return new SortPlacementResult(
                    SortPlacementStatus.WrongDestination,
                    currentCount,
                    requiredCount,
                    IsComplete);
            }

            if (currentCount >= requiredCount)
            {
                return new SortPlacementResult(
                    SortPlacementStatus.PalletFull,
                    currentCount,
                    requiredCount,
                    IsComplete);
            }

            currentCount++;
            sorted[palletDestination] = currentCount;
            sortedParcelIds.Add(parcelId);
            sortedParcels++;

            return new SortPlacementResult(
                SortPlacementStatus.Accepted,
                currentCount,
                requiredCount,
                IsComplete);
        }
    }
}
