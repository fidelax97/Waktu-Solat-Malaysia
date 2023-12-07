using Microsoft.Web.WebView2.Core;
using System.Diagnostics;

namespace Waktu_Solat_Malaysia
{
    public partial class Form1 : Form
    {
        public string classToModify = "main-header";
        public string classToHide1 = "navbar";
        public int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
        public int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
        //public int screenWidth = 3440;
        //public int screenHeight = 1440;
        public double compactwidthFactor;
        public double compactheightFactor;
        public double mediumwidthFactor;
        public double mediumheightFactor;
        

        private LoadingForm loadingForm;

        public Form1()
        {
            InitializeComponent();
            if (screenWidth >= 2000 && screenHeight >= 1300)
            {
                compactwidthFactor = 7.4;
                compactheightFactor = 3.72;
                mediumwidthFactor = 3;
                mediumheightFactor = 2.5;
            }
            else if (screenWidth >= 1500 && screenHeight >= 1000)
            {
                compactwidthFactor = 4.4;
                compactheightFactor = 2.6;
                mediumwidthFactor = 2.1;
                mediumheightFactor = 2.2;
            }
            else
            {
                compactwidthFactor = 3.73;
                compactheightFactor = 2.20;
                mediumwidthFactor = 1.8;
                mediumheightFactor = 1.8;
            }

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Size = new Size(290, 300);

            this.StartPosition = FormStartPosition.CenterScreen;
            Debug.WriteLine(Properties.Settings.Default.UserSelectedSize);
            string SelectedSize = Properties.Settings.Default.UserSelectedSize;

            loadingForm = new LoadingForm(); // Create the loading form
            loadingForm.TopMost = true;
            loadingForm.Show();
            this.Hide();
            webView21.Source = new Uri("https://www.waktusolat.digital/");
            webView21.CoreWebView2InitializationCompleted += WebViewInitialized;

            // Override WndProc to prevent minimizing on double-click of the title bar
            this.SetStyle(ControlStyles.StandardDoubleClick, false);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NCLBUTTONDBLCLK = 0xA3;

            if (m.Msg == WM_NCLBUTTONDBLCLK)
            {
                // Ignore the double-click on the non-client area (title bar).
                return;
            }
            base.WndProc(ref m);
        }

        private void WebViewInitialized(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                webView21.CoreWebView2.NavigationCompleted += WebViewLoaded;
            }
            else
            {
                Debug.WriteLine("Failed");
            }
        }

        private void WebViewLoaded(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            this.Hide();
            menuStrip1.Visible = true;
            string userChoice = Properties.Settings.Default.UserSelectedSize;
            Debug.WriteLine(userChoice);
            if (userChoice == "Compact")
            {
                // Calculate the desired width and height based on the percentages.
                int desiredWidth = (int)(screenWidth / compactwidthFactor);
                int desiredHeight = (int)(screenHeight / compactheightFactor);
                this.Size = new Size(desiredWidth, desiredHeight);

                this.StartPosition = FormStartPosition.Manual;
                // Calculate the x-coordinate for the form's top-left corner to position it on the right side
                int xCoordinate = screenWidth - desiredWidth;

                // Make sure the form is entirely within the screen boundaries
                xCoordinate = Math.Max(0, Math.Min(xCoordinate, screenWidth));
                this.Location = new Point(xCoordinate, 0);

                compactToolStripMenuItem.Checked = true;
                compactSettings();
            }
            if (userChoice == "Medium")
            {
                // Calculate the desired width and height based on the percentages.
                int desiredWidth = (int)(screenWidth / mediumwidthFactor);
                int desiredHeight = (int)(screenHeight / mediumheightFactor);
                this.Size = new Size(desiredWidth, desiredHeight);

                this.StartPosition = FormStartPosition.Manual;
                // Calculate the x-coordinate for the form's top-left corner to position it on the right side
                int xCoordinate = screenWidth - desiredWidth;

                // Make sure the form is entirely within the screen boundaries
                xCoordinate = Math.Max(0, Math.Min(xCoordinate, screenWidth));
                this.Location = new Point(xCoordinate, 0);

                compactToolStripMenuItem.Checked = true;
                mediumSettings();
            }
            if (userChoice == "HalfScreen")
            {
                this.StartPosition = FormStartPosition.Manual;

                // Get the working area of the primary screen (excluding taskbar).
                Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;

                // Calculate the width and height for the form (half of the screen's width and full height).
                int formWidth = workingArea.Width / 2;
                int formHeight = workingArea.Height;

                // Set the form's size and position it to the left.
                this.Size = new Size(formWidth, formHeight);
                this.Location = new Point(workingArea.Right, workingArea.Top);
                halfScreenToolStripMenuItem.Checked = true;
                standardSettings();
            }
            if (userChoice == "Large")
            {
                this.StartPosition = FormStartPosition.CenterScreen; // Center the form
                this.WindowState = FormWindowState.Maximized;
                largeToolStripMenuItem.Checked = true;
                largeSettings();
            }
            loadingForm.Close();
            this.Show();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            // Ensure the WebView2 control resizes with the form.
            webView21.Size = new Size(this.ClientSize.Width, this.ClientSize.Height);

            if (this.WindowState == FormWindowState.Maximized)
            {
                toggleWindowSizeStatus(largeToolStripMenuItem);

                // Save the user's choice
                Properties.Settings.Default.UserSelectedSize = "Large";
                Properties.Settings.Default.Save();
                largeSettings();
            }

            Debug.WriteLine("Window Size: Width = " + this.Width + ", Height = " + this.Height);
        }

        private void largeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.Manual;
            // Set the window size for the "Large" option.
            this.WindowState = FormWindowState.Maximized;
            toggleWindowSizeStatus(largeToolStripMenuItem);

            // Save the user's choice
            Properties.Settings.Default.UserSelectedSize = "Large";
            Properties.Settings.Default.Save();
            largeSettings();
        }

        private void halfScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.Manual;
            this.WindowState = FormWindowState.Normal;
            this.StartPosition = FormStartPosition.Manual;

            // Get the working area of the primary screen (excluding taskbar).
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;

            // Calculate the width and height for the form (half of the screen's width and full height).
            int formWidth = workingArea.Width / 2;
            int formHeight = workingArea.Height;

            // Set the form's size and position it to the left.
            this.Size = new Size(formWidth, formHeight);
            this.Location = new Point(workingArea.Left, workingArea.Top);
            toggleWindowSizeStatus(halfScreenToolStripMenuItem);

            // Save the user's choice
            Properties.Settings.Default.UserSelectedSize = "HalfScreen";
            Properties.Settings.Default.Save();
            standardSettings();
        }

        private void mediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.FormBorderStyle = (this.FormBorderStyle == FormBorderStyle.Sizable)
                ? FormBorderStyle.FixedSingle
                : FormBorderStyle.Sizable;
            this.StartPosition = FormStartPosition.Manual;
            this.WindowState = FormWindowState.Normal;

            // Calculate the desired width and height based on the factor
            int desiredWidth = (int)(screenWidth / mediumwidthFactor);
            int desiredHeight = (int)(screenHeight / mediumheightFactor);
            this.Size = new Size(desiredWidth, desiredHeight);

            this.StartPosition = FormStartPosition.Manual;
            // Calculate the x-coordinate for the form's top-left corner to position it on the right side
            int xCoordinate = screenWidth - desiredWidth;

            // Make sure the form is entirely within the screen boundaries
            xCoordinate = Math.Max(0, Math.Min(xCoordinate, screenWidth));

            this.Location = new Point(xCoordinate, 0);

            toggleWindowSizeStatus(mediumToolStripMenuItem);

            // Save the user's choice
            Properties.Settings.Default.UserSelectedSize = "Medium";
            Properties.Settings.Default.Save();
            mediumSettings();
        }

        private void compactToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.Manual;
            this.WindowState = FormWindowState.Normal;

            // Calculate the desired width and height based on the factor
            int desiredWidth = (int)(screenWidth / compactwidthFactor);
            int desiredHeight = (int)(screenHeight / compactheightFactor);
            this.Size = new Size(desiredWidth, desiredHeight);

            this.StartPosition = FormStartPosition.Manual;

            // Calculate the x-coordinate for the form's top-left corner to position it on the right side
            int xCoordinate = screenWidth - desiredWidth;

            // Make sure the form is entirely within the screen boundaries
            xCoordinate = Math.Max(0, Math.Min(xCoordinate, screenWidth));

            this.Location = new Point(xCoordinate, 0);

            toggleWindowSizeStatus(compactToolStripMenuItem);

            // Save the user's choice
            Properties.Settings.Default.UserSelectedSize = "Compact";
            Properties.Settings.Default.Save();
            compactSettings();
        }

        private async void compactSettings()
        {
            if (screenWidth >= 1500 && screenHeight >= 1000)
            {
                webView21.ZoomFactor = 0.6;
            }
            else
            {
                webView21.ZoomFactor = 0.5;
            }

            string cssScript = $"var elementToModify = document.querySelector('.{classToModify}'); " +
              "if (elementToModify) { " +
              "elementToModify.style.position = 'static'; " +
              "elementToModify.style.top = '0'; " +
              "}";
            await webView21.ExecuteScriptAsync(cssScript);

            // Scroll to top of the page
            string scrollScript = "window.scrollTo(0, 255);"; 
            await webView21.ExecuteScriptAsync(scrollScript);

            // Second script to disable the scrollbar
            string disableScrollbarScript = "document.documentElement.style.overflow = 'hidden';";
            await webView21.ExecuteScriptAsync(disableScrollbarScript);
        }

        private async void mediumSettings()
        {
            if (screenWidth >= 1500 && screenHeight >= 1000)
            {
                webView21.ZoomFactor = 0.65;
            }
            else
            {
                webView21.ZoomFactor = 0.55;
            }

            string cssScript = $"var elementToModify = document.querySelector('.{classToModify}'); " +
              "if (elementToModify) { " +
              "elementToModify.style.position = 'static'; " +
              "elementToModify.style.top = '0'; " +
              "}";
            await webView21.ExecuteScriptAsync(cssScript);

            // Scroll to top of the page
            string scrollScript = "window.scrollTo(0, 235);"; 
            await webView21.ExecuteScriptAsync(scrollScript);

            // Second script to disable the scrollbar
            string disableScrollbarScript = "document.documentElement.style.overflow = 'hidden';";
            await webView21.ExecuteScriptAsync(disableScrollbarScript);
        }

        private async void standardSettings()
        {
            webView21.ZoomFactor = 0.6;  

            // Inject custom CSS to show elements with a specific class.
            string cssScript1 = $"var elementToModify = document.querySelector('.{classToModify}'); " +
              "if (elementToModify) { " +
              "elementToModify.style.position = 'fixed'; " +
              "elementToModify.style.top = '0'; " +
              "}";

            await webView21.ExecuteScriptAsync(cssScript1);

            string cssScript2 = $"var elementToModify = document.querySelector('.{classToHide1}'); " +
              "if (elementToModify) { " +
              "elementToModify.style.display = 'none'; " +
              "}";
            await webView21.ExecuteScriptAsync(cssScript2);

            // To show the scrollbar again
            string showScrollbarScript = "document.documentElement.style.overflow = 'auto';";
            await webView21.ExecuteScriptAsync(showScrollbarScript);
        }

        private async void largeSettings()
        {
            webView21.ZoomFactor = 0.9;  // Set the default zoom factor.

            // Inject custom CSS to show elements with a specific class.
            string cssScript1 = $"var elementToModify = document.querySelector('.{classToModify}'); " +
              "if (elementToModify) { " +
              "elementToModify.style.position = 'fixed'; " +
              "elementToModify.style.top = '0'; " +
              "}";

            await webView21.ExecuteScriptAsync(cssScript1);

            string cssScript2 = $"var elementToModify = document.querySelector('.{classToHide1}'); " +
              "if (elementToModify) { " +
              "elementToModify.style.display = 'none'; " +
              "}";
            await webView21.ExecuteScriptAsync(cssScript2);

            // To show the scrollbar again
            string showScrollbarScript = "document.documentElement.style.overflow = 'auto';";
            await webView21.ExecuteScriptAsync(showScrollbarScript);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutForm aboutForm = new())
            {
                aboutForm.ShowDialog();
            }
        }

        private void toggleWindowSizeStatus(ToolStripMenuItem selectedMenuItem)
        {
            // Uncheck all menu items
            largeToolStripMenuItem.Checked = false;
            compactToolStripMenuItem.Checked = false;
            halfScreenToolStripMenuItem.Checked = false;
            mediumToolStripMenuItem.Checked = false;

            // Check the selected menu item
            selectedMenuItem.Checked = true;
        }

    }
}