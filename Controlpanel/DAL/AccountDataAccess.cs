using System;
using System.Collections.Generic;
using System.IO;
using Controlpanel.Model;
using Controlpanel.Utilities;
using Newtonsoft.Json;

namespace Controlpanel.DAL
{
    public class AccountDataAccess
    {
        private readonly string _rootFolder = $"{AppDomain.CurrentDomain.BaseDirectory}\\Accounts";

        public AccountDataAccess()
        {
            CreateRootFolder();
        }
        public bool Create(Account account)
        {
            if(File.Exists(GetUserFile(account))) return false;
            string json = JsonConvert.SerializeObject(account);
            json = Obfuscator.encode(json);
            File.WriteAllText(GetUserFile(account), json);
            return true;
        }

        public Account Get(string username)
        {
            if(!File.Exists($"{_rootFolder}\\{username}.json")) return null;
            string json = File.ReadAllText($"{_rootFolder}\\{username}.json");
            json = Obfuscator.decode(json);
            return JsonConvert.DeserializeObject<Account>(json);
        }
        
        public List<Account> GetAll()
        {
            List<Account> accounts = new List<Account>();
            foreach (string file in Directory.GetFiles(_rootFolder))
            {
                string json = File.ReadAllText(file);
                json = Obfuscator.decode(json);
                accounts.Add(JsonConvert.DeserializeObject<Account>(json));
            }

            return accounts;
        }

        public bool Update(Account account)
        {
            if(!File.Exists(GetUserFile(account))) return false;
            string json = JsonConvert.SerializeObject(account);
            json = Obfuscator.encode(json);
            File.WriteAllText(GetUserFile(account), json);
            return true;
        }

        public bool Delete(Account account)
        {
            if(!File.Exists(GetUserFile(account))) return false;
            File.Delete(GetUserFile(account));
            return true;
        }
        
        private void CreateRootFolder()
        {
            if (!Directory.Exists(_rootFolder))
            {
                Directory.CreateDirectory(_rootFolder);
            }
        }

        private string GetUserFile(Account user)
        {
            return $"{_rootFolder}\\{user.Username}.json";
        }

        private Account ConvertFromJson(string json)
        {
            return JsonConvert.DeserializeObject<Account>(json);
        }
    }
}