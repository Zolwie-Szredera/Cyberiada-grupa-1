#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

// Editor helper to wire up level1 boss gates and unlock button to existing project scripts
public static class BossGateSetupEditor
{
    [MenuItem("Tools/Boss Gate/Setup Level1")] 
    public static void SetupLevel1()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene == null)
        {
            EditorUtility.DisplayDialog("Error", "No active scene.", "OK");
            return;
        }

        // optional: ensure user is in Level1 scene (case-insensitive contains)
        if (!scene.name.ToLower().Contains("level1"))
        {
            if (!EditorUtility.DisplayDialog("Scene mismatch", $"Active scene is '{scene.name}'. Continue anyway?", "Continue", "Cancel"))
                return;
        }

        // Find objects by name
        GameObject gateRight = GameObject.Find("gatebossright");
        GameObject gateLeft = GameObject.Find("gatebossleft");
        GameObject boss = GameObject.Find("encauntebossmonument");
        GameObject dashBtn = GameObject.Find("dashunlockbutton");

        if (dashBtn == null)
        {
            EditorUtility.DisplayDialog("Missing object", "Could not find GameObject named 'dashunlockbutton' in the scene.", "OK");
            return;
        }

        // Ensure AbilityUnlockButton exists and is configured
        var abilityBtn = dashBtn.GetComponent<AbilityUnlockButton>();
        if (abilityBtn == null)
            abilityBtn = dashBtn.AddComponent<AbilityUnlockButton>();

        // Set ability type to Dash
        abilityBtn.unlockAbility = AbilityUnlockButton.UnlockableAbility.Dash;
        abilityBtn.deactivateAfterUnlock = true;
        abilityBtn.executeActionOnInteract = true; // so actions array on the Button will run

        // Ensure OpenDoor action component exists on the same GameObject and points to gates
        var openDoor = dashBtn.GetComponent<OpenDoor>();
        if (openDoor == null)
            openDoor = dashBtn.AddComponent<OpenDoor>();

        // Prepare doors array
        var doorList = new System.Collections.Generic.List<GameObject>();
        if (gateRight != null) doorList.Add(gateRight);
        if (gateLeft != null) doorList.Add(gateLeft);
        openDoor.doors = doorList.ToArray();

        // Assign the OpenDoor component into abilityBtn.actions (serialized property)
        SerializedObject so = new SerializedObject(abilityBtn);
        SerializedProperty actionsProp = so.FindProperty("actions");
        if (actionsProp == null)
        {
            Debug.LogWarning("Could not find 'actions' property on AbilityUnlockButton (inherited from Button).");
        }
        else
        {
            actionsProp.arraySize = 1;
            actionsProp.GetArrayElementAtIndex(0).objectReferenceValue = openDoor as Action;
            so.ApplyModifiedProperties();
        }

        // Remove any OpenDoor action in EncounterHandler.ExecuteOnEncounterEnd on the boss object to avoid auto-opening on encounter end
        if (boss != null)
        {
            var encounter = boss.GetComponent<EncounterHandler>();
            if (encounter != null)
            {
                SerializedObject encSO = new SerializedObject(encounter);
                SerializedProperty endProp = encSO.FindProperty("ExecuteOnEncounterEnd");
                if (endProp != null)
                {
                    endProp.ClearArray();
                    encSO.ApplyModifiedProperties();
                }
            }
        }

        // Mount CloseDoorOnPass directly on both gate doors
        // This way: when player passes through a door, it closes itself + the other door
        if (gateRight != null)
        {
            var closeRight = gateRight.GetComponent<CloseDoorOnPass>();
            if (closeRight == null) closeRight = gateRight.AddComponent<CloseDoorOnPass>();
            // gateRight closes itself + gateLeft
            if (gateLeft != null)
            {
                closeRight.otherDoors = new[] { gateLeft };
            }
        }

        if (gateLeft != null)
        {
            var closeLeft = gateLeft.GetComponent<CloseDoorOnPass>();
            if (closeLeft == null) closeLeft = gateLeft.AddComponent<CloseDoorOnPass>();
            // gateLeft closes itself + gateRight
            if (gateRight != null)
            {
                closeLeft.otherDoors = new[] { gateRight };
            }
        }

        // Mark scene dirty and save
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveOpenScenes();

        EditorUtility.DisplayDialog("Boss gate setup", "Level1 boss gate setup completed.\n- dashunlockbutton configured to unlock Dash and execute OpenDoor action\n- EncounterHandler.ExecuteOnEncounterEnd cleared on boss object (if present)\n- GateExitTrigger created/configured to close doors on pass", "OK");
    }
}
#endif

