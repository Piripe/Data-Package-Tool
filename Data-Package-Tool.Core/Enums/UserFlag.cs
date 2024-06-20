using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPackageTool.Core.Enums
{
    public enum UserFlag : long
    {
        Staff = 1L<<0,
        Partner = 1L<<1,
        Hypesquad = 1L<<2,
        BugHunter1 = 1L<<3,
        HypesquadBravery = 1L<<6,
        HypesquadBrilliance = 1L<<7,
        HypesquadBalance = 1L<<8,
        EarlySupporter = 1L<<9,
        BugHunter2 = 1L<<14,
        VerifiedDeveloper = 1L<<17,
        ActiveDeveloper = 1L<<22,
    }
}
