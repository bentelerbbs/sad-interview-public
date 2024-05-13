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
        public GitHubApi()
        {

        }

        /// <summary>
        /// Create GitHubReport in the required time range, take only 'maxCommitsToLoad' commits if there are more in the range
        /// </summary>
        /// <param name="url"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="maxCommitsToLoad"></param>
        /// <returns></returns>
        public GitHubReport GetRepositoryReport(string url, DateTime from, DateTime to, int maxCommitsToLoad)
        {
            return null;
        }
    }
}
