using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kanpachi.Lib{

    public class ProfileService: BaseService{

        private string ProfilesPath;

        public ProfileService(): base(){
            ProfilesPath = Path.Combine(KanpachiPath, "profiles");

            if(!Directory.Exists(ProfilesPath)){
                Directory.CreateDirectory(ProfilesPath);
            }
        }

        // Create new connection profile (prompting for host,user,password)
        public void AddProfile(string name){
            if(ProfileExists(name)){
                throw new KanpachiProfileException($"Profile {name} already exists.");
            }
            NetworkCredential creds = ClientUtils.GetCredentials();
            AddProfile(name, creds.Domain, creds.UserName, creds.Password);
        }

        // Create new connection profile
        public void AddProfile(string name, string host, string user, string password){
            if(ProfileExists(name)){
                throw new KanpachiProfileException($"Profile {name} already exists.");
            }
            KanpachiProfile profile = new KanpachiProfile(name, host, user);

            try{
                if(GetActiveProfile(true).Name == name){
                    System.Console.WriteLine($"Profile {name} is already active.");
                    return;
                }
            } catch(KanpachiProfileException){
                profile.IsActive = true; // no active profile, so set this new one
            }

            if(password.Length > 0){
                profile.PasswordDecrypted = password;
                profile.Password = SecUtils.EncryptProfile(profile);
            }
            WriteProfile(profile);
            Console.WriteLine($"Added profile {name}.");
        }

        // List profiles present in local cache
        public void ListProfiles(){
            Console.WriteLine($"Profile configurations located at {ProfilesPath}\n");

            foreach(var f in Directory.GetFiles(ProfilesPath, "*.json")){
                string[] splitPath = f.Split(Path.DirectorySeparatorChar);
                KanpachiProfile profile = ReadProfile(splitPath[splitPath.Length-1].Split('.')[0]);
                Console.WriteLine((profile.IsActive ? "*": "") + $"{profile.Name}");
            }            
        }

        // Remove profile from local cache
        public void RemoveProfile(string name){
            if(GetActiveProfile(true).Name == name){
                throw new KanpachiProfileException($"Cannot remove active profile '{name}'");
            } 
            else if(!ProfileExists(name)){
                throw new KanpachiProfileException($"Profile {name} does not exist.");
            }
            File.Delete(GetProfilePath(name));
            Console.WriteLine($"Removed profile {name}.");
        }

        // Get active profile from local cache
        public KanpachiProfile GetActiveProfile(bool skipDecryption=false){
            foreach(var filename in Directory.GetFiles(ProfilesPath, "*.json")){
                using(StreamReader f = File.OpenText(filename)){
                    JsonSerializer serializer = new JsonSerializer();
                    KanpachiProfile profile = (KanpachiProfile) serializer.Deserialize(f, typeof(KanpachiProfile));

                    if(profile.IsActive){
                        if(!skipDecryption){
                            profile.PasswordDecrypted = SecUtils.DecryptProfile(profile);
                        }
                        return profile;
                    }
                }
            }
            throw new KanpachiProfileException("Could not find an active profile.");
        }

        // Set active profile from profiles in local cache
        public void SetActiveProfile(string nextActive){
            try{
                KanpachiProfile lastProfile = GetActiveProfile(true);
                lastProfile.IsActive = false;
                WriteProfile(lastProfile);
            } catch(KanpachiProfileException){
                Console.WriteLine("No active profile was found.");
            }
            KanpachiProfile nextProfile = ReadProfile(nextActive);
            nextProfile.IsActive = true;
            WriteProfile(nextProfile);

            Console.WriteLine($"Set active profile to {nextActive}");
        }

        // set profile value
        public void SetValue(string key, string value){
            KanpachiProfile original = GetActiveProfile();
            KanpachiProfile updated = ClientUtils.CopyProfile(original);
            // skip Name
            switch(key.ToUpper()){
                case "HOST":
                    updated.Host = value;
                    break;
                case "USER":
                    updated.User = value;
                    break;
                case "PASSWORD":
                    updated.PasswordDecrypted = value;
                    break;
                case "PORT":
                    int parsePort;
                    if(!Int32.TryParse(value, out parsePort)){
                        throw new KanpachiConfigException($"Could not parse value {value} to Int32.");
                    }
                    updated.Port = parsePort;
                    break;
                case "TIMEOUT":
                    double parseTimeout;
                    if(!Double.TryParse(value, out parseTimeout)){
                        throw new KanpachiConfigException($"Could not parse value {value} to Double.");
                    }
                    updated.Timeout = Double.Parse(value);
                    break;
                case "CONNECTATTEMPTS":
                    int parseConnectAttempts;
                    if(!Int32.TryParse(value, out parseConnectAttempts)){
                        throw new KanpachiConfigException($"Could not parse value {value} to Int32.");
                    }
                    updated.Port = parseConnectAttempts;
                    break;
                case "IFSUSERPATH":
                    updated.IfsUserPath = value;
                    break;
                case "ODBCDRIVER":
                    updated.OdbcDriver = value;
                    break;
                default:
                    throw new KanpachiConfigException("Could not find key {key}");
            }
            WriteProfile(updated);
        }

        // get connection profile value
        public void GetValue(string key){
            KanpachiProfile active = GetActiveProfile(true);
            string v = string.Empty;

            // Skip password
            switch (key.ToUpper()){
                case "NAME":
                    v = active.Name;
                    break;
                case "HOST":
                    v = active.Host;
                    break;
                case "USER":
                    v = active.User;
                    break;
                case "PORT":
                    v = active.Port.ToString();
                    break;
                case "TIMEOUT":
                    v = active.Timeout.ToString();
                    break;
                case "CONNECTATTEMPTS":
                    v = active.ConnectAttempts.ToString();
                    break;
                case "IFSUSERPATH":
                    v = active.IfsUserPath;
                    break;
                case "ODBCDRIVER":
                    v = active.OdbcDriver;
                    break;
                default: 
                    throw new KanpachiConfigException("Could not find key {key}");
            }
            Console.WriteLine(v);
        }

        // Write profile to json file
        public void WriteProfile(KanpachiProfile profile){
            if(!Directory.Exists(ProfilesPath)){
                Directory.CreateDirectory(ProfilesPath);
            }
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
                throw new KanpachiProfileException($"Could not find profile at {profilePath}");
            }
            using(StreamReader f = File.OpenText(profilePath)){
                JsonSerializer serializer = new JsonSerializer();
                return (KanpachiProfile) serializer.Deserialize(f, typeof(KanpachiProfile));
            }
        }

        // get full profile path
        public string GetProfilePath(string name){
            return Path.Combine(ProfilesPath, $"{name}.json");
        }

        // check if profile exists
        public bool ProfileExists(string name){
            return File.Exists(GetProfilePath(name));
        }
    }
}