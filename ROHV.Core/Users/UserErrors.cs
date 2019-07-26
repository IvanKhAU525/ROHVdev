using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ROHV.Core.User
{
    public class UserErrors
    {
        public const Int32 ERROR_USER_BLOCKED = 1;
        public const Int32 ERROR_USER_NO_ACTIVE = 2;
        public const Int32 ERROR_WRONG_DATA = 3;
        public const Int32 ERROR_USER_PASSWOD_INCORECT = 4;
        public const Int32 ERROR_USER_LOCK_OUT = 5;

        public static String GetErrorString(Int32 code)
        {
            switch (code)
            {
                case ERROR_USER_BLOCKED:
                    {
                        return "The user was blocked by some reason. Please contact to support.";
                    }
                case ERROR_USER_NO_ACTIVE:
                    {
                        return "The user has't activited yet. Please contact to support.";
                    }
                case ERROR_WRONG_DATA:
                    {
                        return "The user has used corrupted auth data. Please contact to support team.";
                    }
                case ERROR_USER_PASSWOD_INCORECT:
                    {
                        return "The email or password is incorrect.";
                    }
                case ERROR_USER_LOCK_OUT:
                    {
                        return "The user was locked out. Please contact to support team.";
                    }  
            }
            return "";
            
        }

        public static Object GetErrorObj(Int32 code)
        {
            return new { result = "fail", error = GetErrorString(code) };
        }

    }
}
