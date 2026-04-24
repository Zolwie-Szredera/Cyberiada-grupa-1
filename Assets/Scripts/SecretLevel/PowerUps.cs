using UnityEngine;

namespace SecretLevel
{
    public enum PowerUpType
    {
        [InspectorName("okaut")]
        DamageBoost,

        [InspectorName("ieśmiertelność")]
        Invincibility,

        [InspectorName("atarcie")]
        Onslaught,

        [InspectorName("adwaga")]
        Slowness,

        [InspectorName("iewidzialność")]
        Invisibility,

        [InspectorName("asycenie")]
        Saturation
    }
}