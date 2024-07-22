namespace ReClass.Controls
{
	partial class HotkeyBox
	{
		/// <summary> 
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent() {
            components = new System.ComponentModel.Container();
            timer = new System.Windows.Forms.Timer(components);
            textBox = new TextBox();
            clearButton = new Button();
            SuspendLayout();
            // 
            // timer
            // 
            timer.Enabled = true;
            timer.Interval = 50;
            timer.Tick += timer_Tick;
            // 
            // textBox
            // 
            textBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBox.Enabled = false;
            textBox.Location = new Point(0, 0);
            textBox.Margin = new Padding(4, 3, 4, 3);
            textBox.Name = "textBox";
            textBox.Size = new Size(163, 23);
            textBox.TabIndex = 0;
            textBox.Enter += textBox_Enter;
            textBox.Leave += textBox_Leave;
            // 
            // clearButton
            // 
            clearButton.Anchor = AnchorStyles.Right;
            clearButton.Image = Properties.Resources.B16x16_Button_Delete;
            clearButton.Location = new Point(166, 0);
            clearButton.Margin = new Padding(4, 3, 4, 3);
            clearButton.Name = "clearButton";
            clearButton.Size = new Size(23, 23);
            clearButton.TabIndex = 1;
            clearButton.UseVisualStyleBackColor = true;
            clearButton.Click += clearButton_Click;
            // 
            // HotkeyBox
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(clearButton);
            Controls.Add(textBox);
            Margin = new Padding(4, 3, 4, 3);
            Name = "HotkeyBox";
            Size = new Size(189, 23);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.TextBox textBox;
		private System.Windows.Forms.Button clearButton;
	}
}
