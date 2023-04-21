using System;
using System.Collections.Generic;
using System.IO;
using Controlpanel.Model;
using Newtonsoft.Json;

namespace Controlpanel.DAL
{
    public class AccountDataAccess : CRUD<Account>
    {
        private readonly string _rootFolder = $"{AppDomain.CurrentDomain.BaseDirectory}\\Accounts";

        public AccountDataAccess()
        {
            CreateRootFolder();
        }
        public bool Create(Account account)
        {
            if(File.Exists($"{account.Username}.json")) return false;
            string json = JsonConvert.SerializeObject(account);
            File.WriteAllText(GetUserFile(account), json);
            return true;
        }

        public Account Get(string id)
        {
            throw new System.NotImplementedException();
        }
        
        public List<Account> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public bool Update(Account account)
        {
            throw new System.NotImplementedException();
        }

        public bool Delete(Account account)
        {
            throw new System.NotImplementedException();
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
    }
}