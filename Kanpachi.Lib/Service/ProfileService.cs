using System;
using System.IO;
using System.Net;
using System.Security;
using Kanpachi.Lib;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kanpachi.CLI{

    public class ProfileService: BaseService{

        private string ProfilesPath;

        public ProfileService(): base(){
            ProfilesPath = Path.Combine(KanpachiPath, "profiles");
        }

        //TODO: ??? bool IsValidProfileName() => regex [A-Za-z0-9_]+   
        //TODO: ??? void UpdateProfile(KanpachiProfile profile)  =>  route to AddProfile if not exist


        // Create new connection profile (prompting for host,user,password)
        public void AddProfile(string name){
            if(ProfileExists(name)){
                throw new KanpachiException($"Profile '{name}' already exists.");
            }
            NetworkCredential creds = GetCredentials();
            AddProfile(name, creds.Domain, creds.UserName, creds.Password);
        }

        // Create new connection profile
        public void AddProfile(string name, string host, string user, string password){
            if(ProfileExists(name)){
                throw new KanpachiException($"Profile '{name}' already exists.");
            }
            KanpachiProfile profile = new KanpachiProfile(name, host, user);

            if(password.Length > 0){
                profile.PasswordDecrypted = password;
                profile.Password = SecUtils.EncryptProfile(profile);
            }
            WriteProfile(profile);
            Console.WriteLine($"Added profile '{name}'.");
        }

        // List profiles present in local cache
        public void ListProfiles(){
            foreach(var f in Directory.GetFiles(ProfilesPath, "*.json")){
                string[] splitPath = f.Split(Path.DirectorySeparatorChar);
                KanpachiProfile profile = ReadProfile(splitPath[splitPath.Length-1].Split('.')[0]);

                Console.WriteLine((profile.IsActive ? "*": "") + $"{profile.Name}");
            }
        }

        // Remove profile from local cache
        public void RemoveProfile(string name){
            if(GetActiveProfile().Name == name){
                throw new KanpachiException($"Cannot remove active profile '{name}'");
            } 
            else if(!ProfileExists(name)){
                throw new KanpachiException($"Profile '{name}' does not exist.");
            }
            File.Delete(GetProfilePath(name));
            Console.WriteLine($"Removed profile '{name}'.");
        }

        // Get active profile from local cache
        public KanpachiProfile GetActiveProfile(){
            foreach(var filename in Directory.GetFiles(ProfilesPath, "*.json")){
                using(StreamReader f = File.OpenText(filename)){
                    JsonSerializer serializer = new JsonSerializer();
                    KanpachiProfile profile = (KanpachiProfile) serializer.Deserialize(f, typeof(KanpachiProfile));

                    if(profile.IsActive){
                        profile.PasswordDecrypted = SecUtils.DecryptProfile(profile);
                        return profile;
                    }
                }
            }
            throw new KanpachiException("Could not find an active profile.");
        }

        // Set active profile from profiles in local cache
        public void SetActiveProfile(string nextActive){
            try{
                KanpachiProfile lastProfile = GetActiveProfile();
                lastProfile.IsActive = false;
                WriteProfile(lastProfile);
            } catch(KanpachiException){
                Console.WriteLine("No active profile was found.");
            }
            KanpachiProfile nextProfile = ReadProfile(nextActive);
            nextProfile.IsActive = true;
            WriteProfile(nextProfile);

            Console.WriteLine($"Set active profile to '{nextActive}'");
        }

        // Write profile to json file
        public void WriteProfile(KanpachiProfile profile){
            using(StreamWriter f = File.CreateText(GetProfilePath(profile.Name))){
                if(profile.PasswordDecrypted != null && profile.PasswordDecrypted.Length > 0){
                    profile.Password = SecUtils.EncryptProfile(profile);
                }
                f.Write(JsonConvert.SerializeObject(profile, Formatting.Indented, new JsonSerializerSettings{
                     ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));
            }
        }

        // Read profile from json file
        public KanpachiProfile ReadProfile(string profileName){
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
            return Path.Combine(ProfilesPath, $"{name}.json");
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