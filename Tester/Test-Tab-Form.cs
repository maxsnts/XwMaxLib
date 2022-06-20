using KRBTabControlNS.CustomTab;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tester
{
    public partial class Test_Tab_Form : Form
    {
        public Test_Tab_Form()
        {
            InitializeComponent();
        }

        private void Test_Tab_Form_Load(object sender, EventArgs e)
        {
            TabPageEx tab1 = new TabPageEx("Tab11");
            ServerTabs._tabCloseBtn = KRBTabControlNS.CustomTab.KRBTabControl.TabCloseImage.Normal;
            ServerTabs.TabPages.Add(tab1);
            //tab.ImageIndex = (int)server.Type;
            ServerTabs.SelectTab(ServerTabs.TabPages.Count - 1);

            TabPageEx tab2 = new TabPageEx("Tab12");
            ServerTabs._tabCloseBtn = KRBTabControlNS.CustomTab.KRBTabControl.TabCloseImage.Normal;
            ServerTabs.TabPages.Add(tab2);
            //tab.ImageIndex = (int)server.Type;
            ServerTabs.SelectTab(ServerTabs.TabPages.Count - 1);

            TabPageEx tab3 = new TabPageEx("Tab13");
            ServerTabs._tabCloseBtn = KRBTabControlNS.CustomTab.KRBTabControl.TabCloseImage.Normal;
            ServerTabs.TabPages.Add(tab3);
            //tab.ImageIndex = (int)server.Type;
            ServerTabs.SelectTab(ServerTabs.TabPages.Count - 1);
        }
    }
}
