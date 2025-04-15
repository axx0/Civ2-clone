namespace Engine.Tests.TestFiles
{
    internal static class TestFileUtils
    {
        internal static string GetTestFileDirectory()
        {
            string testDirectory = Directory.GetCurrentDirectory();
            return Path.Combine(testDirectory, "TestFiles");
        }
        internal static string GetTestFilePath(string fileName)
        {
            string testDirectory = Directory.GetCurrentDirectory();
            return Path.Combine(testDirectory, "TestFiles", fileName);
        }
    }
}
