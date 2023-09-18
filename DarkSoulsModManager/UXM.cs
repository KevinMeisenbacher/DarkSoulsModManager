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
    public partial class UXM : Form
    {
        private string ds3, steamLib, game, selectedDrive, modengine, moddingPath;                  // paths
        private string text, gameTracker, gameDirConfig, modDirConfig, modTool, tool;                             // string objects
        private string blockLine, paramLine, uxmLine, overrideLine, overrideDirLine, altSaveLine;   // Modengine lines
        private string[] lines, dirs, moddingDir, modFiles, tools, games;              // Mostly file navimagation
        private List<Mod> mods, activeMods;
        private List<String> gameFiles, moddableGames;
        private Mod activeMod;
        private ModManager manager;
        public UXM()
        {
            InitializeComponent();

            // UXM handling
            activeMods = new List<Mod>();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if ((bool)this.dataGridView1.CurrentCell.Value == true)
            {
                foreach (Mod mod in mods)
                {
                    if (mod.modName.Equals(this.dataGridView1.Rows[e.RowIndex].Cells[1].Value))
                    {
                        activeMods.Add(mod);
                    }
                }
                enableMod();

                //foreach (Mod mod in activeMods)
                //{
                //    if (mod.modName.Equals(this.dataGridView1.Rows[e.RowIndex].Cells[1].Value))
                //    {
                //        foreach (string dir in dirs)
                //        {
                //            if (dir.Contains(mod.modName))
                //            {
                //                dir = 
                //            }
                //        }
                //    }
                //}
            }
            else // if a mod is unticked
            {
                disableMod();
            }
        }

        public static void CopyAndReplaceAll(string SourcePath, string DestinationPath, string backupPath)
        {
            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory($"{DestinationPath}{dirPath.Remove(0, SourcePath.Length)}");
                Directory.CreateDirectory($"{backupPath}{dirPath.Remove(0, SourcePath.Length)}");
            }
            foreach (string newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
            {
                if (!File.Exists($"{ DestinationPath}{newPath.Remove(0, SourcePath.Length)}"))
                    File.Copy(newPath, $"{ DestinationPath}{newPath.Remove(0, SourcePath.Length)}");
                else
                    File.Replace(newPath
                        , $"{ DestinationPath}{newPath.Remove(0, SourcePath.Length)}"
                        , $"{ backupPath}{newPath.Remove(0, SourcePath.Length)}", false);
            }
        }

        private void enableMod()
        {
            // Programmer's reference of what the mod manager is doing when a mod is clicked
            Console.WriteLine("Mods:");
            foreach (Mod mod in activeMods)
            {
                foreach (string modDir in dirs)
                {
                    if (modDir.Contains(mod.modName))
                    {
                        Console.WriteLine(mod.modName);
                        string[] modContent = Directory.GetDirectories(modDir);
                    }
                }
            }


        }
        private void disableMod()
        {
            Console.WriteLine("Mod is disabled!");
        }
    }
}
