using System.Collections.Generic;
using Controlpanel.DAL;
using Controlpanel.Model;

namespace Controlpanel.Controller;

public class AccountController
{
    private readonly AccountDataAccess _accountDataAccess;
        
    public AccountController()
    {
        _accountDataAccess = new AccountDataAccess();
    }

    public bool CreateAccount(Account account)
    {
        return _accountDataAccess.Create(account);
            
    }
        
    public Account GetAccount(string id)
    {
        return _accountDataAccess.Get(id);
    }
        
    public List<Account> GetAllAccounts()
    {
        return _accountDataAccess.GetAll();
    }
        
    public bool UpdateAccount(Account account)
    {
        return _accountDataAccess.Update(account);
    }
        
    public bool DeleteAccount(Account account)
    {
        return _accountDataAccess.Delete(account);
    }
        
}