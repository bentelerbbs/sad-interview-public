using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAD.Interview.Case2.Tests
{
    public class GitHubReport
    {
        public string Url { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int CommitsToLoad { get; set; }
        /// <summary>
        /// Ordered by commits count from highest to lowest
        /// </summary>
        public List<AuthorInfo> Authors { get; set; } = new List<AuthorInfo>();
        public int RealCommitsLoaded => Authors.Sum(a => a.Commits);

        public GitHubReport(string url, DateTime from, DateTime to, int commitsToLoad)
        {
            Url = url;
            From = from;
            To = to;
            CommitsToLoad = commitsToLoad;
        }
    }

    public class AuthorInfo
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int Commits { get; set; }

        public AuthorInfo(string name, string email, int commits)
        {
            Name = name;
            Email = email;
            Commits = commits;
        }
    }
}
