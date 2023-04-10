using BrawlCrate.KRTDL.Recolour;
using BrawlCrate.UI;
using System.Drawing;
using System.Windows.Forms;

namespace BrawlCrate.KRTDL
{
    public class Hook
    {
        private Hook() { }

        public static Hook Instance { get; private set; } = new Hook();

        public Icon kirbyIcon { get; private set; }
        public Icon kirbyIconAlt { get; private set; }

        public void ModifyMainFormUI(MainForm mainForm)
        {
            // get controls we will be modifying
            var menuStrip1 = mainForm.MainMenuStrip;
            var resourceTree = mainForm.resourceTree;
            var splitContainer1 = mainForm.Controls.Find("splitContainer1", true)[0] as SplitContainer;
            var splitContainer2 = mainForm.Controls.Find("splitContainer2", true)[0] as SplitContainer;

            // some sensible changes
            mainForm.StartPosition = FormStartPosition.CenterScreen;
            mainForm.Size = new Size(820, 550);
            splitContainer1.SplitterDistance = 240;

            // kirby specific changes
            mainForm.BackColor = Color.Pink;
            //mainForm.BackColor = Color.FromArgb(0xFF, 0xFF, 0xC0, 0xE0);
            mainForm.BackColor = Color.FromArgb(0xFF, 0xFF, 0xE0, 0xF0);
            menuStrip1.BackColor = mainForm.BackColor;
            kirbyIcon = Icon.ExtractAssociatedIcon("KRTDL\\Resources\\kirby-star.ico");
            kirbyIconAlt = Icon.ExtractAssociatedIcon("KRTDL\\Resources\\kirby-blue.ico");
            mainForm.Icon = kirbyIcon;

            // add krtdl menu, for any commands I fancy
            RecolorHelper.Instance.InjectMenuItems(resourceTree, menuStrip1, 2);

            // move menu bar outside split panel
            splitContainer1.Panel1.Controls.Remove(menuStrip1);
            menuStrip1.Dock = DockStyle.Top;
            mainForm.Controls.Add(menuStrip1);
            resourceTree.Dock = DockStyle.Fill;

            // Margins to splitter 1, just for aethetics
            var s = new Size(4, 4);
            var cx = SystemColors.Control;
            var ct1 = new TransControl() { Dock = DockStyle.Top, Size = s, BackColor = cx };
            var ct2 = new TransControl() { Dock = DockStyle.Top, Size = s, BackColor = cx };
            var cl1 = new TransControl() { Dock = DockStyle.Left, Size = s, BackColor = cx };
            var cr1 = new TransControl() { Dock = DockStyle.Right, Size = s, BackColor = cx };
            var cb1 = new TransControl() { Dock = DockStyle.Bottom, Size = s, BackColor = cx };
            var cb2 = new TransControl() { Dock = DockStyle.Bottom, Size = s, BackColor = cx };
            splitContainer1.Panel1.Controls.Add(cl1);
            splitContainer1.Panel1.Controls.Add(cb1);
            splitContainer1.Panel2.Controls.Add(cr1);
            splitContainer1.Panel2.Controls.Add(cb2);
            splitContainer1.BackColor = Color.Transparent;

            // visual improvements to panel 2, such as white + border + watermark
            splitContainer2.Panel2.BackColor = Color.White;
            var kirbyWatermark = Image.FromFile("KRTDL\\Resources\\kirby-watermark.png");
            splitContainer2.Panel2.BackgroundImage = kirbyWatermark;
            splitContainer2.Panel2.BackgroundImageLayout = ImageLayout.None;
            splitContainer2.Panel2.Paint += Panel2_Paint; ;
        }

        // qwe - just adds a border around the default empty panel
        private void Panel2_Paint(object sender, PaintEventArgs e)
        {
            Panel p = sender as Panel;
            ControlPaint.DrawBorder(e.Graphics, p.DisplayRectangle, SystemColors.ControlDark, ButtonBorderStyle.Solid);
        }
    }
}
