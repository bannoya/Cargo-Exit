using System.Collections.Generic;
using System.Linq;

namespace BannoyasGames.CargoExit.Core
{
    public sealed class DeliveryResult
    {
        private DeliveryResult(
            bool succeeded,
            string failingParcelId,
            IEnumerable<string> blockingParcelIds)
        {
            Succeeded = succeeded;
            FailingParcelId = failingParcelId;
            BlockingParcelIds = blockingParcelIds?.Distinct().ToArray() ?? new string[0];
        }

        public bool Succeeded { get; }
        public string FailingParcelId { get; }
        public IReadOnlyList<string> BlockingParcelIds { get; }

        public static DeliveryResult Success()
        {
            return new DeliveryResult(true, null, null);
        }

        public static DeliveryResult Blocked(string failingParcelId, IEnumerable<string> blockers)
        {
            return new DeliveryResult(false, failingParcelId, blockers);
        }
    }
}

