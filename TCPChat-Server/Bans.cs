using System;
using System.IO;

namespace TCPChat_Server
{
    class Bans
    {
        public static string bansFileLocation;
        public static bool BanDataFileExists()
        {
            if (File.Exists(bansFileLocation))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void CreateBanFile()
        {
            bansFileLocation = String.Format(Path.Combine(Directory.GetCurrentDirectory(), "bans.dat"));
            if (!BanDataFileExists())
            {
                File.Create(bansFileLocation).Dispose();
            }
        }
        public static void AddNewBan(string IP)
        {
            if (!BanDataFileExists())
            {
                File.Create(bansFileLocation).Dispose();
            }
            File.AppendAllText(bansFileLocation, IP);
        }
        public static bool IsBanned(string IP)
        {
            if (!BanDataFileExists())
            {
                File.Create(bansFileLocation).Dispose();
            }
            foreach (string line in File.ReadLines(bansFileLocation))
            {
                if (line.Contains(IP))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
