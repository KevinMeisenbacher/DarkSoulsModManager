﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DarkSoulsModManager
{
    public partial class Form1 : Form
    {
        private string ds3, steamLib, game, selectedDrive, modengine, moddingPath;                  // paths
        private string text, modDirConfig, modTool, tool;                                           // string objects
        private string blockLine, paramLine, uxmLine, overrideLine, overrideDirLine, altSaveLine;   // Modengine lines
        private string[] lines, dirs, moddingDir, modFiles, tools, games;                           // Mostly file navimigation
        private List<Mod> mods;
        private List<String> gameFiles, moddableGames;
        private Mod activeMod;

        public Form1()
        {
            InitializeComponent();

            // Populate gamesDD with moddable games
            moddableGames = new List<string>();
            moddableGames.Add("DARK SOULS III");
            moddableGames.Add("Sekiro");

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

            // File browsing and notifications
            notificationLabel.Text = "";
            if (File.Exists("mod dir config.txt"))
            {
                modDirConfig = "mod dir config.txt";
                moddingPath = File.ReadAllText(modDirConfig);
                moddingDir = Directory.GetDirectories(moddingPath);
                //selectedDrive = memoryLines[1];
                loadModdingTools();
                //initializeTable();
                loadToolTips();
            }
            else
            {
                moddingPath = "";
            }

            dataGridView1.AllowUserToResizeRows = true;
        }

        private void loadToolTips()
        {
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
        }

        private void selectPath_Click(object sender, EventArgs e)
        {
            buildTable();
        }

        private void buildTable()
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog()
            { Description = "Pick the drive Steam is in and click OK" })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    // Automatically find Dark Souls 3's directory
                    selectedDrive = fbd.SelectedPath;
                    string[] driveDirs = Directory.GetDirectories(selectedDrive);
                    string[] contents;
                    foreach(string dir in driveDirs)
                    {
                        try
                        {
                            contents = Directory.GetDirectories(dir);
                        } catch(System.UnauthorizedAccessException)
                        {
                            continue;
                        }
                        foreach(string folder in contents)
                        {
                            if (folder.Contains("Steam"))
                            {
                                steamLib = folder + @"\steamapps\common";
                            }
                        }
                    }

                    games = Directory.GetDirectories(steamLib);
                    foreach (string item in games)
                    {
                        Console.WriteLine("Steam game: " + item);
                    }

                    foreach (string item in moddableGames)
                    {
                        gamesDD.Items.Add(item);
                    }

                    foreach(string item in gamesDD.Items)
                    {
                        if (item.Contains("Game"))
                            continue;
                        else {
                            gamesDD.SelectedItem = item;
                            break;
                        }
                    }

                    determineGame();
                    initializeTable();
                }
            }
        }

        private void determineGame()
        {
            if (gamesDD.Text.Contains("DARK SOULS III"))
                game = steamLib + "/" + gamesDD.Text + "/Game";
            else
                game = steamLib + "/" + gamesDD.Text;

            // Setup ability to parse modengine.ini
            modengine = game + @"\modengine.ini";
            text = File.ReadAllText(modengine);
            lines = File.ReadAllLines(modengine);
        }

        private void gamesDD_SelectedIndexChanged(object sender, EventArgs e)
        {
            determineGame();
            initializeTable();
        }

        private void initializeTable()
        {
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
                        if (!mods.Contains(mod) && folderName.Equals(gameFile) && !dir.Contains("backup"))
                            mods.Add(mod);
                    }
                }


                // Trim file extensions from mod names
                mod.modName = dir.Substring(game.Length + 1);

                if (overrideDirLine.Contains(mod.modName))
                {
                    activeMod = mod;
                    mod.active = true;
                }
            }

            // Save the chosen directory to file
            Console.ReadLine();
            dataGridView1.DataSource = mods;
            dataGridView1.AutoResizeColumns();

            colourModEngineButtons();
        }

        private void readModEngine()
        {
            foreach(string line in lines)
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

            Console.WriteLine("\n");
            Console.WriteLine(blockLine);
            Console.WriteLine(paramLine);
            Console.WriteLine(uxmLine);
            Console.WriteLine(overrideLine);
            Console.WriteLine(overrideDirLine);
            Console.WriteLine(altSaveLine);
        }

        // Change the active mod whenever another mod is clicked
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Register whenever a checkbox is clicked
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                this.dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }

            // When a checkbox is clicked, change the active mod to the corresponding one
            if ((bool)this.dataGridView1.CurrentCell.Value == true)
            {
                Console.WriteLine(this.dataGridView1.Rows[e.RowIndex].Cells[1].Value);
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

        private void pickModdingDirectory_Click(object sender, EventArgs e)
        {
            initModdingTools();
        }

        private void initModdingTools() {
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
            try
            {
                Console.WriteLine("Selected item: " + moddingPath + "\\" + toolsDD.SelectedItem);
                tools = Directory.GetFiles(moddingPath + "\\" + toolsDD.SelectedItem);
                string folderPath = "";
                string toolPath = "";
                foreach (string item in tools)
                {
                    Console.WriteLine(item);
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

        private void launchDS3_Click(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine(ds3 + "/DARKSOULSIII");
                Process.Start(ds3 + "/DARKSOULSIII");
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("Whoops; you forgot to set the directory! " +
                    "\nLemme bring up the wizard for you");
                buildTable();
            }
        }

        private void viewModEngine_Click(object sender, EventArgs e)
        {
            Console.WriteLine(game + "/modengine");
            try
            {
                //MessageBox.Show(text);
                Process.Start(game + "/modengine.ini");
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
            if (lines != null)
            {
                if (lines[18].Contains("blockNetworkAccess=0"))
                    lines[18] = "blockNetworkAccess=1";
                else
                    lines[18] = "blockNetworkAccess=0";

                text = string.Join("\n", lines);
                File.WriteAllText(modengine, text);

                colourModEngineButtons();
            } else 
            {
                MessageBox.Show("Lemme pull up the game dir picker so these buttons can actually do stuff with ModEngine");
                buildTable();
            }
        }

        private void useAlternateSaveFile_Click(object sender, EventArgs e)
        {
            if (lines != null)
            {
                if (lines[25].Contains("useAlternateSaveFile=0"))
                lines[25] = "useAlternateSaveFile=1";
            else
                lines[25] = "useAlternateSaveFile=0";

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
            if (lines != null)
            {
                if (lines[31].Contains("loadLooseParams=0"))
                lines[31] = "loadLooseParams=1";
            else
                lines[31] = "loadLooseParams=0";

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
            if (lines != null)
            {
                if (lines[35].Contains("loadUXMFiles=0"))
                lines[35] = "loadUXMFiles=1";
            else
                lines[35] = "loadUXMFiles=0";

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
            if (lines != null)
            {
                if (lines[38].Contains("useModOverrideDirectory=0"))
                    lines[38] = "useModOverrideDirectory=1";
                else
                    lines[38] = "useModOverrideDirectory=0";

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
            // Change active mod
            lines[41] = "modOverrideDirectory=\"\\" + activeMod.modName + "\"";

            string line41 = string.Join("", lines[41]);

            text = string.Join("\n", lines);
            string trunc = line41.Substring(23);
            char quotes = '"';
            notificationLabel.Text = "Active mod changed to " + trunc.Trim(quotes);
            File.WriteAllText(modengine, text);
        }
    }
}
