using System.Collections.Generic;
using System.Linq;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Exceptions;

namespace Ardalis.GuardClauses
{
    public static class ScriptGuardExtensions
    {
        public static void DuplicateScript(this IGuardClause guardClause, IEnumerable<Script> existingScripts, Script newScript, string parameterName)
        {
            if (existingScripts.Any(a => a.ScriptId == newScript.ScriptId))
            {
                throw new DuplicateScriptException("Cannot add duplicate script.", parameterName);
            }
        }
    }
}

