using System;
using System.IO;
using System.Net;
using Kanpachi.Lib;
using Newtonsoft.Json;

namespace Kanpachi.CLI{

    public class ProfileService : BaseService{

        public ProfileService(): base(){}
        public ProfileService(KanpachiClient client): base(client){}

        private readonly string profilesPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".kanpachi", "profiles");


        // Create new connection profile (prompting for host,user,password)
        public void AddProfile(string name){
            try{
                if(ProfileExists(name)){
                    throw new KanpachiException($"Profile '{name}' already exists.");
                }
                AddProfile(name, KanpachiUtils.GetCredentials());
            } catch(KanpachiException e){
                Console.WriteLine($"ERROR: {e.Message}");
            }
        }

        // Create new connection profile
        public void AddProfile(string name, NetworkCredential creds){
            WriteProfile(new KanpachiProfile(name, creds));
            Console.WriteLine($"Added profile '{name}'.");
        }

        // List profiles present in local cache
        public void ListProfiles(){
            foreach(var f in Directory.GetFiles(profilesPath, "*.json")){
                Console.WriteLine($"{f}");
            }
        }

        // Remove profile from local cache
        public void RemoveProfile(string name){
            if(!ProfileExists(name)){
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
            Console.WriteLine("WARNING: Could not find default profile.");
            return string.Empty;
        }

        // Set default profile from profiles in local cache
        public void SetDefaultProfile(string name){
            if(!ProfileExists(name)){
                throw new KanpachiException($"Profile '{name}' does not exist.");
            }
            
            KanpachiProfile lastDefault = ReadProfile(GetDefaultProfile());
            lastDefault.IsDefault = false;
            WriteProfile(lastDefault);

            KanpachiProfile newDefault = ReadProfile(name);
            newDefault.IsDefault = true;
            WriteProfile(newDefault);
            Console.WriteLine($"Set default profile to '{name}'");
        }

        // Write profile to json file
        private void WriteProfile(KanpachiProfile profile){
            string profilePath = GetProfilePath(profile.Name);

            if(File.Exists(profilePath)){
                throw new KanpachiException($"Profile at '{profilePath}' already exists.");
            }
            using(StreamWriter f = File.CreateText(profilePath)){
                // f.Write(JsonConvert.SerializeObject(profile, Formatting.Indented, new JsonSerializerSettings{
                //     ContractResolver = new CamelCasePropertyNamesContractResolver()
                // }));
                f.Write(JsonConvert.SerializeObject(profile, Formatting.Indented));
            }
        }

        // Read profile from json file
        private KanpachiProfile ReadProfile(string profileName){
            string profilePath = GetProfilePath(profileName);

            if(!File.Exists(profilePath)){
                throw new KanpachiException($"Could not read profile from '{profilePath}'");
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
    }
}