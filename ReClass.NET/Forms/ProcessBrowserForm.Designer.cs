using ReClassNET.Controls;

namespace ReClassNET.Forms
{
	partial class ProcessBrowserForm
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
        private void InitializeComponent() {
            processDataGridView = new DataGridView();
            filterCheckBox = new CheckBox();
            refreshButton = new Button();
            attachToProcessButton = new Button();
            loadSymbolsCheckBox = new CheckBox();
            filterGroupBox = new GroupBox();
            previousProcessLinkLabel = new LinkLabel();
            label2 = new Label();
            label1 = new Label();
            filterTextBox = new TextBox();
            bannerBox = new BannerBox();
            iconColumn = new DataGridViewImageColumn();
            processNameColumn = new DataGridViewTextBoxColumn();
            pidColumn = new DataGridViewTextBoxColumn();
            pathColumn = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)processDataGridView).BeginInit();
            filterGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bannerBox).BeginInit();
            SuspendLayout();
            // 
            // processDataGridView
            // 
            processDataGridView.AllowUserToAddRows = false;
            processDataGridView.AllowUserToDeleteRows = false;
            processDataGridView.AllowUserToResizeColumns = false;
            processDataGridView.AllowUserToResizeRows = false;
            processDataGridView.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            processDataGridView.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            processDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            processDataGridView.Columns.AddRange(new DataGridViewColumn[] { iconColumn, processNameColumn, pidColumn, pathColumn });
            processDataGridView.Location = new Point(14, 230);
            processDataGridView.Margin = new Padding(4, 3, 4, 3);
            processDataGridView.MultiSelect = false;
            processDataGridView.Name = "processDataGridView";
            processDataGridView.ReadOnly = true;
            processDataGridView.RowHeadersVisible = false;
            processDataGridView.ScrollBars = ScrollBars.Vertical;
            processDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            processDataGridView.Size = new Size(640, 336);
            processDataGridView.TabIndex = 0;
            processDataGridView.CellMouseDoubleClick += processDataGridView_CellMouseDoubleClick;
            // 
            // filterCheckBox
            // 
            filterCheckBox.AutoSize = true;
            filterCheckBox.Checked = true;
            filterCheckBox.CheckState = CheckState.Checked;
            filterCheckBox.Location = new Point(10, 83);
            filterCheckBox.Margin = new Padding(4, 3, 4, 3);
            filterCheckBox.Name = "filterCheckBox";
            filterCheckBox.Size = new Size(173, 19);
            filterCheckBox.TabIndex = 1;
            filterCheckBox.Text = "Exclude common processes";
            filterCheckBox.UseVisualStyleBackColor = true;
            filterCheckBox.CheckedChanged += filterCheckBox_CheckedChanged;
            // 
            // refreshButton
            // 
            refreshButton.Image = Properties.Resources.B16x16_Arrow_Refresh;
            refreshButton.Location = new Point(10, 114);
            refreshButton.Margin = new Padding(4, 3, 4, 3);
            refreshButton.Name = "refreshButton";
            refreshButton.Size = new Size(184, 27);
            refreshButton.TabIndex = 2;
            refreshButton.Text = "Refresh";
            refreshButton.TextAlign = ContentAlignment.MiddleRight;
            refreshButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            refreshButton.UseVisualStyleBackColor = true;
            refreshButton.Click += refreshButton_Click;
            // 
            // attachToProcessButton
            // 
            attachToProcessButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            attachToProcessButton.DialogResult = DialogResult.OK;
            attachToProcessButton.Image = Properties.Resources.B16x16_Accept;
            attachToProcessButton.Location = new Point(14, 599);
            attachToProcessButton.Margin = new Padding(4, 3, 4, 3);
            attachToProcessButton.Name = "attachToProcessButton";
            attachToProcessButton.Size = new Size(640, 27);
            attachToProcessButton.TabIndex = 3;
            attachToProcessButton.Text = "Attach to Process";
            attachToProcessButton.TextAlign = ContentAlignment.MiddleRight;
            attachToProcessButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            attachToProcessButton.UseVisualStyleBackColor = true;
            // 
            // loadSymbolsCheckBox
            // 
            loadSymbolsCheckBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            loadSymbolsCheckBox.AutoSize = true;
            loadSymbolsCheckBox.Location = new Point(14, 573);
            loadSymbolsCheckBox.Margin = new Padding(4, 3, 4, 3);
            loadSymbolsCheckBox.Name = "loadSymbolsCheckBox";
            loadSymbolsCheckBox.Size = new Size(100, 19);
            loadSymbolsCheckBox.TabIndex = 4;
            loadSymbolsCheckBox.Text = "Load Symbols";
            loadSymbolsCheckBox.UseVisualStyleBackColor = true;
            // 
            // filterGroupBox
            // 
            filterGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            filterGroupBox.Controls.Add(previousProcessLinkLabel);
            filterGroupBox.Controls.Add(label2);
            filterGroupBox.Controls.Add(label1);
            filterGroupBox.Controls.Add(filterCheckBox);
            filterGroupBox.Controls.Add(refreshButton);
            filterGroupBox.Controls.Add(filterTextBox);
            filterGroupBox.Location = new Point(14, 69);
            filterGroupBox.Margin = new Padding(4, 3, 4, 3);
            filterGroupBox.Name = "filterGroupBox";
            filterGroupBox.Padding = new Padding(4, 3, 4, 3);
            filterGroupBox.Size = new Size(640, 153);
            filterGroupBox.TabIndex = 5;
            filterGroupBox.TabStop = false;
            filterGroupBox.Text = "Filter";
            // 
            // previousProcessLinkLabel
            // 
            previousProcessLinkLabel.AutoSize = true;
            previousProcessLinkLabel.Location = new Point(120, 54);
            previousProcessLinkLabel.Margin = new Padding(4, 0, 4, 0);
            previousProcessLinkLabel.Name = "previousProcessLinkLabel";
            previousProcessLinkLabel.Size = new Size(23, 15);
            previousProcessLinkLabel.TabIndex = 3;
            previousProcessLinkLabel.TabStop = true;
            previousProcessLinkLabel.Text = "<>";
            previousProcessLinkLabel.LinkClicked += previousProcessLinkLabel_LinkClicked;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(7, 54);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(98, 15);
            label2.TabIndex = 2;
            label2.Text = "Previous Process:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(7, 25);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(85, 15);
            label1.TabIndex = 1;
            label1.Text = "Process Name:";
            // 
            // filterTextBox
            // 
            filterTextBox.Location = new Point(120, 22);
            filterTextBox.Margin = new Padding(4, 3, 4, 3);
            filterTextBox.Name = "filterTextBox";
            filterTextBox.Size = new Size(314, 23);
            filterTextBox.TabIndex = 0;
            filterTextBox.TextChanged += filterTextBox_TextChanged;
            // 
            // bannerBox
            // 
            bannerBox.Dock = DockStyle.Top;
            bannerBox.Icon = Properties.Resources.B32x32_Magnifier;
            bannerBox.Location = new Point(0, 0);
            bannerBox.Margin = new Padding(4, 3, 4, 3);
            bannerBox.Name = "bannerBox";
            bannerBox.Size = new Size(668, 48);
            bannerBox.TabIndex = 6;
            bannerBox.Text = "Select the process to which ReClass.NET is to be attached.";
            bannerBox.Title = "Attach to Process";
            // 
            // iconColumn
            // 
            iconColumn.DataPropertyName = "icon";
            iconColumn.HeaderText = "";
            iconColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
            iconColumn.MinimumWidth = 18;
            iconColumn.Name = "iconColumn";
            iconColumn.ReadOnly = true;
            iconColumn.Resizable = DataGridViewTriState.True;
            iconColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            iconColumn.Width = 18;
            // 
            // processNameColumn
            // 
            processNameColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            processNameColumn.DataPropertyName = "name";
            processNameColumn.HeaderText = "Process";
            processNameColumn.Name = "processNameColumn";
            processNameColumn.ReadOnly = true;
            processNameColumn.Width = 72;
            // 
            // pidColumn
            // 
            pidColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            pidColumn.DataPropertyName = "id";
            pidColumn.HeaderText = "PID";
            pidColumn.Name = "pidColumn";
            pidColumn.ReadOnly = true;
            pidColumn.Width = 50;
            // 
            // pathColumn
            // 
            pathColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            pathColumn.DataPropertyName = "path";
            pathColumn.HeaderText = "Path";
            pathColumn.Name = "pathColumn";
            pathColumn.ReadOnly = true;
            // 
            // ProcessBrowserForm
            // 
            AcceptButton = attachToProcessButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(668, 639);
            Controls.Add(bannerBox);
            Controls.Add(filterGroupBox);
            Controls.Add(loadSymbolsCheckBox);
            Controls.Add(attachToProcessButton);
            Controls.Add(processDataGridView);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProcessBrowserForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "ReClass.NET - Attach to Process";
            ((System.ComponentModel.ISupportInitialize)processDataGridView).EndInit();
            filterGroupBox.ResumeLayout(false);
            filterGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)bannerBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataGridView processDataGridView;
		private System.Windows.Forms.CheckBox filterCheckBox;
		private System.Windows.Forms.Button refreshButton;
		private System.Windows.Forms.Button attachToProcessButton;
		private System.Windows.Forms.CheckBox loadSymbolsCheckBox;
		private System.Windows.Forms.GroupBox filterGroupBox;
		private System.Windows.Forms.LinkLabel previousProcessLinkLabel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox filterTextBox;
		private BannerBox bannerBox;
        private DataGridViewImageColumn iconColumn;
        private DataGridViewTextBoxColumn processNameColumn;
        private DataGridViewTextBoxColumn pidColumn;
        private DataGridViewTextBoxColumn pathColumn;
    }
}