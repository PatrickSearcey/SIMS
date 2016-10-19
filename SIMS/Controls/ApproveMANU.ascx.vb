Imports Telerik.Web.UI
Imports System.Data.SqlClient
Imports System.Web.HttpContext

Public Class ApproveMANU
    Inherits System.Web.UI.UserControl

    Private s As Site
    Private er As ElementReport
    Private _dataItem As Object = Nothing
    Private _site_id As Integer = Nothing
    Private _showclosebutton As Boolean = True
    Private uid As String = Current.User.Identity.Name
    Private u As New User(uid)
    Public Delegate Sub ApproveButtonClickEventHandler(sender As Object, e As CommandEventArgs)
    Public Event SubmitEvent As ApproveButtonClickEventHandler

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim site_id As Object = DataBinder.Eval(DataItem, "site_id")
        If Not site_id = Nothing Then
            Session.Clear()
            Session("site_id") = site_id
            SiteID = site_id
        ElseIf Not SiteID = Nothing Then
            Session.Clear()
            Session("site_id") = SiteID
        End If

        If Not ShowCloseButton Then
            btnCancel.Visible = False
        End If

        RefreshMANU()
    End Sub

#Region "Properties"
    Public Property DataItem() As Object
        Get
            Return Me._dataItem
        End Get
        Set(ByVal value As Object)
            Me._dataItem = value
        End Set
    End Property

    Public Property SiteID() As Integer
        Get
            Return Me._site_id
        End Get
        Set(value As Integer)
            Me._site_id = value
        End Set
    End Property

    Public Property ShowCloseButton() As Boolean
        Get
            Return Me._showclosebutton
        End Get
        Set(value As Boolean)
            Me._showclosebutton = value
        End Set
    End Property
#End Region

#Region "Public Methods"
    Public Sub RefreshMANU()
        If Not SiteID = Nothing Then
            s = New Site(SiteID)
            s.SetElemReportIDs(SiteID)
        Else
            s = New Site(Session("site_id"))
            s.SetElemReportIDs(Session("site_id"))
        End If
        
        er = New ElementReport(s.MANUSummaryID)

        'u.AccessLevel = "None" Or 
        If u.AccessLevel = "None" Or er.NeedsApproval = "NO" Then
            lblComments.Visible = False
            rtbComments.Visible = False
            btnApprove.Visible = False
        Else
            lblComments.Visible = True
            rtbComments.Visible = True
            btnApprove.Visible = True
        End If

        rlvElem.DataSource = Nothing
        rlvElem.Rebind()
    End Sub
#End Region
    
#Region "RadListView Events"
    Protected Sub rlvElem_NeedDataSource(ByVal source As Object, ByVal e As RadListViewNeedDataSourceEventArgs)
        rlvElem.DataSource = er.GetAllElementsByReport(s.Number, s.AgencyCode, "MANU")
    End Sub

    Protected Sub rlvElem_ItemDataBound(ByVal sender As Object, ByVal e As RadListViewItemEventArgs)
        If TypeOf e.Item Is RadListViewDataItem Then
            Dim item As RadListViewDataItem = DirectCast(e.Item, RadListViewDataItem)

            Dim revised_dt As String
            Try
                revised_dt = DirectCast(item.DataItem, DataRowView)("revised_dt")
                revised_dt = revised_dt.Substring(0, InStr(revised_dt, " "))
            Catch ex As Exception
                revised_dt = "N/A"
            End Try

            Dim revised_by As String = DirectCast(item.DataItem, DataRowView)("revised_by").ToString()

            If String.IsNullOrEmpty(revised_by) Then
                revised_by = "N/A"
            End If
            If String.IsNullOrEmpty(revised_dt) Then
                revised_dt = "N/A"
            End If

            Dim element_nm As String = DirectCast(item.DataItem, DataRowView)("element_nm").ToString()
            Dim element_id As String = DirectCast(item.DataItem, DataRowView)("element_id").ToString()

            Dim lblRevisedBy As Label = DirectCast(item.FindControl("lblRevisedBy"), Label)
            lblRevisedBy.Text = revised_by

            Dim lblRevisedDt As Label = DirectCast(item.FindControl("lblRevisedDt"), Label)
            lblRevisedDt.Text = revised_dt

            'Create the Revision History link
            Dim hlRevisionHistory As HyperLink = DirectCast(item.FindControl("hlRevisionHistory"), HyperLink)
            hlRevisionHistory.NavigateUrl = "~/Archives.aspx?element_nm=" & element_nm & "&element_id=" & element_id & "&site_id=" & s.ID & "&office_id=" & s.OfficeID

            'Hide the EXTREMES FOR CURRENT YEAR element by setting panel visibility to false
            'Change panel background color if FOOTNOTES element and add icon with tooltip
            Dim pnlElement As Panel = DirectCast(item.FindControl("pnlElement"), Panel)
            Dim ibIcon As Image = DirectCast(item.FindControl("ibIcon"), Image)

            'Don't show EXTREMES FOR CURRENT YEAR or PEAK DISCHARGES FOR CURRENT YEAR elements in MANU
            If InStr(element_nm, "EXTREMES FOR CURRENT YEAR") > 0 Or InStr(element_nm, "PEAK DISCHARGES FOR CURRENT YEAR") Then
                pnlElement.Visible = False
            Else
                pnlElement.Visible = True
            End If

            If InStr(element_nm, "FOOTNOTE") > 0 Then
                pnlElement.CssClass = "footnoteElemAlert"
                ibIcon.Visible = True
                ibIcon.ImageUrl = "../Images/nofootnoteicon.png"
            ElseIf InStr(element_nm, "REMARKS") > 0 Or InStr(element_nm, "WELL CHARACTERISTICS") > 0 Or InStr(element_nm, "DATUM") > 0 Then
                pnlElement.CssClass = "remarksElemAlert"
                ibIcon.Visible = True
                ibIcon.ImageUrl = "../Images/cleanicon.png"
            Else
                pnlElement.CssClass = ""
                ibIcon.Visible = False
            End If

            'For modal pop-ups to work
            Select Case element_nm
                Case "WELL CHARACTERISTICS"
                    rwWC.OpenerElementID = ibIcon.ClientID
                    Exit Select
                Case "DATUM"
                    rwDatum.OpenerElementID = ibIcon.ClientID
                    Exit Select
                Case "FOOTNOTES"
                    rwFootnotes.OpenerElementID = ibIcon.ClientID
                    Exit Select
                Case "FOOTNOTES (WQ)"
                    rwFootnotesWQ.OpenerElementID = ibIcon.ClientID
                    Exit Select
                Case "FOOTNOTES (CLIM)"
                    rwFootnotesCLIM.OpenerElementID = ibIcon.ClientID
                    Exit Select
                Case "FOOTNOTES (ECO)"
                    rwFootnotesECO.OpenerElementID = ibIcon.ClientID
                    Exit Select
                Case "REMARKS"
                    rwRemarks.OpenerElementID = ibIcon.ClientID
                    Exit Select
                Case "REMARKS (MANU WQ)"
                    rwRemarksWQ.OpenerElementID = ibIcon.ClientID
                    Exit Select
                Case "REMARKS (MANU CLIM)"
                    rwRemarksCLIM.OpenerElementID = ibIcon.ClientID
                    Exit Select
                Case "REMARKS (MANU ECO)"
                    rwRemarksECO.OpenerElementID = ibIcon.ClientID
                    Exit Select
            End Select
        End If
    End Sub
#End Region
    

    Protected Sub btnApprove_Command(sender As Object, e As CommandEventArgs) Handles btnApprove.Command
        Dim approved As Boolean = False
        approved = er.ApproveReport(u.ID, rtbComments.Text)

        If approved Then
            lblComments.Visible = False
            rtbComments.Text = ""
            rtbComments.Visible = False
            btnApprove.Visible = False

            RaiseEvent SubmitEvent(Me, e)
        Else
            ltlConfirm.Text = "<br /><span style='color:red;font-weight:bold;'>There was an error approving the MANU.</span>"
        End If
    End Sub

    Protected Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        Response.Redirect(Config.SitePath & "StationInfo.asp?site_id=" & s.ID, "_blank", "")
    End Sub

    Protected Sub ibFAQ_Click(sender As Object, e As EventArgs) Handles ibFAQ.Click
        Response.Redirect("https://collaboration.usgs.gov/wg/owi/specialprojects/SIMS/Shared%20Documents/PADRE/FAQ.html", "_blank", "")
    End Sub

    Protected Sub ibRefresh_Command(sender As Object, e As CommandEventArgs) Handles ibRefresh.Command
        rlvElem.Rebind()

        RaiseEvent SubmitEvent(Me, e)
    End Sub
End Class