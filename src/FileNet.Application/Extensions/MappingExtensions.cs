using AutoMapper;
using System.Reflection;

namespace FileNet.Application.Extensions;

public static class MappingExtensions
{
    private static IMappingExpression<TSrc, TDest> IgnoreIfMemberExists<TSrc, TDest>(
        this IMappingExpression<TSrc, TDest> exp, string memberName)
    {
        var t = typeof(TDest);
        var hasProp = t.GetProperty(memberName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase) != null;
        var hasField = t.GetField(memberName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase) != null;

        if (hasProp || hasField)
            exp.ForMember(memberName, o => o.Ignore());

        return exp;
    }

    public static IMappingExpression<TSrc, TDest> IgnoreAudit<TSrc, TDest>(
        this IMappingExpression<TSrc, TDest> exp)
    {
        return exp
            .IgnoreIfMemberExists("CreatedBy")
            .IgnoreIfMemberExists("CreatedOn")
            .IgnoreIfMemberExists("UpdatedBy")
            .IgnoreIfMemberExists("UpdatedOn");
    }
}
