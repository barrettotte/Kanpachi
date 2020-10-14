using System;
using System.IO;
using System.Net;
using System.Security;
using Kanpachi.Lib;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kanpachi.CLI{

    public class ProfileService : BaseService{

        public ProfileService(): base(){}
        public ProfileService(KanpachiClient client): base(client){}

        private readonly string profilesPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".kanpachi", "profiles");


        // Create new connection profile (prompting for host,user,password)
        public void AddProfile(string name){
            if(ProfileExists(name)){
                throw new KanpachiException($"Profile '{name}' already exists.");
            }
            WriteProfile(new KanpachiProfile(name, GetCredentials()));
            Console.WriteLine($"Added profile '{name}'.");
        }

        // Create new connection profile
        public void AddProfile(string name, NetworkCredential creds){
            if(ProfileExists(name)){
                throw new KanpachiException($"Profile '{name}' already exists.");
            }
            //TODO: regex [A-Za-z0-9_]+   bool IsValidProfileName()

            WriteProfile(new KanpachiProfile(name, creds));
            Console.WriteLine($"Added profile '{name}'.");
        }

        // List profiles present in local cache
        public void ListProfiles(){
            foreach(var f in Directory.GetFiles(profilesPath, "*.json")){
                string[] splitPath = f.Split(Path.DirectorySeparatorChar);
                KanpachiProfile profile = ReadProfile(splitPath[splitPath.Length-1].Split('.')[0]);

                Console.WriteLine((profile.IsDefault ? "*": "") + $"{profile.Name}");
            }
        }

        // Remove profile from local cache
        public void RemoveProfile(string name){
            if(GetDefaultProfile() == name){
                throw new KanpachiException($"Cannot remove default profile '{name}'");
            } 
            else if(!ProfileExists(name)){
                throw new KanpachiException($"Profile '{name}' does not exist.");
            }
            File.Delete(GetProfilePath(name));
            Console.WriteLine($"Removed profile '{name}'.");
        }

        // Get default profile from local cache
        public string GetDefaultProfile(){
            foreach(var filename in Directory.GetFiles(profilesPath, "*.json")){
                using(StreamReader f = File.OpenText(filename)){
                    JsonSerializer serializer = new JsonSerializer();
                    KanpachiProfile profile = (KanpachiProfile) serializer.Deserialize(f, typeof(KanpachiProfile));

                    if(profile.IsDefault){
                        return profile.Name;
                    }
                }
            }
            throw new KanpachiException("Could not find a default profile set.");
        }

        // Set default profile from profiles in local cache
        public void SetDefaultProfile(string nextDefault){
            try{
                KanpachiProfile lastProfile = ReadProfile(GetDefaultProfile());
                lastProfile.IsDefault = false;
                WriteProfile(lastProfile);
            } catch(KanpachiException){
                Console.WriteLine("No default profile was found.");
            }
            KanpachiProfile nextProfile = ReadProfile(nextDefault);
            nextProfile.IsDefault = true;
            WriteProfile(nextProfile);

            Console.WriteLine($"Set default profile to '{nextDefault}'");
        }

        // Write profile to json file
        private void WriteProfile(KanpachiProfile profile){
            string profilePath = GetProfilePath(profile.Name);

            using(StreamWriter f = File.CreateText(profilePath)){
                f.Write(JsonConvert.SerializeObject(profile, Formatting.Indented, new JsonSerializerSettings{
                     ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));
                //f.Write(JsonConvert.SerializeObject(profile, Formatting.Indented));
            }
        }

        // Read profile from json file
        private KanpachiProfile ReadProfile(string profileName){
            string profilePath = GetProfilePath(profileName);

            if(!File.Exists(profilePath)){
                throw new KanpachiException($"Could not find profile at '{profilePath}'");
            }
            using(StreamReader f = File.OpenText(profilePath)){
                JsonSerializer serializer = new JsonSerializer();
                return (KanpachiProfile) serializer.Deserialize(f, typeof(KanpachiProfile));
            }
        }

        // get full profile path
        private string GetProfilePath(string name){
            return Path.Combine(profilesPath, $"{name}.json");
        }

        // check if profile exists
        private bool ProfileExists(string name){
            return File.Exists(GetProfilePath(name));
        }

        // Get host, user, and password interactively
        private NetworkCredential GetCredentials(string inHost="", string inUser=""){
            string user = string.Empty;
            string host = string.Empty;
            SecureString pwd = new SecureString();

            if(string.IsNullOrEmpty(inHost)){
                Console.Write($"Enter host: ");
                inHost = Console.ReadLine();

                if(string.IsNullOrEmpty(inHost)){
                    throw new KanpachiException("Host cannot be blank.");
                }
            }
            host = inHost;

            if(string.IsNullOrEmpty(inUser)){
                Console.Write($"Enter user for {host}: ");
                inUser = Console.ReadLine();

                if(string.IsNullOrEmpty(inUser)){
                    throw new KanpachiException("User cannot be blank.");
                }
            }
            user = inUser;

            Console.Write($"Enter password for {user}@{host}: ");
            while(true){
                ConsoleKeyInfo i = Console.ReadKey(true);
                if(i.Key == ConsoleKey.Enter){
                    break;
                } else if(i.Key == ConsoleKey.Backspace && pwd.Length > 0){
                    pwd.RemoveAt(pwd.Length - 1);
                    Console.Write("\b \b");
                } else if(i.KeyChar != '\u0000'){
                    pwd.AppendChar(i.KeyChar);
                    Console.Write("*");
                }
            }
            Console.Write("\n");
            return new NetworkCredential(user, pwd, host);
        }
    }
}