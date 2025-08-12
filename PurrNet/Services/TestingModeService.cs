namespace Purrnet.Services
{
    public class TestingModeService
    {
        public bool IsTestingMode { get; }

        public TestingModeService(bool isTestingMode)
        {
            IsTestingMode = isTestingMode;
        }
    }
}
