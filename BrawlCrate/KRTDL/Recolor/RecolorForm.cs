using BrawlCrate.KRTDL.Recolour;
using System;
using System.Windows.Forms;

namespace BrawlBox.UI
{
    public partial class RecolorForm : Form
    {
        public RecolorSettings RecolorSettings { get; private set; }

        private RecolorForm()
        {
            InitializeComponent();

            Shown += RecolorForm_Shown;
        }

        public RecolorForm(RecolorSettings recolorSettings, bool all) : this()
        {
            this.RecolorSettings = recolorSettings;
            if (all)
            {
                Text = "Recolor All Nodes!";
                label6.Text = "This tool will recolor all palettes and any non-palettized textures!";
            }
            else
            {
                Text = "Recolor All Nodes!";
                label6.Text = "This tool will recolor the currently selected node.";
            }
        }

        private void RecolorForm_Shown(object sender, EventArgs e)
        {
            numericHue.Value = RecolorSettings.HueShift;
            checkBoxInvertColors.Checked = RecolorSettings.ColorInvert;
            checkBoxDesaturateColors.Checked = RecolorSettings.ColorDesaturate;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var hs = (int) numericHue.Value;
            if (hs < 0 || hs > 240)
            {
                DialogResult = DialogResult.Cancel;
            }
            else
            {
                RecolorSettings.HueShift = hs;
                RecolorSettings.ColorInvert = checkBoxInvertColors.Checked;
                RecolorSettings.ColorDesaturate = checkBoxDesaturateColors.Checked;
                DialogResult = DialogResult.OK;
            }
        }

        void buttonShift(bool backwards)
        {
            int hs = RecolorHelper.Instance.GetControlledHueStep(backwards);
            var hue = (int) numericHue.Value;
            hue += hs;
            hue = hue % 240;
            if (hue < 0) hue += 240;
            numericHue.Value = hue;
        }

        private void buttonMinus_Click(object sender, EventArgs e)
        {
            buttonShift(true);
        }

        private void buttonPlus_Click(object sender, EventArgs e)
        {
            buttonShift(false);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Oemcomma)
                { buttonShift(true); e.Handled = true; }

            else if (e.KeyCode == Keys.OemPeriod)
                { buttonShift(false); e.Handled = true; }

            else if (e.KeyCode == Keys.Enter)
                { button1_Click(null, null); e.Handled = true; }

            else if (e.KeyCode == Keys.Escape)
                { DialogResult = DialogResult.Cancel; e.Handled = true; }

            base.OnKeyDown(e);
        }
    }
}
