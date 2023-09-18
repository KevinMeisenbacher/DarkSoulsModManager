using System;
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
    public partial class DSDaD : Form
    {
        private string steamLib, game, selectedDrive, moddingPath;                                  // paths
        private string[] lines, dirs, moddingDir, modFiles, tools, games;                           // Mostly file navimagation
        private string text, gameDirConfig, modDirConfig, modTool, tool;                                           // string objects

        private void launchDadmod_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(game + "/dsdad");
            } catch(FileNotFoundException)
            {
                Process.Start("https://www.nexusmods.com/darksouls/mods/1355?tab=files&file_id=1000002299");
            }
        }

        private List<Mod> mods;
        private List<String> gameFiles, moddableGames;

        private void selectPath_Click(object sender, EventArgs e)
        {
            buildTable();
        }

        public DSDaD()
        {
            InitializeComponent();

            // Populate gamesDD with moddable games
            moddableGames = new List<string>();
            moddableGames.Add("Dark Souls Prepare to Die Edition");
            moddableGames.Add("DARK SOULS REMASTERED");
            moddableGames.Add("DARK SOULS II");
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
                if (gamesDD.Items.Contains("Die"))
                {
                    gamesDD.SelectedItem = item;
                    break;
                }
                else
                {
                    continue;
                }
            }
            Console.WriteLine(gamesDD.SelectedItem);

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
                    foreach (string dir in driveDirs)
                    {
                        try
                        {
                            contents = Directory.GetDirectories(dir);
                        }
                        catch (System.UnauthorizedAccessException)
                        {
                            continue;
                        }
                        foreach (string folder in contents)
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

                    foreach (string item in gamesDD.Items)
                    {
                        if (item.Contains("Game"))
                            continue;
                        else
                        {
                            gamesDD.SelectedItem = item;
                            break;
                        }
                    }
                    gameDirConfig = "game dir config.txt";
                    File.WriteAllText(gameDirConfig, steamLib);

                    initializeTable();
                }
            }
        }

        private void determineGame()
        {
            if (gamesDD.Text.Contains("DARK SOULS III"))
            {
                Console.WriteLine("GamesDD text: " + gamesDD.Text);
                game = steamLib + "/" + gamesDD.SelectedItem + "/Game";
            }
            else if (gamesDD.Text.Contains("Die"))
            {
                game = steamLib + "/" + gamesDD.Text + "/DATA";
                var form2 = new DSDaD();
                form2.Location = this.Location;
                form2.Show();
                this.Hide();
            }
            else
            {
                game = steamLib + "/" + gamesDD.Text;
            }
        }

        private void initializeTable()
        {
            if (gamesDD.Text.Contains("Game"))
                gamesDD.SelectedItem = "Dark Souls Prepare to Die Edition";
            determineGame();
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
            }

            // Save the chosen directory to file
            Console.ReadLine();
            dataGridView1.DataSource = mods;
            dataGridView1.AutoResizeColumns();
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

    }
}
