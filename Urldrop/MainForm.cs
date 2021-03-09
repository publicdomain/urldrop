// <copyright file="MainForm.cs" company="PUblicDomain.com">
//     CC0 1.0 Universal (CC0 1.0) - Public Domain Dedication
//     https://creativecommons.org/publicdomain/zero/1.0/legalcode
// </copyright>

namespace Urldrop
{
    // Directives
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;
    using HtmlAgilityPack;

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

            // Set public domain weekly tool strip menu item image
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
            // TODO Add code
        }

        /// <summary>
        /// Hndles the open tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Hndles the save tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSaveToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Hndles the exit tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Hndles the cut tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnCutToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Hndles the copy tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnCopyToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Hndles the paste tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnPasteToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Hndles the options tool strip menu item drop down item clicked.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOptionsToolStripMenuItemDropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Hndles the more releases public domain giftcom tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnMoreReleasesPublicDomainGiftcomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Hndles the original thread donation codercom tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOriginalThreadDonationCodercomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Hndles the source code githubcom tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSourceCodeGithubcomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Hndles the about tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnAboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Hndles the edit button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnEditButtonClick(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Hndles the delete button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnDeleteButtonClick(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Hndles the multiselect tool strip menu item drop down item clicked.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnMultiselectToolStripMenuItemDropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // TODO Add code
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
    }
}
