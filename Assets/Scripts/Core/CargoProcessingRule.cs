using System;

namespace BannoyasGames.CargoExit.Core
{
    public enum CargoWeight
    {
        Standard,
        Heavy
    }

    public readonly struct CargoAttributes
    {
        public CargoAttributes(CargoWeight weight, bool isFragile)
        {
            Weight = weight;
            IsFragile = isFragile;
        }

        public CargoWeight Weight { get; }

        public bool IsFragile { get; }
    }

    public enum CargoProcessingResult
    {
        Normal,
        Inefficient,
        Damaged
    }

    public static class CargoProcessingRule
    {
        public static bool CanProcessAt(
            CargoAttributes attributes,
            WorkStationType station)
        {
            return station == WorkStationType.HeavyCargo ||
                   station == WorkStationType.FragileCargo;
        }

        public static CargoProcessingResult Process(
            CargoAttributes attributes,
            WorkStationType station,
            EmployeeSkill assignedSkill)
        {
            if (!CanProcessAt(attributes, station))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(station));
            }

            if (station == WorkStationType.HeavyCargo && attributes.IsFragile)
            {
                return CargoProcessingResult.Damaged;
            }

            if (station == WorkStationType.FragileCargo &&
                attributes.Weight == CargoWeight.Heavy)
            {
                return CargoProcessingResult.Inefficient;
            }

            if (station == WorkStationType.HeavyCargo &&
                assignedSkill != EmployeeSkill.Strength)
            {
                return CargoProcessingResult.Inefficient;
            }

            if (station == WorkStationType.FragileCargo &&
                assignedSkill != EmployeeSkill.Care)
            {
                return CargoProcessingResult.Damaged;
            }

            return CargoProcessingResult.Normal;
        }
    }
}
