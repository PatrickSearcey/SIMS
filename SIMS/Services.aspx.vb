Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.ServiceModel

Partial Public Class Services
    Inherits System.Web.UI.Page
    Private svcSIMS As SIMSService.SIMSServiceClient

    Protected Sub Page_Load(sender As Object, e As EventArgs)

    End Sub

    Protected Sub btnSubmit1_Click(sender As Object, e As System.EventArgs)
        svcSIMS = New SIMSService.SIMSServiceClient()
        Dim lstSites As Array = Nothing

        Try
            lstSites = svcSIMS.GetSiteByNumber(tbSiteNo1.Text)
        Catch ex As Exception
        End Try

        btnReset1.Visible = True
        rgService1.Visible = True
        rgService1.DataSource = lstSites
        rgService1.DataBind()

        svcSIMS.Close()
    End Sub

    Protected Sub btnReset1_Click(sender As Object, e As System.EventArgs)
        tbSiteNo1.Text = ""
        rgService1.Visible = False
        btnReset1.Visible = False
    End Sub

    Protected Sub btnSubmit2_Click(sender As Object, e As System.EventArgs)
        svcSIMS = New SIMSService.SIMSServiceClient()
        Dim lstSites As Array = Nothing

        Try
            lstSites = svcSIMS.GetElementsBySite(tbSiteNo2.Text)
        Catch ex As Exception
        End Try

        btnReset2.Visible = True
        rgService2.Visible = True
        rgService2.DataSource = lstSites
        rgService2.DataBind()

        svcSIMS.Close()
    End Sub

    Protected Sub btnReset2_Click(sender As Object, e As System.EventArgs)
        tbSiteNo2.Text = ""
        rgService2.Visible = False
        btnReset2.Visible = False
    End Sub

    Protected Sub btnSubmit3_Click(sender As Object, e As System.EventArgs)
        svcSIMS = New SIMSService.SIMSServiceClient()
        Dim lstSites As Array = Nothing

        Try
            lstSites = svcSIMS.GetElementsBySiteAndReport(tbSiteNo3.Text, tbAgencyCd3.Text, tbReportType.Text)
        Catch ex As Exception
        End Try

        btnReset3.Visible = True
        rgService3.Visible = True
        rgService3.DataSource = lstSites
        rgService3.DataBind()

        svcSIMS.Close()
    End Sub

    Protected Sub btnReset3_Click(sender As Object, e As System.EventArgs)
        tbSiteNo3.Text = ""
        tbReportType.Text = ""
        rgService3.Visible = False
        btnReset3.Visible = False
    End Sub
End Class