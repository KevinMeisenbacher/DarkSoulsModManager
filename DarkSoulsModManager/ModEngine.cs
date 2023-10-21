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

namespace DarkSoulsModManager
{
    public partial class ModEngine : Form
    {
        private string steamLib, selectedDrive, game, selectedGame, modengine, moddingPath;         // paths
        private string text, gameTracker, gameDirConfig, modDirConfig, modTool, tool;               // string objects
        private string blockLine, paramLine, uxmLine, overrideLine, overrideDirLine, altSaveLine;   // Modengine lines
        private string[] lines, dirs, moddingDir, modFiles, tools, games;                           // Mostly file navimagation
        private List<Mod> mods, activeMods;
        private List<String> gameFiles, moddableGames;
        private Mod activeMod;
        private ModManager manager;

        public ModEngine()
        {
            InitializeComponent();
            loadToolTips();

            // Populate gamesDD with moddable games
            moddableGames = new List<string>();
            //moddableGames.Add("Dark Souls Prepare to Die Edition");
            //moddableGames.Add("DARK SOULS REMASTERED");
            //moddableGames.Add("DARK SOULS II Scholar of the First Sin");
            moddableGames.Add("DARK SOULS III");
            moddableGames.Add("Sekiro");
            moddableGames.Add("ELDEN RING");
            moddableGames.Add("ARMORED CORE VI");

            // Array for figuring out if a folder is a mod
            gameFiles = new List<string>();
            gameFiles.Add("action");
            gameFiles.Add("chr");
            gameFiles.Add("event");
            gameFiles.Add("map");
            gameFiles.Add("msg");
            gameFiles.Add("param");
            gameFiles.Add("parts");
            gameFiles.Add("sound");

            // Initialize the notification label
            notificationLabel.Text = "";

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

            // Load games
            foreach (string item in moddableGames)
            {
                gamesDD.Items.Add(item);
            }

            foreach (string item in gamesDD.Items)
            {
                if (isModEngine())
                {
                    gamesDD.SelectedItem = item;
                    break;
                }
                else
                {
                    continue;
                }
            }

            if (File.Exists("game dir config.txt"))
            {
                steamLib = File.ReadAllText("game dir config.txt");
            }
            else
            {
                steamLib = "";
            }

            dataGridView1.AllowUserToResizeRows = true;
            if (File.Exists("selected game.txt"))
                initializeTable();
        }

        private void loadToolTips()
        {
            // Modengine Controls
            ToolTip blockNetworkAccessToolTip = new ToolTip();
            blockNetworkAccessToolTip.BackColor = Color.Black;
            blockNetworkAccessToolTip.ForeColor = Color.White;
            blockNetworkAccessToolTip.SetToolTip(this.blockNetworkAccess, "Boots you offline before validation to prevent you from getting banned");

            ToolTip useAlternateSaveFileToolTip = new ToolTip();
            useAlternateSaveFileToolTip.BackColor = Color.Black;
            useAlternateSaveFileToolTip.ForeColor = Color.White;
            useAlternateSaveFileToolTip.SetToolTip(this.useAlternateSaveFile, "Loads an alternate save file to keep your main save clean");

            ToolTip loadLooseParamsToolTip = new ToolTip();
            loadLooseParamsToolTip.BackColor = Color.Black;
            loadLooseParamsToolTip.ForeColor = Color.White;
            loadLooseParamsToolTip.SetToolTip(this.loadLooseParams, "Loads loose param data from files instead of from encrypted data0.bdt archive. " +
                "This is mod specific and should only be enabled by modders who know what they are doing. End users should have no reason to touch this.");

            ToolTip loadUXMFilesToolTip = new ToolTip();
            loadUXMFilesToolTip.BackColor = Color.Black;
            loadUXMFilesToolTip.ForeColor = Color.White;
            loadUXMFilesToolTip.SetToolTip(this.loadUXMFiles, "Loads extracted files from UXM instead of data from the archives. " +
                "Requires a complete UXM extraction and should generally only be used by mod creators.");

            ToolTip useModOverrideDirectoryToolTip = new ToolTip();
            useModOverrideDirectoryToolTip.BackColor = Color.Black;
            useModOverrideDirectoryToolTip.ForeColor = Color.White;
            useModOverrideDirectoryToolTip.SetToolTip(this.useModOverrideDirectory, "The directory from which to load a mod");

            // Modding Tool Buttons
            ToolTip moddingDirToolTip = new ToolTip();
            moddingDirToolTip.SetToolTip(this.openModDirWizard, "Click here to navigate to your modding directory");

            ToolTip toolsDDToolTip = new ToolTip();
            toolsDDToolTip.SetToolTip(this.toolsDD, "Automatically launches whatever tool is selected");

            ToolTip launchToolToolTip = new ToolTip();
            launchToolToolTip.SetToolTip(this.launchTool, "Click here to launch the selected modding tool");

            ToolTip mergeToolTip = new ToolTip();
            mergeToolTip.SetToolTip(this.mergeMods, "Opens up a menu for mod merging. In beta and works, but gameplay mods will overwrite each other");

            // Mod Manager Buttons
            ToolTip refreshToolTip = new ToolTip();
            refreshToolTip.SetToolTip(this.refresh, "Reloads the mod manager to update the list of mods and tools");

            ToolTip steamDirToolTip = new ToolTip();
            steamDirToolTip.SetToolTip(this.selectPath, "Click here to go to the drive Steam is in. For example, mine is in C:");

            ToolTip viewModEngineToolTip = new ToolTip();
            viewModEngineToolTip.SetToolTip(this.viewModEngine, "Opens up modengine.ini to double check config or if you still want to manually edit it like a troglodyte");

            ToolTip gamesDDToolTip = new ToolTip();
            gamesDDToolTip.SetToolTip(this.gamesDD, "Pick a game to manage");

            ToolTip launchGameToolTip = new ToolTip();
            launchGameToolTip.SetToolTip(this.launchTool, "Launches ther game!");
        }

        private void selectPath_Click(object sender, EventArgs e)
        {
            buildTable();
        }

        private void buildTable()
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog()
            { Description = "Navigate to the folder that has Steam and click OK" })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    // Automatically find Dark Souls 3's directory
                    selectedDrive = fbd.SelectedPath;
                    string[] driveDirs = Directory.GetDirectories(selectedDrive);
                    foreach (string dir in driveDirs)
                    {
                        try
                        {
                            string[] contents = Directory.GetDirectories(dir);
                        }
                        catch (System.UnauthorizedAccessException)
                        {
                            continue;
                        }
                        if (dir.Contains("Steam"))
                        {
                            steamLib = dir + @"\steamapps\common";
                        }
                    }

                    games = Directory.GetDirectories(steamLib);

                    gameDirConfig = "game dir config.txt";
                    File.WriteAllText(gameDirConfig, steamLib);
                    initializeTable();
                }
            }
        }

        private void determineGame()
        {
            if (gamesDD.Text.Contains("DARK SOULS II"))
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
                try
                {
                    modengine = game + @"\modengine.ini";
                    text = File.ReadAllText(modengine);
                    lines = File.ReadAllLines(modengine);
                }
                catch (DirectoryNotFoundException)
                {
                    MessageBox.Show("Whoops! Looks like you forgot to set the game's directory. Lemme get that for ya");
                    buildTable();
                }
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
                if (!File.Exists("game dir config.txt"))
                    buildTable();
                else
                {
                    File.WriteAllText("me2 game.txt", gamesDD.Text);
                    var me2 = new ME2();
                    me2.Location = this.Location;
                    me2.Show();
                    this.Hide();
                }
            }

            // Detect whether Dark Souls 3 or Sekiro is selected
            if (gamesDD.Text.Contains("DARK SOULS III"))
            {
                selectedGame = "DARK SOULS III";
            }
            else if (gamesDD.Text.Contains("Sekiro"))
            {
                selectedGame = "Sekiro";
            }

            // Remember what game the mod manager is on
            gameTracker = "game tracker.txt";
            File.WriteAllText(gameTracker, game);
            Console.WriteLine("Game: " + game);
        }

        private void gamesDD_SelectedIndexChanged(object sender, EventArgs e)
        {
            initializeTable();
        }

        private void initializeTable()
        {
            determineGame();
            if (gamesDD.Text.Contains("Game"))
            {
                if (File.Exists("selected game.txt"))
                    gamesDD.Text = File.ReadAllText("selected game.txt");
                else
                    gamesDD.Text = "DARK SOULS III";
            }
            if (isModEngine())
                readModEngine();

            // Create file browser and file list
            webBrowser1.Url = new Uri(game);
            mods = new List<Mod>();
            dirs = Directory.GetDirectories(game);

            // Fetch mod archives and trim them to mod names
            int modTrim = game.Length;

            // These 2 things will be used to trim file extensions
            string modTrimmed;
            int trimLength;

            foreach (string dir in dirs)
            {
                // Add mods by checking if their extension is whatever zip a mod would be
                Mod mod = new Mod();
                modTrimmed = dir.Substring(game.Length + 1);
                trimLength = modTrimmed.Length;

                // Filter to folders that contain mod files
                string[] modFolders = Directory.GetDirectories(dir);
                foreach (string folder in modFolders)
                {
                    string folderName = folder.Substring(1 + dir.Length);
                    foreach (string gameFile in gameFiles)
                    {
                        if (!mods.Contains(mod) && gameFile.Contains(folderName) && !dir.Contains("backup"))
                            mods.Add(mod);
                    }
                }

                // Trim file extensions from mod names
                mod.modName = modTrimmed;

                if (isModEngine())
                {
                    if (overrideDirLine.Contains(mod.modName))
                    {
                        activeMod = mod;
                        mod.active = true;
                    }
                }
            }

            // Save the chosen directory to file
            Console.ReadLine();
            dataGridView1.DataSource = mods;
            dataGridView1.AutoResizeColumns();

            if (isModEngine())
                colourModEngineButtons();

            // Save the selected game
            if (isModEngine())
                File.WriteAllText("selected game.txt", gamesDD.Text);
        }

        private void launchTool_Click(object sender, EventArgs e)
        {
            launchSelectedTool();
        }

        private void GoBack_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        private void GoForward_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }

        private void mergeMods_Click(object sender, EventArgs e)
        {
            if (isModEngine())
            {
                var form3 = new ModMerger();
                form3.Location = this.Location;
                form3.Show();
            }
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            initializeTable();
        }

        // Change the active mod whenever another mod is clicked
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Register whenever a checkbox is clicked
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                this.dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
            if (isModEngine())
            {

                // When a checkbox is clicked, change the active mod to the corresponding one
                if ((bool)this.dataGridView1.CurrentCell.Value == true)
                {
                    //Console.WriteLine(this.dataGridView1.Rows[e.RowIndex].Cells[1].Value);
                    foreach (Mod mod in mods)
                    {
                        if (mod.modName.Equals(this.dataGridView1.Rows[e.RowIndex].Cells[1].Value))
                        {
                            activeMod = mod;
                            changeActiveMod();
                        }
                        else
                            mod.active = false;
                    }
                    Console.WriteLine("Active mod: " + activeMod.modName);
                }
            }
        }

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
            try
            {
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
            catch (System.UnauthorizedAccessException)
            {
                MessageBox.Show("Did you mean to pick the game directory?");
                buildTable();
            }
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

        private void launchGame_Click(object sender, EventArgs e)
        {
            determineGame();
            try
            {
                string gameExe = "";
                string[] gameFiles = Directory.GetFiles(game);
                foreach (string file in gameFiles)
                {
                    if (file.Contains("exe") && !file.Contains("bak"))
                        gameExe = file;
                }

                Process.Start(gameExe);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("Whoops; you forgot to set the directory! " +
                    "\nLemme bring up the wizard for you");
                buildTable();
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("Whoops; you forgot to set the directory! " +
                    "\nLemme bring up the wizard for you");
                buildTable();
            }
        }

        #region modengine

        private void readModEngine()
        {
            foreach (string line in lines)
            {
                if (line.Contains("blockNetworkAccess"))
                    blockLine = line;
                if (line.Contains("loadLooseParams"))
                    paramLine = line;
                if (line.Contains("loadUXMFiles"))
                    uxmLine = line;
                if (line.Contains("useModOverrideDirectory"))
                    overrideLine = line;
                if (line.Contains("modOverrideDirectory="))
                    overrideDirLine = line;
                if (line.Contains("useAlternateSaveFile"))
                    altSaveLine = line;
            }
        }

        private void viewModEngine_Click(object sender, EventArgs e)
        {
            Console.WriteLine(game + "\\modengine");
            try
            {
                //MessageBox.Show(text);
                Process.Start(game + "\\modengine.ini");
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("Whoops; you forgot to set the directory! " +
                    "\nLemme bring up the wizard for you");
                buildTable();
            }
        }

        private void blockNetworkAccess_Click(object sender, EventArgs e)
        {
            readModEngine();
            Console.WriteLine("Block line: " + blockLine);
            if (lines != null)
            {
                if (blockLine.Contains("0"))
                    blockLine = "blockNetworkAccess=1";
                else if (blockLine.Contains("1"))
                    blockLine = "blockNetworkAccess=0";

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("blockNetworkAccess"))
                    {
                        lines[i] = blockLine;
                    }
                }

                text = string.Join("\n", lines);
                File.WriteAllText(modengine, text);

                colourModEngineButtons();
            }
            else
            {
                MessageBox.Show("Lemme pull up the game dir picker so these buttons can actually do stuff with ModEngine");
                buildTable();
            }
        }

        private void useAlternateSaveFile_Click(object sender, EventArgs e)
        {
            readModEngine();
            if (lines != null)
            {
                if (altSaveLine.Contains("useAlternateSaveFile=0"))
                    altSaveLine = "useAlternateSaveFile=1";
                else
                    altSaveLine = "useAlternateSaveFile=0";

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("useAlternateSaveFile"))
                    {
                        lines[i] = altSaveLine;
                    }
                }

                text = string.Join("\n", lines);
                File.WriteAllText(modengine, text);

                colourModEngineButtons();
            }
            else
            {
                MessageBox.Show("Lemme pull up the game dir picker so these buttons can actually do stuff with ModEngine");
                buildTable();
            }
        }

        private void loadLooseParams_Click(object sender, EventArgs e)
        {
            readModEngine();
            if (lines != null)
            {
                if (paramLine.Contains("loadLooseParams=0"))
                    paramLine = "loadLooseParams=1";
                else
                    paramLine = "loadLooseParams=0";

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("loadLooseParams"))
                    {
                        lines[i] = paramLine;
                    }
                }

                text = string.Join("\n", lines);
                File.WriteAllText(modengine, text);

                colourModEngineButtons();
            }
            else
            {
                MessageBox.Show("Lemme pull up the game dir picker so these buttons can actually do stuff with ModEngine");
                buildTable();
            }
        }

        private void loadUXMFiles_Click(object sender, EventArgs e)
        {
            readModEngine();
            if (lines != null)
            {
                if (uxmLine.Contains("loadUXMFiles=0"))
                    uxmLine = "loadUXMFiles=1";
                else
                    uxmLine = "loadUXMFiles=0";

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("loadUXMFiles"))
                    {
                        lines[i] = uxmLine;
                    }
                }

                text = string.Join("\n", lines);
                File.WriteAllText(modengine, text);

                colourModEngineButtons();
            }
            else
            {
                MessageBox.Show("Lemme pull up the game dir picker so these buttons can actually do stuff with ModEngine");
                buildTable();
            }
        }

        private void useModOverrideDirectory_Click(object sender, EventArgs e)
        {
            readModEngine();
            if (lines != null)
            {
                if (overrideLine.Contains("useModOverrideDirectory=0"))
                    overrideLine = "useModOverrideDirectory=1";
                else
                    overrideLine = "useModOverrideDirectory=0";

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("useModOverrideDirectory"))
                    {
                        lines[i] = overrideLine;
                    }
                }

                text = string.Join("\n", lines);
                File.WriteAllText(modengine, text);

                colourModEngineButtons();
            }
            else
            {
                MessageBox.Show("Lemme pull up the game dir picker so these buttons can actually do stuff with ModEngine");
                buildTable();
            }
        }

        // Colour the buttons on the left based on their booleans
        private void colourModEngineButtons()
        {
            readModEngine();
            if (selectedGame.Contains("DARK"))
            {
                if (blockLine.Contains("blockNetworkAccess=1"))
                    blockNetworkAccess.BackColor = Color.FromArgb(128, 64, 0);
                else
                    blockNetworkAccess.BackColor = Color.Black;

                if (altSaveLine.Contains("useAlternateSaveFile=1"))
                    useAlternateSaveFile.BackColor = Color.FromArgb(128, 64, 0);
                else
                    useAlternateSaveFile.BackColor = Color.Black;

            if (paramLine.Contains("loadLooseParams=1"))
                loadLooseParams.BackColor = Color.FromArgb(128, 64, 0);
            else
                loadLooseParams.BackColor = Color.Black;
            }

            if (uxmLine.Contains("loadUXMFiles=1"))
                loadUXMFiles.BackColor = Color.FromArgb(128, 64, 0);
            else
                loadUXMFiles.BackColor = Color.Black;

            if (overrideLine.Contains("useModOverrideDirectory=1"))
                useModOverrideDirectory.BackColor = Color.FromArgb(128, 64, 0);
            else
                useModOverrideDirectory.BackColor = Color.Black;

        }

        private void changeActiveMod()
        {
            readModEngine();
            // Change active mod
            overrideDirLine = "modOverrideDirectory=\"\\" + activeMod.modName + "\"";
            Console.WriteLine("Override: " + overrideDirLine);

            string line41 = string.Join("", overrideDirLine);
            Console.WriteLine("Line 41: " + line41);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("modOverrideDirectory="))
                {
                    lines[i] = overrideDirLine;
                    Console.WriteLine("Override dir: " + lines[i]);
                }
            }

            text = string.Join("\n", lines);
            string trunc = line41.Substring(23);
            char quotes = '"';
            notificationLabel.Text = "Active mod changed to " + trunc.Trim(quotes);
            File.WriteAllText(modengine, text);
        }

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
            return gamesDD.Text.Contains("ELDEN") || gamesDD.Text.Contains("ARMORED");
        }

        private bool isPTDE()
        {
            return gamesDD.Text.Contains("Die");
        }
        #endregion
    }
}