using System;
using System.Collections.Generic;

namespace LLC_Size41.classes
{
    public static class Variables
    {
        public static string ConnStr = "server=localhost;user=root;pwd=;database=LLC_Size41";
        public static bool authClosed = true;
        public static bool mainClosed = true;
        public static string role = String.Empty;
        public static string surname = String.Empty;
        public static string name = String.Empty;
        public static string patronymic = String.Empty;
        public static List<TrashItem> trash = new List<TrashItem>();
        public static bool trashVisible = false;
    }
}
