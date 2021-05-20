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

            do
            {
                Console.WriteLine("Azure Devops copy to local repo!");
                Console.WriteLine("Write location to local repo: (C:\\Projects\\MeridianPortal)");
                localRepoLoaction = Console.ReadLine();
                Console.WriteLine("Write location to your Azure Devops location:");
                originRepoLocation = Console.ReadLine();
                Console.WriteLine("Enter your personal token from Azure DevOps:");
                personalToken = Console.ReadLine();
            } while (string.IsNullOrEmpty(personalToken));
            CloneWholeAzureDevopsRepo(personalToken, localRepoLoaction, originRepoLocation);
        }

        private static void CloneWholeAzureDevopsRepo(string personalToken, string localRepoLocation, 
            string originRepoLocation)
        {
            if (string.IsNullOrEmpty(localRepoLocation))
            {
                localRepoLocation = "C:\\Projects\\MeridianPortal";
            }

            if (string.IsNullOrEmpty(originRepoLocation))
            {
                originRepoLocation = "https://bluecielo.visualstudio.com/DefaultCollection/M360/_apis/git/repositories";
            }
            bool success = false;
            var client = new RestClient();
            var request = new RestRequest(originRepoLocation, Method.GET);
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
                success = results.Count == 0;
                Console.WriteLine($"Number of Repos: {numberOfRepos}, Number of items to clone: {results.Count}");
            }

            if (success)
            {
                Console.WriteLine("Git clone of whole Azure Devops repo finished");
            }
            else
            {
                Console.WriteLine("Git clone of whole Azure Devops repo failed");
            }
            Console.ReadKey(true);

        }
    }
}
