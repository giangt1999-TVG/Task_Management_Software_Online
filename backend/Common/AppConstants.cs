using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.Common
{
    public class AppConstants
    {
        public const string VIETNAMESE = "vietnamese";
        public const string ENGLISH = "english";

        public class TaskStatus
        {
            public const int NONE = 0;
            public const int CREATE = 1;
            public const int REVIEW = 2;
            public const int UPDATE_AFTER_REVIEW = 3;
            public const int DONE = 4;
        }

        public class TaskPriority
        {
            public const int LOW = 1;
            public const int MEDIUM = 2;
            public const int HIGH = 3;
        }

        public class Dependency
        {
            public const int START_TO_FINISH = 1;
            public const int FINISH_TO_FINISH = 2;
        }
    }
}
