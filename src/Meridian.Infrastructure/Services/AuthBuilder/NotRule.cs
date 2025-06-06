namespace Meridian.Infrastructure.Services.AuthBuilder;

using Core;
using Core.Interfaces.AuthBuilder;

internal class NotRule(IAssignmentRule inner) : IAssignmentRule
{
    public bool IsUserAuthorized(UserContext user) => !inner.IsUserAuthorized(user);
}