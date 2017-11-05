using UnityEngine;
using System.Collections;

#region enumerators

//public enum ShooterPlayerID { SNIPER, ASSAULT, SMG }
public enum EnemyType { HANADAR, RAJAKAR, GRENADIER, MORTAR }
public enum RepositionShowCause { SPWN, BOREDOM, PAIN, ALERT }
public enum HitType { HEAD, BODY, LIMB, BLAST, KNIFE }
public enum HitSource {PLAYER, SNEAKY_PLAYER,ENEMY, PLAYER_AI, GAYEBI }
public enum ProjectileType {NADE, SHELL, MININADE}
#endregion
#region interfaces
public interface I_AIcommon
{
    Animator selfAnimator { set; get; }
}

public interface iPreHitQuery
{
    HitResult GetHitResults(float damage);
}
public interface iBulletImpact
{
    void TakeImapct(RaycastHit hit, float damageValue, HitSource hitSource);
}
public interface iEnemyRefKeeper
{
    AIPersonnel personnelScriptProperty{ set; get;}
}
public interface iProjectileImpact
{
    void TakeImpact(float damageValue, Vector3 blastForce, HitSource hitSource, ProjectileType projectileType, bool shouldIgnoreHidingDamageReduction, bool failNade);
}
#endregion
#region mini_classes
public class HitResult
{
    public bool willDie = false;
    public float HPafterDamage = 100;
    public float damageOutput = 0;
    public AIPersonnel aipersonnelsReference;
}
public class WasHitCheck
{
    public bool wasHit = false;
}
#endregion