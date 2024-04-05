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
        components = new System.ComponentModel.Container();
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
        productLabel = new Label();
        versionLabel = new Label();
        copyrightLabel = new Label();
        logoPictureBox = new PictureBox();
        websiteLinkLabel = new LinkLabel();
        listenLabel = new Label();
        listenLinkLabel = new LinkLabel();
        availabilityStatusLabel = new Label();
        lastAvailabilityHandlerStatusLabel = new Label();
        teamsStatusTooltip = new ToolTip(components);
        ((System.ComponentModel.ISupportInitialize)logoPictureBox).BeginInit();
        SuspendLayout();
        // 
        // productLabel
        // 
        productLabel.AutoSize = true;
        productLabel.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        productLabel.Location = new Point(122, 18);
        productLabel.Name = "productLabel";
        productLabel.Size = new Size(134, 25);
        productLabel.TabIndex = 0;
        productLabel.Text = "Product Name";
        // 
        // versionLabel
        // 
        versionLabel.AutoSize = true;
        versionLabel.Font = new Font("Segoe UI", 9F);
        versionLabel.Location = new Point(122, 47);
        versionLabel.Name = "versionLabel";
        versionLabel.Size = new Size(45, 15);
        versionLabel.TabIndex = 1;
        versionLabel.Text = "Version";
        // 
        // copyrightLabel
        // 
        copyrightLabel.AutoSize = true;
        copyrightLabel.Location = new Point(122, 62);
        copyrightLabel.Name = "copyrightLabel";
        copyrightLabel.Size = new Size(60, 15);
        copyrightLabel.TabIndex = 2;
        copyrightLabel.Text = "Copyright";
        // 
        // logoPictureBox
        // 
        logoPictureBox.Image = (Image)resources.GetObject("logoPictureBox.Image");
        logoPictureBox.Location = new Point(12, 12);
        logoPictureBox.Name = "logoPictureBox";
        logoPictureBox.Size = new Size(104, 85);
        logoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        logoPictureBox.TabIndex = 3;
        logoPictureBox.TabStop = false;
        // 
        // websiteLinkLabel
        // 
        websiteLinkLabel.AutoSize = true;
        websiteLinkLabel.Location = new Point(122, 78);
        websiteLinkLabel.Name = "websiteLinkLabel";
        websiteLinkLabel.Size = new Size(97, 15);
        websiteLinkLabel.TabIndex = 4;
        websiteLinkLabel.TabStop = true;
        websiteLinkLabel.Text = "websiteLinkLabel";
        // 
        // listenLabel
        // 
        listenLabel.AutoSize = true;
        listenLabel.Location = new Point(12, 117);
        listenLabel.Name = "listenLabel";
        listenLabel.Size = new Size(72, 15);
        listenLabel.TabIndex = 5;
        listenLabel.Text = "Listening on";
        // 
        // listenLinkLabel
        // 
        listenLinkLabel.AutoSize = true;
        listenLinkLabel.Location = new Point(82, 117);
        listenLinkLabel.Name = "listenLinkLabel";
        listenLinkLabel.Size = new Size(119, 15);
        listenLinkLabel.TabIndex = 6;
        listenLinkLabel.TabStop = true;
        listenLinkLabel.Text = "http://127.0.0.1:8080/";
        // 
        // availabilityStatusLabel
        // 
        availabilityStatusLabel.AutoSize = true;
        availabilityStatusLabel.Location = new Point(12, 135);
        availabilityStatusLabel.Name = "availabilityStatusLabel";
        availabilityStatusLabel.Size = new Size(75, 15);
        availabilityStatusLabel.TabIndex = 7;
        availabilityStatusLabel.Text = "Last status in";
        // 
        // lastAvailabilityHandlerStatusLabel
        // 
        lastAvailabilityHandlerStatusLabel.AutoSize = true;
        lastAvailabilityHandlerStatusLabel.Location = new Point(83, 135);
        lastAvailabilityHandlerStatusLabel.Name = "lastAvailabilityHandlerStatusLabel";
        lastAvailabilityHandlerStatusLabel.Size = new Size(89, 15);
        lastAvailabilityHandlerStatusLabel.TabIndex = 8;
        lastAvailabilityHandlerStatusLabel.Text = "Thing: not busy";
        // 
        // AboutForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(456, 171);
        Controls.Add(lastAvailabilityHandlerStatusLabel);
        Controls.Add(availabilityStatusLabel);
        Controls.Add(listenLinkLabel);
        Controls.Add(listenLabel);
        Controls.Add(websiteLinkLabel);
        Controls.Add(logoPictureBox);
        Controls.Add(copyrightLabel);
        Controls.Add(versionLabel);
        Controls.Add(productLabel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
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
    private Label availabilityStatusLabel;
    private Label lastAvailabilityHandlerStatusLabel;
    private ToolTip teamsStatusTooltip;
}
