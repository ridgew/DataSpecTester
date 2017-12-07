using System;
using System.Drawing;
using System.Windows.Forms;
using Gwsoft.DataSpec;
using System.Drawing.Imaging;

namespace DataSpecTester
{
    public partial class PicViewForm : Form
    {
        public PicViewForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 获取或设置图片的二进制序列
        /// </summary>
        public byte[] PicBytes { get; set; }

        /// <summary>
        /// Handles the Load event of the PicViewForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void PicViewForm_Load(object sender, EventArgs e)
        {
            if (PicBytes != null && PicBytes.Length > 0)
            {
                RefreshPic();
            }
        }

        public static string GetMimeType(Image i)
        {
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
            {
                if (codec.FormatID == i.RawFormat.Guid)
                    return codec.MimeType;
            }

            return "image/unknown";
        }

        /// <summary>
        /// Refreshes the pic.
        /// </summary>
        public void RefreshPic()
        {
            picBox.Invalidate(); 
            picBox.Image = System.Drawing.Image.FromStream(PicBytes.AsMemoryStream());

            picInfo.Text = string.Format("{1}x{2} Size:{0} Format:{3}", PicBytes.Length,
                picBox.Image.Width,
                picBox.Image.Height, 
                GetMimeType(picBox.Image));
        }

        private void PicViewForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Owner != null)
            {
                Desktop top = Owner as Desktop;
                if (top != null)
                {
                    top.viewForm = null;
                }
            }
        }
    }
}
