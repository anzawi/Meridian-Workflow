namespace Meridian.Core.Builders.AuthBuilder;

using Contexts;
using Core;
using Meridian.Core.Interfaces.AuthBuilder;

internal class NotRule(IAssignmentRule inner) : IAssignmentRule
{
    public bool IsUserAuthorized(UserContext user) => !inner.IsUserAuthorized(user);
}