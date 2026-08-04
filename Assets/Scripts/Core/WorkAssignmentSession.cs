using System;
using System.Collections.Generic;

namespace BannoyasGames.CargoExit.Core
{
    public enum EmployeeSkill
    {
        Strength,
        Care
    }

    public enum WorkStationType
    {
        HeavyCargo,
        FragileCargo
    }

    public readonly struct WorkEmployee
    {
        public WorkEmployee(string id, EmployeeSkill skill)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException(
                    "An employee needs a stable identifier.",
                    nameof(id));
            }

            Id = id;
            Skill = skill;
        }

        public string Id { get; }

        public EmployeeSkill Skill { get; }
    }

    public sealed class WorkAssignmentSession
    {
        private readonly Dictionary<string, EmployeeSkill> employeeSkills = new();
        private readonly Dictionary<string, WorkStationType> employeeStations = new();
        private readonly Dictionary<WorkStationType, string> stationEmployees = new();

        public WorkAssignmentSession(IEnumerable<WorkEmployee> employees)
        {
            if (employees == null)
            {
                throw new ArgumentNullException(nameof(employees));
            }

            foreach (var employee in employees)
            {
                if (!employeeSkills.TryAdd(employee.Id, employee.Skill))
                {
                    throw new ArgumentException(
                        $"Employee id '{employee.Id}' is duplicated.",
                        nameof(employees));
                }
            }

            if (employeeSkills.Count == 0)
            {
                throw new ArgumentException(
                    "An assignment session needs at least one employee.",
                    nameof(employees));
            }
        }

        public bool IsConfirmed { get; private set; }

        public bool IsComplete =>
            stationEmployees.ContainsKey(WorkStationType.HeavyCargo) &&
            stationEmployees.ContainsKey(WorkStationType.FragileCargo);

        public bool HasImperfectAssignment =>
            IsComplete &&
            (!IsCompatible(WorkStationType.HeavyCargo) ||
             !IsCompatible(WorkStationType.FragileCargo));

        public bool TryAssign(string employeeId, WorkStationType station)
        {
            if (IsConfirmed || !employeeSkills.ContainsKey(employeeId))
            {
                return false;
            }

            if (employeeStations.TryGetValue(employeeId, out var currentStation) &&
                currentStation == station)
            {
                return true;
            }

            var hadCurrentStation = employeeStations.TryGetValue(
                employeeId,
                out currentStation);
            var stationWasOccupied = stationEmployees.TryGetValue(
                station,
                out var displacedEmployeeId);

            if (hadCurrentStation)
            {
                stationEmployees.Remove(currentStation);
            }

            if (stationWasOccupied)
            {
                employeeStations.Remove(displacedEmployeeId);
            }

            stationEmployees[station] = employeeId;
            employeeStations[employeeId] = station;

            if (hadCurrentStation && stationWasOccupied)
            {
                stationEmployees[currentStation] = displacedEmployeeId;
                employeeStations[displacedEmployeeId] = currentStation;
            }

            return true;
        }

        public bool TryGetStationFor(
            string employeeId,
            out WorkStationType station)
        {
            return employeeStations.TryGetValue(employeeId, out station);
        }

        public bool TryGetEmployeeAt(
            WorkStationType station,
            out string employeeId)
        {
            return stationEmployees.TryGetValue(station, out employeeId);
        }

        public EmployeeSkill GetEmployeeSkill(string employeeId)
        {
            if (!employeeSkills.TryGetValue(employeeId, out var skill))
            {
                throw new ArgumentException(
                    $"Unknown employee '{employeeId}'.",
                    nameof(employeeId));
            }

            return skill;
        }

        public EmployeeSkill GetAssignedSkill(WorkStationType station)
        {
            if (!stationEmployees.TryGetValue(station, out var employeeId))
            {
                throw new InvalidOperationException(
                    $"Station '{station}' has no assigned employee.");
            }

            return employeeSkills[employeeId];
        }

        public bool IsCompatible(WorkStationType station)
        {
            return stationEmployees.TryGetValue(station, out var employeeId) &&
                   employeeSkills[employeeId] == RequiredSkill(station);
        }

        public bool Confirm()
        {
            if (IsConfirmed || !IsComplete)
            {
                return false;
            }

            IsConfirmed = true;
            return true;
        }

        private static EmployeeSkill RequiredSkill(WorkStationType station)
        {
            return station switch
            {
                WorkStationType.HeavyCargo => EmployeeSkill.Strength,
                WorkStationType.FragileCargo => EmployeeSkill.Care,
                _ => throw new ArgumentOutOfRangeException(nameof(station))
            };
        }
    }
}
