Imports Microsoft.VisualBasic
Imports Telerik.Web.UI
Imports System.Data
Imports System.Net

Public Class GridViewTemplate
    Implements ITemplate

    Protected label As Label
    Protected hyperlink As HyperLink
    Protected imagebutton As ImageButton
    Protected image As Image
    Private colname As String
    Private user_id As String
    Private listtype As String
    Private src As String

    Public Sub New(ByVal cName As String, ByVal userID As String, ByVal list_type As String)
        colname = cName
        user_id = userID
        If InStr(list_type, "CRP") Then
            listtype = Replace(list_type, "CRP", "")
            src = "CRP"
        Else
            listtype = list_type
            src = Nothing
        End If
    End Sub

    Public Sub InstantiateIn(ByVal container As System.Web.UI.Control) Implements ITemplate.InstantiateIn

        Select Case colname
            Case "lcks"
                image = New Image
                image.ID = "templateColumnImage"
                AddHandler image.DataBinding, AddressOf image_DataBinding

                container.Controls.Add(image)
            Case "select", "record-type"
                hyperlink = New HyperLink
                hyperlink.ID = "hl" & listtype
                AddHandler hyperlink.DataBinding, AddressOf hyperlink_DataBinding

                container.Controls.Add(hyperlink)
            Case "site no"
                hyperlink = New HyperLink
                hyperlink.ID = "ibSiteNo"
                AddHandler hyperlink.DataBinding, AddressOf hyperlinkImage_DataBinding

                container.Controls.Add(hyperlink)
        End Select

    End Sub

    Sub label_DataBinding(ByVal sender As Object, ByVal e As EventArgs)
        Dim l As Label = DirectCast(sender, Label)
        Dim container As GridViewRow = DirectCast(l.NamingContainer, GridViewRow)
        Dim site_no As String = DataBinder.Eval(container.DataItem, "site_no").ToString()

        l.Text = site_no & " "
    End Sub

    Sub hyperlinkImage_DataBinding(ByVal sender As Object, ByVal e As EventArgs)
        Dim hl As HyperLink = DirectCast(sender, HyperLink)
        Dim container As GridViewRow = DirectCast(hl.NamingContainer, GridViewRow)
        Dim site_id As Integer = DataBinder.Eval(container.DataItem, "site_id")

        Dim site As New Site(site_id)
        Dim office_id As Integer = site.OfficeID

        hl.NavigateUrl = Config.SitePath & "StationInfo.asp?alledits=yes&office_id=" & office_id & "&site_id=" & site_id
        hl.Text = site.Number
        hl.ToolTip = "Go to the Station Information page for this site"
        hl.Target = "_blank"
    End Sub

    Sub hyperlink_DataBinding(ByVal sender As Object, ByVal e As EventArgs)
        Dim h As HyperLink = DirectCast(sender, HyperLink)
        Dim container As GridViewRow = DirectCast(h.NamingContainer, GridViewRow)
        Dim rms_record_id As Integer = DataBinder.Eval(container.DataItem, "rms_record_id")

        Dim record As New Record(rms_record_id)
        Dim office_id As Integer = record.AltOfficeID
        Dim src_str As String = ""

        If src IsNot Nothing Then
            src_str = "&src=CRP"
        End If

        h.NavigateUrl = Config.SitePath & "processrec.asp?tasktype=" & listtype & "rec&rms_record_id=" & _
            rms_record_id & "&office_id=" & office_id & src_str

        Select Case colname
            Case "select"
                h.Text = "select"
            Case "record-type"
                If Len(record.TypeDS) > 21 Then
                    h.Text = Mid(record.TypeDS, 1, 21) & "..."
                Else
                    h.Text = record.TypeDS
                End If
                h.ToolTip = record.TypeDS
            Case Else
                h.Text = "select"
        End Select

    End Sub

    Sub image_DataBinding(ByVal sender As Object, ByVal e As EventArgs)
        Dim i As Image = DirectCast(sender, Image)
        Dim container As GridViewRow = DirectCast(i.NamingContainer, GridViewRow)
        Dim lock_type As String
        Dim lock_uid As String
        Dim lock_dt As String
        Dim rms_record_id As Integer

        If DataBinder.Eval(container.DataItem, "lock_type").ToString Is DBNull.Value Then
            lock_type = ""
        Else
            lock_type = DataBinder.Eval(container.DataItem, "lock_type").ToString
        End If

        If DataBinder.Eval(container.DataItem, "lock_uid").ToString Is DBNull.Value Then
            lock_uid = ""
        Else
            lock_uid = DataBinder.Eval(container.DataItem, "lock_uid").ToString
        End If

        If DataBinder.Eval(container.DataItem, "lock_dt").ToString Is DBNull.Value Then
            lock_dt = ""
        Else
            lock_dt = DataBinder.Eval(container.DataItem, "lock_dt").ToString
        End If

        rms_record_id = DataBinder.Eval(container.DataItem, "rms_record_id").ToString

        Dim record As New Record(rms_record_id)
        Dim imageURL As String = record.GetLockImageProperties("imageURL", user_id, listtype, lock_type, lock_uid, lock_dt)
        Dim imageAltTag As String = record.GetLockImageProperties("imageAltTag", user_id, listtype, lock_type, lock_uid, lock_dt)

        If imageURL = Nothing Then
            i.ImageUrl = "images/spacer.gif"
            i.AlternateText = "no locks"
            i.Width = 1
            i.Height = 1
        Else
            i.ImageUrl = imageURL
            i.AlternateText = imageAltTag
        End If

    End Sub

End Class
