namespace Controlpanel.DAL
{
    public interface CRUD<C>
    {
        bool Create(C c);
        C Get(string id);
        bool Update(C c);
        bool Delete(C c);
    }
}