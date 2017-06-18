using SpriteTranslator;
using System;
using System.IO;
using System.Windows.Forms;

namespace STGUI {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }
        SpriteTL Editor;
        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "All SL2 files|*.sl2";
            if (fd.ShowDialog() != DialogResult.OK)
                return;

            byte[] file = File.ReadAllBytes(fd.FileName);
            Editor = new SpriteTL(file);
            listBox1.Items.Clear();
            foreach (string str in Editor.Import()) {
                listBox1.Items.Add(str);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveFileDialog fd = new SaveFileDialog();
            fd.Filter = "All SL2 Files|*.sl2";
            if (fd.ShowDialog() != DialogResult.OK)
                return;
            string[] Strs = new string[listBox1.Items.Count];
            for (int i = 0; i < Strs.Length; i++)
                Strs[i] = listBox1.Items[i].ToString();
            File.WriteAllBytes(fd.FileName, Editor.Export(Strs));

            MessageBox.Show("Script Saved", "STGUI", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) {
            try {
                textBox1.Text = listBox1.Items[listBox1.SelectedIndex].ToString();
            }
            catch { }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == '\n' || e.KeyChar == '\r') {
                try {
                    listBox1.Items[listBox1.SelectedIndex] = textBox1.Text;
                }
                catch {

                }
            }
        }

        private void recoveryScriptsToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog FD1 = new OpenFileDialog() {
                Multiselect = true,
                Filter = "All SL2 Scripts|*.sl2",
                Title = "Select Original Scripts to Process"
            };
            FolderBrowserDialog FD2 = new FolderBrowserDialog() {
                Description = "Select Corrupted Scripts Directory"
            };
            if (FD1.ShowDialog() != DialogResult.OK || FD2.ShowDialog() != DialogResult.OK)
                return;
            if (!FD2.SelectedPath.EndsWith("\\"))
                FD2.SelectedPath += "\\";
            foreach (string f in FD1.FileNames) {
                string CS = FD2.SelectedPath + Path.GetFileName(f);
                if (!File.Exists(CS))
                    continue;

                SpriteListEditor Ori = new SpriteListEditor(File.ReadAllBytes(f));
                Ori.Import();
                SpriteListEditor Corrupted = new SpriteListEditor(File.ReadAllBytes(CS));
                byte[] Result = Ori.Export(Corrupted.Import());
                File.WriteAllBytes(CS, Result);
            }
            MessageBox.Show("Scripts Restored.", "STGUI", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void testFilterToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog FD1 = new OpenFileDialog() {
                Multiselect = true,
                Filter = "All SL2 Scripts|*.sl2",
                Title = "Select Scripts to Test"
            };

            if (FD1.ShowDialog() != DialogResult.OK)
                return;

            byte[] Ori = File.ReadAllBytes(FD1.FileName);

            SpriteTL TST = new SpriteTL(Ori);
            byte[] Edt = TST.Export(TST.Import());

            SpriteListEditor tst1 = new SpriteListEditor(Ori);
            SpriteListEditor tst2 = new SpriteListEditor(Edt);
            System.Diagnostics.Debug.Assert(tst1.Import() == tst2.Import());
            string Dir = AppDomain.CurrentDomain.BaseDirectory;
            File.WriteAllText(Dir + "f1.txt", tst1.Import().Replace("[", "[\n\r").Replace("]", "]\n\r"), System.Text.Encoding.UTF8);
            File.WriteAllText(Dir + "f2.txt", tst2.Import().Replace("[", "[\n\r").Replace("]", "]\n\r"), System.Text.Encoding.UTF8);


        }
    }
}
