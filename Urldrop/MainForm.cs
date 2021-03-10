// <copyright file="MainForm.cs" company="PUblicDomain.com">
//     CC0 1.0 Universal (CC0 1.0) - Public Domain Dedication
//     https://creativecommons.org/publicdomain/zero/1.0/legalcode
// </copyright>

namespace Urldrop
{
    // Directives
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Windows.Forms;
    using HtmlAgilityPack;
    using Microsoft.VisualBasic;
    using PublicDomain;

    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Gets or sets the associated icon.
        /// </summary>
        /// <value>The associated icon.</value>
        private Icon associatedIcon = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Urldrop.MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            // The InitializeComponent() call is required for Windows Forms designer support.
            this.InitializeComponent();

            /* Set icons */

            // Set associated icon from exe file
            this.associatedIcon = Icon.ExtractAssociatedIcon(typeof(MainForm).GetTypeInfo().Assembly.Location);

            // Set public domain gift tool strip menu item image
            this.moreReleasesPublicDomainGiftcomToolStripMenuItem.Image = this.associatedIcon.ToBitmap();

            /* Configure */

            /* Load */

            // Check for passed files
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                // Set file list 
                var fileList = Environment.GetCommandLineArgs().ToList();

                // Remove first item (Executable)
                fileList.RemoveAt(0);

                // files
                this.PopulateByFile(fileList);
            }
        }

        /// <summary>
        /// Hndles the new tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnNewToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Clear URL list
            this.urlListBox.Items.Clear();

            // Reset count
            this.countToolStripStatusLabel.Text = "0";
        }

        /// <summary>
        /// Handles the open tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Prepare dialog properties [Move to Mainform.Designer]
            this.openFileDialog.Title = "Open file(s) with URLs";
            this.openFileDialog.Multiselect = true;
            this.openFileDialog.Filter = "Supported Files|*.txt;*.htm;*.html|TXT Files|*.txt|HTML Files|*.htm;*.html|All files (*.*)|*.*";

            // Show open file dialog
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Populate by opened file(s)
                    this.PopulateByFile(new List<string>(this.openFileDialog.FileNames));
                }
                catch (Exception exception)
                {
                    // Inform user
                    MessageBox.Show($"Error when opening \"{Path.GetFileName(this.openFileDialog.FileName)}\":{Environment.NewLine}{exception.Message}", "Open file error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Handles the save tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSaveToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Open save file dialog
            if (this.urlListBox.Items.Count > 0 && this.saveFileDialog.ShowDialog() == DialogResult.OK && this.saveFileDialog.FileName.Length > 0)
            {
                try
                {
                    // Save URL list to file
                    File.WriteAllText(this.saveFileDialog.FileName, string.Join(Environment.NewLine, this.urlListBox.Items.Cast<string>().ToList()));
                }
                catch (Exception exception)
                {
                    // Inform user
                    MessageBox.Show($"Error when saving to \"{Path.GetFileName(this.saveFileDialog.FileName)}\":{Environment.NewLine}{exception.Message}", "Save file error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Inform user
                MessageBox.Show($"Saved file to \"{Path.GetFileName(this.saveFileDialog.FileName)}\"", "URL file", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Hndles the exit tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Close program
            this.Close();
        }

        /// <summary>
        /// Hndles the cut tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnCutToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Prevent drawing
            this.urlListBox.BeginUpdate();

            // Cut list
            var cutList = new List<string>();

            // Remove selected items
            for (int i = this.urlListBox.SelectedIndices.Count - 1; i >= 0; i--)
            {
                // Insert into list 
                cutList.Insert(0, this.urlListBox.Items[this.urlListBox.SelectedIndices[i]].ToString());

                // Remove last selected item
                this.urlListBox.Items.RemoveAt(this.urlListBox.SelectedIndices[i]);
            }

            // Copy to clipboard
            Clipboard.SetText(string.Join(Environment.NewLine, cutList));

            // Resume drawing
            this.urlListBox.EndUpdate();

            // Update count
            this.countToolStripStatusLabel.Text = this.urlListBox.Items.Count.ToString();
        }

        /// <summary>
        /// Hndles the copy tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnCopyToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Prevent drawing
            this.urlListBox.BeginUpdate();

            // Copy string bulder
            var copyStringBuilder = new StringBuilder();

            // Edit selected items
            for (int i = 0; i < this.urlListBox.SelectedIndices.Count; i++)
            {
                // Add item line
                copyStringBuilder.AppendLine(this.urlListBox.Items[this.urlListBox.SelectedIndices[i]].ToString());
            }

            // Copy to clipboard
            Clipboard.SetText(copyStringBuilder.ToString().Trim());

            // Resume drawing
            this.urlListBox.EndUpdate();
        }

        /// <summary>
        /// Hndles the paste tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnPasteToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Prevent drawing
            this.urlListBox.BeginUpdate();

            // Split clipboard lines (discarding empty ones)
            string[] lines = Clipboard.GetText().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // Iterate lines
            for (int i = 0; i < lines.Length; i++)
            {
                // Set trimmed line
                string trimmedLine = lines[i].Trim();

                // Validate
                if (this.ValidateUri(trimmedLine))
                {
                    // Add to URL list
                    this.urlListBox.Items.Add(trimmedLine);
                }
            }

            // Resume drawing
            this.urlListBox.EndUpdate();

            // Update count
            this.countToolStripStatusLabel.Text = this.urlListBox.Items.Count.ToString();
        }

        /// <summary>
        /// Hndles the options tool strip menu item drop down item clicked.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOptionsToolStripMenuItemDropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // Prevent multiselect checking
            if (e.ClickedItem == this.selectionmodeToolStripMenuItem)
            {
                // Halt flow
                return;
            }

            // Set tool strip menu item
            ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)e.ClickedItem;

            // Toggle checked
            toolStripMenuItem.Checked = !toolStripMenuItem.Checked;

            // Set topmost
            this.TopMost = this.alwaysOnTopToolStripMenuItem.Checked;

            // Set sorted
            this.urlListBox.Sorted = this.sortedListToolStripMenuItem.Checked;
        }

        /// <summary>
        /// Hndles the more releases public domain giftcom tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnMoreReleasesPublicDomainGiftcomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Open current website
            Process.Start("https://publicdomaingift.com");
        }

        /// <summary>
        /// Hndles the original thread donation codercom tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOriginalThreadDonationCodercomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Open original thread @ DonationCoder
            Process.Start("https://www.donationcoder.com/forum/index.php?topic=34285");
        }

        /// <summary>
        /// Hndles the source code githubcom tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSourceCodeGithubcomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Open GitHub repository
            Process.Start("https://github.com/publicdomain/urldrop");
        }

        /// <summary>
        /// Hndles the about tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnAboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Set about message text
            var aboutMessage =
                $"RELEASE SPONSORS:{Environment.NewLine}{Environment.NewLine}" +
                $"* Jesse Reichler (A.K.A. Mouser from DonationCoder.com){Environment.NewLine}" +
                $"* Tim Parsons{Environment.NewLine}{Environment.NewLine}" +
                $"=========={Environment.NewLine}{Environment.NewLine}" +
                $"CC0 1.0 Universal (CC0 1.0) - Public Domain Dedication{Environment.NewLine}" +
                $"https://creativecommons.org/publicdomain/zero/1.0/legalcode{Environment.NewLine}{Environment.NewLine}" +
                $"Libraries and icons have separate licenses.{Environment.NewLine}{Environment.NewLine}" +
               $"Target icon by madartzgraphics - Pixabay License{Environment.NewLine}" +
               $"https://pixabay.com/vectors/target-arrow-bulls-eye-bullseye-2070972/{Environment.NewLine}{Environment.NewLine}" +
               $"HtmlAgilityPack by ZZZ Projects - MIT License{Environment.NewLine}" +
               $"https://github.com/zzzprojects/html-agility-pack/{Environment.NewLine}{Environment.NewLine}" +
               $"Patreon icon used according to published brand guidelines{Environment.NewLine}" +
               $"https://www.patreon.com/brand{Environment.NewLine}{Environment.NewLine}" +
               $"GitHub mark icon used according to published logos and usage guidelines{Environment.NewLine}" +
               $"https://github.com/logos{Environment.NewLine}{Environment.NewLine}" +
               $"DonationCoder icon used with permission{Environment.NewLine}" +
               $"https://www.donationcoder.com/forum/index.php?topic=48718{Environment.NewLine}{Environment.NewLine}" +
               $"PublicDomain icon is based on the following source images:{Environment.NewLine}{Environment.NewLine}" +
               $"Bitcoin by GDJ - Pixabay License{Environment.NewLine}" +
               $"https://pixabay.com/vectors/bitcoin-digital-currency-4130319/{Environment.NewLine}{Environment.NewLine}" +
               $"Letter P by ArtsyBee - Pixabay License{Environment.NewLine}" +
               $"https://pixabay.com/illustrations/p-glamour-gold-lights-2790632/{Environment.NewLine}{Environment.NewLine}" +
               $"Letter D by ArtsyBee - Pixabay License{Environment.NewLine}" +
               $"https://pixabay.com/illustrations/d-glamour-gold-lights-2790573/{Environment.NewLine}{Environment.NewLine}";

            // Set title
            string programTitle = typeof(MainForm).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;

            // Set version for generating semantic version 
            Version version = typeof(MainForm).GetTypeInfo().Assembly.GetName().Version;

            // Set about form
            var aboutForm = new AboutForm(
                $"About {programTitle}",
                $"{programTitle} {version.Major}.{version.Minor}.{version.Build}",
                $"Made for: nkormanik{Environment.NewLine}DonationCoder.com{Environment.NewLine}Day #69, Week #10 @ March 10, 2021",
                aboutMessage,
                this.Icon.ToBitmap())

            {
                // Set about form icon
                Icon = this.associatedIcon,

                // Set topmost
                TopMost = this.alwaysOnTopToolStripMenuItem.Checked
            };

            // Show about form
            aboutForm.ShowDialog();
        }

        /// <summary>
        /// Hndles the edit button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnEditButtonClick(object sender, EventArgs e)
        {
            // Prevent drawing
            this.urlListBox.BeginUpdate();

            // Edit selected items
            for (int i = 0; i < this.urlListBox.SelectedIndices.Count; i++)
            {
                // Edit item
                string itemText = Interaction.InputBox("Please edit item text to update:", "Edit", this.urlListBox.Items[this.urlListBox.SelectedIndices[i]].ToString());

                // Check length
                if (itemText.Length > 0)
                {
                    // Edit item text
                    this.urlListBox.Items[this.urlListBox.SelectedIndices[i]] = itemText;
                }
            }

            // Resume drawing
            this.urlListBox.EndUpdate();
        }

        /// <summary>
        /// Hndles the delete button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnDeleteButtonClick(object sender, EventArgs e)
        {
            // Prevent drawing
            this.urlListBox.BeginUpdate();

            // Remove selected items
            for (int i = this.urlListBox.SelectedIndices.Count - 1; i >= 0; i--)
            {
                // last selected item
                this.urlListBox.Items.RemoveAt(this.urlListBox.SelectedIndices[i]);
            }

            // Resume drawing
            this.urlListBox.EndUpdate();

            // Update count
            this.countToolStripStatusLabel.Text = this.urlListBox.Items.Count.ToString();
        }

        /// <summary>
        /// Hndles the selection mode tool strip menu item drop down item clicked event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSelectionModeToolStripMenuItemDropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // Uncheck all items
            foreach (ToolStripMenuItem item in this.selectionmodeToolStripMenuItem.DropDownItems)
            {
                // Uncheck
                item.Checked = false;
            }

            // Set tool strip menu item
            ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)e.ClickedItem;

            // Toggle checked
            toolStripMenuItem.Checked = !toolStripMenuItem.Checked;

            // Set selection mode
            switch (toolStripMenuItem.Text.Replace("&", string.Empty))
            {
                // None
                case "None":
                    this.urlListBox.SelectionMode = SelectionMode.None;
                    break;

                // One
                case "One":
                    this.urlListBox.SelectionMode = SelectionMode.One;
                    break;

                // MultiSimple
                case "MultiSimple":
                    this.urlListBox.SelectionMode = SelectionMode.MultiSimple;
                    break;

                // MultiExtended
                case "MultiExtended":
                    this.urlListBox.SelectionMode = SelectionMode.MultiSimple;
                    break;
            }
        }

        /// <summary>
        /// Populates the URL list by file.
        /// </summary>
        /// <param name="filePathList">File path list.</param>
        private void PopulateByFile(List<string> filePathList)
        {
            try
            {
                // Declare link list
                var linkList = new List<string>();

                // Iterate dropped files
                foreach (string droppedFile in filePathList)
                {
                    // Process extensions
                    switch (Path.GetExtension(droppedFile).ToLowerInvariant())
                    {
                        // TEXT
                        case ".txt":

                            // Add valid links to list
                            linkList.AddRange(this.ProcessTextFile(droppedFile));

                            // Halt flow
                            break;

                        // HTML
                        case ".htm":
                        case ".html":

                            // Add valid links to list
                            linkList.AddRange(this.ProcessHtmlFile(droppedFile));

                            // Halt flow
                            break;

                        // URL
                        case ".url":

                            // Add valid links to list
                            linkList.AddRange(this.ProcesUrlFile(droppedFile));

                            // Halt flow
                            break;
                    }
                }

                // Add links
                foreach (var link in linkList)
                {
                    // Append to list box
                    this.urlListBox.Items.Add(link);
                }
            }
            catch (Exception ex)
            {
                // Inform user
                MessageBox.Show($"Could not finish operation:{Environment.NewLine}{ex.Message}", "Populate by file(s) error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Update count
            this.countToolStripStatusLabel.Text = this.urlListBox.Items.Count.ToString();
        }

        /// <summary>
        /// Validates the URI.
        /// </summary>
        /// <returns><c>true</c>, if URI was validated, <c>false</c> otherwise.</returns>
        /// <param name="possibleUri">Possible URI.</param>
        private bool ValidateUri(string possibleUri)
        {
            // Return TryCreate result
            return Uri.TryCreate(possibleUri, UriKind.Absolute, out var uri) &&
                            (uri.Scheme == Uri.UriSchemeHttps ||
                            uri.Scheme == Uri.UriSchemeHttp ||
                            uri.Scheme == Uri.UriSchemeFtp ||
                            uri.Scheme == Uri.UriSchemeMailto ||
                            uri.Scheme == Uri.UriSchemeFile ||
                            uri.Scheme == Uri.UriSchemeNews ||
                            uri.Scheme == Uri.UriSchemeNntp ||
                            uri.Scheme == Uri.UriSchemeGopher ||
                            uri.Scheme == Uri.UriSchemeNetTcp ||
                            uri.Scheme == Uri.UriSchemeNetPipe);
        }

        /// <summary>
        /// Processes the text file.
        /// </summary>
        /// <returns>The link list.</returns>
        /// <param name="filePath">File path.</param>
        private List<string> ProcessTextFile(string filePath)
        {
            // Set link list 
            var linkList = new List<string>();

            // Declare trimmed line
            string trimmedline = string.Empty;

            // Iterate lines
            foreach (string line in File.ReadAllLines(filePath))
            {
                // Set trimmed line
                trimmedline = line.Trim();

                // Validate current line
                if (this.ValidateUri(trimmedline))
                {
                    // Add valid URI
                    linkList.Add(trimmedline);
                }
            }

            // Return valid link list
            return linkList;
        }

        /// <summary>
        /// Processes the html file.
        /// </summary>
        /// <returns>The links list.</returns>
        /// <param name="filePath">File path.</param>
        private List<string> ProcessHtmlFile(string filePath)
        {
            // Set document
            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();

            // Load current dropped file
            htmlDocument.Load(filePath);

            // Set link list 
            var linkList = new List<string>();

            // Extract links
            foreach (HtmlNode link in htmlDocument.DocumentNode.SelectNodes("//a[@href]"))
            {
                // Set attribute
                HtmlAttribute htmlAttribute = link.Attributes["href"];

                // Check
                if (htmlAttribute.Value.Contains("a") && this.ValidateUri(htmlAttribute.Value))
                {
                    // Add to link list 
                    linkList.Add(htmlAttribute.Value);
                }
            }

            // Return validlink list
            return linkList;
        }

        /// <summary>
        /// TODO Proceses the URL file. [May yconsider returning string. Returns list for uniformity purposes with TXT and HTML processing functions]
        /// </summary>
        /// <returns>The extracted URL.</returns>
        /// <param name="filePath">File path.</param>
        private List<string> ProcesUrlFile(string filePath)
        {
            // Set link list 
            var linkList = new List<string>();

            // Extracted link
            var link = string.Empty;

            // TODO Iterate lines [Can be done via indexOf]
            foreach (var line in File.ReadAllLines(filePath))
            {
                // Check for "URL="
                if (line.StartsWith("URL=", StringComparison.InvariantCultureIgnoreCase))
                {
                    // Extract link
                    link = line.Split(new char[] { '=' })[1];

                    // Add to link list
                    linkList.Add(link);

                    // Halt flow
                    break;
                }
            }

            // Return extracted link list
            return linkList;
        }

        /// <summary>
        /// Handles the URL list box drag drop event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnUrlListBoxDragDrop(object sender, DragEventArgs e)
        {
            // Populate URL list by dropped files
            this.PopulateByFile(new List<string>((IEnumerable<string>)e.Data.GetData(DataFormats.FileDrop)));
        }

        /// <summary>
        /// Handles the URL list box drag enter event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnUrlListBoxDragEnter(object sender, DragEventArgs e)
        {
            // Set effect
            e.Effect = DragDropEffects.Copy;
        }
    }
}
