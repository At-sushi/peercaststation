﻿// PeerCastStation, a P2P streaming servent.
// Copyright (C) 2011 Ryuichi Sakamoto (kumaryu@kumaryu.net)
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
namespace PeerCastStation.GUI
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
      if (disposing && (logFileWriter != null)) {
        logFileWriter.Close();
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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label6;
			System.Windows.Forms.Label label7;
			System.Windows.Forms.Label label8;
			System.Windows.Forms.Label label9;
			System.Windows.Forms.Label label10;
			System.Windows.Forms.Label label11;
			System.Windows.Forms.Label label12;
			System.Windows.Forms.Label label13;
			System.Windows.Forms.ToolStripMenuItem showGUIMenuItem;
			System.Windows.Forms.ToolStripMenuItem quitMenuItem;
			System.Windows.Forms.Label label14;
			System.Windows.Forms.Label label21;
			System.Windows.Forms.Label label20;
			System.Windows.Forms.Label label19;
			System.Windows.Forms.Label label18;
			System.Windows.Forms.Label label17;
			System.Windows.Forms.Label label16;
			System.Windows.Forms.Label label15;
			System.Windows.Forms.Label label22;
			System.Windows.Forms.Label label23;
			System.Windows.Forms.Label label24;
			System.Windows.Forms.Label label26;
			System.Windows.Forms.Panel panel1;
			System.Windows.Forms.Label label25;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.chanInfoUpdateButton = new System.Windows.Forms.Button();
			this.notifyIconMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.mainTab = new System.Windows.Forms.TabControl();
			this.tabChannels = new System.Windows.Forms.TabPage();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.channelBump = new System.Windows.Forms.Button();
			this.channelClose = new System.Windows.Forms.Button();
			this.channelPlay = new System.Windows.Forms.Button();
			this.channelList = new System.Windows.Forms.ListBox();
			this.tabControl2 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.outputList = new System.Windows.Forms.ListBox();
			this.downStreamClose = new System.Windows.Forms.Button();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.chanTrackContactURL = new System.Windows.Forms.TextBox();
			this.chanTrackAlbum = new System.Windows.Forms.TextBox();
			this.chanTrackTitle = new System.Windows.Forms.TextBox();
			this.chanTrackArtist = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.chanInfoBitrate = new System.Windows.Forms.TextBox();
			this.chanInfoContentType = new System.Windows.Forms.TextBox();
			this.chanInfoContactURL = new System.Windows.Forms.TextBox();
			this.chanInfoGenre = new System.Windows.Forms.TextBox();
			this.chanInfoDesc = new System.Windows.Forms.TextBox();
			this.chanInfoChannelID = new System.Windows.Forms.TextBox();
			this.chanInfoChannelName = new System.Windows.Forms.TextBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.relayTree = new System.Windows.Forms.TreeView();
			this.tabBroadcast = new System.Windows.Forms.TabPage();
			this.bcBitrate = new System.Windows.Forms.TextBox();
			this.bcContactUrl = new System.Windows.Forms.TextBox();
			this.bcGenre = new System.Windows.Forms.TextBox();
			this.bcDescription = new System.Windows.Forms.TextBox();
			this.bcChannelName = new System.Windows.Forms.TextBox();
			this.bcStart = new System.Windows.Forms.Button();
			this.bcYP = new System.Windows.Forms.ComboBox();
			this.bcStreamUrl = new System.Windows.Forms.TextBox();
			this.bcContentType = new System.Windows.Forms.ComboBox();
			this.tabSettings = new System.Windows.Forms.TabPage();
			this.ypListEditButton = new System.Windows.Forms.Button();
			this.applySettings = new System.Windows.Forms.Button();
			this.maxUpstreamRate = new System.Windows.Forms.NumericUpDown();
			this.maxDirects = new System.Windows.Forms.NumericUpDown();
			this.maxRelays = new System.Windows.Forms.NumericUpDown();
			this.port = new System.Windows.Forms.NumericUpDown();
			this.tabLog = new System.Windows.Forms.TabPage();
			this.logToGUICheck = new System.Windows.Forms.CheckBox();
			this.logToConsoleCheck = new System.Windows.Forms.CheckBox();
			this.logClearButton = new System.Windows.Forms.Button();
			this.selectLogFileName = new System.Windows.Forms.Button();
			this.logToFileCheck = new System.Windows.Forms.CheckBox();
			this.logText = new System.Windows.Forms.TextBox();
			this.logFileNameText = new System.Windows.Forms.TextBox();
			this.logLevelList = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.logSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.statusBar = new System.Windows.Forms.StatusStrip();
			this.portLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.portOpenedLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.chanInfoComment = new System.Windows.Forms.TextBox();
			label4 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			label10 = new System.Windows.Forms.Label();
			label11 = new System.Windows.Forms.Label();
			label12 = new System.Windows.Forms.Label();
			label13 = new System.Windows.Forms.Label();
			showGUIMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			quitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			label14 = new System.Windows.Forms.Label();
			label21 = new System.Windows.Forms.Label();
			label20 = new System.Windows.Forms.Label();
			label19 = new System.Windows.Forms.Label();
			label18 = new System.Windows.Forms.Label();
			label17 = new System.Windows.Forms.Label();
			label16 = new System.Windows.Forms.Label();
			label15 = new System.Windows.Forms.Label();
			label22 = new System.Windows.Forms.Label();
			label23 = new System.Windows.Forms.Label();
			label24 = new System.Windows.Forms.Label();
			label26 = new System.Windows.Forms.Label();
			panel1 = new System.Windows.Forms.Panel();
			label25 = new System.Windows.Forms.Label();
			panel1.SuspendLayout();
			this.notifyIconMenu.SuspendLayout();
			this.mainTab.SuspendLayout();
			this.tabChannels.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tabControl2.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabBroadcast.SuspendLayout();
			this.tabSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.maxUpstreamRate)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.maxDirects)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.maxRelays)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.port)).BeginInit();
			this.tabLog.SuspendLayout();
			this.statusBar.SuspendLayout();
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
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(8, 19);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(73, 12);
			label6.TabIndex = 0;
			label6.Text = "ストリームURL:";
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(8, 169);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(33, 12);
			label7.TabIndex = 12;
			label7.Text = "タイプ:";
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(8, 195);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(21, 12);
			label8.TabIndex = 14;
			label8.Text = "YP:";
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.Location = new System.Drawing.Point(8, 44);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(65, 12);
			label9.TabIndex = 2;
			label9.Text = "チャンネル名:";
			// 
			// label10
			// 
			label10.AutoSize = true;
			label10.Location = new System.Drawing.Point(8, 94);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(42, 12);
			label10.TabIndex = 6;
			label10.Text = "ジャンル";
			// 
			// label11
			// 
			label11.AutoSize = true;
			label11.Location = new System.Drawing.Point(8, 69);
			label11.Name = "label11";
			label11.Size = new System.Drawing.Size(29, 12);
			label11.TabIndex = 4;
			label11.Text = "概要";
			// 
			// label12
			// 
			label12.AutoSize = true;
			label12.Location = new System.Drawing.Point(8, 119);
			label12.Name = "label12";
			label12.Size = new System.Drawing.Size(70, 12);
			label12.TabIndex = 8;
			label12.Text = "コンタクトURL:";
			// 
			// label13
			// 
			label13.AutoSize = true;
			label13.Location = new System.Drawing.Point(8, 144);
			label13.Name = "label13";
			label13.Size = new System.Drawing.Size(55, 12);
			label13.TabIndex = 10;
			label13.Text = "ビットレート";
			// 
			// showGUIMenuItem
			// 
			showGUIMenuItem.AutoToolTip = true;
			showGUIMenuItem.Name = "showGUIMenuItem";
			showGUIMenuItem.Size = new System.Drawing.Size(129, 22);
			showGUIMenuItem.Text = "GUIを表示(&G)";
			showGUIMenuItem.ToolTipText = "PeerCastStationのGUIを表示します";
			showGUIMenuItem.Click += new System.EventHandler(this.showGUIMenuItem_Click);
			// 
			// quitMenuItem
			// 
			quitMenuItem.AutoToolTip = true;
			quitMenuItem.Name = "quitMenuItem";
			quitMenuItem.Size = new System.Drawing.Size(129, 22);
			quitMenuItem.Text = "終了(&Q)";
			quitMenuItem.ToolTipText = "PeerCastStationを終了します";
			quitMenuItem.Click += new System.EventHandler(this.quitMenuItem_Click);
			// 
			// label14
			// 
			label14.AutoSize = true;
			label14.Location = new System.Drawing.Point(8, 118);
			label14.Name = "label14";
			label14.Size = new System.Drawing.Size(43, 12);
			label14.TabIndex = 11;
			label14.Text = "YP一覧";
			// 
			// label21
			// 
			label21.AutoSize = true;
			label21.Location = new System.Drawing.Point(19, 196);
			label21.Name = "label21";
			label21.Size = new System.Drawing.Size(57, 12);
			label21.TabIndex = 21;
			label21.Text = "ビットレート:";
			// 
			// label20
			// 
			label20.AutoSize = true;
			label20.Location = new System.Drawing.Point(45, 171);
			label20.Name = "label20";
			label20.Size = new System.Drawing.Size(31, 12);
			label20.TabIndex = 20;
			label20.Text = "形式:";
			// 
			// label19
			// 
			label19.AutoSize = true;
			label19.Location = new System.Drawing.Point(12, 146);
			label19.Name = "label19";
			label19.Size = new System.Drawing.Size(64, 12);
			label19.TabIndex = 19;
			label19.Text = "チャンネルID:";
			// 
			// label18
			// 
			label18.AutoSize = true;
			label18.Location = new System.Drawing.Point(6, 96);
			label18.Name = "label18";
			label18.Size = new System.Drawing.Size(70, 12);
			label18.TabIndex = 18;
			label18.Text = "コンタクトURL:";
			// 
			// label17
			// 
			label17.AutoSize = true;
			label17.Location = new System.Drawing.Point(45, 71);
			label17.Name = "label17";
			label17.Size = new System.Drawing.Size(31, 12);
			label17.TabIndex = 17;
			label17.Text = "概要:";
			// 
			// label16
			// 
			label16.AutoSize = true;
			label16.Location = new System.Drawing.Point(32, 46);
			label16.Name = "label16";
			label16.Size = new System.Drawing.Size(44, 12);
			label16.TabIndex = 15;
			label16.Text = "ジャンル:";
			// 
			// label15
			// 
			label15.AutoSize = true;
			label15.Location = new System.Drawing.Point(11, 21);
			label15.Name = "label15";
			label15.Size = new System.Drawing.Size(65, 12);
			label15.TabIndex = 14;
			label15.Text = "チャンネル名:";
			// 
			// label22
			// 
			label22.AutoSize = true;
			label22.Location = new System.Drawing.Point(11, 21);
			label22.Name = "label22";
			label22.Size = new System.Drawing.Size(59, 12);
			label22.TabIndex = 24;
			label22.Text = "アーティスト:";
			// 
			// label23
			// 
			label23.AutoSize = true;
			label23.Location = new System.Drawing.Point(28, 46);
			label23.Name = "label23";
			label23.Size = new System.Drawing.Size(42, 12);
			label23.TabIndex = 25;
			label23.Text = "タイトル:";
			// 
			// label24
			// 
			label24.AutoSize = true;
			label24.Location = new System.Drawing.Point(24, 71);
			label24.Name = "label24";
			label24.Size = new System.Drawing.Size(46, 12);
			label24.TabIndex = 26;
			label24.Text = "アルバム:";
			// 
			// label26
			// 
			label26.AutoSize = true;
			label26.Location = new System.Drawing.Point(24, 96);
			label26.Name = "label26";
			label26.Size = new System.Drawing.Size(43, 12);
			label26.TabIndex = 28;
			label26.Text = "連絡先:";
			label26.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// panel1
			// 
			panel1.AutoSize = true;
			panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			panel1.Controls.Add(this.chanInfoUpdateButton);
			panel1.Dock = System.Windows.Forms.DockStyle.Top;
			panel1.Location = new System.Drawing.Point(0, 360);
			panel1.Name = "panel1";
			panel1.Size = new System.Drawing.Size(406, 32);
			panel1.TabIndex = 21;
			// 
			// chanInfoUpdateButton
			// 
			this.chanInfoUpdateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.chanInfoUpdateButton.Enabled = false;
			this.chanInfoUpdateButton.Location = new System.Drawing.Point(328, 6);
			this.chanInfoUpdateButton.Name = "chanInfoUpdateButton";
			this.chanInfoUpdateButton.Size = new System.Drawing.Size(75, 23);
			this.chanInfoUpdateButton.TabIndex = 30;
			this.chanInfoUpdateButton.Text = "更新";
			this.chanInfoUpdateButton.UseVisualStyleBackColor = true;
			this.chanInfoUpdateButton.Click += new System.EventHandler(this.chanInfoUpdateButton_Click);
			// 
			// notifyIconMenu
			// 
			this.notifyIconMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            showGUIMenuItem,
            this.toolStripMenuItem1,
            quitMenuItem});
			this.notifyIconMenu.Name = "notifyIconMenu";
			this.notifyIconMenu.ShowImageMargin = false;
			this.notifyIconMenu.Size = new System.Drawing.Size(130, 54);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(126, 6);
			// 
			// mainTab
			// 
			this.mainTab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mainTab.Controls.Add(this.tabChannels);
			this.mainTab.Controls.Add(this.tabBroadcast);
			this.mainTab.Controls.Add(this.tabSettings);
			this.mainTab.Controls.Add(this.tabLog);
			this.mainTab.Location = new System.Drawing.Point(0, 0);
			this.mainTab.Name = "mainTab";
			this.mainTab.SelectedIndex = 0;
			this.mainTab.Size = new System.Drawing.Size(445, 435);
			this.mainTab.TabIndex = 0;
			// 
			// tabChannels
			// 
			this.tabChannels.Controls.Add(this.splitContainer1);
			this.tabChannels.Location = new System.Drawing.Point(4, 22);
			this.tabChannels.Name = "tabChannels";
			this.tabChannels.Padding = new System.Windows.Forms.Padding(3);
			this.tabChannels.Size = new System.Drawing.Size(437, 409);
			this.tabChannels.TabIndex = 1;
			this.tabChannels.Text = "チャンネル一覧";
			this.tabChannels.UseVisualStyleBackColor = true;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(3, 3);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.channelBump);
			this.splitContainer1.Panel1.Controls.Add(this.channelClose);
			this.splitContainer1.Panel1.Controls.Add(this.channelPlay);
			this.splitContainer1.Panel1.Controls.Add(this.channelList);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.tabControl2);
			this.splitContainer1.Size = new System.Drawing.Size(431, 403);
			this.splitContainer1.SplitterDistance = 144;
			this.splitContainer1.TabIndex = 9;
			// 
			// channelBump
			// 
			this.channelBump.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.channelBump.Location = new System.Drawing.Point(365, 72);
			this.channelBump.Name = "channelBump";
			this.channelBump.Size = new System.Drawing.Size(61, 30);
			this.channelBump.TabIndex = 9;
			this.channelBump.Text = "再接続";
			this.channelBump.UseVisualStyleBackColor = true;
			this.channelBump.Click += new System.EventHandler(this.channelBump_Click);
			// 
			// channelClose
			// 
			this.channelClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.channelClose.Location = new System.Drawing.Point(365, 36);
			this.channelClose.Name = "channelClose";
			this.channelClose.Size = new System.Drawing.Size(61, 30);
			this.channelClose.TabIndex = 8;
			this.channelClose.Text = "切断";
			this.channelClose.UseVisualStyleBackColor = true;
			this.channelClose.Click += new System.EventHandler(this.channelClose_Click);
			// 
			// channelPlay
			// 
			this.channelPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.channelPlay.Location = new System.Drawing.Point(365, 0);
			this.channelPlay.Name = "channelPlay";
			this.channelPlay.Size = new System.Drawing.Size(61, 30);
			this.channelPlay.TabIndex = 7;
			this.channelPlay.Text = "再生";
			this.channelPlay.UseVisualStyleBackColor = true;
			this.channelPlay.Click += new System.EventHandler(this.channelPlay_Click);
			// 
			// channelList
			// 
			this.channelList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.channelList.DisplayMember = "Name";
			this.channelList.FormattingEnabled = true;
			this.channelList.IntegralHeight = false;
			this.channelList.ItemHeight = 12;
			this.channelList.Location = new System.Drawing.Point(0, 0);
			this.channelList.Name = "channelList";
			this.channelList.Size = new System.Drawing.Size(361, 144);
			this.channelList.TabIndex = 6;
			this.channelList.SelectedIndexChanged += new System.EventHandler(this.channelList_SelectedIndexChanged);
			// 
			// tabControl2
			// 
			this.tabControl2.Controls.Add(this.tabPage1);
			this.tabControl2.Controls.Add(this.tabPage3);
			this.tabControl2.Controls.Add(this.tabPage2);
			this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl2.Location = new System.Drawing.Point(0, 0);
			this.tabControl2.Name = "tabControl2";
			this.tabControl2.SelectedIndex = 0;
			this.tabControl2.Size = new System.Drawing.Size(431, 255);
			this.tabControl2.TabIndex = 9;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.outputList);
			this.tabPage1.Controls.Add(this.downStreamClose);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(423, 229);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "接続一覧";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// outputList
			// 
			this.outputList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.outputList.FormattingEnabled = true;
			this.outputList.IntegralHeight = false;
			this.outputList.ItemHeight = 12;
			this.outputList.Location = new System.Drawing.Point(0, 0);
			this.outputList.Name = "outputList";
			this.outputList.Size = new System.Drawing.Size(357, 229);
			this.outputList.TabIndex = 8;
			// 
			// downStreamClose
			// 
			this.downStreamClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.downStreamClose.Location = new System.Drawing.Point(361, 6);
			this.downStreamClose.Name = "downStreamClose";
			this.downStreamClose.Size = new System.Drawing.Size(61, 30);
			this.downStreamClose.TabIndex = 7;
			this.downStreamClose.Text = "下流切断";
			this.downStreamClose.UseVisualStyleBackColor = true;
			this.downStreamClose.Click += new System.EventHandler(this.downStreamClose_Click);
			// 
			// tabPage3
			// 
			this.tabPage3.AutoScroll = true;
			this.tabPage3.Controls.Add(panel1);
			this.tabPage3.Controls.Add(this.groupBox2);
			this.tabPage3.Controls.Add(this.groupBox1);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(423, 229);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "チャンネル情報";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.AutoSize = true;
			this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.groupBox2.Controls.Add(label26);
			this.groupBox2.Controls.Add(label24);
			this.groupBox2.Controls.Add(label23);
			this.groupBox2.Controls.Add(label22);
			this.groupBox2.Controls.Add(this.chanTrackContactURL);
			this.groupBox2.Controls.Add(this.chanTrackAlbum);
			this.groupBox2.Controls.Add(this.chanTrackTitle);
			this.groupBox2.Controls.Add(this.chanTrackArtist);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox2.Location = new System.Drawing.Point(0, 230);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(406, 130);
			this.groupBox2.TabIndex = 20;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "トラック情報";
			// 
			// chanTrackContactURL
			// 
			this.chanTrackContactURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.chanTrackContactURL.Location = new System.Drawing.Point(82, 93);
			this.chanTrackContactURL.Name = "chanTrackContactURL";
			this.chanTrackContactURL.ReadOnly = true;
			this.chanTrackContactURL.Size = new System.Drawing.Size(321, 19);
			this.chanTrackContactURL.TabIndex = 23;
			this.chanTrackContactURL.Text = "連絡先";
			// 
			// chanTrackAlbum
			// 
			this.chanTrackAlbum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.chanTrackAlbum.Location = new System.Drawing.Point(82, 68);
			this.chanTrackAlbum.Name = "chanTrackAlbum";
			this.chanTrackAlbum.ReadOnly = true;
			this.chanTrackAlbum.Size = new System.Drawing.Size(321, 19);
			this.chanTrackAlbum.TabIndex = 21;
			this.chanTrackAlbum.Text = "アルバム";
			// 
			// chanTrackTitle
			// 
			this.chanTrackTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.chanTrackTitle.Location = new System.Drawing.Point(82, 43);
			this.chanTrackTitle.Name = "chanTrackTitle";
			this.chanTrackTitle.ReadOnly = true;
			this.chanTrackTitle.Size = new System.Drawing.Size(321, 19);
			this.chanTrackTitle.TabIndex = 20;
			this.chanTrackTitle.Text = "タイトル";
			// 
			// chanTrackArtist
			// 
			this.chanTrackArtist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.chanTrackArtist.Location = new System.Drawing.Point(82, 18);
			this.chanTrackArtist.Name = "chanTrackArtist";
			this.chanTrackArtist.ReadOnly = true;
			this.chanTrackArtist.Size = new System.Drawing.Size(321, 19);
			this.chanTrackArtist.TabIndex = 19;
			this.chanTrackArtist.Text = "アーティスト";
			// 
			// groupBox1
			// 
			this.groupBox1.AutoSize = true;
			this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.groupBox1.Controls.Add(this.chanInfoComment);
			this.groupBox1.Controls.Add(label25);
			this.groupBox1.Controls.Add(this.chanInfoBitrate);
			this.groupBox1.Controls.Add(this.chanInfoContentType);
			this.groupBox1.Controls.Add(this.chanInfoContactURL);
			this.groupBox1.Controls.Add(this.chanInfoGenre);
			this.groupBox1.Controls.Add(this.chanInfoDesc);
			this.groupBox1.Controls.Add(this.chanInfoChannelID);
			this.groupBox1.Controls.Add(label21);
			this.groupBox1.Controls.Add(label20);
			this.groupBox1.Controls.Add(label19);
			this.groupBox1.Controls.Add(label18);
			this.groupBox1.Controls.Add(label17);
			this.groupBox1.Controls.Add(this.chanInfoChannelName);
			this.groupBox1.Controls.Add(label16);
			this.groupBox1.Controls.Add(label15);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(406, 230);
			this.groupBox1.TabIndex = 19;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "チャンネル情報";
			// 
			// chanInfoBitrate
			// 
			this.chanInfoBitrate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.chanInfoBitrate.Location = new System.Drawing.Point(82, 193);
			this.chanInfoBitrate.Name = "chanInfoBitrate";
			this.chanInfoBitrate.ReadOnly = true;
			this.chanInfoBitrate.Size = new System.Drawing.Size(321, 19);
			this.chanInfoBitrate.TabIndex = 27;
			this.chanInfoBitrate.Text = "ビットレート";
			// 
			// chanInfoContentType
			// 
			this.chanInfoContentType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.chanInfoContentType.Location = new System.Drawing.Point(82, 168);
			this.chanInfoContentType.Name = "chanInfoContentType";
			this.chanInfoContentType.ReadOnly = true;
			this.chanInfoContentType.Size = new System.Drawing.Size(321, 19);
			this.chanInfoContentType.TabIndex = 26;
			this.chanInfoContentType.Text = "形式";
			// 
			// chanInfoContactURL
			// 
			this.chanInfoContactURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.chanInfoContactURL.Location = new System.Drawing.Point(82, 93);
			this.chanInfoContactURL.Name = "chanInfoContactURL";
			this.chanInfoContactURL.ReadOnly = true;
			this.chanInfoContactURL.Size = new System.Drawing.Size(321, 19);
			this.chanInfoContactURL.TabIndex = 25;
			this.chanInfoContactURL.Text = "コンタクトURL";
			// 
			// chanInfoGenre
			// 
			this.chanInfoGenre.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.chanInfoGenre.Location = new System.Drawing.Point(82, 43);
			this.chanInfoGenre.Name = "chanInfoGenre";
			this.chanInfoGenre.ReadOnly = true;
			this.chanInfoGenre.Size = new System.Drawing.Size(321, 19);
			this.chanInfoGenre.TabIndex = 24;
			this.chanInfoGenre.Text = "ジャンル";
			// 
			// chanInfoDesc
			// 
			this.chanInfoDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.chanInfoDesc.Location = new System.Drawing.Point(82, 68);
			this.chanInfoDesc.Name = "chanInfoDesc";
			this.chanInfoDesc.ReadOnly = true;
			this.chanInfoDesc.Size = new System.Drawing.Size(321, 19);
			this.chanInfoDesc.TabIndex = 23;
			this.chanInfoDesc.Text = "概要";
			// 
			// chanInfoChannelID
			// 
			this.chanInfoChannelID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.chanInfoChannelID.Location = new System.Drawing.Point(82, 143);
			this.chanInfoChannelID.Name = "chanInfoChannelID";
			this.chanInfoChannelID.ReadOnly = true;
			this.chanInfoChannelID.Size = new System.Drawing.Size(321, 19);
			this.chanInfoChannelID.TabIndex = 22;
			this.chanInfoChannelID.Text = "チャンネルID";
			// 
			// chanInfoChannelName
			// 
			this.chanInfoChannelName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.chanInfoChannelName.Location = new System.Drawing.Point(82, 18);
			this.chanInfoChannelName.Name = "chanInfoChannelName";
			this.chanInfoChannelName.ReadOnly = true;
			this.chanInfoChannelName.Size = new System.Drawing.Size(321, 19);
			this.chanInfoChannelName.TabIndex = 16;
			this.chanInfoChannelName.Text = "チャンネル名";
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.relayTree);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(423, 229);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "リレーツリー";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// relayTree
			// 
			this.relayTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.relayTree.Location = new System.Drawing.Point(0, 0);
			this.relayTree.Name = "relayTree";
			this.relayTree.Size = new System.Drawing.Size(422, 229);
			this.relayTree.TabIndex = 5;
			// 
			// tabBroadcast
			// 
			this.tabBroadcast.Controls.Add(this.bcBitrate);
			this.tabBroadcast.Controls.Add(this.bcContactUrl);
			this.tabBroadcast.Controls.Add(this.bcGenre);
			this.tabBroadcast.Controls.Add(this.bcDescription);
			this.tabBroadcast.Controls.Add(this.bcChannelName);
			this.tabBroadcast.Controls.Add(label13);
			this.tabBroadcast.Controls.Add(label12);
			this.tabBroadcast.Controls.Add(label11);
			this.tabBroadcast.Controls.Add(label10);
			this.tabBroadcast.Controls.Add(label9);
			this.tabBroadcast.Controls.Add(this.bcStart);
			this.tabBroadcast.Controls.Add(label8);
			this.tabBroadcast.Controls.Add(label7);
			this.tabBroadcast.Controls.Add(label6);
			this.tabBroadcast.Controls.Add(this.bcYP);
			this.tabBroadcast.Controls.Add(this.bcStreamUrl);
			this.tabBroadcast.Controls.Add(this.bcContentType);
			this.tabBroadcast.Location = new System.Drawing.Point(4, 22);
			this.tabBroadcast.Name = "tabBroadcast";
			this.tabBroadcast.Size = new System.Drawing.Size(437, 409);
			this.tabBroadcast.TabIndex = 4;
			this.tabBroadcast.Text = "配信";
			this.tabBroadcast.UseVisualStyleBackColor = true;
			// 
			// bcBitrate
			// 
			this.bcBitrate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.bcBitrate.Location = new System.Drawing.Point(87, 141);
			this.bcBitrate.Name = "bcBitrate";
			this.bcBitrate.Size = new System.Drawing.Size(309, 19);
			this.bcBitrate.TabIndex = 11;
			// 
			// bcContactUrl
			// 
			this.bcContactUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.bcContactUrl.Location = new System.Drawing.Point(87, 116);
			this.bcContactUrl.Name = "bcContactUrl";
			this.bcContactUrl.Size = new System.Drawing.Size(309, 19);
			this.bcContactUrl.TabIndex = 9;
			// 
			// bcGenre
			// 
			this.bcGenre.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.bcGenre.Location = new System.Drawing.Point(87, 91);
			this.bcGenre.Name = "bcGenre";
			this.bcGenre.Size = new System.Drawing.Size(309, 19);
			this.bcGenre.TabIndex = 7;
			// 
			// bcDescription
			// 
			this.bcDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.bcDescription.Location = new System.Drawing.Point(87, 66);
			this.bcDescription.Name = "bcDescription";
			this.bcDescription.Size = new System.Drawing.Size(309, 19);
			this.bcDescription.TabIndex = 5;
			// 
			// bcChannelName
			// 
			this.bcChannelName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.bcChannelName.Location = new System.Drawing.Point(87, 41);
			this.bcChannelName.Name = "bcChannelName";
			this.bcChannelName.Size = new System.Drawing.Size(309, 19);
			this.bcChannelName.TabIndex = 3;
			// 
			// bcStart
			// 
			this.bcStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bcStart.Location = new System.Drawing.Point(306, 258);
			this.bcStart.Name = "bcStart";
			this.bcStart.Size = new System.Drawing.Size(90, 26);
			this.bcStart.TabIndex = 16;
			this.bcStart.Text = "配信開始";
			this.bcStart.UseVisualStyleBackColor = true;
			this.bcStart.Click += new System.EventHandler(this.bcStart_Click);
			// 
			// bcYP
			// 
			this.bcYP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.bcYP.DisplayMember = "Name";
			this.bcYP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.bcYP.FormattingEnabled = true;
			this.bcYP.Location = new System.Drawing.Point(87, 192);
			this.bcYP.Name = "bcYP";
			this.bcYP.Size = new System.Drawing.Size(309, 20);
			this.bcYP.TabIndex = 15;
			// 
			// bcStreamUrl
			// 
			this.bcStreamUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.bcStreamUrl.Location = new System.Drawing.Point(87, 16);
			this.bcStreamUrl.Name = "bcStreamUrl";
			this.bcStreamUrl.Size = new System.Drawing.Size(309, 19);
			this.bcStreamUrl.TabIndex = 1;
			// 
			// bcContentType
			// 
			this.bcContentType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.bcContentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.bcContentType.FormattingEnabled = true;
			this.bcContentType.Location = new System.Drawing.Point(87, 166);
			this.bcContentType.Name = "bcContentType";
			this.bcContentType.Size = new System.Drawing.Size(309, 20);
			this.bcContentType.TabIndex = 13;
			// 
			// tabSettings
			// 
			this.tabSettings.Controls.Add(label14);
			this.tabSettings.Controls.Add(this.ypListEditButton);
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
			this.tabSettings.Size = new System.Drawing.Size(437, 409);
			this.tabSettings.TabIndex = 2;
			this.tabSettings.Text = "設定";
			this.tabSettings.UseVisualStyleBackColor = true;
			// 
			// ypListEditButton
			// 
			this.ypListEditButton.Location = new System.Drawing.Point(117, 113);
			this.ypListEditButton.Name = "ypListEditButton";
			this.ypListEditButton.Size = new System.Drawing.Size(75, 23);
			this.ypListEditButton.TabIndex = 10;
			this.ypListEditButton.Text = "編集...";
			this.ypListEditButton.UseVisualStyleBackColor = true;
			this.ypListEditButton.Click += new System.EventHandler(this.ypListEditButton_Click);
			// 
			// applySettings
			// 
			this.applySettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.applySettings.Location = new System.Drawing.Point(321, 261);
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
			// tabLog
			// 
			this.tabLog.Controls.Add(this.logToGUICheck);
			this.tabLog.Controls.Add(this.logToConsoleCheck);
			this.tabLog.Controls.Add(this.logClearButton);
			this.tabLog.Controls.Add(this.selectLogFileName);
			this.tabLog.Controls.Add(this.logToFileCheck);
			this.tabLog.Controls.Add(this.logText);
			this.tabLog.Controls.Add(this.logFileNameText);
			this.tabLog.Controls.Add(this.logLevelList);
			this.tabLog.Controls.Add(this.label5);
			this.tabLog.Location = new System.Drawing.Point(4, 22);
			this.tabLog.Name = "tabLog";
			this.tabLog.Padding = new System.Windows.Forms.Padding(3);
			this.tabLog.Size = new System.Drawing.Size(437, 409);
			this.tabLog.TabIndex = 3;
			this.tabLog.Text = "ログ";
			this.tabLog.UseVisualStyleBackColor = true;
			// 
			// logToGUICheck
			// 
			this.logToGUICheck.AutoSize = true;
			this.logToGUICheck.Location = new System.Drawing.Point(8, 38);
			this.logToGUICheck.Name = "logToGUICheck";
			this.logToGUICheck.Size = new System.Drawing.Size(76, 16);
			this.logToGUICheck.TabIndex = 10;
			this.logToGUICheck.Text = "GUIに出力";
			this.logToGUICheck.UseVisualStyleBackColor = true;
			this.logToGUICheck.CheckedChanged += new System.EventHandler(this.logToGUICheck_CheckedChanged);
			// 
			// logToConsoleCheck
			// 
			this.logToConsoleCheck.AutoSize = true;
			this.logToConsoleCheck.Location = new System.Drawing.Point(90, 38);
			this.logToConsoleCheck.Name = "logToConsoleCheck";
			this.logToConsoleCheck.Size = new System.Drawing.Size(103, 16);
			this.logToConsoleCheck.TabIndex = 9;
			this.logToConsoleCheck.Text = "コンソールに出力";
			this.logToConsoleCheck.UseVisualStyleBackColor = true;
			this.logToConsoleCheck.CheckedChanged += new System.EventHandler(this.logToConsoleCheck_CheckedChanged);
			// 
			// logClearButton
			// 
			this.logClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.logClearButton.Location = new System.Drawing.Point(306, 255);
			this.logClearButton.Name = "logClearButton";
			this.logClearButton.Size = new System.Drawing.Size(90, 26);
			this.logClearButton.TabIndex = 8;
			this.logClearButton.Text = "クリア";
			this.logClearButton.UseVisualStyleBackColor = true;
			this.logClearButton.Click += new System.EventHandler(this.logClearButton_Click);
			// 
			// selectLogFileName
			// 
			this.selectLogFileName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.selectLogFileName.Location = new System.Drawing.Point(375, 35);
			this.selectLogFileName.Name = "selectLogFileName";
			this.selectLogFileName.Size = new System.Drawing.Size(21, 18);
			this.selectLogFileName.TabIndex = 7;
			this.selectLogFileName.Text = "...";
			this.selectLogFileName.UseVisualStyleBackColor = true;
			this.selectLogFileName.Click += new System.EventHandler(this.selectLogFileName_Click);
			// 
			// logToFileCheck
			// 
			this.logToFileCheck.AutoSize = true;
			this.logToFileCheck.Location = new System.Drawing.Point(199, 37);
			this.logToFileCheck.Name = "logToFileCheck";
			this.logToFileCheck.Size = new System.Drawing.Size(91, 16);
			this.logToFileCheck.TabIndex = 6;
			this.logToFileCheck.Text = "ファイルに出力";
			this.logToFileCheck.UseVisualStyleBackColor = true;
			this.logToFileCheck.CheckedChanged += new System.EventHandler(this.logToFileCheck_CheckedChanged);
			// 
			// logText
			// 
			this.logText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.logText.Location = new System.Drawing.Point(8, 60);
			this.logText.Multiline = true;
			this.logText.Name = "logText";
			this.logText.ReadOnly = true;
			this.logText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.logText.Size = new System.Drawing.Size(388, 189);
			this.logText.TabIndex = 5;
			// 
			// logFileNameText
			// 
			this.logFileNameText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.logFileNameText.Location = new System.Drawing.Point(296, 35);
			this.logFileNameText.Name = "logFileNameText";
			this.logFileNameText.Size = new System.Drawing.Size(73, 19);
			this.logFileNameText.TabIndex = 4;
			this.logFileNameText.Validated += new System.EventHandler(this.logFileNameText_Validated);
			// 
			// logLevelList
			// 
			this.logLevelList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.logLevelList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.logLevelList.FormattingEnabled = true;
			this.logLevelList.Items.AddRange(new object[] {
            "なし",
            "致命的エラーのみ",
            "エラー全般",
            "エラーと警告",
            "通知メッセージも含む",
            "デバッグメッセージも含む"});
			this.logLevelList.Location = new System.Drawing.Point(105, 9);
			this.logLevelList.Name = "logLevelList";
			this.logLevelList.Size = new System.Drawing.Size(264, 20);
			this.logLevelList.TabIndex = 2;
			this.logLevelList.SelectedIndexChanged += new System.EventHandler(this.logLevelList_SelectedIndexChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(8, 12);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(76, 12);
			this.label5.TabIndex = 1;
			this.label5.Text = "ログ出力レベル";
			// 
			// logSaveFileDialog
			// 
			this.logSaveFileDialog.DefaultExt = "txt";
			this.logSaveFileDialog.Filter = "ログファイル(*.txt;*.log)|*.txt;*.log|全てのファイル(*.*)|*.*";
			this.logSaveFileDialog.Title = "ログ記録ファイルの選択";
			// 
			// statusBar
			// 
			this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.portLabel,
            this.portOpenedLabel});
			this.statusBar.Location = new System.Drawing.Point(0, 433);
			this.statusBar.Name = "statusBar";
			this.statusBar.Size = new System.Drawing.Size(443, 23);
			this.statusBar.TabIndex = 1;
			// 
			// portLabel
			// 
			this.portLabel.Name = "portLabel";
			this.portLabel.Size = new System.Drawing.Size(49, 18);
			this.portLabel.Text = "ポート:";
			// 
			// portOpenedLabel
			// 
			this.portOpenedLabel.Name = "portOpenedLabel";
			this.portOpenedLabel.Size = new System.Drawing.Size(134, 18);
			this.portOpenedLabel.Text = "toolStripStatusLabel1";
			// 
			// chanInfoComment
			// 
			this.chanInfoComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.chanInfoComment.Location = new System.Drawing.Point(82, 118);
			this.chanInfoComment.Name = "chanInfoComment";
			this.chanInfoComment.ReadOnly = true;
			this.chanInfoComment.Size = new System.Drawing.Size(321, 19);
			this.chanInfoComment.TabIndex = 29;
			this.chanInfoComment.Text = "配信コメント";
			// 
			// label25
			// 
			label25.AutoSize = true;
			label25.Location = new System.Drawing.Point(12, 121);
			label25.Name = "label25";
			label25.Size = new System.Drawing.Size(64, 12);
			label25.TabIndex = 28;
			label25.Text = "配信コメント:";
			label25.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(443, 456);
			this.Controls.Add(this.statusBar);
			this.Controls.Add(this.mainTab);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainForm";
			this.Text = "PeerCastStation";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			panel1.ResumeLayout(false);
			this.notifyIconMenu.ResumeLayout(false);
			this.mainTab.ResumeLayout(false);
			this.tabChannels.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.tabControl2.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.tabPage3.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.tabBroadcast.ResumeLayout(false);
			this.tabBroadcast.PerformLayout();
			this.tabSettings.ResumeLayout(false);
			this.tabSettings.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.maxUpstreamRate)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.maxDirects)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.maxRelays)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.port)).EndInit();
			this.tabLog.ResumeLayout(false);
			this.tabLog.PerformLayout();
			this.statusBar.ResumeLayout(false);
			this.statusBar.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TabControl mainTab;
    private System.Windows.Forms.TabPage tabChannels;
    private System.Windows.Forms.TabPage tabSettings;
    private System.Windows.Forms.NumericUpDown maxUpstreamRate;
    private System.Windows.Forms.NumericUpDown maxDirects;
    private System.Windows.Forms.NumericUpDown maxRelays;
    private System.Windows.Forms.NumericUpDown port;
    private System.Windows.Forms.Button applySettings;
    private System.Windows.Forms.TabPage tabLog;
    private System.Windows.Forms.ComboBox logLevelList;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Button selectLogFileName;
    private System.Windows.Forms.CheckBox logToFileCheck;
    private System.Windows.Forms.TextBox logText;
    private System.Windows.Forms.TextBox logFileNameText;
    private System.Windows.Forms.SaveFileDialog logSaveFileDialog;
    private System.Windows.Forms.Button logClearButton;
    private System.Windows.Forms.StatusStrip statusBar;
    private System.Windows.Forms.ToolStripStatusLabel portLabel;
    private System.Windows.Forms.ToolStripStatusLabel portOpenedLabel;
    private System.Windows.Forms.CheckBox logToGUICheck;
    private System.Windows.Forms.CheckBox logToConsoleCheck;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.Button channelBump;
    private System.Windows.Forms.Button channelClose;
    private System.Windows.Forms.Button channelPlay;
    private System.Windows.Forms.ListBox channelList;
    private System.Windows.Forms.TabControl tabControl2;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.ListBox outputList;
    private System.Windows.Forms.Button downStreamClose;
    private System.Windows.Forms.TabPage tabPage3;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.TreeView relayTree;
    private System.Windows.Forms.TabPage tabBroadcast;
    private System.Windows.Forms.TextBox bcBitrate;
    private System.Windows.Forms.TextBox bcContactUrl;
    private System.Windows.Forms.TextBox bcGenre;
    private System.Windows.Forms.TextBox bcDescription;
    private System.Windows.Forms.TextBox bcChannelName;
    private System.Windows.Forms.Button bcStart;
    private System.Windows.Forms.ComboBox bcYP;
    private System.Windows.Forms.TextBox bcStreamUrl;
    private System.Windows.Forms.ComboBox bcContentType;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
    private System.Windows.Forms.ContextMenuStrip notifyIconMenu;
    private System.Windows.Forms.Button ypListEditButton;
    private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox chanTrackContactURL;
    private System.Windows.Forms.TextBox chanTrackAlbum;
    private System.Windows.Forms.TextBox chanTrackTitle;
    private System.Windows.Forms.TextBox chanTrackArtist;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TextBox chanInfoBitrate;
    private System.Windows.Forms.TextBox chanInfoContentType;
    private System.Windows.Forms.TextBox chanInfoContactURL;
    private System.Windows.Forms.TextBox chanInfoGenre;
    private System.Windows.Forms.TextBox chanInfoDesc;
    private System.Windows.Forms.TextBox chanInfoChannelID;
    private System.Windows.Forms.TextBox chanInfoChannelName;
    private System.Windows.Forms.Button chanInfoUpdateButton;
		private System.Windows.Forms.TextBox chanInfoComment;

  }
}

