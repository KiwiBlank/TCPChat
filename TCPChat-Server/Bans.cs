using System;
using System.IO;

namespace TCPChat_Server
{
    class Bans
    {
        public static string bansFilePath;
        public static string bansFileName;
        public static string bansFileCombined;
        public static void CreateBanFile()
        {
            bansFilePath = String.Format(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TCPChat"));
            bansFileName = "bans.dat";
            bansFileCombined = Path.Combine(bansFilePath, bansFileName);
            BanFileCheck();
        }
        public static void AddNewBan(string IP)
        {
            BanFileCheck();
            File.AppendAllText(bansFileCombined, IP);
        }
        public static bool IsBanned(string IP)
        {
            BanFileCheck();
            foreach (string line in File.ReadLines(bansFileCombined))
            {
                if (line.Contains(IP))
                {
                    return true;
                }
            }
            return false;
        }
        public static void BanFileCheck()
        {
            if (!Directory.Exists(bansFilePath) || !File.Exists(bansFileCombined))
            {
                Directory.CreateDirectory(bansFilePath);
                File.Create(bansFileCombined);
            }
        }
    }
}
