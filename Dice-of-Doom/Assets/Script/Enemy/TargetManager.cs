public class TargetManager
{
    public ITarget CurrentTarget { get; private set; }

    public void SetTarget(ITarget newTarget)
    {
        if (CurrentTarget == newTarget) return;

        CurrentTarget?.Deselect();
        CurrentTarget = newTarget;
        CurrentTarget?.Select();
    }

    public void ClearTarget()
    {
        CurrentTarget?.Deselect();
        CurrentTarget = null;
    }
}
