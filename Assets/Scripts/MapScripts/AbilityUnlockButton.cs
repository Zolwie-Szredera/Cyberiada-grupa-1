using UnityEngine;

public class AbilityUnlockButton : Button
{
    public enum UnlockableAbility
    {
        None,
        Dash,
        DoubleJump
    }

    [Header("Ability Unlock")]
    public UnlockableAbility unlockAbility;
    public bool deactivateAfterUnlock = true;

    public override void Interaction()
    {
        base.Interaction();

        bool unlockedAnyAbility = UnlockSelectedAbility();
        if (unlockedAnyAbility && deactivateAfterUnlock)
        {
            gameObject.SetActive(false);
        }
    }

    private bool UnlockSelectedAbility()
    {
        if (unlockAbility == UnlockableAbility.Dash && !PlayerStats.isDashUnlocked)
        {
            PlayerStats.UnlockDash();
            Debug.Log("Dash unlocked!");
            return true;
        }

        if (unlockAbility == UnlockableAbility.DoubleJump && !PlayerStats.isDoubleJumpUnlocked)
        {
            PlayerStats.UnlockDoubleJump();
            Debug.Log("Double jump unlocked!");
            return true;
        }

        return false;
    }
}



