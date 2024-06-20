using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPackageTool.Core.Enums
{
    public enum RelationshipType
    {
        NONE = 0,
        FRIEND = 1,
        BLOCKED = 2,
        PENDING_INCOMING = 3,
        PENDING_OUTGOING = 4,
        IMPLICIT = 5
    }
}
