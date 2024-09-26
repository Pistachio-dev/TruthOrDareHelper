namespace TruthOrDareHelper.Modules.Chat.Interface
{
    public interface IChatListener
    {
        void AddConditionalDelegate(ConditionalDelegatePayload payload);
        void AttachListener();
    }
}