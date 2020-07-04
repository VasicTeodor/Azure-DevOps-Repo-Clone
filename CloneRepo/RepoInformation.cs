using System;
using System.Collections.Generic;
using System.Text;

namespace CloneRepo
{
    public class RepoInformation
    {
        public List<Repo> Value { get; set; }
    }
    public class Repo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public Project Project { get; set; }
        public string DefaultBranch { get; set; }
        public long Size { get; set; }
        public string RemoteUrl { get; set; }
        public string SshUrl { get; set; }
        public string WebUrl { get; set; }
    }

    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string State { get; set; }
        public int Revision { get; set; }
        public string Visibility { get; set; }
        public DateTime LastUpdateTime { get; set; }
    }
}
