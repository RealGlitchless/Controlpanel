using System.Collections.Generic;
using Controlpanel.Model;

namespace Controlpanel.Controller;

public class PresetController
{
    private readonly AccountController _accountController = new();
    private readonly Account _user;
    public PresetController(Account user)
    {
        _user = user;
    }
        
    public bool Create(Preset preset)
    {
        _user.Presets.Add(preset);
        _accountController.UpdateAccount(_user);
        return true;
    }

    public Preset Get(string name)
    {
        return _user.Presets.Find(preset => preset.Name == name);
    }

    public List<Preset> GetAll()
    {
        return _user.Presets;
    }

    public bool Update(Preset preset)
    {
        Preset oldPreset = Get(preset.Name);
        if (oldPreset == null)
            return false;
        oldPreset.URL = preset.URL;
        _accountController.UpdateAccount(_user);
        return true;
    }

    public bool Delete(Preset preset)
    {
        return _user.Presets.Remove(preset);
    }
}