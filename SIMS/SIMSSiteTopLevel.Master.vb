Public Class SIMSSiteTopLevel
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        hlTitle.NavigateUrl = "Default.aspx"
        hlSiteURL.Text = "https://sims.water.usgs.gov/SIMS/"
        hlSiteURL.NavigateUrl = "https://sims.water.usgs.gov/SIMS/"
    End Sub

End Class