using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApiHttpJson
{
    public class GitHubApi
    {
        #region Help
        /// <summary>
        /// Link to the GitHub commits endpoint documentation
        /// </summary>
        private const string gitHubAPiDocsUrl = "https://docs.github.com/en/rest/commits/commits?apiVersion=2022-11-28";

        /// <summary>
        /// Link to the repository for humans
        /// </summary>
        private const string vsCodeRepoUrl = "https://github.com/microsoft/vscode";
        #endregion

        public GitHubApi()
        {

        }

        /// <summary>
        /// Create GitHubReport in the required time range, limit the returned count of commits by 'maxCommitsToLoad'.
        /// </summary>
        /// <param name="commitsUrl">Complete commits API URL of the repository. Example: https://api.github.com/repos/microsoft/vscode/commits</param>
        /// <param name="from">Filter to include only commits since this date</param>
        /// <param name="to">Filter to include only commits until this date</param>
        /// <param name="maxCommitsToLoad"></param>
        /// <returns></returns>
        public GitHubReport GetRepositoryReport(string commitsUrl, DateTime from, DateTime to, int maxCommitsToLoad)
        {
            throw new NotImplementedException();
        }
    }
}
