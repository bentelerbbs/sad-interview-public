using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ApiHttpJson
{
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
            GitHubReport result = gitHubApi.GetRepositoryReport(vsCodeRepoUrl_Api, new DateTime(2020,1,1), new DateTime(2024,1,1), 1000);
            string resultAsJson = JsonConvert.SerializeObject(result, Formatting.Indented);
            File.WriteAllText($"Current_GitHub_Report.json", resultAsJson);
            Console.WriteLine(resultAsJson);
            string expectedJson = File.ReadAllText($"Test/2020_01_01_TO_2024_01_01_LOAD_1000.json");
            Assert.IsTrue(expectedJson.Equals(resultAsJson, StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(result.RealCommitsLoaded == 1000);
        }
    }
}