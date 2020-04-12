using Microsoft.AspNetCore.Authorization;
using System.Reflection;

namespace MailManagement.Scope
{
	public static class AuthorizationOptionsExtensions
	{
		public static void AddScopePolicy(this AuthorizationOptions options, string issuer, string scope)
		{
			options.AddPolicy(scope, policy =>
			{
				policy.Requirements.Add(new ScopeRequirement(issuer, scope));
			});
		}

		public static void AddScopePolicies(this AuthorizationOptions options, string issuer)
		{
			FieldInfo[] fields = typeof(Scopes).GetFields(
				BindingFlags.Public | BindingFlags.Static);
			
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.IsLiteral && !fieldInfo.IsInitOnly)
				{
					string scope = (string)fieldInfo.GetRawConstantValue();
					AddScopePolicy(options, issuer, scope);
				}
			}
		}
	}
}
