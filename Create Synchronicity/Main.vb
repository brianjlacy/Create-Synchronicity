﻿<Assembly: CLSCompliant(True)> 

Module Main
    Friend Translation As LanguageHandler
    Friend ProgramConfig As ConfigHandler
    Friend Profiles As Dictionary(Of String, ProfileHandler)
    Friend ReloadNeeded As Boolean
    Friend MainFormInstance As MainForm

    Friend MsgLoop As MessageLoop
    Friend Delegate Sub Action() 'LATER: replace with .Net 4.0 standards.

    <STAThread()> _
    Sub Main()
        ' Must come first
        Application.EnableVisualStyles()

        Try
            MsgLoop = New MessageLoop
            If Not MsgLoop.ExitNeeded Then Application.Run(MsgLoop)

        Catch Ex As Exception
            If MessageBox.Show("A critical error has occured. Can we upload the error log? " & Environment.NewLine & "Here's what we would send:" & Environment.NewLine & Environment.NewLine & Ex.ToString & Environment.NewLine & Environment.NewLine & "If not, you can copy this message using Ctrl+C and send it to createsoftware@users.sourceforge.net." & Environment.NewLine, "Critical error", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                Dim ReportingClient As New Net.WebClient
                Try
                    ReportingClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded")
                    MessageBox.Show(ReportingClient.UploadString(Website & "code/bug.php", "POST", "msg=" & Ex.ToString), "Bug report submitted!", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch SubEx As Net.WebException
                    MessageBox.Show("Unable to submit report. Plead send the following to createsoftware@users.sourceforge.net (Ctrl+C): " & Environment.NewLine & Ex.ToString, "Unable to submit report", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Finally
                    ReportingClient.Dispose()
                End Try
            End If
            Throw
        End Try
    End Sub
End Module

Friend NotInheritable Class MessageLoop
    Inherits ApplicationContext
    Public ExitNeeded As Boolean '= False

    Private Blocker As Threading.Mutex '= Nothing
    Dim ScheduledProfiles As New List(Of SchedulerEntry) '= Nothing

#Region "Main program loop & first run"
    Sub New()
        MyBase.New()

        ' Initialize ProgramConfig, Translation 
        InitializeSharedObjects()

        'Read command line settings
        CommandLine.ReadArgs(New List(Of String)(Environment.GetCommandLineArgs()))

        ' Start logging
        ConfigHandler.LogAppEvent("Program started: " & Application.StartupPath)
        ConfigHandler.LogAppEvent(String.Format("Profiles folder: {0}.", ProgramConfig.ConfigRootDir))
        Interaction.ShowDebug(Translation.Translate("\DEBUG_WARNING"), Translation.Translate("\DEBUG_MODE"))

        ' Check if multiple instances are allowed.
        If CommandLine.RunAs = CommandLine.RunMode.Scheduler AndAlso SchedulerAlreadyRunning() Then
            ConfigHandler.LogAppEvent("Scheduler already running; exiting.")
            ExitNeeded = True : Exit Sub
        Else
            AddHandler Me.ThreadExit, AddressOf MessageLoop_ThreadExit
        End If

        ' Setup settings
        ReloadProfiles()
        ProgramConfig.LoadProgramSettings()
        If Not ProgramConfig.ProgramSettingsSet(ProgramSetting.AutoUpdates) Or Not ProgramConfig.ProgramSettingsSet(ProgramSetting.Language) Then
            HandleFirstRun()
        End If

        ' Initialize Main, Updates
        InitializeForms()

        ' Look for updates
        If (Not CommandLine.NoUpdates) And ProgramConfig.GetProgramSetting(Of Boolean)(ProgramSetting.AutoUpdates, False) Then
            Dim UpdateThread As New Threading.Thread(AddressOf Updates.CheckForUpdates)
            UpdateThread.Start()
        End If

        If CommandLine.Help Then
            Interaction.ShowMsg(String.Format("Create Synchronicity, version {1}.{0}{0}Profiles folder: ""{2}"".{0}{0}Available commands: see manual.{0}{0}License information: See ""Release notes.txt"".{0}{0}Full manual: See {3}help.html.{0}{0}You can help this software! See {3}contribute.html.{0}{0}Happy syncing!", Environment.NewLine, Application.ProductVersion, ProgramConfig.ConfigRootDir, ProgramSetting.Website), "Help!")
        Else
            If CommandLine.RunAs = CommandLine.RunMode.Queue Or CommandLine.RunAs = CommandLine.RunMode.Scheduler Then
                Interaction.ToggleStatusIcon(True)

                If CommandLine.RunAs = CommandLine.RunMode.Queue Then
                    MainFormInstance.ApplicationTimer.Interval = 1000
                    AddHandler MainFormInstance.ApplicationTimer.Tick, AddressOf StartQueue
                ElseIf CommandLine.RunAs = CommandLine.RunMode.Scheduler Then
                    MainFormInstance.ApplicationTimer.Interval = 15000
                    AddHandler MainFormInstance.ApplicationTimer.Tick, AddressOf Scheduling_Tick
                End If
                MainFormInstance.ApplicationTimer.Start() 'First tick fires after ApplicationTimer.Interval milliseconds.
            Else
                AddHandler MainFormInstance.FormClosed, AddressOf ReloadMainForm
                MainFormInstance.Show()
            End If
        End If
    End Sub

    Private Sub MessageLoop_ThreadExit(ByVal sender As Object, ByVal e As System.EventArgs)
        ExitNeeded = True
        Interaction.ToggleStatusIcon(False)

        ' Save last window information. Don't overwrite config file if running in scheduler mode.
        If Not CommandLine.RunAs = CommandLine.RunMode.Scheduler Then ProgramConfig.SaveProgramSettings()

        'Calling ReleaseMutex would be the same, since Blocker necessary holds the mutex at this point (otherwise the app would have closed already).
        If CommandLine.RunAs = CommandLine.RunMode.Scheduler Then Blocker.Close()
        ConfigHandler.LogAppEvent("Program exited")

#If Debug And 0 Then
        SynchronizeForm.Check_NTFSToFATTime()
#End If
    End Sub

    Private Sub ReloadMainForm(ByVal sender As Object, ByVal e As FormClosedEventArgs)
        If ReloadNeeded Then
            MainFormInstance = New MainForm
            AddHandler MainFormInstance.FormClosed, AddressOf Me.ReloadMainForm
            MainFormInstance.Show()
        Else
            Application.Exit()
        End If
    End Sub

    Shared Sub InitializeSharedObjects()
        ' Load program configuration
        ProgramConfig = ConfigHandler.GetSingleton
        Translation = LanguageHandler.GetSingleton

        ' Create required folders
        IO.Directory.CreateDirectory(ProgramConfig.LogRootDir)
        IO.Directory.CreateDirectory(ProgramConfig.ConfigRootDir)
        IO.Directory.CreateDirectory(ProgramConfig.LanguageRootDir)
    End Sub

    Shared Sub InitializeForms()
        ' Create MainForm
        MainFormInstance = New MainForm()

        'Load status icon
        Interaction.LoadStatusIcon()
        MainFormInstance.ToolStripHeader.Image = Interaction.StatusIcon.Icon.ToBitmap
        Interaction.StatusIcon.ContextMenuStrip = MainFormInstance.StatusIconMenu
    End Sub

    Shared Sub HandleFirstRun()
        If Not ProgramConfig.ProgramSettingsSet(ProgramSetting.Language) Then
            Dim Lng As New LanguageForm : Lng.ShowDialog()
            Translation = LanguageHandler.GetSingleton(True)
        End If

        If Not ProgramConfig.ProgramSettingsSet(ProgramSetting.AutoUpdates) Then
            Dim AutoUpdates As Boolean = If(Interaction.ShowMsg(Translation.Translate("\WELCOME_MSG"), Translation.Translate("\FIRST_RUN"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes, True, False)
            ProgramConfig.SetProgramSetting(Of Boolean)(ProgramSetting.AutoUpdates, AutoUpdates)
        End If

        ProgramConfig.SaveProgramSettings()
    End Sub

    Shared Sub ReloadProfiles()
        Profiles = New Dictionary(Of String, ProfileHandler)

        For Each ConfigFile As String In IO.Directory.GetFiles(ProgramConfig.ConfigRootDir, "*.sync")
            Dim Name As String = IO.Path.GetFileNameWithoutExtension(ConfigFile)
            Profiles.Add(Name, New ProfileHandler(Name))
        Next
    End Sub
#End Region

#Region "Scheduling"
    Private Function SchedulerAlreadyRunning() As Boolean
        Dim MutexName As String = "[[Create Synchronicity scheduler]] " & Application.ExecutablePath.Replace(ProgramSetting.DirSep, "!"c).ToLower(Interaction.InvariantCulture)
        ConfigHandler.LogDebugEvent(String.Format("Registering mutex: ""{0}""", MutexName))

        Try
            Blocker = New Threading.Mutex(False, MutexName)
        Catch Ex As Threading.AbandonedMutexException
            ConfigHandler.LogDebugEvent("Abandoned mutex detected")
            Return False
        End Try

        Return (Not Blocker.WaitOne(0, False))
    End Function

    Shared Sub RedoSchedulerRegistration()
        Dim NeedToRunAtBootTime As Boolean = False
        For Each Profile As ProfileHandler In Profiles.Values
            NeedToRunAtBootTime = NeedToRunAtBootTime Or (Profile.Scheduler.Frequency <> ScheduleInfo.Freq.Never)
            If Profile.Scheduler.Frequency <> ScheduleInfo.Freq.Never Then ConfigHandler.LogAppEvent(String.Format("Profile {0} requires the scheduler to run.", Profile.ProfileName))
        Next

        Try
            If NeedToRunAtBootTime Then
                ConfigHandler.RegisterBoot()
                ConfigHandler.LogAppEvent("Registered program in startup list, trying to start scheduler")
                If CommandLine.RunAs = CommandLine.RunMode.Normal Then Diagnostics.Process.Start(Application.ExecutablePath, "/scheduler /noupdates" & If(CommandLine.Log, " /log", ""))
            Else
                If Microsoft.Win32.Registry.GetValue(ProgramSetting.RegistryRootedBootKey, ProgramSetting.RegistryBootVal, Nothing) IsNot Nothing Then
                    ConfigHandler.LogAppEvent("Unregistering program from startup list")
                    Microsoft.Win32.Registry.CurrentUser.OpenSubKey(ProgramSetting.RegistryBootKey, True).DeleteValue(ProgramSetting.RegistryBootVal)
                End If
            End If
        Catch Ex As Exception
            Interaction.ShowMsg(Translation.Translate("\UNREG_ERROR"), Translation.Translate("\ERROR"), MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub StartQueue(ByVal sender As System.Object, ByVal e As System.EventArgs)
        MainFormInstance.ApplicationTimer.Interval = 5000 'Wait 5s between profiles k and k+1, k > 0
        MainFormInstance.ApplicationTimer.Stop()
        ProcessProfilesQueue()
    End Sub

    Private Sub ProcessProfilesQueue()
        Static ProfilesQueue As Queue(Of String) = Nothing

        If ProfilesQueue Is Nothing Then
            ProfilesQueue = New Queue(Of String)

            ConfigHandler.LogAppEvent("Profiles queue: Queue created.")
            For Each Profile As String In CommandLine.TasksToRun.Split(ProgramSetting.EnqueuingSeparator)
                If Profiles.ContainsKey(Profile) Then
                    If Profiles(Profile).ValidateConfigFile() Then
                        ConfigHandler.LogAppEvent("Profiles queue: Registered profile " & Profile)
                        ProfilesQueue.Enqueue(Profile)
                    Else
                        Interaction.ShowMsg(Translation.Translate("\INVALID_CONFIG"), Translation.Translate("\INVALID_CMD"), , MessageBoxIcon.Error)
                    End If
                Else
                    Interaction.ShowMsg(Translation.Translate("\INVALID_PROFILE"), Translation.Translate("\INVALID_CMD"), , MessageBoxIcon.Error)
                End If
            Next
        End If

        If ProfilesQueue.Count = 0 Then
            ConfigHandler.LogAppEvent("Profiles queue: Synced all profiles.")
            Application.Exit()
        Else
            Dim SyncForm As New SynchronizeForm(ProfilesQueue.Dequeue(), CommandLine.ShowPreview, CommandLine.Quiet, False)
            AddHandler SyncForm.SyncFinished, Sub(Name As String, Completed As Boolean) MainFormInstance.ApplicationTimer.Start() 'Wait for 5 seconds before moving on.
            SyncForm.StartSynchronization(False)
        End If
    End Sub

    Private Sub ScheduledProfileCompleted(ByVal ProfileName As String, ByVal Completed As Boolean)
        If Completed Then ConfigHandler.LogAppEvent("Scheduler: " & ProfileName & " completed successfully.")
        If Not Profiles.ContainsKey(ProfileName) Then Exit Sub

        If Completed Then
            ScheduledProfiles.Add(New SchedulerEntry(ProfileName, Profiles(ProfileName).Scheduler.NextRun(), False, False))
        Else
            ConfigHandler.LogAppEvent("Scheduler: " & ProfileName & " reported an error, will run again in 4 hours.")
            ScheduledProfiles.Add(New SchedulerEntry(ProfileName, Date.Now.AddHours(4), True, True))
        End If
    End Sub

    Private Sub Scheduling_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If ProgramConfig.CanGoOn = False Then Exit Sub 'Don't start next sync yet.

        ReloadScheduledProfiles()
        If ScheduledProfiles.Count = 0 Then
            ConfigHandler.LogAppEvent("Scheduler: No profiles left to run, exiting.")
            Application.Exit()
            Exit Sub
        Else
            Dim NextInQueue As SchedulerEntry = ScheduledProfiles(0)
            Dim Status As String = String.Format(Translation.Translate("\SCH_WAITING"), NextInQueue.Name, If(NextInQueue.NextRun = ScheduleInfo.DATE_CATCHUP, "", NextInQueue.NextRun.ToString))
            Interaction.StatusIcon.Text = If(Status.Length >= 64, Status.Substring(0, 63), Status)

            If Date.Compare(NextInQueue.NextRun, Date.Now) <= 0 Then
                ConfigHandler.LogAppEvent("Scheduler: Launching " & NextInQueue.Name)

                Dim SyncForm As New SynchronizeForm(NextInQueue.Name, False, True, NextInQueue.CatchUp)
                AddHandler SyncForm.SyncFinished, AddressOf ScheduledProfileCompleted
                ScheduledProfiles.RemoveAt(0)
                SyncForm.StartSynchronization(False)
            End If
        End If
    End Sub

    Private Needle As String
    Private Function EqualityPredicate(ByVal Item As SchedulerEntry) As Boolean
        Return (Item.Name = Needle)
    End Function

    'Logic of this function:
    ' A new entry is created. The need for catching up is calculated regardless of the current state of the list.
    ' Then, a corresponding entry (same name) is searched for. If not found, then the new entry is simply added to the list.
    ' OOH, if a corresponding entry is found, then
    '    If it's already late, or if changes would postpone it, then nothing happens.
    '    But if it's not late, and the change will bring the sync forward, then the new entry superseedes the previous one.
    '       Note: In the latter case, if current entry is marked as failed, then the next run time is loaded from it
    '             (that's to avoid infinite loops when eg. the backup medium is unplugged)
    Private Sub ReloadScheduledProfiles()
        ReloadProfiles() 'Needed! This allows to detect config changes.

        For Each Profile As KeyValuePair(Of String, ProfileHandler) In Profiles
            Dim Name As String = Profile.Key
            Dim Handler As ProfileHandler = Profile.Value
            Static OneDay As TimeSpan = New TimeSpan(1, 0, 0, 0)

            If Handler.Scheduler.Frequency <> ScheduleInfo.Freq.Never Then
                Dim NewEntry As New SchedulerEntry(Name, Handler.Scheduler.NextRun(), False, False)

                '<catchup>
                Dim LastRun As Date = Handler.GetLastRun()
                'LATER: Customizable time span?
                If Handler.GetSetting(Of Boolean)(ProfileSetting.CatchUpSync, False) And LastRun <> ScheduleInfo.DATE_NEVER And (NewEntry.NextRun - LastRun) > (Handler.Scheduler.GetInterval() + OneDay) Then
                    ConfigHandler.LogAppEvent("Scheduler: Profile " & Name & " was last executed on " & LastRun.ToString & ", marked for catching up.")
                    NewEntry.NextRun = ScheduleInfo.DATE_CATCHUP
                    NewEntry.CatchUp = True
                End If
                '</catchup>

                Needle = Name
                Dim ProfileIndex As Integer = ScheduledProfiles.FindIndex(New Predicate(Of SchedulerEntry)(AddressOf EqualityPredicate))
                If ProfileIndex <> -1 Then
                    Dim CurEntry As SchedulerEntry = ScheduledProfiles(ProfileIndex)

                    If NewEntry.NextRun <> CurEntry.NextRun And CurEntry.NextRun >= Date.Now() Then 'Don't postpone queued late backups
                        NewEntry.HasFailed = CurEntry.HasFailed
                        If CurEntry.HasFailed Then NewEntry.NextRun = CurEntry.NextRun

                        ScheduledProfiles.RemoveAt(ProfileIndex)
                        ScheduledProfiles.Add(NewEntry)
                        ConfigHandler.LogAppEvent("Scheduler: Re-registered profile for delayed run on " & NewEntry.NextRun.ToString & ": " & Name)
                    End If
                Else
                    ScheduledProfiles.Add(NewEntry)
                    ConfigHandler.LogAppEvent("Scheduler: Registered profile for delayed run on " & NewEntry.NextRun.ToString & ": " & Name)
                End If
            End If
        Next

        'Remove deleted or disabled profiles
        For ProfileIndex As Integer = ScheduledProfiles.Count - 1 To 0 Step -1
            If Not Profiles.ContainsKey(ScheduledProfiles(ProfileIndex).Name) OrElse Profiles(ScheduledProfiles(ProfileIndex).Name).Scheduler.Frequency = ScheduleInfo.Freq.Never Then
                ScheduledProfiles.RemoveAt(ProfileIndex)
            End If
        Next

        'Tracker #3000728
        ScheduledProfiles.Sort(Function(First As SchedulerEntry, Second As SchedulerEntry) First.NextRun.CompareTo(Second.NextRun))
    End Sub
#End Region

#If Debug And 0 Then
    Sub VariousTests()
        MessageBox.Show(Nothing = "")
        MessageBox.Show("" = Nothing)
        'MessageBox.Show(Nothing.ToString = "")
        'MessageBox.Show(Nothing.ToString = String.Empty)
        MessageBox.Show(CStr(Nothing) = "")
        MessageBox.Show(CStr(Nothing) = String.Empty)

        'MessageBox.Show(CBool(""))
        'If "" Then MessageBox.Show(""""" -> True")
        If Nothing Then MessageBox.Show("Nothing -> True")
        If Not Nothing Then MessageBox.Show("Nothing -> False")
        MessageBox.Show(CBool(Nothing))
        MessageBox.Show(CStr(Nothing))

        MessageBox.Show(CType("", String) = "")
        MessageBox.Show(CType(Nothing, String) = "")
    End Sub
#End If
End Class
