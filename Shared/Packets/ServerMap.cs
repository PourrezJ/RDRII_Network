using MessagePack;
using System.Collections.Generic;

namespace Shared
{
    [MessagePackObject]
    public class ServerMap
    {
        public ServerMap()
        {
            Objects = new Dictionary<int, EntityPropertiesAbstract>();
            Vehicles = new Dictionary<int, VehicleProperties>();
            Blips = new Dictionary<int, BlipProperties>();
            Markers = new Dictionary<int, MarkerProperties>();
            Pickups = new Dictionary<int, PickupProperties>();
            Players = new Dictionary<int, PlayerProperties>();
            TextLabels = new Dictionary<int, TextLabelProperties>();
            Peds = new Dictionary<int, PedProperties>();
            Particles = new Dictionary<int, ParticleProperties>();
        }

        [Key(0)]
        public Dictionary<int, EntityPropertiesAbstract> Objects { get; set; }

        [Key(1)]
        public Dictionary<int, VehicleProperties> Vehicles { get; set; }

        [Key(2)]
        public Dictionary<int, BlipProperties> Blips { get; set; }

        [Key(3)]
        public Dictionary<int, MarkerProperties> Markers { get; set; }

        [Key(4)]
        public Dictionary<int, PickupProperties> Pickups { get; set; }

        [Key(5)]
        public Dictionary<int, PlayerProperties> Players { get; set; }

        [Key(6)]
        public Dictionary<int, TextLabelProperties> TextLabels { get; set; }

        [Key(7)]
        public Dictionary<int, PedProperties> Peds { get; set; }

        [Key(8)]
        public Dictionary<int, ParticleProperties> Particles { get; set; }

        [Key(9)]
        public WorldProperties World { get; set; }
    }
}