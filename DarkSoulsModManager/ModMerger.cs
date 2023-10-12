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
    public partial class ModMerger : Form
    {
        private string ds3, steamLib, game, selectedDrive, modengine, moddingPath, folderName;      // paths
        private string text, gameTracker, gameDirConfig, modDirConfig, modTool, tool;               // string objects
        private string[] lines, dirs, moddingDir, tools, games;                           // Mostly file navimagation
        private List<Mod> mods, activeMods, queuedMods;
        private List<String> gameFiles, moddableGames, queuedDirs;
        private Mod activeMod;
        private ModManager manager;
        public ModMerger()
        {
            InitializeComponent();

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

            // Create a check to refresh the mod list in Form1 (main form)
            manager = new ModManager();

            initializeTable();
        }

        private void determineGame()
        {
            gameTracker = "game tracker.txt";
            game = File.ReadAllText(gameTracker);
        }

        private void initializeTable()
        {
            determineGame();
            gameDirConfig = "game dir config.txt"; mods = new List<Mod>();
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
                        {
                            mods.Add(mod);
                        }
                    }
                }

                // Trim file extensions from mod names
                mod.modName = modTrimmed;
            }
            dataGridView1.DataSource = mods;
            dataGridView1.AutoResizeColumns();
        }

        // Creates a new folder based on specified name and adds contents of whatever is clicked to it
        private void mergeMods_Click(object sender, EventArgs e)
        {
            folderName = game + "/" + folderBox.Text;
            Directory.CreateDirectory(folderName);
            Console.WriteLine("Merging mods to " + folderName);
            queuedMods = new List<Mod>();
            queuedDirs = new List<string>();
            foreach (Mod mod in mods)
            {
                if (mod.active == true)
                {
                    queuedMods.Add(mod);
                }
            }

            Console.WriteLine("Mods:");
            foreach (string dir in dirs)
            {
                foreach (Mod mod in queuedMods)
                {
                    if (dir.Contains(mod.modName))
                    {
                        CopyFilesRecursively(dir, folderName);
                        queuedDirs.Add(dir);
                        //MergeFiles();
                    }
                }
            }
            initializeTable();
        }

        private void MergeFiles()
        {
            int chunkSize = 1024 * 1024;
            var inputFiles = Directory.GetFiles("D:/Users/Kevin/Documents");
            using (var output = File.Create("D:/Users/Kevin/Documents/FullFile.txt"))
            {
                foreach (var file in inputFiles)
                {
                    try
                    {
                        using (var input = File.OpenRead(file))
                        {
                            var buffer = new byte[chunkSize];
                            int bytesRead;
                            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                output.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                    catch (IOException e)
                    {
                        continue;
                    }
                }
                Console.WriteLine(output.Name);
            }
        }

        // Got this from the "much easier" answer on https://stackoverflow.com/questions/58744/copy-the-entire-contents-of-a-directory-in-c-sharp
        private static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }
    }
}
