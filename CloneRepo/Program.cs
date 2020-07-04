using RestSharp;
using System;
using System.Buffers.Text;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Text;

namespace CloneRepo
{
    class Program
    {
        static void Main(string[] args)
        {
            var localRepoLoaction = "";
            var originRepoLocation = "";
            var personalToken = "";

            Console.WriteLine("Azure Devops copy to local repo!");
            Console.WriteLine("Write location to local repo: (C:\\Projects\\MeridianPortal)");
            localRepoLoaction = Console.ReadLine();
            Console.WriteLine("Write location to your Azure Devops location:");
            originRepoLocation = Console.ReadLine();
            Console.WriteLine("Enter your personal token from Azure DevOps:");
            personalToken = Console.ReadLine();
            CloneWholeAzureDevopsRepo(personalToken, localRepoLoaction, originRepoLocation);
        }

        private static void CloneWholeAzureDevopsRepo(string personalToken, string localRepoLocation = "C:\\Projects\\MeridianPortal", 
            string originRepoLoaction = "https://bluecielo.visualstudio.com/DefaultCollection/M360/_apis/git/repositories")
        {
            bool success = false;
            var client = new RestClient();
            var request = new RestRequest(originRepoLoaction, Method.GET);
            var tokenArray = Encoding.UTF8.GetBytes($":{personalToken}");
            var token = Convert.ToBase64String(tokenArray);
            request.AddHeader("Authorization", $"Basic {token}");
            IRestResponse<RepoInformation> response = client.Execute<RepoInformation>(request);
            var numberOfRepos = 0;
            using (PowerShell powerShell = PowerShell.Create())
            {
                foreach (var item in response.Data.Value)
                {
                    powerShell.AddScript($"git clone {item.RemoteUrl} {localRepoLocation}\\{item.Name}");
                    numberOfRepos += 1;
                }
                Console.Clear();
                Console.WriteLine("Cloning of remote repo is in progress");
                Collection<PSObject> results = powerShell.Invoke();
                success = results.Count == numberOfRepos;
                Console.WriteLine($"Number of Repos: {numberOfRepos}, Number of items to clone: {results.Count}");
            }

            if (success)
            {
                Console.WriteLine("Git clone of whole Azure Devops reop finished");
            }
            else
            {
                Console.WriteLine("Git clone of whole Azure Devops reop failed");
            }
            Console.ReadKey(true);

        }
    }
}
