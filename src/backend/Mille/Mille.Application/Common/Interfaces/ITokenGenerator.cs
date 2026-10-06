using Mille.Application.Common.DTOs;
using Mille.Domain.Entities;

namespace Mille.Application.Common.Interfaces
{
    public interface ITokenGenerator
    {
        TokenResult GenerateToken(User user);
    }
}
