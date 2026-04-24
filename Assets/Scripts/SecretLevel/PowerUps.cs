using UnityEngine;

namespace SecretLevel
{
    public enum PowerUpType
    {
        [InspectorName("Nokaut")]
        DamageBoost,

        [InspectorName("Nieśmiertelność")]
        Invincibility,

        [InspectorName("Natarcie")]
        Onslaught,

        [InspectorName("Nadwaga")]
        Slowness,

        [InspectorName("Niewidzialność")]
        Invisibility,

        [InspectorName("Nasycenie")]
        Saturation
    }
}