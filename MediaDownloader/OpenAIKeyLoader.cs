using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaDownloader
{
    public class OpenAIKeyLoader
    {
        public static string LoadApiKey()
        {
            string userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string appDirectory = Path.Combine(userDirectory, ".mitchcodesmediadownloader");
            string apiKeyFilePath = Path.Combine(appDirectory, "openaikey.txt");

            if (!File.Exists(apiKeyFilePath))
            {
                Console.Write("Enter your OpenAI API Key: ");
                string apiKey = Console.ReadLine().Trim();

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    throw new Exception("API key cannot be empty.");
                }

                Directory.CreateDirectory(appDirectory);
                File.WriteAllText(apiKeyFilePath, apiKey);

                return apiKey;
            }
            else
            {
                string apiKey = File.ReadAllText(apiKeyFilePath).Trim();

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    throw new Exception("API key file is empty.");
                }

                return apiKey;
            }
        }
    }
}
