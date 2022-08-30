using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GetFontFileName
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private Dictionary<string, string> m_fontFileInReg = null;

        private void Form1_Load(object sender, EventArgs e)
        {
            comboFontName.Items.Clear();
            comboFontName.Items.Add(string.Empty);

            InstalledFontCollection ifc = new InstalledFontCollection();
            foreach (FontFamily ff in ifc.Families)
            {
                comboFontName.Items.Add(ff.Name);
            }

            m_fontFileInReg = GetFontFileInfoInReg();
        }

        private void btnDo_Click(object sender, EventArgs e)
        {
            if (m_fontFileInReg.ContainsKey(comboFontName.Text))
            {
                txtFileName.Text = m_fontFileInReg[comboFontName.Text];
            }
            else
            {
                txtFileName.Text = GetFontFileName(comboFontName.Text);
            }
        }

        public static string GetFontFileName(string fontname)
        {
            string folderFullName = System.Environment.GetEnvironmentVariable("windir") + "\\fonts";
            DirectoryInfo TheFolder = new DirectoryInfo(folderFullName);
            foreach (FileInfo NextFile in TheFolder.GetFiles())
            {
                if (NextFile.Exists)
                {
                    if (fontname == GetFontName(NextFile.FullName))
                    {

                        return NextFile.Name;
                    }
                }
            }
            return "";
        }

        private static string GetFontName(string fontfilename)
        {
            PrivateFontCollection pfc = new PrivateFontCollection();
            //只要ttf和TTF, 其它的本项目不需要
            if (fontfilename.EndsWith(".ttf") || fontfilename.EndsWith(".TTF") || fontfilename.EndsWith(".ttc"))
            {
                pfc.AddFontFile(fontfilename);
            }
            if (pfc.Families.Length > 0)
            {
                return pfc.Families[0].Name;
            }
            return "";
        }

        private Dictionary<string, string> GetFontFileInfoInReg()
        {
            Dictionary<string, string> result= new Dictionary<string, string>();

            RegistryKey localMachineKey = Registry.LocalMachine;  
            RegistryKey localMachineKeySub = localMachineKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts", false);

            string[] mynames = localMachineKeySub.GetValueNames();

            foreach (string name in mynames)
            {
                //获取字体的文件名  
                string myvalue = localMachineKeySub.GetValue(name).ToString();

                if (myvalue.Substring(myvalue.Length - 4).ToUpper() == ".TTF" && myvalue.Substring(1, 2).ToUpper() != @":")
                {
                    string val = name.Substring(0, name.Length - 11);
                    result[val] = myvalue;
                }
            }
            localMachineKeySub.Close();
            return result;
        }
    }
}
