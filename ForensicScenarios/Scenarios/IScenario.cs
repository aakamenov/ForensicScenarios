namespace ForensicScenarios.Scenarios
{
    public interface IScenario
    {
        void Run();
        string Description { get; }
        string Status { get; }
        int StatusValue { get; }
    }
}
