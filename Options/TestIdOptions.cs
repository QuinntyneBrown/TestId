namespace TestId.Options;

public class TestIdOptions
{
    public const string SectionName = "TestId";
    
    public string DefaultRepository { get; set; } = string.Empty;
    public string DefaultCommit { get; set; } = string.Empty;
    public string PythonScriptPath { get; set; } = "scripts/generate_test_id.py";
}
