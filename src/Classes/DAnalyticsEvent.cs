﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Package_Tool.Classes
{
    public class DAnalyticsEvent
    {
        public string event_type;

        public string guild; // the guild id on invite events
        public string invite;

        public string guild_id;
        public string join_type;
        public string join_method;
        public string application_id;
        public string location;
        public string invite_code;
        public string timestamp;
    }
}
