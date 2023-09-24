using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tomlyn;
using Tomlyn.Model;
using Tomlyn.Parsing;
using Tomlyn.Syntax;

namespace DarkSoulsModManager
{
    public partial class ME2 : Form
    {
        private string steamLib, game, selectedDrive, modengine, moddingPath, modDirConfig, userPath;                         // paths
        private string saveDir, saveDirFile, saveSpot, saveSpotFile, backupDir, backupDirFile, backupSpot, backupSpotFile;    // Save backup
        private string text, gameTracker, gameDirConfig, memory, me2, me2Config, modTool, tool, modList, activeModList;       // string objects
        private string[] lines, dirs, moddingDir, modFiles, tools, games;                                                     // Line and file navigation
        private List<Mod> mods, activeMods, listedMods, unlistedMods;

        private List<String> gameFiles, trueMods, modLines, moddableGames;

        private Mod activeMod;
        private ModManager manager;

        #region startup
        public ME2()
        {
            InitializeComponent();
            loadTooltips();

            // Populate gamesDD with moddable games
            moddableGames = new List<string>();
            //moddableGames.Add("Dark Souls Prepare to Die Edition");
            //moddableGames.Add("DARK SOULS REMASTERED");
            //moddableGames.Add("DARK SOULS II Scholar of the First Sin");
            moddableGames.Add("DARK SOULS III");
            moddableGames.Add("Sekiro");
            moddableGames.Add("ELDEN RING");

            // Set the selected game in GamesDD to Elden Ring
            gamesDD.SelectedItem = "ELDEN RING";

            // Array for figuring out if a folder is a mod
            gameFiles = new List<string>();
            gameFiles.Add("action");
            gameFiles.Add("regulation");
            gameFiles.Add("chr");
            gameFiles.Add("dcx");
            gameFiles.Add("event");
            gameFiles.Add("json");
            gameFiles.Add("map");
            gameFiles.Add("msg");
            gameFiles.Add("param");
            gameFiles.Add("parts");
            gameFiles.Add("sound");

            // Load mods
            if (File.Exists("me2 config.txt"))
            {
                me2Config = "me2 config.txt";
                activeModList = "active mod list.txt";
                me2 = File.ReadAllText(me2Config) + @"\config_eldenring.toml";
                lines = File.ReadAllLines(me2);
                moddingPath = File.ReadAllText(me2Config);
                moddingDir = Directory.GetDirectories(moddingPath);
                activeMods = new List<Mod>();
                listedMods = new List<Mod>();
                unlistedMods = new List<Mod>();
                modLines = new List<string>();
                //loadModdingTools();
            }
            else
            {
                moddingPath = "";
            }

            // Load games
            foreach (string item in moddableGames)
            {
                gamesDD.Items.Add(item);
            }

            foreach (string item in gamesDD.Items)
            {
                if (isME2())
                {
                    gamesDD.SelectedItem = item;
                    break;
                }
                else
                {
                    continue;
                }
            }
            //Console.WriteLine(gamesDD.SelectedItem);

            if (File.Exists("game dir config.txt"))
            {
                steamLib = File.ReadAllText("game dir config.txt");
                initializeTable();
            }
            else
            {
                steamLib = "";
            }

            dataGridView1.AllowUserToResizeRows = true;
        }

        private void loadTooltips()
        {
            ToolTip checkDriveToolTip = new ToolTip();
            checkDriveToolTip.SetToolTip(this.checkDrive, "Bring up info for all of your save and backup locations");

            ToolTip navToSaveToolTip = new ToolTip();
            navToSaveToolTip.SetToolTip(this.navToSave, "Click this to point to where your save is located");

            ToolTip navToBackupToolTip = new ToolTip();
            navToBackupToolTip.SetToolTip(this.navToBackup, "Click this to point to where you want your save to be backed up");

            ToolTip backupSaveToolTip = new ToolTip();
            backupSaveToolTip.SetToolTip(this.backup, "Click this to backup your save. You should do this before ever playing with mods!");

            ToolTip restoreBackupToolTip = new ToolTip();
            restoreBackupToolTip.SetToolTip(this.restore, "Click this to restore your backed up save if you're ready for more vanilla play");

            ToolTip navToME2ToolTip = new ToolTip();
            navToME2ToolTip.SetToolTip(this.navToME2, "Navigates to where Mod Engine 2's config is");

            ToolTip loadME2ToolTip = new ToolTip();
            loadME2ToolTip.SetToolTip(this.loadME2Btn, "Launches ME2's config so you can update its list with mods you add");

            ToolTip launchGameToolTip = new ToolTip();
            launchGameToolTip.SetToolTip(this.launchGame, "Doesn't work yet. Currently just tells you to launch the old school way.");

            ToolTip refreshToolTip = new ToolTip();
            refreshToolTip.SetToolTip(this.refresh, "Reloads the table with fresh information!");

            ToolTip updateToolTip = new ToolTip();
            updateToolTip.SetToolTip(this.updateModConfig, "Update the mod list in the config_eldenring_toml. ONLY click this when you add new mods!");
        }

        private void determineGame()
        {
            if (gamesDD.Text.Contains("DARK SOULS II") || gamesDD.Text.Contains("ELDEN"))
            {
                game = steamLib + "\\" + gamesDD.SelectedItem + "\\Game";
            }
            else if (gamesDD.Text.Contains("Die"))
            {
                game = steamLib + "\\" + gamesDD.Text + "\\DATA";
            }
            else
            {
                game = steamLib + "\\" + gamesDD.Text;
            }

            // Setup ability to parse modengine.ini
            if (isModEngine())
            {
                var modEngine = new ModEngine();
                modEngine.Location = this.Location;
                modEngine.Show();
                this.Hide();
            }

            else if (isPTDE())
            {
                var dad = new DSDaD();
                dad.Location = this.Location;
                dad.Show();
                this.Hide();
            }

            else if (isME2())
            {
                if (File.Exists("me2 config.txt"))
                {
                    me2 = File.ReadAllText("me2 config.txt") + "\\" + "config_eldenring.toml";
                }
                else
                {
                    me2 = steamLib + "\\" + gamesDD.SelectedItem + "\\" + "Mod Engine 2" + "config_eldenring.toml";
                }
            }

            // Remember what game the mod manager is on
            gameTracker = "game tracker.txt";
            File.WriteAllText(gameTracker, game);
            Console.WriteLine("Game: " + game);
        }


        private void updateModConfig_Click(object sender, EventArgs e)
        {
            // Pre-tick active mods
            foreach (Mod mod in mods)
            {
                foreach (string line in File.ReadAllLines(me2))
                {
                    if (line.Contains(mod.modName))
                    {
                        modLines.Add(line);
                        listedMods.Add(mod);
                        if (line.Contains("true"))
                        {
                            activeMods.Add(mod);
                            mod.active = true;
                        }
                    }
                }
                // Automatically add lines to eldenring_config.toml
                if (!listedMods.Contains(mod))
                {
                    unlistedMods.Add(mod);
                    string[] newLines = new string[lines.Length + 1];

                    for (int i = 0; i < 29; i++)
                    {
                        newLines[i] = lines[i];
                    }

                    if (!newLines.Contains(mod.modName))
                        newLines[29] = "    { enabled = false, name = \"" + mod.modName + "\", path = \"" + mod.modName + "\" },";

                    for (int i = 30; i < newLines.Length; i++)
                    {
                        newLines[i] = lines[i - 1];
                    }

                    Array.Resize(ref lines, lines.Length + 1);
                    for (int i = 0; i < newLines.Length; i++)
                    {
                        lines[i] = newLines[i];
                    }
                }
            }

            // Sort lines in eldenring_config.toml
            string[] sortedLines = new string[mods.Count];

            for (int i = 0; i < mods.Count; i++)
            {
                for (int j = 29; j < lines.Length - mods.Count; j++)
                {
                    sortedLines[i] = lines[j];
                }
            }

            for (int i = 0; i < mods.Count; i++)
            {
                Console.WriteLine(sortedLines[i]);
            }

            // Update eldenring_config.toml
            text = string.Join("\n", lines);
            File.WriteAllText(me2, text);

            // Display mods that aren't in config
            if (unlistedMods.Count > 0) Console.WriteLine("Unlisted mods:");
            foreach (Mod mod in unlistedMods)
            {
                Console.WriteLine(mod.modName);
            }
            unlistedMods.Clear();
            listedMods.Clear();
        }
        private void initializeTable()
        {
            notificationLabel.Text = "";
            // Remember save and backup spots
            selectedDrive = File.ReadAllText("game dir config.txt").Substring(0, 3);
            if (File.Exists("save dir.txt"))
            {
                saveDir = File.ReadAllText("save dir.txt");
                Console.WriteLine("Save dir: " + saveDir);
            }
            if (File.Exists("save spot.txt"))
            {
                saveSpot = File.ReadAllText("save spot.txt");
                Console.WriteLine("Save spot: " + saveSpot);
            }
            if (File.Exists("backup dir.txt"))
            {
                backupDir = File.ReadAllText("backup dir.txt");
                Console.WriteLine("Backup dir: " + backupDir);
            }
            if (File.Exists("backup spot.txt"))
            {
                backupSpot = File.ReadAllText("backup spot.txt");
                Console.WriteLine("Backup spot: " + backupSpot);
            }

            // Load game
            if (gamesDD.Text.Contains("Game"))
                gamesDD.SelectedItem = "ELDEN RING";
            determineGame();

            // Load mods
            mods = new List<Mod>();
            if (File.Exists("me2 config.txt"))
            {
                me2Config = File.ReadAllText("me2 config.txt");
                modList = File.ReadAllText("me2 config.txt");
            }
            else
            {
                modList = steamLib + "\\" + gamesDD.SelectedItem + "\\" + "Mod Engine 2";
            }
            if (File.Exists("me2 config.txt"))
            {
                webBrowser1.Url = new Uri(me2Config);
            }
            else
            {
                webBrowser1.Url = new Uri(steamLib + "\\" + gamesDD.SelectedItem + "\\" + "Mod Engine 2");
                notificationLabel.Text = steamLib;
            }
            dirs = Directory.GetDirectories(modList);

            // Fetch mod archives and trim them to mod names
            int modTrim = modList.Length;

            // These 2 things will be used to trim file extensions
            string modTrimmed;
            Console.WriteLine(modList);

            foreach (string dir in dirs)
            {
                // Add mods by checking if their extension is whatever zip a mod would be
                Mod mod = new Mod();
                modTrimmed = dir.Substring(modTrim + 1);

                // Filter to folders that contain mod files
                string[] modFolders = Directory.GetDirectories(dir);
                string[] modFiles = Directory.GetFiles(dir);
                foreach (string folder in modFolders)
                {
                    string folderName = folder.Substring(1 + dir.Length);
                    foreach (string gameFile in gameFiles)
                    {
                        if (!mods.Contains(mod) && gameFile.Contains(folderName) && !dir.Contains("backup"))
                            mods.Add(mod);
                    }
                }
                foreach (string file in modFiles)
                {
                    string fileName = file.Substring(1 + dir.Length);
                    foreach (string gameFile in gameFiles)
                    {
                        if (!mods.Contains(mod) && gameFile.Contains(fileName) && !dir.Contains("backup"))
                        {
                            mods.Add(mod);
                            Console.WriteLine(mod.modName);
                        }
                    }
                }

                // Trim file extensions from mod names
                mod.modName = modTrimmed;
            }

            // Save the chosen directory to file
            Console.ReadLine();
            dataGridView1.DataSource = mods;
            dataGridView1.AutoResizeColumns();
        }
        #endregion

        #region me2

        private void loadME2Btn_Click(object sender, EventArgs e)
        {
            loadME2();
        }
        private void loadME2()
        {
            Process.Start(me2);
            Console.WriteLine("me2 path: " + me2);
        }

        // Code when a checkbox is clicked
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                this.dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }

            foreach (Mod mod in mods)
            {
                if (mod.modName.Equals(this.dataGridView1.Rows[e.RowIndex].Cells[1].Value))
                {
                    if (!activeMods.Contains(mod))
                        activeMods.Add(mod);
                    else
                        activeMods.Remove(mod);
                    tickMod();
                }
            }
        }

        // Code to modify ME2's config file
        private void tickMod()
        {
            // Group up the mods in ME2's config file
            modLines = new List<string>();
            trueMods = new List<string>();
            for (int i = 0; i < lines.Length; i++)
            {
                foreach (Mod mod in mods)
                {
                    if (lines[i].Contains(mod.modName))
                    {
                        lines[i] = "    { enabled = false, name = \"" + mod.modName + "\", path = \"" + mod.modName + "\" },";
                    }
                }
                foreach (Mod mod in activeMods)
                {
                    if (lines[i].Contains(mod.modName))
                    {
                        lines[i] = "    { enabled = true, name = \"" + mod.modName + "\", path = \"" + mod.modName + "\" },";
                        trueMods.Add(mod.modName);
                    }

                }
            }

            // Update Elden Ring's mod config
            text = string.Join("\n", lines);
            Console.WriteLine("Saving to " + me2);
            File.WriteAllText(me2, text);

            // Save active mods
            string modCache = string.Join("\n", trueMods);
            File.WriteAllText(activeModList, modCache);
        }

        private void GoBack_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        private void GoForward_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }
        #endregion

        #region moddingTools
        private void pickModdingDirectory_Click(object sender, EventArgs e)
        {
            initModdingTools();
        }

        private void initModdingTools()
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog()
            { Description = "Go to where your modding stuff is and click OK" })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    // Pick the file path
                    moddingPath = fbd.SelectedPath;
                    // Get files in the dir's folders
                    moddingDir = Directory.GetDirectories(fbd.SelectedPath);
                    loadModdingTools();
                }
            }
        }

        private void loadModdingTools()
        {
            // Clear items to just keep modding tools in selected directory
            toolsDD.Items.Clear();

            // item is each folder in ModdingDir
            foreach (string item in moddingDir)
            {
                modFiles = Directory.GetFiles(item);
                // modFile is each file in modFiles
                foreach (string modFile in modFiles)
                {
                    if (modFile.Contains("exe") && !modFile.Contains("config"))
                        toolsDD.Items.Add(item.Substring(moddingPath.Length + 1));
                }
            }
            modDirConfig = "mod dir config.txt";
            File.WriteAllText(modDirConfig, moddingPath);
        }

        private void toolsDD_SelectedIndexChanged(object sender, EventArgs e)
        {
            launchSelectedTool();
        }

        private void launchSelectedTool()
        {
            try
            {
                Console.WriteLine("Selected item: " + moddingPath + "\\" + toolsDD.SelectedItem);
                tools = Directory.GetFiles(moddingPath + "\\" + toolsDD.SelectedItem);
                string folderPath = "";
                string toolPath = "";
                foreach (string item in tools)
                {
                    // Rewrite generated paths to launch properly
                    if (item.Contains("exe") && !item.Contains("config"))
                    {
                        folderPath = moddingPath + "\\" + toolsDD.SelectedItem + "\\";
                        toolPath = item.Substring(0, item.Length - 4);
                        tool = toolPath.Substring(folderPath.Length, toolPath.Length - folderPath.Length);
                    }
                }
                modTool = folderPath + tool;
                Console.WriteLine("Launching " + modTool);
                Process.Start(modTool);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("Failed to launch because C# can't launch files with dots in their names");
            }
        }
        #endregion

        private void navToME2_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog()
            {
                Description = "Navigate to where Mod Engine 2 is and click OK",
                SelectedPath = game
            })
            {
                Console.WriteLine("Selected drive: " + game);
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    selectedDrive = fbd.SelectedPath;
                    modList = selectedDrive;
                }
            }

            me2Config = "me2 config.txt";
            File.WriteAllText(me2Config, modList);
            initializeTable();
        }

        #region saveBackup
        private void navToSave_Click(object sender, EventArgs e)
        {
            navigateToSave();
        }

        private void navigateToSave()
        {
            // Navigate to save
            using (FolderBrowserDialog fbd = new FolderBrowserDialog()
            {
                Description = "Go to your user account, and the navigator will take you right to your save",
                RootFolder = Environment.SpecialFolder.UserProfile
            })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    userPath = fbd.SelectedPath;
                    saveDir = userPath + "\\AppData\\Roaming\\EldenRing";
                    foreach (string dir in Directory.GetDirectories(saveDir))
                    {
                        foreach (string file in Directory.GetFiles(dir))
                        {
                            if (file.Contains("sl2") && !file.Contains("bak"))
                            {
                                saveSpotFile = file;
                            }
                        }
                    }
                } // end wizard
            } // end fbd
            saveDirFile = "save dir.txt";
            File.WriteAllText(saveDirFile, saveDir);
            try
            {
                foreach (string dir in Directory.GetDirectories(userPath))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(dir);
                    dirInfo.Attributes = FileAttributes.Normal;
                    foreach (string file in Directory.GetFiles(saveDir))
                    {
                        FileInfo info = new FileInfo(file);
                        info.Attributes = FileAttributes.Normal;
                    }
                }
            }
            catch (UnauthorizedAccessException e)
            {
                foreach (string dir in Directory.GetDirectories(userPath))
                {
                    DirectoryInfo info = new DirectoryInfo(dir);
                    info.Attributes = FileAttributes.Normal;
                    continue;
                }
            }
            foreach (string dir in Directory.GetDirectories(saveDir))
            {
                foreach (string file in Directory.GetFiles(dir))
                {
                    if (file.Contains("sl2") && !file.Contains("bak"))
                    {
                        saveSpot = file;
                        Console.WriteLine("Save spot: " + saveSpot);
                    }
                }
            }
            File.WriteAllText("save spot.txt", saveSpot);
            notificationLabel.Text = "Save data: " + saveSpot;
        }

        private void navToBackup_Click(object sender, EventArgs e)
        {
            navigateToBackup();
        }

        private void navigateToBackup()
        {
            // Pick a backup spot
            using (FolderBrowserDialog fbd = new FolderBrowserDialog()
            {
                Description = "Now, pick where you want your save backed up to and press OK, but first, " +
                "check the folder's properties to make sure it isn't read-only, or the mod manager will panic and die"
            })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    backupDir = fbd.SelectedPath;
                }
            }

            backupDirFile = "backup dir.txt";
            backupSpotFile = "backup spot.txt";
            File.WriteAllText(backupDirFile, backupDir);
            File.WriteAllText(backupSpotFile, backupDir + @"\" + "ER0000.sl2");
        }

        private void backup_Click(object sender, EventArgs e)
        {
            if (File.Exists("save spot.txt"))
            {
                if (File.Exists("backup spot.txt"))
                {
                    if (backupDir == null) navigateToBackup();
                    else
                    {
                        Console.WriteLine("Backup dir: " + backupDir);
                        foreach (string file in Directory.GetFiles(backupDir))
                        {
                            DirectoryInfo info = new DirectoryInfo(file);
                            info.Attributes = FileAttributes.Normal;
                            File.Delete(file);
                        }
                        backupSpotFile = File.ReadAllText("backup spot.txt");

                        // Delete the save backup before replacing to prevent complications
                        if (File.Exists("backup spot.txt"))
                        {
                            DirectoryInfo info = new DirectoryInfo(backupDir);
                            info.Attributes = FileAttributes.Normal;
                            File.Delete(backupDir + @"\ER0000.sl2");
                        }

                        // Copy save file to backup
                        File.Copy(saveSpot, backupDir + @"\ER0000.sl2");
                        notificationLabel.Text = "Save file " + saveSpot + " backed up to " + backupDir;
                    }
                }
                else
                {
                    backupSpot = backupDir + @"\ER0000.sl2";
                    navigateToBackup();
                }
            }
            else
                navigateToSave();
        }

        private void restore_Click(object sender, EventArgs e)
        {
            if (File.Exists("save spot.txt") && File.Exists("backup spot.txt"))
            {
                restoreSave();
            }
            else
            {
                navigateToSave();
                restoreSave();
            }
        }

        private void restoreSave()
        {
            if (File.Exists("backup dir.txt")) backupSpot = backupDir + @"\ER0000.sl2";

            else
            {
                navigateToBackup();
                backupSpot = backupDir + @"ER0000.sl2";
            }

            if (File.Exists(saveSpot))
            {
                DirectoryInfo info = new DirectoryInfo(saveSpot);
                info.Attributes = FileAttributes.Normal;
                File.Delete(saveSpot);
            }
            File.Copy(backupSpot, saveSpot);
            notificationLabel.Text = "Backup restored to " + backupDir;
        }
        private void checkDrive_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Save dir: " + saveDir +
                "\n" + "Save spot: " + saveSpot +
                "\n" + "Backup dir: " + backupDir +
                "\n" + "Backup spot: " + backupDir + @"\ER0000.sl2");
        }
        #endregion

        #region launch
        private void launchGame_Click(object sender, EventArgs e)
        {
            //string batFilePath = me2Config + "\\launchmod_eldenring.bat";
            //ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe", "/c " + batFilePath);
            //processStartInfo.CreateNoWindow = true;
            //processStartInfo.UseShellExecute = false;
            //Process process = new Process();
            //process.StartInfo = processStartInfo;
            //Console.WriteLine("Attempting to launch " + batFilePath);
            //process.Start();

            //Process.Start(me2Config + @"\modengine2_launcher.exe -t er -c \config_eldenring.toml");
            MessageBox.Show("Please double click the bat file in the browser to launch. Can't figure out how to do this programmatically!");
        }
        #endregion

        #region infoAndMaintenance
        private void gamesDD_SelectedIndexChanged(object sender, EventArgs e)
        {
            determineGame();
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            initializeTable();
        }

        // Close the app when you click the X
        private void ME2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
        #endregion

        #region engineBools
        private bool isModEngine()
        {
            return gamesDD.Text.Contains("DARK SOULS III") || gamesDD.Text.Contains("Sekiro");
        }

        private bool isUXM()
        {
            return gamesDD.Text.Contains("Scholar") || gamesDD.Text.Contains("Remastered");
        }

        private bool isME2()
        {
            return gamesDD.Text.Contains("ELDEN");
        }

        private bool isPTDE()
        {
            return gamesDD.Text.Contains("Die");
        }
        #endregion
    }
}