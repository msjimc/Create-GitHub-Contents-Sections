using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateGitHubContentsSection
{
    public partial class Form1 : Form
    {
        private string selectedFolder = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            string folder = FileString.GetFolder("Select Folder Containing Files to Process", "");
            if (System.IO.Directory.Exists(folder) == false) { return; }
            List<string> files= new List<string>();

            ProcessThisFolder(folder, files);
            files.Sort();

            int len = folder.Length + 1;
            lbIgnore.Items.Clear();
            cboList.Items.Clear();
            cboList.Items.Add("Select");
            foreach (string file in files)
            {cboList.Items.Add(file.Substring(len)); }
            cboList.SelectedIndex = 0;
            selectedFolder = folder + "\\";
        }

        private void ProcessThisFolder(string folder, List<string> files)
        {
            string[] allFiles = System.IO.Directory.GetFiles(folder, "*.md");
            foreach (string file in allFiles)
            {
                files.Add(file);
            }

            foreach(string aFolder in System.IO.Directory.GetDirectories(folder))
            {
                ProcessThisFolder(aFolder, files);
            }
        }

        private void btnmake_Click(object sender, EventArgs e)
        {
            List<string> titles = new List<string>();

            for(int index = 0; index < lbIgnore.Items.Count; index++)
            {
                if (lbIgnore.CheckedIndices.Contains(index) == false)
                {
                    string file = selectedFolder + lbIgnore.Items[index].ToString();
                    string fileNmae = lbIgnore.Items[index].ToString();
                    titles.Add("   ");
                    titles.Add("## " + fileNmae.Substring(0,fileNmae.Length-3));
                    titles.Add("## List of contents");
                    titles = getTitles(file, titles, lbIgnore.Items[index].ToString());                    
                }
            }

            System.IO.StreamWriter sw = null;
            try 
            {
                sw = new System.IO.StreamWriter(selectedFolder + "ContentsSection.md", false);
                foreach(string title in titles)
                {
                    sw.WriteLine(title);
                }

            }
            catch (Exception ex)
            { MessageBox.Show("Error: " + ex.Message); }
            finally
            {
                if (sw != null)
                { sw.Close(); }
            }
        }

        private List<string> getTitles(string file, List<string> titles, string path)
        {
           string[] lines = System.IO.File.ReadAllLines(file);
            foreach(string line in lines)
            {
                if (line.Trim().StartsWith("#"))
                {
                    string title = line.Trim();
                    string link = title.Replace("_", "").Replace("*", "");
                    int len = 0;
                    while (len != link.Length)
                    {
                        link = link.Replace("  ", " ");
                        len = link.Length;
                    }

                    int hashCount = 0;
                    for(int index = 0; index < link.Length; index++)
                    {
                        if (link[index] == '#')
                        { hashCount++; }
                        else
                        { break; }
                    }

                    hashCount = (hashCount - 1) * 2;
                    string space = new string(' ', hashCount) + "- ";
                    link = CleanLink(link).Trim();
                    link = "#" + link.Replace(" ", "-");
                    title = title.Replace("# ", "");
                    title = title.Replace("#", "");
                    titles.Add(space + "[" + title + "](" + path +  link + ")" );
                }
            }
            return titles;
        }

        private string CleanLink(string link)
        {
            string answer = "";
            foreach(char c in link)
            {
                if (char.IsLetterOrDigit(c) == true || c == ' ' || c == '-')
                { answer += c; }
            }
            return answer.ToLower();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cboList.SelectedIndex > 0)
            { lbIgnore.Items.Add(cboList.SelectedItem.ToString()); }
        }
    }
}
