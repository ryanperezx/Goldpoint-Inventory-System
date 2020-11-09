using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Diagnostics;
using System.Xml;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using System.Windows.Forms.Integration;
using System.Reflection;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.SfSkinManager;
using Syncfusion.Windows.Tools;
using Syncfusion.Themes.Office2019Colorful.WPF;
using WinForms = System.Windows.Forms;

namespace Goldpoint_Inventory_System
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            SfSkinManager.ApplyStylesOnApplication = true;
            InitializeComponent();
            DockingManager.SetState(StockIn, DockState.Hidden);
            DockingManager.SetState(Account, DockState.Hidden);
            DockingManager.SetState(ModifyInvent, DockState.Hidden);
            DockingManager.SetState(InventCheck, DockState.Hidden);
            DockingManager.SetState(Photocopy, DockState.Hidden);
            DockingManager.SetState(StockOut, DockState.Hidden);
            DockingManager.SetState(Sales, DockState.Hidden);
            DockingManager.SetState(TransactionLog, DockState.Hidden);
            DockingManager.SetState(TransactionDetails, DockState.Hidden);
            DockingManager.SetState(JobOrder, DockState.Hidden);

            Office2019ColorfulThemeSettings themeSettings = new Office2019ColorfulThemeSettings();
            themeSettings.PrimaryBackground = new SolidColorBrush(Colors.DarkGoldenrod);
            themeSettings.PrimaryForeground = new SolidColorBrush(Colors.White);
            themeSettings.BodyFontSize = 14;
            themeSettings.HeaderFontSize = 14;
            themeSettings.SubHeaderFontSize = 14;
            themeSettings.TitleFontSize = 14;
            themeSettings.SubTitleFontSize = 14;
            themeSettings.FontFamily = new FontFamily("Calibri");
            SfSkinManager.RegisterThemeSettings("Office2019Colorful", themeSettings);
            SfSkinManager.SetTheme(this, new Theme("Office2019Colorful"));


        }

        bool m_layoutflag = true;
        private void DockingManager_LayoutUpdated(object sender, EventArgs e)
        {
            if (m_layoutflag)
            {

                // SfSkinManager.SetVisualStyle(this, VisualStyles.Metro);
                m_layoutflag = false;
            }
        }

        private void DockingManager_CloseAllTabs(object sender, CloseTabEventArgs e)
        {
            string closingtabs = "";
            MessageBoxResult result = MessageBox.Show("Do you want to close the tabs? ", "Closing Tabs", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                for (int i = 0; i < e.ClosingTabItems.Count; i++)
                {
                    TabItemExt tabitem = e.ClosingTabItems[i] as TabItemExt;
                    if (tabitem.Content != null && (tabitem.Content as ContentPresenter) != null)
                    {
                        ContentPresenter presenter = tabitem.Content as ContentPresenter;
                        if (presenter != null && presenter.Content != null)
                        {
                            closingtabs = closingtabs + "\n\t" + DockingManager.GetHeader(presenter.Content as DependencyObject);
                        }
                    }
                }
            }
            else if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void DockingManager_CloseOtherTabs(object sender, CloseTabEventArgs e)
        {
            string closingtabs = "";
            MessageBoxResult result = MessageBox.Show("Do you want to close the tabs? ", "Closing Tabs", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                for (int i = 0; i < e.ClosingTabItems.Count; i++)
                {
                    TabItemExt tabitem = e.ClosingTabItems[i] as TabItemExt;
                    if (tabitem.Content != null && (tabitem.Content as ContentPresenter) != null)
                    {
                        ContentPresenter presenter = tabitem.Content as ContentPresenter;
                        if (presenter != null && presenter.Content != null)
                        {
                            closingtabs = closingtabs + "\n\t" + DockingManager.GetHeader(presenter.Content as DependencyObject);
                        }
                    }
                }
            }
            else if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void DockingManager_IsSelectedDocument(FrameworkElement sender, IsSelectedChangedEventArgs e)
        {
            if (DockingManager.DocContainer != null && SfSkinManager.GetVisualStyle(this) != SfSkinManager.GetVisualStyle(DockingManager.DocContainer as DependencyObject))
            {
                SfSkinManager.SetVisualStyle(DockingManager.DocContainer as DependencyObject, SfSkinManager.GetVisualStyle(this));
            }
        }

        private void ActivateWindow(object sender, RoutedEventArgs e)
        {
            string name = (sender as MenuItem).Tag as string;
            DockingManager.ActivateWindow(name);

        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            string sMessageBoxText = "Do you want to log out?";
            string sCaption = "Logout";
            MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
            MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

            MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

            switch (dr)
            {
                case MessageBoxResult.Yes:
                    this.DialogResult = false;
                    Close();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }



        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            string sMessageBoxText = "Do you want to exit the application?";
            string sCaption = "Exit";
            MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
            MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

            MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

            switch (dr)
            {
                case MessageBoxResult.Yes:
                    this.DialogResult = true;
                    Application.Current.Shutdown();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }
    }
}
