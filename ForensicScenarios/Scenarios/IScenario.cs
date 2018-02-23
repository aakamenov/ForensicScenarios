namespace ForensicScenarios.Scenarios
{
    public interface IScenario
    {
        void Run();
        bool IsSelected { get; set; }
        string Description { get; }
        string Status { get; }
    }
}
