using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrawlCrate.KRTDL
{
    // This is a transparent control.
    // I just use it to pad the mainform's border.
    // Because it is see-through, I can see through to the form's color.
    public class TransControl : Control
   {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20;
                return cp;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // do nothing
        }
    }
}
