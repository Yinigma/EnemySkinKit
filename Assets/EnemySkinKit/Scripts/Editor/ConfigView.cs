using System;

namespace AntlerShed.EnemySkinKit
{
    [Serializable]
    internal struct DefaultSkinConfigurationView
    {
        public string skinId;
        public DefaultMoonFrequencyView[] defaultEntries;
        public float defaultFrequency;
        public float vanillaFallbackFrequency;
    }

    [Serializable]
    internal struct DefaultMoonFrequencyView
    {
        public string moonId;
        public float frequency;
    }

}