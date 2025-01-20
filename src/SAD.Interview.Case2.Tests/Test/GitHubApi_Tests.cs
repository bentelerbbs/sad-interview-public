using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SharedLibraryTests;

namespace ApiHttpJson
{
    /// <summary>
    /// This class is Read-only, please do not modify it.
    /// </summary>
    [TestClass]
    public class GitHubApi_Tests
    {
        /// <summary>
        /// Link to the commits endpoint of GitHub API
        /// </summary>
        private const string vsCodeRepoUrl_Api = "https://api.github.com/repos/microsoft/vscode/commits";

        [TestMethod]
        public void GetRepositoryReport_2020_to_2024_1k_commits_needs_to_be_exactly_the_same_always()
        {
            GitHubApi gitHubApi = new GitHubApi();
            GitHubReport result = gitHubApi.GetRepositoryReport(vsCodeRepoUrl_Api, new DateTime(2020, 1, 1), new DateTime(2024, 1, 1), 1000);
            string resultAsJson = JsonConvert.SerializeObject(result, Formatting.Indented);
            File.WriteAllText($"Current_GitHub_Report.json", resultAsJson);
            // Console.WriteLine(resultAsJson);
            //
            if (result == null)
            {
                Assert.Fail($"GitHubReport cannot be null, please implement 'GetRepositoryReport' function properly");
            }
            Test.Compare(1000, result.RealCommitsLoaded, "RealCommitsLoaded == 1000");
            string expectedJson = File.ReadAllText($"Test/2020_01_01_TO_2024_01_01_LOAD_1000.json");
            GitHubReport gitHubReportExpected = JsonConvert.DeserializeObject<GitHubReport>(expectedJson);
            Test.Compare(gitHubReportExpected.Authors.Count, result.Authors.Count, $"Authors count ({result.Authors.Count}) == expected authors count ({gitHubReportExpected.Authors.Count})");
            string[] emailsExpected = gitHubReportExpected.Authors.Select(i => i.Email).OrderBy(i => i).ToArray();
            string[] emailsActual = result.Authors.Select(i => i.Email).OrderBy(i => i).ToArray();
            Test.Compare(emailsExpected, emailsActual, "All emails match");
            Console.WriteLine($"Checking if result json is exactly the same as expected:");
            Assert.IsTrue(expectedJson.Equals(resultAsJson, StringComparison.OrdinalIgnoreCase));
            Console.WriteLine($"Everything 100% OK");
        }
    }
}