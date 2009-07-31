﻿'This file is part of Create Synchronicity.
'
'Create Synchronicity is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
'Create Synchronicity is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
'You should have received a copy of the GNU General Public License along with Create Synchronicity.  If not, see <http://www.gnu.org/licenses/>.
'Created by:	Clément Pit--Claudel.
'Web site:		http://synchronicity.sourceforge.net.

Public Class SynchronizeForm
    Dim Log As LogHandler
    Dim Handler As SettingsHandler

    Dim ValidFiles As New Dictionary(Of String, Boolean)
    Dim SyncingList As New Dictionary(Of SideOfSource, List(Of SyncingItem))

    Dim [STOP] As Boolean
    Dim Status_StartTime As Date
    Dim Status_BytesCopied As Long
    Dim Status_FilesScanned As Long
    Dim Status_ActionsDone As Integer
    Dim Status_CreatedFiles As Integer
    Dim Status_CreatedFolders As Integer
    Dim Status_TotalActionsCount As Integer
    Dim Status_CurrentStep As Integer
    Dim Status_TimeElapsed As TimeSpan
    Dim Status_MillisecondsSpeed As Double
    Dim Status_BytesToCreate As Long

    Dim DisplayPreview As Boolean, PreviewFinished As Boolean

    Dim FullSyncThread As Threading.Thread
    Dim FirstSyncThread As Threading.Thread
    Dim SecondSyncThread As Threading.Thread

    Delegate Sub UpdateListCallBack()
    Delegate Sub LaunchTimerCallBack()
    Delegate Sub TaskDoneCallBack(ByVal Id As Integer)
    Delegate Sub LabelCallBack(ByVal Id As Integer, ByVal Text As String)
    Delegate Sub SetElapsedTimeCallBack(ByVal CurrentTimeSpan As TimeSpan)
    Delegate Sub ProgressSetMaxCallBack(ByVal Id As Integer, ByVal Max As Integer)
    Delegate Sub SetProgessCallBack(ByVal Id As Integer, ByVal Progress As Integer)

#Region " Events "
    Sub New(ByVal ConfigName As String, ByVal _DisplayPreview As Boolean)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        [STOP] = False

        DisplayPreview = _DisplayPreview
        PreviewFinished = Not DisplayPreview

        SyncBtn.Enabled = False
        SyncBtn.Visible = DisplayPreview

        Status_BytesCopied = 0
        Status_FilesScanned = 0
        Status_ActionsDone = 0
        Status_CreatedFiles = 0
        Status_CreatedFolders = 0
        Status_TotalActionsCount = 0
        Status_CurrentStep = 1

        Log = New LogHandler(ConfigName)
        Handler = New SettingsHandler(ConfigName)

        FullSyncThread = New Threading.Thread(AddressOf Synchronize)
        FirstSyncThread = New Threading.Thread(AddressOf Do_FirstStep)
        SecondSyncThread = New Threading.Thread(AddressOf Do_SecondThirdStep)

        Me.CreateHandle()

        If DisplayPreview Then
            PreviewList.Items.Clear()
            FirstSyncThread.Start()
        Else
            FullSyncThread.Start()
        End If
    End Sub

    Private Sub SynchronizeForm_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        EndAll()
    End Sub

    Private Sub CancelBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StopBtn.Click
        Select Case StopBtn.Text
            Case StopBtn.Tag.ToString.Split(";"c)(0)
                EndAll()
            Case StopBtn.Tag.ToString.Split(";"c)(1)
                Close()
        End Select
    End Sub

    Private Sub SyncBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SyncBtn.Click
        PreviewList.Visible = False
        SyncBtn.Visible = False
        StopBtn.Text = StopBtn.Tag.Split(";"c)(0)

        SecondSyncThread.Start()
    End Sub

    Private Sub SyncingTimeCounter_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SyncingTimeCounter.Tick
        UpdateStatuses()
    End Sub

    Sub UpdateStatuses()
        Status_TimeElapsed = DateTime.Now - Status_StartTime
        Status_MillisecondsSpeed = Status_BytesCopied / (If(Status_TimeElapsed.TotalMilliseconds = 0, New System.TimeSpan(1), Status_TimeElapsed).TotalMilliseconds / 1000)
        ElapsedTime.Text = If(Status_TimeElapsed.Hours = 0, "", Status_TimeElapsed.Hours.ToString & "h, ") & If(Status_TimeElapsed.Minutes = 0, "", Status_TimeElapsed.Minutes.ToString & "m, ") & Status_TimeElapsed.Seconds.ToString & "s."

        If Status_CurrentStep = 1 Then
            Speed.Text = Math.Round(Status_FilesScanned / (If(Status_TimeElapsed.TotalMilliseconds = 0, New System.TimeSpan(1), Status_TimeElapsed).TotalMilliseconds / 1000)).ToString & " files/s"
        Else
            Select Case Status_MillisecondsSpeed
                Case Is > 1024 * 1000 * 1000
                    Speed.Text = Math.Round(Status_MillisecondsSpeed / (1024 * 1000 * 1000), 2).ToString & "GB/s"
                Case Is > 1024 * 1000
                    Speed.Text = Math.Round(Status_MillisecondsSpeed / (1024 * 1000), 2).ToString & "MB/s"
                Case Is > 1024
                    Speed.Text = Math.Round(Status_MillisecondsSpeed / 1024, 2).ToString & "kB/s"
                Case Else
                    Speed.Text = Math.Round(Status_MillisecondsSpeed, 2).ToString & "B/s"
            End Select
        End If
        Done.Text = Status_ActionsDone : FilesCreated.Text = Status_CreatedFiles : FoldersCreated.Text = Status_CreatedFolders
    End Sub
#End Region

#Region " Processes interaction "
    Sub UpdateLabel(ByVal Id As Integer, ByVal Text As String)
        Select Case Id
            Case 1
                Step1StatusLabel.Text = Text
            Case 2
                Step2StatusLabel.Text = Text
            Case 3
                Step3StatusLabel.Text = Text
        End Select
    End Sub

    Sub SetProgess(ByVal Id As Integer, ByVal Progress As Integer)
        Select Case Id
            Case 1
                If Step1ProgressBar.Value + Progress < Step1ProgressBar.Maximum Then Step1ProgressBar.Value += Progress
            Case 2
                If Step2ProgressBar.Value + Progress < Step2ProgressBar.Maximum Then Step2ProgressBar.Value += Progress
            Case 3
                If Step3ProgressBar.Value + Progress < Step3ProgressBar.Maximum Then Step3ProgressBar.Value += Progress
        End Select
    End Sub

    Sub SetMaxProgess(ByVal Id As Integer, ByVal MaxValue As Integer)
        Select Case Id
            Case 1
                If MaxValue = -1 Then
                    Step1ProgressBar.Style = ProgressBarStyle.Marquee
                Else
                    Step1ProgressBar.Style = ProgressBarStyle.Blocks
                    Step1ProgressBar.Value = 0
                    Step1ProgressBar.Maximum = MaxValue
                End If
            Case 2
                If MaxValue = -1 Then
                    Step2ProgressBar.Style = ProgressBarStyle.Marquee
                Else
                    Step2ProgressBar.Style = ProgressBarStyle.Blocks
                    Step2ProgressBar.Value = 0
                    Step2ProgressBar.Maximum = MaxValue
                End If
            Case 3
                If MaxValue = -1 Then
                    Step3ProgressBar.Style = ProgressBarStyle.Marquee
                Else
                    Step3ProgressBar.Style = ProgressBarStyle.Blocks
                    Step3ProgressBar.Value = 0
                    Step3ProgressBar.Maximum = MaxValue
                End If
        End Select
    End Sub

    Sub TaskDone(ByVal Id As Integer)
        Select Case Id
            Case 1
                UpdateLabel(1, "Done !")
                Step1ProgressBar.Maximum = 100
                Step1ProgressBar.Value = Step1ProgressBar.Maximum
                Step1ProgressBar.Style = ProgressBarStyle.Blocks
                If Not PreviewFinished Then
                    UpdatePreviewList()
                    StopBtn.Text = StopBtn.Tag.ToString.Split(";"c)(1)
                End If
                SyncingTimeCounter.Stop()
                Status_CurrentStep = 2
                TotalCount.Text = SyncingList(SideOfSource.Left).Count + SyncingList(SideOfSource.Right).Count

            Case 2
                UpdateLabel(2, "Done !")
                Step2ProgressBar.Maximum = 100
                Step2ProgressBar.Value = Step2ProgressBar.Maximum
                Step2ProgressBar.Style = ProgressBarStyle.Blocks
                Status_CurrentStep = 3

            Case 3
                UpdateLabel(3, "Done !")
                Step3ProgressBar.Maximum = 100
                Step3ProgressBar.Value = Step3ProgressBar.Maximum
                Step3ProgressBar.Style = ProgressBarStyle.Blocks

                UpdateStatuses()
                If Log.Errors.Count > 0 Then
                    PreviewList.Visible = True
                    PreviewList.Items.Clear()
                    PreviewList.Columns.Clear()
                    PreviewList.Columns.Add("Error")
                    Dim ErrorColumn As ColumnHeader = PreviewList.Columns.Add("Error detail")

                    Dim ErrorsList As New List(Of Exception)(Log.Errors)
                    For Each Ex As Exception In ErrorsList
                        Dim ErrorItem As New ListViewItem(Ex.Source)
                        ErrorItem.SubItems.Add(Ex.Message)
                        PreviewList.Items.Add(ErrorItem)
                        ErrorItem.ImageIndex = 5
                    Next

                    PreviewList.Columns(0).AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent)
                    ErrorColumn.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent)
                End If

                Log.SaveAndDispose()
                SyncingTimeCounter.Stop()
                StopBtn.Text = StopBtn.Tag.ToString.Split(";"c)(1)
        End Select
    End Sub

    Sub UpdatePreviewList()
        PreviewList.Visible = True

        If Not PreviewList.Items.Count = 0 Then
            For Each C As ColumnHeader In PreviewList.Columns
                C.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent)
            Next
        End If

        PreviewFinished = True
        If Not [STOP] Then SyncBtn.Enabled = True
    End Sub

    Sub AddPreviewItem(ByRef Item As SyncingItem, ByVal Side As SideOfSource)
        Dim ListItem As New ListViewItem
        ListItem = PreviewList.Items.Add(Item.FormatType)

        ListItem.SubItems.Add(Item.FormatAction)
        Dim DirectionString As String = ""
        Select Case Side
            Case SideOfSource.Left
                DirectionString = "Left->Right"
            Case SideOfSource.Right
                DirectionString = "Right->Left"
        End Select
        ListItem.SubItems.Add(DirectionString)
        ListItem.SubItems.Add(Item.Path)

        Select Case Item.Action
            Case TypeOfAction.Create
                If Item.Type = TypeOfItem.Folder Then ListItem.ImageIndex = 3
                If Item.Type = TypeOfItem.File Then
                    Select Case Side
                        Case SideOfSource.Left
                            ListItem.ImageIndex = 0
                        Case SideOfSource.Right
                            ListItem.ImageIndex = 1
                    End Select
                End If
            Case TypeOfAction.Delete
                If Item.Type = TypeOfItem.Folder Then ListItem.ImageIndex = 4
                If Item.Type = TypeOfItem.File Then ListItem.ImageIndex = 2
        End Select
    End Sub

    Sub LaunchTimer()
        Status_BytesCopied = 0
        Status_StartTime = DateTime.Now
        SyncingTimeCounter.Start()
    End Sub

    Sub EndAll()
        [STOP] = True
        FullSyncThread.Abort()
        FirstSyncThread.Abort() : SecondSyncThread.Abort()
        TaskDone(1) : TaskDone(2) : TaskDone(3)
    End Sub
#End Region

#Region " Syncing code "
    Sub Synchronize()
        Do_FirstStep()
        Do_SecondThirdStep()
    End Sub

    Sub Do_FirstStep()
        Dim Context As New SyncingAction
        Dim TaskDoneDelegate As New TaskDoneCallBack(AddressOf TaskDone)
        Dim Left As String = Handler.GetSetting(ConfigOptions.Source)
        Dim Right As String = Handler.GetSetting(ConfigOptions.Destination)

        SyncingList.Clear()
        SyncingList.Add(SideOfSource.Left, New List(Of SyncingItem))
        SyncingList.Add(SideOfSource.Right, New List(Of SyncingItem))

        ValidFiles.Clear()

        Me.Invoke(New LaunchTimerCallBack(AddressOf LaunchTimer))
        Context.Source = SideOfSource.Left
        Context.SourcePath = Handler.GetSetting(ConfigOptions.Source)
        Context.DestinationPath = Handler.GetSetting(ConfigOptions.Destination)
        Context.Action = TypeOfAction.Create
        Init_Synchronization(Handler.LeftCheckedNodes, Context)

        Context.Source = SideOfSource.Right
        Context.SourcePath = Handler.GetSetting(ConfigOptions.Destination)
        Context.DestinationPath = Handler.GetSetting(ConfigOptions.Source)
        Select Case Handler.GetSetting(ConfigOptions.Method)
            Case "0"
                Context.Action = TypeOfAction.Delete
                Init_Synchronization(Handler.RightCheckedNodes, Context)
            Case "2"
                Context.Action = TypeOfAction.Create
                Init_Synchronization(Handler.RightCheckedNodes, Context)
        End Select
        Me.Invoke(TaskDoneDelegate, 1)
    End Sub

    Sub Do_SecondThirdStep()
        Dim TaskDoneDelegate As New TaskDoneCallBack(AddressOf TaskDone)
        Dim SetProgessDelegate As New SetProgessCallBack(AddressOf SetProgess)
        Dim ProgessSetMaxCallBack As New ProgressSetMaxCallBack(AddressOf SetMaxProgess)
        Dim LabelDelegate As New LabelCallBack(AddressOf UpdateLabel)

        Dim Left As String = Handler.GetSetting(ConfigOptions.Source)
        Dim Right As String = Handler.GetSetting(ConfigOptions.Destination)

        Me.Invoke(New LaunchTimerCallBack(AddressOf LaunchTimer))
        Me.Invoke(ProgessSetMaxCallBack, New Object() {2, SyncingList(SideOfSource.Left).Count})
        Do_Task(SyncingList(SideOfSource.Left), Left, Right, 2)
        Me.Invoke(TaskDoneDelegate, 2)

        Me.Invoke(ProgessSetMaxCallBack, New Object() {3, SyncingList(SideOfSource.Right).Count})
        Do_Task(SyncingList(SideOfSource.Right), Right, Left, 3)
        Me.Invoke(TaskDoneDelegate, 3)
    End Sub

    Sub Do_Task(ByRef ListOfActions As List(Of SyncingItem), ByVal Source As String, ByVal Destination As String, ByVal CurrentStep As Integer)
        Dim SetProgessDelegate As New SetProgessCallBack(AddressOf SetProgess)
        Dim LabelDelegate As New LabelCallBack(AddressOf UpdateLabel)

        For Each Entry As SyncingItem In ListOfActions
            Try
                Me.Invoke(LabelDelegate, New Object() {CurrentStep, Destination & Entry.Path})

                Select Case Entry.Type
                    Case TypeOfItem.File
                        Select Case Entry.Action
                            Case TypeOfAction.Create
                                CopyFile(Entry.Path, Source, Destination)
                            Case TypeOfAction.Delete
                                IO.File.SetAttributes(Source & Entry.Path, IO.FileAttributes.Normal)
                                IO.File.Delete(Source & Entry.Path)
                        End Select

                    Case TypeOfItem.Folder
                        Select Case Entry.Action
                            Case TypeOfAction.Create
                                IO.Directory.CreateDirectory(Destination & Entry.Path)
                                Status_CreatedFolders += 1
                            Case TypeOfAction.Delete
                                If IO.Directory.GetFiles(Source & Entry.Path).GetLength(0) = 0 Then IO.Directory.Delete(Source & Entry.Path)
                        End Select
                End Select
                Status_ActionsDone += 1
                Log.LogAction(Entry, True)

            Catch StopEx As System.Threading.ThreadAbortException

            Catch ex As Exception
                Log.HandleError(ex)
                Log.LogAction(Entry, False)

            Finally
                If Not [STOP] Then Me.Invoke(SetProgessDelegate, New Object() {CurrentStep, 1})
            End Try
        Next
    End Sub


    Sub Init_Synchronization(ByRef FoldersList As Dictionary(Of String, Boolean), ByVal Context As SyncingAction)
        For Each Folder As String In FoldersList.Keys
            If Context.Action = TypeOfAction.Create Then
                SearchForChanges(Folder, FoldersList(Folder), Context)
            ElseIf Context.Action = TypeOfAction.Delete Then
                SearchForCrap(Folder, FoldersList(Folder), Context)
            End If
        Next
    End Sub

    Sub AddToSyncingList(ByVal Side As SideOfSource, ByRef Entry As SyncingItem)
        SyncingList(Side).Add(Entry)
        SyncPreviewList(Side, 1)
        If Entry.Action <> TypeOfAction.Delete Then AddValidFile(Entry.Path)
    End Sub

    Sub AddValidFile(ByVal File As String)
        If Not ValidFiles.ContainsKey(File) Then ValidFiles.Add(File, Nothing)
    End Sub

    Sub RemoveValidFile(ByVal File As String)
        If ValidFiles.ContainsKey(File) Then ValidFiles.Remove(File)
    End Sub

    Sub RemoveFromSyncingList(ByVal Side As SideOfSource)
        ValidFiles.Remove(SyncingList(Side)(SyncingList(Side).Count - 1).Path)
        SyncingList(Side).RemoveAt(SyncingList(Side).Count - 1)
        SyncPreviewList(Side, -1)
    End Sub

    Function CombinePathes(ByVal Dir As String, ByVal File As String) As String
        Return If(Dir.EndsWith(IO.Path.DirectorySeparatorChar), Dir, Dir & IO.Path.DirectorySeparatorChar) & If(File.StartsWith(IO.Path.DirectorySeparatorChar), File.Substring(1), File)
    End Function


    ' This procedure searches for changes in the source directory, in regards
    ' to the status of the destination directory.
    Sub SearchForChanges(ByVal Folder As String, ByVal Recursive As Boolean, ByVal Context As SyncingAction)
        Dim LabelDelegate As New LabelCallBack(AddressOf UpdateLabel)

        Dim Src_FilePath As String = CombinePathes(Context.SourcePath, Folder)
        Dim Dest_FilePath As String = CombinePathes(Context.DestinationPath, Folder)
        Me.Invoke(LabelDelegate, New Object() {1, Src_FilePath})

        Dim PropagateUpdates As Boolean = (Handler.GetSetting(ConfigOptions.PropagateUpdates, "True") = "True")
        Dim EmptyDirectories As Boolean = Handler.GetSetting(ConfigOptions.ReplicateEmptyDirectories, "False") = "True"

        Dim InitialCount As Integer
        Dim IsSingularity As Boolean
        IsSingularity = Not IO.Directory.Exists(Dest_FilePath)

        If IsSingularity Then
            AddToSyncingList(Context.Source, New SyncingItem(Folder, TypeOfItem.Folder, Context.Action))
        Else
            AddValidFile(Folder)
        End If

        InitialCount = ValidFiles.Count

        Try
            For Each SourceFile As String In IO.Directory.GetFiles(Src_FilePath)
                Dim DestinationFile As String = CombinePathes(Dest_FilePath, IO.Path.GetFileName(SourceFile))

                'First check if the file is part of the synchronization profile.
                'Then, check whether it requires updating.
                If HasValidExtension(SourceFile) Then

                    If Not IO.File.Exists(DestinationFile) OrElse (PropagateUpdates AndAlso SourceIsMoreRecent(SourceFile, DestinationFile)) Then
                        AddToSyncingList(Context.Source, New SyncingItem(SourceFile.Substring(Context.SourcePath.Length), TypeOfItem.File, Context.Action))
                    Else
                        'Adds an entry to not delete this when cleaning up the other side.
                        AddValidFile(SourceFile.Substring(Context.SourcePath.Length))
                    End If

                End If

                Status_FilesScanned += 1
            Next
        Catch Ex As Exception
            'Error with entering the folder
        End Try

        If Recursive Then
            Try
                For Each SubFolder As String In IO.Directory.GetDirectories(Src_FilePath)
                    SearchForChanges(SubFolder.Substring(Context.SourcePath.Length), True, Context)
                Next
            Catch Ex As Exception

            End Try
        End If

        If InitialCount = ValidFiles.Count Then
            If Not EmptyDirectories Then
                If IsSingularity Then RemoveFromSyncingList(Context.Source)
                RemoveValidFile(Folder)
            End If
        End If
    End Sub

    Sub SearchForCrap(ByVal Folder As String, ByVal Recursive As Boolean, ByVal Context As SyncingAction)
        Dim LabelDelegate As New LabelCallBack(AddressOf UpdateLabel)

        Dim Src_FilePath As String = CombinePathes(Context.SourcePath, Folder)
        Dim Dest_FilePath As String = CombinePathes(Context.DestinationPath, Folder)
        Me.Invoke(LabelDelegate, New Object() {1, Src_FilePath})

        'Dim PropagateUpdates As Boolean = (Handler.GetSetting(ConfigOptions.PropagateUpdates, "True") = "True")
        'Dim EmptyDirectories As Boolean = Handler.GetSetting(ConfigOptions.ReplicateEmptyDirectories, "False") = "True"

        Try
            For Each File As String In IO.Directory.GetFiles(Src_FilePath)
                Dim RelativeFName As String = File.Substring(Context.SourcePath.Length)
                If Not ValidFiles.ContainsKey(RelativeFName) Then
                    AddToSyncingList(Context.Source, New SyncingItem(RelativeFName, TypeOfItem.File, Context.Action))
                End If
            Next
        Catch Ex As Exception
        End Try

        If Recursive Then
            Try
                For Each SubFolder As String In IO.Directory.GetDirectories(Src_FilePath)
                    SearchForCrap(SubFolder.Substring(Context.SourcePath.Length), True, Context)
                Next
            Catch Ex As Exception
            End Try
        End If

        If Folder <> "" AndAlso Not ValidFiles.ContainsKey(Folder) Then
            AddToSyncingList(Context.Source, New SyncingItem(Folder, TypeOfItem.Folder, Context.Action))
        End If
    End Sub

#If 0 Then
    Sub BuildList(ByVal Folder As String, ByVal Recursive As Boolean, ByVal Context As SyncingAction) ' As Boolean 'Returns whether the directory (or its subdirectories) contains files.
        Dim LabelDelegate As New LabelCallBack(AddressOf UpdateLabel)

        If Not Folder.StartsWith("\") Then Folder = "\" & Folder
        Dim AbsolutePath As String = Context.SourcePath & Folder

        Me.Invoke(LabelDelegate, New Object() {1, AbsolutePath})

        Try
            Dim IsSingularity As Boolean = Not IO.Directory.Exists(Context.DestinationPath & Folder)
            If IsSingularity And Not Context.Action = TypeOfAction.Delete Then
                SyncingList(Context.Source).Add(New SyncingItem(Folder, TypeOfItem.Folder, Context.Action))
                SyncPreviewList(Context.Source, 1)
            End If

            For Each File As String In IO.Directory.GetFiles(AbsolutePath)
                Dim SourceFile As String = File
                Dim DestinationFile As String = Context.DestinationPath & Folder & "\" & GetFileOrFolderName(File)
                'Needs to be more efficient.
                'Status_BytesCopied += My.Computer.FileSystem.GetFileInfo(SourceFile).Length
                Status_FilesScanned += 1

                If Not HasValidExtension(File) Then Continue For
                If IO.File.Exists(DestinationFile) AndAlso (Not FileHasBeenUpdated(SourceFile, DestinationFile) Or Context.Action = TypeOfAction.Delete) Then Continue For

                SyncingList(Context.Source).Add(New SyncingItem(File.Substring(Context.SourcePath.Length), TypeOfItem.File, Context.Action))
                SyncPreviewList(Context.Source, 1)
            Next

            If Recursive Then
                For Each SubFolder As String In IO.Directory.GetDirectories(AbsolutePath)
                    BuildList(SubFolder.Substring(Context.SourcePath.Length), True, Context)
                Next
            End If

            'LOTS OF TESTING NEEDED...
            'If the action is not a deletion
            '   create the directory, unless, in case we don't want empty directories, it results in an empty dir being created
            'Otherwise
            '   If it is not present on the other side, delete the dir.
            '   If it is, and it is empty on the other side, delete unless replicate_empty_directories.

            Dim EmptyDirectoryReplication As Boolean = Handler.GetSetting(ConfigOptions.ReplicateEmptyDirectories, "False") = "True"

            If Not Context.Action = TypeOfAction.Delete Then
                If Not EmptyDirectoryReplication AndAlso SyncingList(Context.Source)(SyncingList(Context.Source).Count - 1).Path = Folder Then
                    SyncingList(Context.Source).RemoveAt(SyncingList(Context.Source).Count - 1)
                    SyncPreviewList(Context.Source, -1)
                End If
            Else
                If IsSingularity OrElse ((Not EmptyDirectoryReplication) AndAlso IO.Directory.GetFiles(Context.DestinationPath & Folder).Length + IO.Directory.GetDirectories(Context.DestinationPath & Folder).Length = 0) Then
                    SyncingList(Context.Source).Add(New SyncingItem(Folder, TypeOfItem.Folder, Context.Action))
                    SyncPreviewList(Context.Source, 1)
                End If
            End If
        Catch Ex As Exception

        End Try
    End Sub
#End If

    Sub SyncPreviewList(ByVal Side As SideOfSource, ByVal Count As Integer)
        If Count > 0 Then
            AddPreviewItem(SyncingList(Side)(SyncingList(Side).Count - 1), Side)
        ElseIf Count < 0 Then
            PreviewList.Items.RemoveAt(PreviewList.Items.Count - 1)
        End If
    End Sub

    Function HasValidExtension(ByVal Path As String) As Boolean
        Try
            Select Case Handler.GetSetting(ConfigOptions.Restrictions)
                Case "1"
                    Return InArray(GetExtension(Path), Handler.GetSetting(ConfigOptions.IncludedTypes).Split(";"c))
                Case "2"
                    Return Not InArray(GetExtension(Path), Handler.GetSetting(ConfigOptions.ExcludedTypes).Split(";"c))
            End Select
        Catch Ex As Exception
        End Try
        Return True
    End Function

    Sub CopyFile(ByVal Path As String, ByVal Source As String, ByVal Dest As String)
        IO.File.Copy(Source & Path, Dest & Path, True)
        IO.File.SetAttributes(Dest & Path, IO.File.GetAttributes(Source & Path))
        Status_CreatedFiles += 1
        Status_BytesCopied += My.Computer.FileSystem.GetFileInfo(Source & Path).Length
    End Sub
#End Region

#Region " Functions "
    Function GetFileOrFolderName(ByVal Path As String) As String
        Return Path.Substring(Path.LastIndexOf("\") + 1)
    End Function

    Function GetExtension(ByVal Path As String) As String
        Return Path.Substring(Path.LastIndexOf(".") + 1)
    End Function

    Function InArray(ByVal Str As String, ByRef ListObject As String()) As Boolean
        For Each SubStr As String In ListObject
            If Str = SubStr Then Return True
        Next
        Return False
    End Function

    Function ComputeFileHash(ByVal Path As String) As String
        Dim CryptObject As New System.Security.Cryptography.MD5CryptoServiceProvider()
        Return Convert.ToBase64String(CryptObject.ComputeHash((New IO.StreamReader(Path)).BaseStream))
    End Function

    Function SourceIsMoreRecent(ByVal Source As String, ByVal Destination As String)
        If Handler.GetSetting(ConfigOptions.PropagateUpdates, "True") = "False" Then Return False
        If IO.File.GetLastWriteTime(Source) <= IO.File.GetLastWriteTime(Destination) Then Return False

        'TODO: Do no check if transfering from NTFS to NTFS: 
        'Stupid fat filesystem...
        If NTFSToFATTime(IO.File.GetLastWriteTime(Source)) <= IO.File.GetLastWriteTime(Destination) Then Return False
        If NTFSToFATTime(IO.File.GetLastWriteTime(Destination)) >= IO.File.GetLastWriteTime(Source) Then Return False

        'More violent version
        'If ToEvenSeconds(IO.File.GetLastWriteTime(Source)) = ToEvenSeconds(IO.File.GetLastWriteTime(Destination)) Then Return False

        If Handler.GetSetting(ConfigOptions.ComputeHash, "False") Then
            Return Not (ComputeFileHash(Source) = ComputeFileHash(Destination))
        Else
            Return True
        End If
    End Function

    Function ToEvenSeconds(ByVal T As Date) As Long
        Dim TotalSeconds As Decimal = T.Ticks / 100000000
        Return 2 * Math.Round(TotalSeconds / 2)
    End Function

    Function NTFSToFATTime(ByVal NTFSTime As Date)
        Return New Date(NTFSTime.Year, NTFSTime.Month, NTFSTime.Day, NTFSTime.Hour, NTFSTime.Minute, NTFSTime.Second + If(NTFSTime.Second Mod 2 = 0, If(NTFSTime.Millisecond = 0, 0, 2), 1))
    End Function
#End Region

    ' This code won't be compiled.
    'Private Sub PreviewList_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PreviewList.DoubleClick
    '    If PreviewList.SelectedIndices.Count = 0 Then Exit Sub
    '    Dim Index As Integer = PreviewList.SelectedIndices(0)
    '    Dim CopyFromLeft As Boolean = (PreviewList.SelectedItems(0).Text = "Left->Right") Xor PreviewList.SelectedItems(0).SubItems(0).Text = "Delete"
    '    System.Diagnostics.Process.Start(If(CopyFromLeft, Handler.GetSetting(ConfigOptions.Source), Handler.GetSetting(ConfigOptions.Source) & SyncingList(
    'End Sub
End Class