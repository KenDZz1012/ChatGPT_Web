using ChatServer.Models;
using System.Threading.Tasks;

namespace ChatServer.Repositories
{
    public interface IChatRepository
    {
        Task<string> SendMsg(Message message);
    }
}
