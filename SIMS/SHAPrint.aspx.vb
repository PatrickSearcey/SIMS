Imports System.Data
Imports System.Data.SqlClient

Public Class SHAPrint
    Inherits System.Web.UI.Page

    Private s As Site
    Private o As Office

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            Dim site_id As Integer = Convert.ToInt32(Request.QueryString("site_id"))
            Dim site_no As String = Request.QueryString("site_no")

            If Not String.IsNullOrEmpty(site_no) Then
                s = New Site(site_no, "USGS")
            Else
                s = New Site(site_id)
            End If

            o = New Office(s.OfficeID)

            pnlNoSite.Visible = False
            pnlSHA.Visible = True

            'Fills out the header information
            PopulateHeader()
            'Fill the page with data
            PopulateReport()

        Catch ex As Exception
            pnlNoSite.Visible = True
            pnlSHA.Visible = False
        End Try
    End Sub

#Region "Properties"
    Private ReadOnly Property DischargeMeasJHA() As DataTable
        Get
            Dim sql As String = "SELECT shsm.sha_site_id, ssj.site_jha_id, slej.elem_jha_id, slj.jha_id, slj.jha_description" & _
                " FROM SHA_Lut_ElemJHA AS slej INNER JOIN" & _
                " SHA_Lut_JHA AS slj ON slej.jha_id = slj.jha_id INNER JOIN" & _
                " SHA_Site_JHA AS ssj ON slej.elem_jha_id = ssj.elem_jha_id INNER JOIN" & _
                " SHA_Site_Master AS shsm ON ssj.sha_site_id = shsm.sha_site_id" & _
                " WHERE (shsm.site_id = " & s.ID & ") And (slej.element_id = " & Config.DischargeMeasElem.ToString() & ")"
            Dim adapter As New SqlDataAdapter(sql, Config.ConnectionInfo)
            Dim dt As New DataTable()
            adapter.Fill(dt)
            DischargeMeasJHA = dt
        End Get
    End Property

    Private ReadOnly Property QWMeasJHA() As DataTable
        Get
            Dim sql As String = "SELECT shsm.sha_site_id, ssj.site_jha_id, slej.elem_jha_id, slj.jha_id, slj.jha_description" & _
                " FROM SHA_Lut_ElemJHA AS slej INNER JOIN" & _
                " SHA_Lut_JHA AS slj ON slej.jha_id = slj.jha_id INNER JOIN" & _
                " SHA_Site_JHA AS ssj ON slej.elem_jha_id = ssj.elem_jha_id INNER JOIN" & _
                " SHA_Site_Master AS shsm ON ssj.sha_site_id = shsm.sha_site_id" & _
                " WHERE (shsm.site_id = " & s.ID & ") And (slej.element_id = " & Config.QWMeasElem.ToString() & ")"
            Dim adapter As New SqlDataAdapter(sql, Config.ConnectionInfo)
            Dim dt As New DataTable()
            adapter.Fill(dt)
            QWMeasJHA = dt
        End Get
    End Property

    Private ReadOnly Property GWMeasJHA() As DataTable
        Get
            Dim sql As String = "SELECT shsm.sha_site_id, ssj.site_jha_id, slej.elem_jha_id, slj.jha_id, slj.jha_description" & _
                " FROM SHA_Lut_ElemJHA AS slej INNER JOIN" & _
                " SHA_Lut_JHA AS slj ON slej.jha_id = slj.jha_id INNER JOIN" & _
                " SHA_Site_JHA AS ssj ON slej.elem_jha_id = ssj.elem_jha_id INNER JOIN" & _
                " SHA_Site_Master AS shsm ON ssj.sha_site_id = shsm.sha_site_id" & _
                " WHERE (shsm.site_id = " & s.ID & ") And (slej.element_id = " & Config.GWMeasElem.ToString() & ")"
            Dim adapter As New SqlDataAdapter(sql, Config.ConnectionInfo)
            Dim dt As New DataTable()
            adapter.Fill(dt)
            GWMeasJHA = dt
        End Get
    End Property

    Private ReadOnly Property LakeMeasJHA() As DataTable
        Get
            Dim sql As String = "SELECT shsm.sha_site_id, ssj.site_jha_id, slej.elem_jha_id, slj.jha_id, slj.jha_description" & _
                " FROM SHA_Lut_ElemJHA AS slej INNER JOIN" & _
                " SHA_Lut_JHA AS slj ON slej.jha_id = slj.jha_id INNER JOIN" & _
                " SHA_Site_JHA AS ssj ON slej.elem_jha_id = ssj.elem_jha_id INNER JOIN" & _
                " SHA_Site_Master AS shsm ON ssj.sha_site_id = shsm.sha_site_id" & _
                " WHERE (shsm.site_id = " & s.ID & ") And (slej.element_id = " & Config.LakeMeasElem.ToString() & ")"
            Dim adapter As New SqlDataAdapter(sql, Config.ConnectionInfo)
            Dim dt As New DataTable()
            adapter.Fill(dt)
            LakeMeasJHA = dt
        End Get
    End Property

    Private ReadOnly Property EcoMeasJHA() As DataTable
        Get
            Dim sql As String = "SELECT shsm.sha_site_id, ssj.site_jha_id, slej.elem_jha_id, slj.jha_id, slj.jha_description" & _
                " FROM SHA_Lut_ElemJHA AS slej INNER JOIN" & _
                " SHA_Lut_JHA AS slj ON slej.jha_id = slj.jha_id INNER JOIN" & _
                " SHA_Site_JHA AS ssj ON slej.elem_jha_id = ssj.elem_jha_id INNER JOIN" & _
                " SHA_Site_Master AS shsm ON ssj.sha_site_id = shsm.sha_site_id" & _
                " WHERE (shsm.site_id = " & s.ID & ") And (slej.element_id = " & Config.EcoMeasElem.ToString() & ")"
            Dim adapter As New SqlDataAdapter(sql, Config.ConnectionInfo)
            Dim dt As New DataTable()
            adapter.Fill(dt)
            EcoMeasJHA = dt
        End Get
    End Property

    Private ReadOnly Property AtmMeasJHA() As DataTable
        Get
            Dim sql As String = "SELECT shsm.sha_site_id, ssj.site_jha_id, slej.elem_jha_id, slj.jha_id, slj.jha_description" & _
                " FROM SHA_Lut_ElemJHA AS slej INNER JOIN" & _
                " SHA_Lut_JHA AS slj ON slej.jha_id = slj.jha_id INNER JOIN" & _
                " SHA_Site_JHA AS ssj ON slej.elem_jha_id = ssj.elem_jha_id INNER JOIN" & _
                " SHA_Site_Master AS shsm ON ssj.sha_site_id = shsm.sha_site_id" & _
                " WHERE (shsm.site_id = " & s.ID & ") And (slej.element_id = " & Config.AtmMeasElem.ToString() & ")"
            Dim adapter As New SqlDataAdapter(sql, Config.ConnectionInfo)
            Dim dt As New DataTable()
            adapter.Fill(dt)
            AtmMeasJHA = dt
        End Get
    End Property
#End Region

    Protected Sub PopulateHeader()
        ltlSiteNoName.Text = s.NumberName
        ltlDate.Text = DateTime.Now.ToString()
        ltlOfficeName.Text = o.Name
        ltlOfficeCity.Text = o.CityStateZip
        ltlOfficeAddress.Text = o.StreetAddress
        ltlOfficeNumber.Text = o.PhoneNo
    End Sub

    Protected Sub PopulateReport()
        lvServicingSiteSpecificCond.DataSource = s.GetServicingSiteSpecificConditions(s.ID)
        lvServicingSiteSpecificCond.DataBind()

        lvServicingSiteRecEquip.DataSource = s.GetRecommendedEquipment(s.ID)
        lvServicingSiteRecEquip.DataBind()

        Dim elements As New DataTable
        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim sql As String = "SELECT DISTINCT element_id FROM Elem_Site_Element WHERE site_id = " & s.ID
            Dim cmd As SqlCommand = New SqlCommand(sql, cnx)
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(elements)

            cnx.Close()
        End Using

        For Each row As DataRow In elements.Rows
            Dim element_id As String = row("element_id").ToString()
            Select Case element_id
                Case Config.DischargeMeasElem.ToString()
                    PopulateDischargeMeasJHA()
                Case Config.QWMeasElem.ToString()
                    PopulateQWMeasJHA()
                Case Config.GWMeasElem.ToString()
                    PopulateGWMeasJHA()
                Case Config.LakeMeasElem.ToString()
                    PopulateLakeMeasJHA()
                Case Config.EcoMeasElem.ToString()
                    PopulateEcoMeasJHA()
                Case Config.AtmMeasElem.ToString()
                    PopulateAtmMeasJHA()
            End Select
        Next

        Dim emerg_service As Boolean = s.EmergencyService

        If emerg_service Then
            ltlEmergService.Text = "911 emergency service is available at this site.<br />"
        Else
            ltlEmergService.Text = "911 emergency service is NOT available at this site.<br />"
        End If

        Dim cell_service As Boolean = s.CellService

        If cell_service Then
            ltlCellService.Text = "Cell service is available at this site.<br /><br />"
        Else
            ltlCellService.Text = "Cell service is NOT available at this site.<br /><br />"
        End If

        lvEmergContacts.DataSource = s.GetSiteContacts(s.ID)
        lvEmergContacts.DataBind()

        lvHospitals.DataSource = s.GetSiteHospitals(s.ID)
        lvHospitals.DataBind()

        ltlReviewedBy.Text = s.SHAReviewedBy
        ltlReviewerComments.Text = s.ReviewerComments
        If s.SHAReviewedDate.ToString("MM/dd/yyyy") = "01/01/1900" Then
            ltlReviewedDate.Text = "never reviewed"
        Else
            ltlReviewedDate.Text = s.SHAReviewedDate.ToString("MM/dd/yyyy")
        End If

        ltlApprovedBy.Text = s.SHAApprovedBy
        If s.SHAApprovedDate.ToString("MM/dd/yyyy") = "01/01/1900" Then
            ltlApprovedDate.Text = "never approved"
        Else
            ltlApprovedDate.Text = s.SHAApprovedDate.ToString("MM/dd/yyyy")
        End If
    End Sub

    Protected Sub PopulateDischargeMeasJHA()
        Dim element As New DataTable
        element = s.GetMeasurementElements(s.ID, Config.DischargeMeasElem.ToString())

        For Each row As DataRow In element.Rows
            ltlDMRevisedBy.Text = row("revised_by").ToString()
            ltlDMRevisedDate.Text = String.Format("{0:MM/dd/yyyy}", row("revised_dt"))
            ltlDischargeMeas.Text = Replace(row("element_info"), vbCrLf & vbCrLf, "<br /><br />" & vbCrLf)
        Next

        lvDischargeMeasJHA.DataSource = DischargeMeasJHA()
        lvDischargeMeasJHA.DataBind()

        pnlDischargeMeas.Visible = True
    End Sub

    Protected Sub PopulateQWMeasJHA()
        Dim element As New DataTable
        element = s.GetMeasurementElements(s.ID, Config.QWMeasElem.ToString())

        For Each row As DataRow In element.Rows
            ltlQWRevisedBy.Text = row("revised_by").ToString()
            ltlQWRevisedDate.Text = String.Format("{0:MM/dd/yyyy}", row("revised_dt"))
            ltlQWMeas.Text = Replace(row("element_info"), vbCrLf & vbCrLf, "<br /><br />" & vbCrLf)
        Next

        lvQWMeasJHA.DataSource = QWMeasJHA()
        lvQWMeasJHA.DataBind()

        pnlQWMeas.Visible = True
    End Sub

    Protected Sub PopulateGWMeasJHA()
        Dim element As New DataTable
        element = s.GetMeasurementElements(s.ID, Config.GWMeasElem.ToString())

        For Each row As DataRow In element.Rows
            ltlGWRevisedBy.Text = row("revised_by").ToString()
            ltlGWRevisedDate.Text = String.Format("{0:MM/dd/yyyy}", row("revised_dt"))
            ltlGWMeas.Text = Replace(row("element_info"), vbCrLf & vbCrLf, "<br /><br />" & vbCrLf)
        Next

        lvGWMeasJHA.DataSource = GWMeasJHA()
        lvGWMeasJHA.DataBind()

        pnlGWMeas.Visible = True
    End Sub

    Protected Sub PopulateLakeMeasJHA()
        Dim element As New DataTable
        element = s.GetMeasurementElements(s.ID, Config.LakeMeasElem.ToString())

        For Each row As DataRow In element.Rows
            ltlLakeRevisedBy.Text = row("revised_by").ToString()
            ltlLakeRevisedDate.Text = String.Format("{0:MM/dd/yyyy}", row("revised_dt"))
            ltlLakeMeas.Text = Replace(row("element_info"), vbCrLf & vbCrLf, "<br /><br />" & vbCrLf)
        Next

        lvLakeMeasJHA.DataSource = LakeMeasJHA()
        lvLakeMeasJHA.DataBind()

        pnlLakeMeas.Visible = True
    End Sub

    Protected Sub PopulateEcoMeasJHA()
        Dim element As New DataTable
        element = s.GetMeasurementElements(s.ID, Config.EcoMeasElem.ToString())

        For Each row As DataRow In element.Rows
            ltlEcoRevisedBy.Text = row("revised_by").ToString()
            ltlEcoRevisedDate.Text = String.Format("{0:MM/dd/yyyy}", row("revised_dt"))
            ltlEcoMeas.Text = Replace(row("element_info"), vbCrLf & vbCrLf, "<br /><br />" & vbCrLf)
        Next

        lvEcoMeasJHA.DataSource = EcoMeasJHA()
        lvEcoMeasJHA.DataBind()

        pnlEcoMeas.Visible = True
    End Sub

    Protected Sub PopulateAtmMeasJHA()
        Dim element As New DataTable
        element = s.GetMeasurementElements(s.ID, Config.AtmMeasElem.ToString())

        For Each row As DataRow In element.Rows
            ltlAtmRevisedBy.Text = row("revised_by").ToString()
            ltlAtmRevisedDate.Text = String.Format("{0:MM/dd/yyyy}", row("revised_dt"))
            ltlAtmMeas.Text = Replace(row("element_info"), vbCrLf & vbCrLf, "<br /><br />" & vbCrLf)
        Next

        lvAtmMeasJHA.DataSource = AtmMeasJHA()
        lvAtmMeasJHA.DataBind()

        pnlAtmMeas.Visible = True
    End Sub

    Protected Sub lvServicingSiteSpecificCond_ItemDataBound(ByVal sender As Object, ByVal e As ListViewItemEventArgs)
        If e.Item.ItemType = ListViewItemType.DataItem Then
            Dim dataitem As ListViewDataItem = DirectCast(e.Item, ListViewDataItem)
            Dim lbl As Label = DirectCast(e.Item.FindControl("lblServicingSiteSpecificCond"), Label)

            Try
                Dim priority As Integer = CInt(DataBinder.Eval(dataitem.DataItem, "priority"))
                If priority < 0 Then
                    lbl.Font.Bold = True
                End If
            Catch ex As Exception
            End Try
        End If
    End Sub

    Protected Function getRemarksForJHA(ByVal site_jha_id As String) As DataTable
        Dim dt As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim sql As String = "SELECT site_specificcond_id, remarks, priority" & _
                " FROM SHA_Site_SpecificCond" & _
                " WHERE (site_jha_id = " & site_jha_id & ")"
            Dim cmd As New SqlCommand(sql, cnx)
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            Return dt

            cnx.Close()
        End Using
    End Function

    Protected Function getJobOpLimitsForJHA(ByVal site_jha_id As String) As DataTable
        Dim dt As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim sql As String = "SELECT shsr.site_reflevel_id, shsr.reflevel_id, shsr.reflevel_units, shsr.reflevel_va, shsr.remarks, shlr.reflevel_tp" & _
                " FROM SHA_Site_RefLevel AS shsr INNER JOIN SHA_Lut_RefLevel AS shlr ON shsr.reflevel_id = shlr.reflevel_id" & _
                " WHERE (site_jha_id = " & site_jha_id & ")"
            Dim cmd As New SqlCommand(sql, cnx)
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            Return dt

            cnx.Close()
        End Using
    End Function

End Class
