using System;
using System.Collections.Generic;
using BannoyasGames.CargoExit.Core;

namespace BannoyasGames.CargoExit.Application
{
    public sealed class CargoDayState
    {
        private readonly List<string> destinationIds = new();

        public string DayId { get; private set; }

        public int TotalCargoCount { get; private set; }

        public int HeavyCargoCount { get; private set; }

        public int FragileCargoCount { get; private set; }

        public IReadOnlyList<string> DestinationIds => destinationIds;

        public string ObjectiveId { get; private set; }

        public bool BriefingPrepared { get; private set; }

        public WorkStationType StrongEmployeeStation { get; private set; }

        public WorkStationType CarefulEmployeeStation { get; private set; }

        public string HeavyStationEmployeeId { get; private set; }

        public string FragileStationEmployeeId { get; private set; }

        public EmployeeSkill HeavyStationEmployeeSkill { get; private set; }

        public EmployeeSkill FragileStationEmployeeSkill { get; private set; }

        public bool AssignmentConfirmed { get; private set; }

        public CargoProcessingResult HeavyCargoResult { get; private set; }

        public CargoProcessingResult FragileCargoResult { get; private set; }

        public bool ProcessingCompleted { get; private set; }

        public void PrepareBriefing(
            string dayId,
            int totalCargoCount,
            int heavyCargoCount,
            int fragileCargoCount,
            IEnumerable<string> provisionalDestinationIds,
            string objectiveId)
        {
            if (BriefingPrepared)
            {
                return;
            }

            DayId = dayId;
            TotalCargoCount = totalCargoCount;
            HeavyCargoCount = heavyCargoCount;
            FragileCargoCount = fragileCargoCount;
            destinationIds.Clear();
            destinationIds.AddRange(provisionalDestinationIds);
            ObjectiveId = objectiveId;
            BriefingPrepared = true;
        }

        public void ConfirmAssignment(
            WorkStationType strongEmployeeStation,
            WorkStationType carefulEmployeeStation,
            string heavyStationEmployeeId,
            string fragileStationEmployeeId,
            EmployeeSkill heavyStationEmployeeSkill,
            EmployeeSkill fragileStationEmployeeSkill)
        {
            StrongEmployeeStation = strongEmployeeStation;
            CarefulEmployeeStation = carefulEmployeeStation;
            HeavyStationEmployeeId = heavyStationEmployeeId;
            FragileStationEmployeeId = fragileStationEmployeeId;
            HeavyStationEmployeeSkill = heavyStationEmployeeSkill;
            FragileStationEmployeeSkill = fragileStationEmployeeSkill;
            AssignmentConfirmed = true;
        }

        public EmployeeSkill GetAssignedSkill(WorkStationType station)
        {
            if (!AssignmentConfirmed)
            {
                throw new InvalidOperationException(
                    "The employee assignment has not been confirmed.");
            }

            return station switch
            {
                WorkStationType.HeavyCargo => HeavyStationEmployeeSkill,
                WorkStationType.FragileCargo => FragileStationEmployeeSkill,
                _ => throw new ArgumentOutOfRangeException(nameof(station))
            };
        }

        public void CompleteProcessing(
            CargoProcessingResult heavyCargoResult,
            CargoProcessingResult fragileCargoResult)
        {
            HeavyCargoResult = heavyCargoResult;
            FragileCargoResult = fragileCargoResult;
            ProcessingCompleted = true;
        }

        public void Reset()
        {
            DayId = null;
            TotalCargoCount = 0;
            HeavyCargoCount = 0;
            FragileCargoCount = 0;
            destinationIds.Clear();
            ObjectiveId = null;
            BriefingPrepared = false;
            StrongEmployeeStation = default;
            CarefulEmployeeStation = default;
            HeavyStationEmployeeId = null;
            FragileStationEmployeeId = null;
            HeavyStationEmployeeSkill = default;
            FragileStationEmployeeSkill = default;
            AssignmentConfirmed = false;
            HeavyCargoResult = default;
            FragileCargoResult = default;
            ProcessingCompleted = false;
        }
    }
}
