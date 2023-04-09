using BrawlBox.UI;
using BrawlCrate.NodeWrappers;
using BrawlCrate.UI;
using BrawlLib.Imaging;
using BrawlLib.SSBB.ResourceNodes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BrawlCrate.KRTDL.Recolour
{
    public class RecolorSettings
    {
        public int HueShift = 200;
        public bool ColorInvert = false;
        public bool ColorDesaturate = false;
    }

    public class RecolorHelper
    {
        private ResourceTree resourceTree;

        private ToolStripMenuItem tsmi;
        private ToolStripItem tsiRecolorThisNode;
        private ToolStripItem tsiRecolorAllNodes;

        private RecolorSettings recolorSettinigs = new RecolorSettings();

        // hue shift steps (in 0 to hueDegrees range)
        // private static int hueStepHuge = 120;
        private static int hueStepNormal = 40;
        private static int hueStepSmall = 20;
        public const int hueDegrees = 240;

        public static RecolorHelper Instance { get; private set; } = new RecolorHelper();

        private RecolorHelper() { }

        public int GetControlledHueStep(bool invert)
        {
            int result = hueStepNormal;
            // if ((Control.ModifierKeys & (Keys.Shift | Keys.Control)) == (Keys.Shift | Keys.Control))
            //    result = hueStepHuge;
            // else
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                result = hueStepSmall;
            return invert ? -result : result;
        }

        // Recolors a single color (e.g. of a pixel)
        Color Recolor(Color color, RecolorSettings recolorSettinigs)
        {
            int hueShift = recolorSettinigs.HueShift;
            bool invert = recolorSettinigs.ColorInvert;
            bool desaturate = recolorSettinigs.ColorDesaturate;

            // convert hueShift from -hueDegrees to hueDegrees to 0 to hueDegrees
            hueShift = (hueShift + hueDegrees) % hueDegrees;

            // invert color
            if (invert)
                color = Color.FromArgb(color.A, 255 - color.R, 255 - color.G, 255 - color.B);

            // convert to HSLColor
            if (hueShift != 0)
            {
                HSLColor hslColor;
                hslColor = new HSLColor(color.R, color.G, color.B);
                if (invert) hslColor = new HSLColor(hueDegrees - color.R, 255 - color.G, 255 - color.B);
                var h2 = (hslColor.Hue + hueShift) % hueDegrees;      // shift it, but cycle if it goes beyond hueDegrees
                hslColor = new HSLColor(h2, hslColor.Saturation, hslColor.Luminosity);

                var rgbColor = (Color)hslColor;
                color = Color.FromArgb(color.A, rgbColor);
            }

            // desaturate color
            if (desaturate)
            {
                // this method is simple
                // var tone = (oldColor.R + oldColor.G + oldColor.B) / (255 * 3);
                // oldColor = Color.FromArgb(oldColor.A, tone, tone, tone);

                // people seem to like this method. I guess because green has more tone than red than blue
                var tone = (int)(.3 * color.R + .59 * color.G + .11 * color.B);
                color = Color.FromArgb(color.A, tone, tone, tone);
            }

            return color;
        }

        // Recolors an entire palette
        public void RecolorPalette(IColorSource colorSource, RecolorSettings recolorSettings)
        {
            var numColors = colorSource.ColorCount(0);

            for (int i = 0; i < numColors; i++)
            {
                var argbColor = colorSource.GetColor(i, 0);
                var drawColor = (Color)argbColor;
                var newColor = Recolor(drawColor, recolorSettinigs);
                colorSource.SetColor(i, 0, new ARGBPixel(newColor.A, newColor.R, newColor.G, newColor.B));
            }
        }

        // Recolors an entire texture, see known issues below.
        public void RecolorTexture(TEX0Node imageSource, RecolorSettings recolorSettings)
        {
            // if (imageSource.ImageCount > 1) return;
            if (imageSource.HasPalette) return;

            // known issues:
            //  - this could be faster by locking the bits on the bitmap.
            //  - some color formats cannot totally desaturate accurately.
            //    (an option could be added to change to a format that can).
            //    (another option could be to snap to the nearest true grey).
            //  - setting the 1st image causes the mip maps to be regenerated,
            //    but the quality of the mipmaps can be worse than the originals
            //    (it would be better to recolor each bitmap/mipmap individually,
            //     in their current memory and format, so they are not regenerated).
            var bm = imageSource.GetImage(0);
            for (int x = 0; x < bm.Width; x++)
            {
                for (int y = 0; y < bm.Height; y++)
                {
                    var oldColor = bm.GetPixel(x, y);
                    var newColor = Recolor(oldColor, recolorSettinigs);
                    bm.SetPixel(x, y, newColor);
                }
            }

            imageSource.Replace(bm);
            imageSource.IsDirty = true;

            // refresh the UI
            // resourceTree_SelectionChanged(null, null);
        }

        // Recolors all texture and palette nodes in the resource tree
        public void RecolorTree(ResourceTree resourceTree, RecolorSettings recolorSettings)
        {
            var oldSelection = resourceTree.SelectedNode;
            foreach (TreeNode n in resourceTree.Nodes)
            {
                RecolorTreeNodeRecursive(n, recolorSettings);
            }

            if (resourceTree.SelectedNode != oldSelection)
            {
                resourceTree.CollapseAll();
                resourceTree.SelectedNode = oldSelection;
            }
        }

        // Recolours the current node and all descendant nodes
        private void RecolorTreeNodeRecursive(TreeNode treeNode, RecolorSettings recolorSettings)
        {
            if (CanRecolorTreeNode(treeNode))
            {
                // select node for visual progress indication 
                resourceTree.SelectedNode = treeNode;
                RecolorTreeNode(treeNode, recolorSettings);
            }

            // Visit each node recursively.  
            foreach (TreeNode tn in treeNode.Nodes)
            {
                RecolorTreeNodeRecursive(tn, recolorSettings);
            }
        }

        // Recolors the given tree node (not recursive)
        private void RecolorTreeNode(TreeNode treeNode, RecolorSettings recolorSettings)
        {
            var bw = treeNode as BaseWrapper;
            var res = bw.Resource;

            if (res is TEX0Node resAsTex0Node)
            {
                // select node for visual indication 
                RecolorTexture(resAsTex0Node, recolorSettings);
            }

            else if (res is IColorSource resAsIColorSource)
            {
                // select node for visual indication 
                RecolorPalette(resAsIColorSource, recolorSettings);
            }
        }

        // Returns true if this tree node can be recolored.
        private bool CanRecolorTreeNode(TreeNode treeNode)
        {
            if (treeNode is BaseWrapper baseWrapper)
            {
                var res = baseWrapper.Resource;
                return res is TEX0Node || res is IColorSource resAsIColorSource;
            }
            return false;
        }


        public void InjectMenuItems(ResourceTree resourceTree, MenuStrip menuStrip, int v)
        {
            this.resourceTree = resourceTree;
            tsmi = new ToolStripMenuItem("&Krtdl");

            tsiRecolorThisNode = tsmi.DropDownItems.Add("Recolor This Node...", null, OnRecolorThisNode);
            tsiRecolorAllNodes = tsmi.DropDownItems.Add("Recolor All Nodes...", null, OnRecolorAllNodes);
            //tsmi.DropDownItems.Add("-");
            //tsmi.DropDownItems.Add("Shift Hue Left", null, OnShiftHueLeft);
            //tsmi.DropDownItems.Add("Shift Hue Right", null, OnShiftHueRight);
            //tsmi.DropDownItems.Add("Invert Colors", null, OnInvertColors);
            //tsmi.DropDownItems.Add("Desaturate", null, OnDesaturate);

            menuStrip.Items.Insert(v, tsmi);

            resourceTree.SelectionChanged += ResourceTree_SelectionChanged;
            UpdateSelectedNode(resourceTree.SelectedNode);
        }

        private void ResourceTree_SelectionChanged(object sender, EventArgs e)
        {
            UpdateSelectedNode(resourceTree.SelectedNode);
        }

        internal void UpdateSelectedNode(TreeNode treeNode)
        {
            if (tsiRecolorThisNode == null || tsiRecolorAllNodes == null || resourceTree == null) return;

            tsiRecolorThisNode.Enabled = CanRecolorTreeNode(treeNode);
            tsiRecolorAllNodes.Enabled = resourceTree.Nodes.Count > 0;
        }

        private static void OnDesaturate(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void OnInvertColors(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void OnShiftHueRight(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void OnShiftHueLeft(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void OnRecolorThisNode(object sender, EventArgs e)
        {
            var rc = new RecolorForm(Instance.recolorSettinigs, false);
            if (rc.ShowDialog() == DialogResult.OK)
            {
                Instance.recolorSettinigs = rc.RecolorSettings;
                Instance.RecolorTreeNode(Instance.resourceTree.SelectedNode, Instance.recolorSettinigs);
            }
        }

        private static void OnRecolorAllNodes(object sender, EventArgs e)
        {
            var rc = new RecolorForm(Instance.recolorSettinigs, true);
            if (rc.ShowDialog() == DialogResult.OK)
            {
                Instance.recolorSettinigs = rc.RecolorSettings;
                Instance.RecolorTree(Instance.resourceTree, Instance.recolorSettinigs);
            }
        }
    }
}
