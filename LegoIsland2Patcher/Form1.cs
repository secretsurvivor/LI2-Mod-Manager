using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using static LegoIsland2Patcher.Utilities;
using LegoIsland2Patcher.Base;
using LegoIsland2Patcher.Mods;

namespace LegoIsland2Patcher
{
	public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        LegoIslandExe LegoIsland { get; set; }
        BackupExe Backup { get; set; }
        IList<Modification> AvailableMods { get; set; }

        Resolution Resolution { get; set; }
		LoadFix LoadFix { get; set; }
		NoVideos NoVideos { get; set; }

        //Global values
        exeVersion exeData; //Holds names and offsets that change depending on the game version
        List<modinfo> mods = new List<modinfo>(); //List containing mod data

        //Program start
        private void ProgramStart(string path)
        {
            LegoIsland = LegoIslandExe.GetLegoIsland(path);

            if (LegoIsland == null)
            {
				//Disable buttons if exe file wasn't found
				btnApply.Enabled = false;
				cbLoadFix.Enabled = false;
				cbEnableResolution.Enabled = false;
				clbPatches.Enabled = false;
				cbNoVideos.Enabled = false;

				lblExeVersion.Text = "No game file found.";

                return;
			}

            if (AdminRights.RequireAdminPrivileges(LegoIsland) && !AdminRights.IsAdministrator())
            {
                if (AdminRights.RestartAsAdministrator())
                {
                    AdminRights.CloseCurrentInstance();
                }
                else
                {
					MessageBox.Show($"Mod Manager requires admin privileges to modify this instance of Lego Island 2 ({LegoIsland.Version.ExeName})");
                    return;
				}
            }

			Backup = BackupExe.BackupLegoIsland(LegoIsland);
            AvailableMods = Modification.GetAvailableMods();

			PopulateModList(AvailableMods);

            if (cbEnableResolution.Checked = Resolution.Exist(LegoIsland, Backup))
			{
				cbEnableAspect.Checked = Resolution.CustomAspect;
                tbWidth.Text = Resolution.Width.ToString();
                tbHeight.Text = Resolution.Height.ToString();
			}
            else
            {
                cbEnableAspect.Checked = false;
                tbWidth.Text = "";
                tbHeight.Text = "";
			}

            cbLoadFix.Checked = LoadFix.Exist(LegoIsland, Backup);
			cbNoVideos.Checked = NoVideos.Exist(LegoIsland, Backup);
		}

		private void ApplyMods(LegoIslandExe legoIsland, BackupExe backup)
        {
            var addMods = new List<IModification>();
            var removeMods = new List<IModification>();

            if (cbEnableResolution.Checked)
            {
                Resolution.CustomAspect = cbEnableAspect.Checked;
                Resolution.Width = StringToInt(tbWidth.Text);
                Resolution.Height = StringToInt(tbHeight.Text);
            }

            (cbEnableResolution.Checked ? addMods : removeMods).Add(Resolution);
            (cbLoadFix.Checked ? addMods : removeMods).Add(LoadFix);
            (cbNoVideos.Checked ? addMods : removeMods).Add(NoVideos);

            for (int i = 0; i < clbPatches.Items.Count; i++)
            {
                (clbPatches.GetItemChecked(i) ? addMods : removeMods).Add(AvailableMods[i]);
            }

            addMods.ForEach(mod => mod.ApplyModification(legoIsland));
            removeMods.ForEach(mod => mod.RemoveModification(legoIsland, backup));
		}

        private void modFileInstall(string dir)
        {
            /*Steps
             * Check if bob/bod files are still present
             * If they are not, backup old files, but only if not already backed up
             * Install new files
             */
            
            bool backupFile = true;

            string[] parts = dir.Split('\\');

            string[] partsagain = dir.Split('/', '\\');
            if (partsagain[2] == "_data" && partsagain.Length >= 4)
            {
                string testbob = "_data/" + partsagain[3];

                if (File.Exists(testbob + ".bob") && File.Exists(testbob + ".bod"))
                {
                    backupFile = false;
                }
            }
                           
            //If directory exists in the LI2 installation
            if (Directory.Exists(dir.Split('/')[1]))
            {
                //Create directory in backup folder
                if (backupFile == true)
                {
                    if (!Directory.Exists("backup/" + dir.Split('/')[1]))
                    {
                        Directory.CreateDirectory("backup/" + dir.Split('/')[1]);
                    }
                }
            }
            else
            {
                //If still using bod/bob files, create new folder                
                Directory.CreateDirectory(dir.Split('/')[1]);
            }

            //Install files in directory
            string[] files = Directory.GetFiles(dir);
            foreach (string f in files)
            {
                parts = f.Split('/');

                if (backupFile == true)
                {
                    //Backup file if not already backed up
                    if (File.Exists(parts[1]) && !File.Exists("backup/" + parts[1]))
                    {                           
                        File.WriteAllBytes("backup/" + parts[1], File.ReadAllBytes(parts[1]));
                    }
                }
                //Install file
                File.WriteAllBytes(parts[1], File.ReadAllBytes(f));
            }

            //Search inner folders
            string[] dirs = Directory.GetDirectories(dir);
            foreach (string s in dirs)
            {
                modFileInstall(s);
            }
        }

        private void modFileUninstall(string dir)
        {
            bool backupFile = true;

            string[] parts = dir.Split('\\');
            string[] partsagain = dir.Split('/', '\\');

            if (partsagain[2] == "_data" && partsagain.Length >= 4)
            {                
                string testbob = "_data/" + partsagain[3];

                if (File.Exists(testbob + ".bob") && File.Exists(testbob + ".bod"))
                {                    
                    backupFile = false;
                }  
            }

            string[] files = Directory.GetFiles(dir);

            foreach (string f in files)
            {
                parts = f.Split('/');

                if (backupFile == true)
                {
                    if (File.Exists("backup/" + parts[1]) )
                    {
                        File.WriteAllBytes( parts[1], File.ReadAllBytes("backup/" + parts[1]) );
                        File.Delete( "backup/" + parts[1] );
                    }
                }
                else
                {
                    //Just delete if bob/bod files exist
                    if (File.Exists(parts[1]))
                    {                        
                        File.Delete(parts[1]);
                    }                    
                }
            }


            //Search inner folders
            string[] dirs = Directory.GetDirectories(dir);
            foreach (string s in dirs)
            {
                modFileUninstall(s);
            }
            
            //Remove empty folders
            parts = dir.Split('\\', '/');

            if (parts[0] == "mods")
            {
                string installPath = "";
                for (int i = 0; i < parts.Length; i++)
                {
                    if (i >= 2)
                    {
                        installPath += parts[i] + "/";
                    }
                }

                //First pass: Delete installed directory
                //Second pass: Delete backedup directory
                for (int i = 0; i < 2; i++)
                {
                    if (Directory.Exists(installPath))
                    {
                        if (Directory.GetDirectories(installPath).Length == 0 && Directory.GetFiles(installPath).Length == 0)
                        {
                            Directory.Delete(installPath);
                        }
                    }
                    installPath = "backup/" + installPath;
                }
            }
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ProgramStart(Directory.GetCurrentDirectory());
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
			ApplyMods(LegoIsland, Backup);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("http://www.rockraidersunited.com/");
            Process.Start(sInfo);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("http://www.github.com/JeffRuLz/LI2-Mod-Manager");
            Process.Start(sInfo);
        }

        private void clbPatches_SelectedIndexChanged(object sender, EventArgs e)
        {
            rtbDescription.Text = mods[clbPatches.SelectedIndex].description;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void cbResolutions_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbEnableResolution_CheckedChanged(object sender, EventArgs e)
        {
            gbResolution.Enabled = cbEnableResolution.Checked;
        }

        private void PopulateModList(IList<Modification> mods)
        {
            foreach (var mod in mods)
            {
				clbPatches.Items.Add($"{mod.Name} - {mod.Author}");
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
            if (ShowFolderBrowserDialog("Select Lego Island installation folder", out var path))
            {
				ProgramStart(path);
			}
		}
	}

	public class modinfo
    {
        public string name;
        public string author;
        public string description;
        public string location;
    }

    public class exeVersion
    {
        public string label;
        public byte checkByte; //Byte value at position 0x128
        public string exeName;
        public long resOffset; //See https://www.rockraidersunited.com/topic/7653-widescreen-hack-high-resolution/?tab=comments#comment-129128
        public int resSep; //Distance from W to H
        public int resDis; //Distance from one resolution to the next
        public long fovOffset; //Search for 00 00 40 3F
        public long loadOffset; //Long loading fix. Search for 90 90 90 90 90 FF. Use the position of FF.
    }
}
