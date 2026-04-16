namespace CDL.Models.Helpers
{
    public static class UserRoles
    {
        public const string Simple = "simple";
        public const string Editor = "editor";
        public const string Admin = "admin";

        public static bool IsValid(string? role)
        {
            if (string.IsNullOrWhiteSpace(role)) return false;
            var r = role.Trim().ToLowerInvariant();
            return r is Simple or Editor or Admin;
        }

        public static string NormalizeFromLegacy(bool admin) => admin ? Admin : Simple;

        /// <summary>Resolve role from stored value; falls back to legacy Admin flag.</summary>
        public static string Resolve(string? role, bool legacyAdmin)
        {
            if (!string.IsNullOrWhiteSpace(role) && IsValid(role))
                return role.Trim().ToLowerInvariant();
            return legacyAdmin ? Admin : Simple;
        }
    }
}
