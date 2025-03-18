using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPackageTool.Core.Enums
{
    public enum UserFlag : long
    {
        STAFF = 1L<<0,
        PARTNER = 1L<<1,
        HYPESQUAD = 1L<<2,
        BUG_HUNTER_LEVEL_1 = 1L<<3,
        HYPESQUAD_ONLINE_HOUSE_1 = 1L<<6,
        HYPESQUAD_ONLINE_HOUSE_2 = 1L<<7,
        HYPESQUAD_ONLINE_HOUSE_3 = 1L<<8,
        PREMIUM_EARLY_SUPPORTER = 1L<<9,
        TEAM_PSEUDO_USER = 1L<<10,
        BUG_HUNTER_LEVEL_2 = 1L<<14,
        VERIFIED_BOT = 1L<<16,
        VERIFIED_DEVELOPER = 1L<<17,
        CERTIFIED_MODERATOR = 1L<<18,
        BOT_HTTP_INTERACTIONS = 1L<<19,
        ACTIVE_DEVELOPER = 1L<<22,

        // unofficial flags
        PREMIUM_PROMO_DISMISSED = 1L<<23,
        USED_DESKTOP_CLIENT = 1L<<24,
        USED_WEB_CLIENT = 1L<<25,
        USED_MOBILE_CLIENT = 1L<<26,
        HAS_SESSION_STARTED = 1L<<27,
    }
}
