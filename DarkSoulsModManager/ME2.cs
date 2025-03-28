﻿using System;
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

namespace DarkSoulsModManager
{
    public partial class ME2 : Form
    {
        private string steamLib, selectedDrive, game, selectedGame, gameSaveDir, gameSave, moddingPath, modDirConfig, userPath; // paths
        private string saveDir, saveDirFile, saveSpot, saveSpotFile, backupDir, backupDirFile, backupSpot, backupSpotFile;      // Save backup
        private string text, gameTracker, gameConfig, memory, me2, me2Config, modTool, tool, modList, activeModList;            // string objects
        private string[] lines, dirs, moddingDir, modFiles, tools, games;                                                       // Line and file navigation
        private List<Mod> mods, activeMods, listedMods, unlistedMods;

        private List<String> gameFiles, trueMods, modLines, moddableGames;
        private List<Button> buttons;
        private Mod activeMod;

        #region startup
        public ME2()
        {
            InitializeComponent();
            loadTooltips();

            trueMods = new List<string>();
            activeMods = new List<Mod>();

            // Populate gamesDD with moddable games
            moddableGames = new List<string>();
            //moddableGames.Add("Dark Souls Prepare to Die Edition");
            moddableGames.Add("DARK SOULS REMASTERED");
            moddableGames.Add("DARK SOULS II Scholar of the First Sin");
            moddableGames.Add("DARK SOULS III");
            moddableGames.Add("Sekiro");
            moddableGames.Add("ELDEN RING");
            moddableGames.Add("ARMORED CORE VI FIRES OF RUBICON");

            determineGame();

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

            // Load mods
            if (File.Exists("mod dir config.txt"))
            {
                modDirConfig = "mod dir config.txt";
                moddingPath = File.ReadAllText(modDirConfig);
                moddingDir = Directory.GetDirectories(moddingPath);
                loadModdingTools();
            }
            else
            {
                moddingPath = "";
            }

            // Initialize active mods
            if (File.Exists("active mod list.txt"))
            {
                activeModList = File.ReadAllText("active mod list.txt");
            }
            else
            {
                File.WriteAllText("active mod list.txt", "");
                activeModList = File.ReadAllText("active mod list.txt");
            }
            initializeTable();

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

            ToolTip launchToolToolTip = new ToolTip();
            launchToolToolTip.SetToolTip(this.launchTool, "Launches the tool you selected without having to menu to it again");

            ToolTip refreshToolTip = new ToolTip();
            refreshToolTip.SetToolTip(this.refresh, "Reloads the table with fresh information!");

            ToolTip updateToolTip = new ToolTip();
            updateToolTip.SetToolTip(this.updateModConfig, "Update the mod list in the config file. ONLY click this when you add new mods!");
        }

        private void determineGame()
        {
            if (gamesDD.Text.Contains("DARK SOULS II") || gamesDD.Text.Contains("ELDEN"))
            {
                game = steamLib + "\\" + gamesDD.Text + "\\Game";
            }
            else if (gamesDD.Text.Contains("Die"))
            {
                game = steamLib + "\\" + gamesDD.Text + "\\DATA";
            }
            else
            {
                game = steamLib + "\\" + gamesDD.Text;
            }

            // Flip to a different view that handles the respective tool's games
            if (isME2())
            {
                if (File.Exists("me2 config.txt"))
                {
                    me2 = File.ReadAllText("me2 config.txt") + "\\" + gameConfig;
                }
                else
                {
                    me2 = steamLib + "\\" + gamesDD.SelectedItem + "\\" + "Mod Engine 2" + gameConfig;
                }
            }

            else if (isModEngine())
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

            // Detect whether Elden Ring or Armoured Core is selected
            if (gamesDD.Text.Contains("ARMORED"))
            {
                gameConfig = @"\config_armoredcore6.toml";
                selectedGame = "ARMORED CORE VI FIRES OF RUBICON";
                gameSaveDir = @"\ArmoredCore6";
                gameSave = @"\AC0000.sl2";
            }
            else if (gamesDD.Text.Contains("ELDEN"))
            {
                gameConfig = @"\config_eldenring.toml";
                selectedGame = "ELDEN RING";
                gameSaveDir = @"\EldenRing";
                gameSave = @"\ER0000.sl2";
            }
            else if (gamesDD.Text.Contains("REMASTERED"))
            {
                gameConfig = @"\config_darksoulsremastered.toml";
                selectedGame = "DARK SOULS REMASTERED";
                gameSaveDir = @"\DARK SOULS REMASTERED";
                gameSave = @"\DRAKS0005.sl2";
            }

            // Remember what game the mod manager is on
            gameTracker = "game tracker.txt";
            File.WriteAllText(gameTracker, game);
        }


        private void initializeTable()
        {
            // Clear the notification label
            notificationLabel.Text = "";

            // Create lists for mod management
            activeMods = new List<Mod>();
            modLines = new List<string>();

            // Remember save and backup spots
            selectedDrive = File.ReadAllText("game dir config.txt").Substring(0, 3);
            if (File.Exists("save dir.txt"))
            {
                saveDir = File.ReadAllText("save dir.txt");
            }
            if (File.Exists("save spot.txt"))
            {
                saveSpot = File.ReadAllText("save spot.txt");
            }
            if (File.Exists("backup dir.txt"))
            {
                backupDir = File.ReadAllText("backup dir.txt");
            }
            if (File.Exists("backup spot.txt"))
            {
                backupSpot = File.ReadAllText("backup spot.txt");
            }

            // Load game
            if (gamesDD.Text.Contains("Game"))
            {
                if (File.Exists("me2 game.txt"))
                {
                    gamesDD.Text = File.ReadAllText("me2 game.txt");
                }
                else
                {
                    File.WriteAllText("me2 game.txt", "ELDEN RING");
                    gamesDD.Text = File.ReadAllText("me2 game.txt");
                }
            }
            determineGame();

            // Load mods
            if (isME2())
            {
                if (File.Exists("game dir config.txt"))
                {
                    steamLib = File.ReadAllText("game dir config.txt");
                }
                else
                {
                    steamLib = "";
                }
                if (!File.Exists("me2 Config.txt")) 
                    navigateToME2();


                me2Config = "me2 config.txt";
                me2 = File.ReadAllText(me2Config) + gameConfig;
                lines = File.ReadAllLines(me2);
            }

            // Load mods
            mods = new List<Mod>();
            if (isME2())
            {
                if (File.Exists("me2 config.txt"))
                {
                    modList = File.ReadAllText("me2 config.txt");
                    webBrowser1.Url = new Uri(modList);
                }
                else
                {
                    webBrowser1.Url = new Uri(steamLib + "\\" + gamesDD.SelectedItem + "\\" + "Mod Engine 2");
                    notificationLabel.Text = steamLib;
                }
            }
            dirs = Directory.GetDirectories(modList);

            // Fetch mod archives and trim them to mod names. These 2 things will be used to trim file extensions.
            int modTrim = modList.Length;
            string modTrimmed;

            foreach (string dir in dirs)
            {
                // Add mods by checking if their extension is whatever zip a mod would be
                Mod mod = new Mod();
                modTrimmed = dir.Substring(modTrim + 1);

                // Forcing Armoured Core mods to be added because regular code doesn't work for some reason even though format still respects my conventions
                if (gamesDD.Text.Contains("ARMORED"))
                {
                    mods.Add(mod);
                }

                // Filter to folders that contain mod files
                string[] modFolders = Directory.GetDirectories(dir);
                string[] modFiles = Directory.GetFiles(dir);
                foreach (string folder in modFolders)
                {
                    string folderName = folder.Substring(1 + dir.Length);
                    foreach (string gameFile in gameFiles)
                    {
                        if (!mods.Contains(mod) && gameFile.Contains(folderName) && !dir.Contains("backup"))
                        {
                            mods.Add(mod);
                        }
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
                        }
                    }
                }

                // Trim file extensions from mod names
                mod.modName = modTrimmed;
            }

            // Update active mods
            foreach (Mod mod in mods)
            {
                if (File.ReadAllText("active mod list.txt").Contains(mod.modName) && !activeMods.Contains(mod))
                {
                    activeMods.Add(mod);
                }
            }

            if (File.Exists("active mod list.txt"))
            {
                activeModList = File.ReadAllText("active mod list.txt");
                foreach (Mod mod in mods)
                {
                    if (activeModList.Contains(mod.modName))
                    {
                        mod.active = true;
                    }
                }
            }

            lines = File.ReadAllLines(me2);
            Console.ReadLine();
            dataGridView1.DataSource = mods;
            dataGridView1.AutoResizeColumns();

            // Save the selected game
            File.WriteAllText("me2 game.txt", gamesDD.Text);
        }

        private void gamesDD_SelectedIndexChanged(object sender, EventArgs e)
        {
            initializeTable();
        }
        #endregion

        #region me2
        private void navToME2_Click(object sender, EventArgs e)
        {
            navigateToME2();
        }

        private void navigateToME2()
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

        private void loadME2Btn_Click(object sender, EventArgs e)
        {
            loadME2();
        }
        private void loadME2()
        {
            Process.Start(me2);
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

        #region modManagement
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
                    if ((bool)this.dataGridView1.CurrentCell.Value == true)
                    {
                        activeMods.Add(mod);
                        Console.WriteLine(mod.modName + " added");
                    }
                    else
                    {
                        activeMods.Remove(mod);
                        Console.WriteLine(mod.modName + " removed");
                    }
                    tickMod();
                }
            }
        }

        // Code to modify ME2's config file
        private void tickMod()
        {
            // Group up the mods in ME2's config file
            modLines = new List<string>();
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

            // Update selected game's mod config
            text = string.Join("\n", lines);
            Console.WriteLine("Saving to " + me2);
            File.WriteAllText(me2, text);

            // Save active mods
            foreach (Mod mod in activeMods)
            {
                trueMods.Add(mod.modName);
            }
            string modCache = string.Join("\n", trueMods);
            File.WriteAllText("active mod list.txt", modCache);
        }

        private void updateModConfig_Click(object sender, EventArgs e)
        {
            listedMods = new List<Mod>();
            unlistedMods = new List<Mod>();
            List<string> configLines = new List<string>();
            // Set up a target line to inject mods into within the config.toml
            int target = 0;
            foreach (string line in File.ReadAllLines(me2))
            {
                configLines.Add(line);
                if (line.Contains("mods = [") && !line.Contains("#"))
                {
                    target = configLines.IndexOf(line);
                }
            }
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
                // Automatically add lines to config.toml
                if (!listedMods.Contains(mod))
                {
                    unlistedMods.Add(mod);
                    string[] newLines = new string[lines.Length + 1];

                    for (int i = 0; i < target + 1; i++)
                    {
                        newLines[i] = lines[i];
                    }

                    if (!newLines.Contains(mod.modName))
                        newLines[target + 1] = "    { enabled = false, name = \"" + mod.modName + "\", path = \"" + mod.modName + "\" },";

                    for (int i = target + 2; i < newLines.Length; i++)
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
                for (int j = target + 1; j < lines.Length - mods.Count; j++)
                {
                    sortedLines[i] = lines[j];
                }
            }

            // Update eldenring_config.toml
            text = string.Join("\n", lines);
            File.WriteAllText(me2, text);

            // Cleanup
            unlistedMods.Clear();
            unlistedMods.TrimExcess();
            listedMods.Clear();
            listedMods.TrimExcess();
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
                Console.WriteLine("tool: " + item);
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

        #region saveBackup
        private void navToSave_Click(object sender, EventArgs e)
        {
            navigateToSave();
        }

        private void navigateToSave()
        {
            determineGame();
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
                    if (selectedGame == "DARK SOULS REMASTERED")
                        saveDir = userPath + "OneDrive\\Documents\\NBGI" + gameSaveDir;
                    else
                        saveDir = userPath + "\\AppData\\Roaming" + gameSaveDir;
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
            determineGame();
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
            File.WriteAllText(backupSpotFile, backupDir + gameSave);
        }

        private void backup_Click(object sender, EventArgs e)
        {
            determineGame();
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
                            File.Delete(backupDir + gameSave);
                        }

                        // Copy save file to backup
                        File.Copy(saveSpot, backupDir + gameSave);
                        notificationLabel.Text = "Save file " + saveSpot + " backed up to " + backupDir;
                    }
                }
                else
                {
                    backupSpot = backupDir + gameSave;
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
            determineGame();
            if (File.Exists("backup dir.txt")) backupSpot = backupDir + gameSave;

            else
            {
                navigateToBackup();
                backupSpot = backupDir + gameSave;
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
                "\n" + "Backup spot: " + backupDir + gameSave);
        }
        #endregion

        #region infoAndMaintenance
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
            return gamesDD.Text.Contains("DARK SOULS II") || gamesDD.Text.Contains("Sekiro");
        }

        private bool isME2()
        {
            return gamesDD.Text.Contains("REMASTERED") || gamesDD.Text.Contains("ELDEN") || gamesDD.Text.Contains("ARMORED");
        }

        private bool isPTDE()
        {
            return gamesDD.Text.Contains("Prepare");
        }
        #endregion
    }
}