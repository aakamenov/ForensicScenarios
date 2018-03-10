namespace ForensicScenarios.Scenarios
{
    public interface IScenario
    {
        void Run();
        bool IsSelected { get; set; }
        string Name { get; }
        string Description { get; }
    }
}
