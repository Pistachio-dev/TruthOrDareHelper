namespace TruthOrDareHelper.Modules.Targeting.Interface
{
    public interface ITargetingHandler
    {
        string? AddReferenceToCurrentTarget();

        void ClearTarget();

        bool Target(string targetFullName);

        bool TryRemoveTargetReference(string targetFullName);
    }
}
