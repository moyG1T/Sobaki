namespace Sobaki.Domain.IServices
{
    public interface INavService
    {
        void Push();
        void Pop();
        void PopAndPush();
    }
}
