﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SchedulingForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Scheduling_ActionsPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.Scheduling_Cancel = New System.Windows.Forms.Button()
        Me.Scheduling_Save = New System.Windows.Forms.Button()
        Me.Scheduling_WarningLabel = New System.Windows.Forms.Label()
        Me.Scheduling_Enable = New System.Windows.Forms.CheckBox()
        Me.FrequencyLayoutPanel = New System.Windows.Forms.FlowLayoutPanel()
        Me.DailyBtn = New System.Windows.Forms.RadioButton()
        Me.WeeklyBtn = New System.Windows.Forms.RadioButton()
        Me.Scheduling_WeekDay = New System.Windows.Forms.ComboBox()
        Me.MonthlyBtn = New System.Windows.Forms.RadioButton()
        Me.Scheduling_MonthDay = New System.Windows.Forms.NumericUpDown()
        Me.Scheduling_Panel = New System.Windows.Forms.Panel()
        Me.Scheduling_AtLabel = New System.Windows.Forms.Label()
        Me.Scheduling_TimeSelectionPanel = New System.Windows.Forms.FlowLayoutPanel()
        Me.Scheduling_Time = New System.Windows.Forms.DateTimePicker()
        Me.Scheduling_OptionsLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.Scheduling_Catchup = New System.Windows.Forms.CheckBox()
        Me.Scheduling_ActionsPanel.SuspendLayout()
        Me.FrequencyLayoutPanel.SuspendLayout()
        CType(Me.Scheduling_MonthDay, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Scheduling_Panel.SuspendLayout()
        Me.Scheduling_TimeSelectionPanel.SuspendLayout()
        Me.Scheduling_OptionsLayoutPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'Scheduling_ActionsPanel
        '
        Me.Scheduling_ActionsPanel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Scheduling_ActionsPanel.ColumnCount = 2
        Me.Scheduling_ActionsPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.Scheduling_ActionsPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.Scheduling_ActionsPanel.Controls.Add(Me.Scheduling_Cancel, 1, 0)
        Me.Scheduling_ActionsPanel.Controls.Add(Me.Scheduling_Save, 0, 0)
        Me.Scheduling_ActionsPanel.Location = New System.Drawing.Point(362, 173)
        Me.Scheduling_ActionsPanel.Name = "Scheduling_ActionsPanel"
        Me.Scheduling_ActionsPanel.RowCount = 1
        Me.Scheduling_ActionsPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.Scheduling_ActionsPanel.Size = New System.Drawing.Size(200, 31)
        Me.Scheduling_ActionsPanel.TabIndex = 4
        '
        'Scheduling_Cancel
        '
        Me.Scheduling_Cancel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Scheduling_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Scheduling_Cancel.Location = New System.Drawing.Point(103, 3)
        Me.Scheduling_Cancel.Name = "Scheduling_Cancel"
        Me.Scheduling_Cancel.Size = New System.Drawing.Size(94, 25)
        Me.Scheduling_Cancel.TabIndex = 1
        Me.Scheduling_Cancel.Text = "\CANCEL"
        Me.Scheduling_Cancel.UseVisualStyleBackColor = True
        '
        'Scheduling_Save
        '
        Me.Scheduling_Save.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Scheduling_Save.Location = New System.Drawing.Point(3, 3)
        Me.Scheduling_Save.Name = "Scheduling_Save"
        Me.Scheduling_Save.Size = New System.Drawing.Size(94, 25)
        Me.Scheduling_Save.TabIndex = 0
        Me.Scheduling_Save.Text = "\SAVE"
        Me.Scheduling_Save.UseVisualStyleBackColor = True
        '
        'Scheduling_WarningLabel
        '
        Me.Scheduling_WarningLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom), System.Windows.Forms.AnchorStyles)
        Me.Scheduling_WarningLabel.BackColor = System.Drawing.Color.Orange
        Me.Scheduling_WarningLabel.Location = New System.Drawing.Point(0, 0)
        Me.Scheduling_WarningLabel.Name = "Scheduling_WarningLabel"
        Me.Scheduling_WarningLabel.Padding = New System.Windows.Forms.Padding(2)
        Me.Scheduling_WarningLabel.Size = New System.Drawing.Size(574, 51)
        Me.Scheduling_WarningLabel.TabIndex = 0
        Me.Scheduling_WarningLabel.Text = "\SCHEDULE_WARNING"
        Me.Scheduling_WarningLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Scheduling_Enable
        '
        Me.Scheduling_Enable.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Scheduling_Enable.AutoSize = True
        Me.Scheduling_Enable.Location = New System.Drawing.Point(3, 3)
        Me.Scheduling_Enable.Name = "Scheduling_Enable"
        Me.Scheduling_Enable.Size = New System.Drawing.Size(144, 20)
        Me.Scheduling_Enable.TabIndex = 1
        Me.Scheduling_Enable.Text = "\SCHEDULE_ENABLE"
        Me.Scheduling_Enable.UseVisualStyleBackColor = True
        '
        'FrequencyLayoutPanel
        '
        Me.FrequencyLayoutPanel.AutoSize = True
        Me.FrequencyLayoutPanel.Controls.Add(Me.DailyBtn)
        Me.FrequencyLayoutPanel.Controls.Add(Me.WeeklyBtn)
        Me.FrequencyLayoutPanel.Controls.Add(Me.Scheduling_WeekDay)
        Me.FrequencyLayoutPanel.Controls.Add(Me.MonthlyBtn)
        Me.FrequencyLayoutPanel.Controls.Add(Me.Scheduling_MonthDay)
        Me.FrequencyLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FrequencyLayoutPanel.Location = New System.Drawing.Point(0, 0)
        Me.FrequencyLayoutPanel.Name = "FrequencyLayoutPanel"
        Me.FrequencyLayoutPanel.Size = New System.Drawing.Size(550, 77)
        Me.FrequencyLayoutPanel.TabIndex = 0
        '
        'DailyBtn
        '
        Me.DailyBtn.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.DailyBtn.AutoSize = True
        Me.DailyBtn.Checked = True
        Me.FrequencyLayoutPanel.SetFlowBreak(Me.DailyBtn, True)
        Me.DailyBtn.Location = New System.Drawing.Point(3, 3)
        Me.DailyBtn.Name = "DailyBtn"
        Me.DailyBtn.Size = New System.Drawing.Size(65, 17)
        Me.DailyBtn.TabIndex = 2
        Me.DailyBtn.TabStop = True
        Me.DailyBtn.Text = "\DAILY"
        Me.DailyBtn.UseVisualStyleBackColor = True
        '
        'WeeklyBtn
        '
        Me.WeeklyBtn.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.WeeklyBtn.AutoSize = True
        Me.WeeklyBtn.Location = New System.Drawing.Point(3, 26)
        Me.WeeklyBtn.Margin = New System.Windows.Forms.Padding(3, 3, 0, 3)
        Me.WeeklyBtn.Name = "WeeklyBtn"
        Me.WeeklyBtn.Size = New System.Drawing.Size(76, 21)
        Me.WeeklyBtn.TabIndex = 3
        Me.WeeklyBtn.TabStop = True
        Me.WeeklyBtn.Text = "\WEEKLY"
        Me.WeeklyBtn.UseVisualStyleBackColor = True
        '
        'Scheduling_WeekDay
        '
        Me.Scheduling_WeekDay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.FrequencyLayoutPanel.SetFlowBreak(Me.Scheduling_WeekDay, True)
        Me.Scheduling_WeekDay.FormattingEnabled = True
        Me.Scheduling_WeekDay.Location = New System.Drawing.Point(79, 26)
        Me.Scheduling_WeekDay.Margin = New System.Windows.Forms.Padding(0, 3, 3, 3)
        Me.Scheduling_WeekDay.Name = "Scheduling_WeekDay"
        Me.Scheduling_WeekDay.Size = New System.Drawing.Size(121, 21)
        Me.Scheduling_WeekDay.TabIndex = 1
        '
        'MonthlyBtn
        '
        Me.MonthlyBtn.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.MonthlyBtn.AutoSize = True
        Me.MonthlyBtn.Location = New System.Drawing.Point(3, 53)
        Me.MonthlyBtn.Margin = New System.Windows.Forms.Padding(3, 3, 0, 3)
        Me.MonthlyBtn.Name = "MonthlyBtn"
        Me.MonthlyBtn.Size = New System.Drawing.Size(84, 21)
        Me.MonthlyBtn.TabIndex = 5
        Me.MonthlyBtn.TabStop = True
        Me.MonthlyBtn.Text = "\MONTHLY"
        Me.MonthlyBtn.UseVisualStyleBackColor = True
        '
        'Scheduling_MonthDay
        '
        Me.Scheduling_MonthDay.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom), System.Windows.Forms.AnchorStyles)
        Me.Scheduling_MonthDay.AutoSize = True
        Me.Scheduling_MonthDay.Location = New System.Drawing.Point(87, 53)
        Me.Scheduling_MonthDay.Margin = New System.Windows.Forms.Padding(0, 3, 3, 3)
        Me.Scheduling_MonthDay.Maximum = New Decimal(New Integer() {28, 0, 0, 0})
        Me.Scheduling_MonthDay.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.Scheduling_MonthDay.Name = "Scheduling_MonthDay"
        Me.Scheduling_MonthDay.Size = New System.Drawing.Size(37, 21)
        Me.Scheduling_MonthDay.TabIndex = 4
        Me.Scheduling_MonthDay.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'Scheduling_Panel
        '
        Me.Scheduling_Panel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Scheduling_Panel.Controls.Add(Me.FrequencyLayoutPanel)
        Me.Scheduling_Panel.Enabled = False
        Me.Scheduling_Panel.Location = New System.Drawing.Point(12, 90)
        Me.Scheduling_Panel.Name = "Scheduling_Panel"
        Me.Scheduling_Panel.Size = New System.Drawing.Size(550, 77)
        Me.Scheduling_Panel.TabIndex = 2
        '
        'Scheduling_AtLabel
        '
        Me.Scheduling_AtLabel.AutoSize = True
        Me.Scheduling_AtLabel.Dock = System.Windows.Forms.DockStyle.Left
        Me.Scheduling_AtLabel.Location = New System.Drawing.Point(3, 0)
        Me.Scheduling_AtLabel.Name = "Scheduling_AtLabel"
        Me.Scheduling_AtLabel.Size = New System.Drawing.Size(27, 27)
        Me.Scheduling_AtLabel.TabIndex = 0
        Me.Scheduling_AtLabel.Text = "\AT"
        Me.Scheduling_AtLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Scheduling_TimeSelectionPanel
        '
        Me.Scheduling_TimeSelectionPanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Scheduling_TimeSelectionPanel.Controls.Add(Me.Scheduling_AtLabel)
        Me.Scheduling_TimeSelectionPanel.Controls.Add(Me.Scheduling_Time)
        Me.Scheduling_TimeSelectionPanel.Enabled = False
        Me.Scheduling_TimeSelectionPanel.Location = New System.Drawing.Point(12, 173)
        Me.Scheduling_TimeSelectionPanel.Name = "Scheduling_TimeSelectionPanel"
        Me.Scheduling_TimeSelectionPanel.Size = New System.Drawing.Size(344, 31)
        Me.Scheduling_TimeSelectionPanel.TabIndex = 3
        '
        'Scheduling_Time
        '
        Me.Scheduling_Time.CustomFormat = "HH:mm"
        Me.Scheduling_Time.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.Scheduling_Time.Location = New System.Drawing.Point(36, 3)
        Me.Scheduling_Time.Name = "Scheduling_Time"
        Me.Scheduling_Time.ShowUpDown = True
        Me.Scheduling_Time.Size = New System.Drawing.Size(63, 21)
        Me.Scheduling_Time.TabIndex = 6
        Me.Scheduling_Time.Value = New Date(2011, 1, 1, 0, 0, 0, 0)
        '
        'Scheduling_OptionsLayoutPanel
        '
        Me.Scheduling_OptionsLayoutPanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Scheduling_OptionsLayoutPanel.ColumnCount = 2
        Me.Scheduling_OptionsLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.Scheduling_OptionsLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.Scheduling_OptionsLayoutPanel.Controls.Add(Me.Scheduling_Catchup, 1, 0)
        Me.Scheduling_OptionsLayoutPanel.Controls.Add(Me.Scheduling_Enable, 0, 0)
        Me.Scheduling_OptionsLayoutPanel.Location = New System.Drawing.Point(12, 58)
        Me.Scheduling_OptionsLayoutPanel.Name = "Scheduling_OptionsLayoutPanel"
        Me.Scheduling_OptionsLayoutPanel.RowCount = 1
        Me.Scheduling_OptionsLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.Scheduling_OptionsLayoutPanel.Size = New System.Drawing.Size(550, 26)
        Me.Scheduling_OptionsLayoutPanel.TabIndex = 5
        '
        'Scheduling_Catchup
        '
        Me.Scheduling_Catchup.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Scheduling_Catchup.AutoSize = True
        Me.Scheduling_Catchup.Enabled = False
        Me.Scheduling_Catchup.Location = New System.Drawing.Point(344, 3)
        Me.Scheduling_Catchup.Name = "Scheduling_Catchup"
        Me.Scheduling_Catchup.Size = New System.Drawing.Size(203, 20)
        Me.Scheduling_Catchup.TabIndex = 2
        Me.Scheduling_Catchup.Tag = "\CATCHUP_MISSED_BACKUPS_TAG"
        Me.Scheduling_Catchup.Text = "\CATCHUP_MISSED_BACKUPS"
        Me.Scheduling_Catchup.UseVisualStyleBackColor = True
        '
        'SchedulingForm
        '
        Me.AcceptButton = Me.Scheduling_Save
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Scheduling_Cancel
        Me.ClientSize = New System.Drawing.Size(574, 216)
        Me.Controls.Add(Me.Scheduling_OptionsLayoutPanel)
        Me.Controls.Add(Me.Scheduling_TimeSelectionPanel)
        Me.Controls.Add(Me.Scheduling_Panel)
        Me.Controls.Add(Me.Scheduling_WarningLabel)
        Me.Controls.Add(Me.Scheduling_ActionsPanel)
        Me.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "SchedulingForm"
        Me.ShowInTaskbar = False
        Me.Text = "\SCHEDULING"
        Me.Scheduling_ActionsPanel.ResumeLayout(False)
        Me.FrequencyLayoutPanel.ResumeLayout(False)
        Me.FrequencyLayoutPanel.PerformLayout()
        CType(Me.Scheduling_MonthDay, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Scheduling_Panel.ResumeLayout(False)
        Me.Scheduling_Panel.PerformLayout()
        Me.Scheduling_TimeSelectionPanel.ResumeLayout(False)
        Me.Scheduling_TimeSelectionPanel.PerformLayout()
        Me.Scheduling_OptionsLayoutPanel.ResumeLayout(False)
        Me.Scheduling_OptionsLayoutPanel.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Scheduling_Cancel As System.Windows.Forms.Button
    Friend WithEvents Scheduling_Save As System.Windows.Forms.Button
    Friend WithEvents Scheduling_ActionsPanel As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Scheduling_WarningLabel As System.Windows.Forms.Label
    Friend WithEvents Scheduling_Enable As System.Windows.Forms.CheckBox
    Friend WithEvents FrequencyLayoutPanel As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents Scheduling_Panel As System.Windows.Forms.Panel
    Friend WithEvents Scheduling_MonthDay As System.Windows.Forms.NumericUpDown
    Friend WithEvents Scheduling_WeekDay As System.Windows.Forms.ComboBox
    Friend WithEvents Scheduling_AtLabel As System.Windows.Forms.Label
    Friend WithEvents Scheduling_TimeSelectionPanel As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents Scheduling_OptionsLayoutPanel As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Scheduling_Catchup As System.Windows.Forms.CheckBox
    Friend WithEvents DailyBtn As System.Windows.Forms.RadioButton
    Friend WithEvents MonthlyBtn As System.Windows.Forms.RadioButton
    Friend WithEvents WeeklyBtn As System.Windows.Forms.RadioButton
    Friend WithEvents Scheduling_Time As System.Windows.Forms.DateTimePicker

End Class
