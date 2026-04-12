public class ForceEncounterStart : Action
{
    public EncounterHandler[] encounterHandlersToStart;
    public override void ExecuteAction()
    {
        foreach (EncounterHandler handler in encounterHandlersToStart)
        {
            handler.StartEncounter();
        }
    }
    public override void UndoAction()
    {
        foreach (EncounterHandler handler in encounterHandlersToStart)
        {
            handler.ResetEncounter();
        }
    }
}
