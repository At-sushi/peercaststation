﻿namespace PeerCastStation.GUI
{
  partial class MainForm
  {
    /// <summary>
    /// 必要なデザイナー変数です。
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// 使用中のリソースをすべてクリーンアップします。
    /// </summary>
    /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows フォーム デザイナーで生成されたコード

    /// <summary>
    /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
    /// コード エディターで変更しないでください。
    /// </summary>
    private void InitializeComponent()
    {
      System.Windows.Forms.Label label4;
      System.Windows.Forms.Label label3;
      System.Windows.Forms.Label label2;
      System.Windows.Forms.Label label1;
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tabChannels = new System.Windows.Forms.TabPage();
      this.channelClose = new System.Windows.Forms.Button();
      this.channelPlay = new System.Windows.Forms.Button();
      this.channelList = new System.Windows.Forms.ListBox();
      this.tabSettings = new System.Windows.Forms.TabPage();
      this.applySettings = new System.Windows.Forms.Button();
      this.maxUpstreamRate = new System.Windows.Forms.NumericUpDown();
      this.maxDirects = new System.Windows.Forms.NumericUpDown();
      this.maxRelays = new System.Windows.Forms.NumericUpDown();
      this.port = new System.Windows.Forms.NumericUpDown();
      label4 = new System.Windows.Forms.Label();
      label3 = new System.Windows.Forms.Label();
      label2 = new System.Windows.Forms.Label();
      label1 = new System.Windows.Forms.Label();
      this.tabControl1.SuspendLayout();
      this.tabChannels.SuspendLayout();
      this.tabSettings.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.maxUpstreamRate)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.maxDirects)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.maxRelays)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.port)).BeginInit();
      this.SuspendLayout();
      // 
      // label4
      // 
      label4.AutoSize = true;
      label4.Location = new System.Drawing.Point(8, 90);
      label4.Name = "label4";
      label4.Size = new System.Drawing.Size(105, 12);
      label4.TabIndex = 8;
      label4.Text = "最大上り帯域(kbps)";
      // 
      // label3
      // 
      label3.AutoSize = true;
      label3.Location = new System.Drawing.Point(8, 65);
      label3.Name = "label3";
      label3.Size = new System.Drawing.Size(65, 12);
      label3.TabIndex = 7;
      label3.Text = "最大視聴数";
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Location = new System.Drawing.Point(8, 40);
      label2.Name = "label2";
      label2.Size = new System.Drawing.Size(67, 12);
      label2.TabIndex = 6;
      label2.Text = "最大リレー数";
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new System.Drawing.Point(8, 15);
      label1.Name = "label1";
      label1.Size = new System.Drawing.Size(57, 12);
      label1.TabIndex = 5;
      label1.Text = "ポート番号";
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.tabChannels);
      this.tabControl1.Controls.Add(this.tabSettings);
      this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl1.Location = new System.Drawing.Point(0, 0);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(412, 355);
      this.tabControl1.TabIndex = 0;
      // 
      // tabChannels
      // 
      this.tabChannels.Controls.Add(this.channelClose);
      this.tabChannels.Controls.Add(this.channelPlay);
      this.tabChannels.Controls.Add(this.channelList);
      this.tabChannels.Location = new System.Drawing.Point(4, 22);
      this.tabChannels.Name = "tabChannels";
      this.tabChannels.Padding = new System.Windows.Forms.Padding(3);
      this.tabChannels.Size = new System.Drawing.Size(404, 329);
      this.tabChannels.TabIndex = 1;
      this.tabChannels.Text = "チャンネル一覧";
      this.tabChannels.UseVisualStyleBackColor = true;
      // 
      // channelClose
      // 
      this.channelClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.channelClose.Location = new System.Drawing.Point(335, 42);
      this.channelClose.Name = "channelClose";
      this.channelClose.Size = new System.Drawing.Size(61, 30);
      this.channelClose.TabIndex = 3;
      this.channelClose.Text = "切断";
      this.channelClose.UseVisualStyleBackColor = true;
      this.channelClose.Click += new System.EventHandler(this.channelClose_Click);
      // 
      // channelPlay
      // 
      this.channelPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.channelPlay.Location = new System.Drawing.Point(335, 6);
      this.channelPlay.Name = "channelPlay";
      this.channelPlay.Size = new System.Drawing.Size(61, 30);
      this.channelPlay.TabIndex = 2;
      this.channelPlay.Text = "再生";
      this.channelPlay.UseVisualStyleBackColor = true;
      this.channelPlay.Click += new System.EventHandler(this.channelPlay_Click);
      // 
      // channelList
      // 
      this.channelList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.channelList.FormattingEnabled = true;
      this.channelList.ItemHeight = 12;
      this.channelList.Location = new System.Drawing.Point(6, 6);
      this.channelList.Name = "channelList";
      this.channelList.Size = new System.Drawing.Size(322, 172);
      this.channelList.TabIndex = 1;
      this.channelList.SelectedIndexChanged += new System.EventHandler(this.channelList_SelectedIndexChanged);
      // 
      // tabSettings
      // 
      this.tabSettings.Controls.Add(this.applySettings);
      this.tabSettings.Controls.Add(label4);
      this.tabSettings.Controls.Add(label3);
      this.tabSettings.Controls.Add(label2);
      this.tabSettings.Controls.Add(label1);
      this.tabSettings.Controls.Add(this.maxUpstreamRate);
      this.tabSettings.Controls.Add(this.maxDirects);
      this.tabSettings.Controls.Add(this.maxRelays);
      this.tabSettings.Controls.Add(this.port);
      this.tabSettings.Location = new System.Drawing.Point(4, 22);
      this.tabSettings.Name = "tabSettings";
      this.tabSettings.Size = new System.Drawing.Size(404, 329);
      this.tabSettings.TabIndex = 2;
      this.tabSettings.Text = "設定";
      this.tabSettings.UseVisualStyleBackColor = true;
      // 
      // applySettings
      // 
      this.applySettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.applySettings.Location = new System.Drawing.Point(321, 298);
      this.applySettings.Name = "applySettings";
      this.applySettings.Size = new System.Drawing.Size(75, 23);
      this.applySettings.TabIndex = 9;
      this.applySettings.Text = "適用";
      this.applySettings.UseVisualStyleBackColor = true;
      this.applySettings.Click += new System.EventHandler(this.applySettings_Click);
      // 
      // maxUpstreamRate
      // 
      this.maxUpstreamRate.Location = new System.Drawing.Point(117, 88);
      this.maxUpstreamRate.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
      this.maxUpstreamRate.Name = "maxUpstreamRate";
      this.maxUpstreamRate.Size = new System.Drawing.Size(162, 19);
      this.maxUpstreamRate.TabIndex = 4;
      // 
      // maxDirects
      // 
      this.maxDirects.Location = new System.Drawing.Point(117, 63);
      this.maxDirects.Name = "maxDirects";
      this.maxDirects.Size = new System.Drawing.Size(82, 19);
      this.maxDirects.TabIndex = 3;
      // 
      // maxRelays
      // 
      this.maxRelays.Location = new System.Drawing.Point(117, 38);
      this.maxRelays.Name = "maxRelays";
      this.maxRelays.Size = new System.Drawing.Size(82, 19);
      this.maxRelays.TabIndex = 2;
      // 
      // port
      // 
      this.port.Location = new System.Drawing.Point(117, 13);
      this.port.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
      this.port.Minimum = new decimal(new int[] {
            1025,
            0,
            0,
            0});
      this.port.Name = "port";
      this.port.Size = new System.Drawing.Size(82, 19);
      this.port.TabIndex = 1;
      this.port.Value = new decimal(new int[] {
            7144,
            0,
            0,
            0});
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(412, 355);
      this.Controls.Add(this.tabControl1);
      this.Name = "MainForm";
      this.Text = "PeerCastStation.GUI";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
      this.tabControl1.ResumeLayout(false);
      this.tabChannels.ResumeLayout(false);
      this.tabSettings.ResumeLayout(false);
      this.tabSettings.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.maxUpstreamRate)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.maxDirects)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.maxRelays)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.port)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabChannels;
    private System.Windows.Forms.TabPage tabSettings;
    private System.Windows.Forms.NumericUpDown maxUpstreamRate;
    private System.Windows.Forms.NumericUpDown maxDirects;
    private System.Windows.Forms.NumericUpDown maxRelays;
    private System.Windows.Forms.NumericUpDown port;
    private System.Windows.Forms.Button applySettings;
    private System.Windows.Forms.ListBox channelList;
    private System.Windows.Forms.Button channelClose;
    private System.Windows.Forms.Button channelPlay;

  }
}

