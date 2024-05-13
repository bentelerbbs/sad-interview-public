using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiHttpJson
{
    /// <summary>
    /// This class is ready, please do NOT change
    /// </summary>
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
