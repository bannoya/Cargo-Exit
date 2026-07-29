using System;

namespace BannoyasGames.CargoExit.Core
{
    public sealed class ParcelDefinition
    {
        public ParcelDefinition(string id, int deliveryOrder, ParcelShape shape)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("A parcel needs an id.", nameof(id));
            }

            if (deliveryOrder < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(deliveryOrder),
                    "Delivery order starts at one.");
            }

            Id = id;
            DeliveryOrder = deliveryOrder;
            Shape = shape ?? throw new ArgumentNullException(nameof(shape));
        }

        public string Id { get; }
        public int DeliveryOrder { get; }
        public ParcelShape Shape { get; }
    }
}

