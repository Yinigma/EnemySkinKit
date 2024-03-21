using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public abstract class BaseSkinner : Skinner
    {
        protected bool origEffectsMute;
        protected bool origVoiceMute;
        protected bool MuteEffects { get; }
        protected bool MuteVoice { get; }

        public BaseSkinner(bool muteSoundEffects, bool muteVoice)
        {
            MuteEffects = muteSoundEffects;
            MuteVoice = muteVoice;
        }

        public virtual void Apply(GameObject enemy)
        {
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            origVoiceMute = enemyAI.creatureVoice.mute;
            origEffectsMute = enemyAI.creatureSFX.mute;
            enemyAI.creatureSFX.mute = MuteEffects;
            enemyAI.creatureVoice.mute = MuteVoice;
        }

        public virtual void Remove(GameObject enemy)
        {
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            enemyAI.creatureSFX.mute = origEffectsMute;
            enemyAI.creatureVoice.mute = origVoiceMute;
        }
    }

}