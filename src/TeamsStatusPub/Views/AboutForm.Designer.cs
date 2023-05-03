namespace TeamsStatusPub.Views;

partial class AboutForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
        productLabel = new Label();
        versionLabel = new Label();
        copyrightLabel = new Label();
        logoPictureBox = new PictureBox();
        websiteLinkLabel = new LinkLabel();
        listenLabel = new Label();
        listenLinkLabel = new LinkLabel();
        ((System.ComponentModel.ISupportInitialize)logoPictureBox).BeginInit();
        SuspendLayout();
        // 
        // productLabel
        // 
        productLabel.AutoSize = true;
        productLabel.Font = new Font("Segoe UI", 13F, FontStyle.Bold, GraphicsUnit.Point);
        productLabel.Location = new Point(296, 74);
        productLabel.Margin = new Padding(7, 0, 7, 0);
        productLabel.Name = "productLabel";
        productLabel.Size = new Size(324, 60);
        productLabel.TabIndex = 0;
        productLabel.Text = "Product Name";
        // 
        // versionLabel
        // 
        versionLabel.AutoSize = true;
        versionLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        versionLabel.Location = new Point(29, 293);
        versionLabel.Margin = new Padding(7, 0, 7, 0);
        versionLabel.Name = "versionLabel";
        versionLabel.Size = new Size(116, 41);
        versionLabel.TabIndex = 1;
        versionLabel.Text = "Version";
        // 
        // copyrightLabel
        // 
        copyrightLabel.AutoSize = true;
        copyrightLabel.Location = new Point(29, 335);
        copyrightLabel.Margin = new Padding(7, 0, 7, 0);
        copyrightLabel.Name = "copyrightLabel";
        copyrightLabel.Size = new Size(150, 41);
        copyrightLabel.TabIndex = 2;
        copyrightLabel.Text = "Copyright";
        // 
        // logoPictureBox
        // 
        logoPictureBox.Image = (Image)resources.GetObject("logoPictureBox.Image");
        logoPictureBox.Location = new Point(29, 33);
        logoPictureBox.Margin = new Padding(7, 8, 7, 8);
        logoPictureBox.Name = "logoPictureBox";
        logoPictureBox.Size = new Size(253, 232);
        logoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        logoPictureBox.TabIndex = 3;
        logoPictureBox.TabStop = false;
        // 
        // websiteLinkLabel
        // 
        websiteLinkLabel.AutoSize = true;
        websiteLinkLabel.Location = new Point(29, 377);
        websiteLinkLabel.Margin = new Padding(7, 0, 7, 0);
        websiteLinkLabel.Name = "websiteLinkLabel";
        websiteLinkLabel.Size = new Size(243, 41);
        websiteLinkLabel.TabIndex = 4;
        websiteLinkLabel.TabStop = true;
        websiteLinkLabel.Text = "websiteLinkLabel";
        // 
        // listenLabel
        // 
        listenLabel.AutoSize = true;
        listenLabel.Location = new Point(296, 152);
        listenLabel.Margin = new Padding(7, 0, 7, 0);
        listenLabel.Name = "listenLabel";
        listenLabel.Size = new Size(180, 41);
        listenLabel.TabIndex = 5;
        listenLabel.Text = "Listening on";
        // 
        // listenLinkLabel
        // 
        listenLinkLabel.AutoSize = true;
        listenLinkLabel.Location = new Point(470, 152);
        listenLinkLabel.Margin = new Padding(7, 0, 7, 0);
        listenLinkLabel.Name = "listenLinkLabel";
        listenLinkLabel.Size = new Size(304, 41);
        listenLinkLabel.TabIndex = 6;
        listenLinkLabel.TabStop = true;
        listenLinkLabel.Text = "http://127.0.0.1:8080/";
        // 
        // AboutForm
        // 
        AutoScaleDimensions = new SizeF(17F, 41F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1107, 467);
        Controls.Add(listenLinkLabel);
        Controls.Add(listenLabel);
        Controls.Add(websiteLinkLabel);
        Controls.Add(logoPictureBox);
        Controls.Add(copyrightLabel);
        Controls.Add(versionLabel);
        Controls.Add(productLabel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        Margin = new Padding(7, 8, 7, 8);
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AboutForm";
        Text = "About";
        ((System.ComponentModel.ISupportInitialize)logoPictureBox).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label productLabel;
    private Label versionLabel;
    private Label copyrightLabel;
    private PictureBox logoPictureBox;
    private LinkLabel websiteLinkLabel;
    private Label listenLabel;
    private LinkLabel listenLinkLabel;
}
