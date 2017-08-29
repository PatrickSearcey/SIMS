<%@ Page Title="" Language="C#" MasterPageFile="~/SIMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="SiteMAI.aspx.cs" Inherits="SIMS2017.StationDoc.SiteMAI" %>
<%@ Register Src="~/Control/SitePageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>
<%@ Register TagPrefix="ucMANU" TagName="ApproveMANU" Src="~/Control/ApproveMANU.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/stationdoc.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="ucApproveMANU">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgSiteDetails" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="ucApproveMANU" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnSubmitSite">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlucMANU" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="tbSiteNo" />
                    <telerik:AjaxUpdatedControl ControlID="rgSiteDetails" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="hlCurrentSite" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnSubmitTBSite">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlucMANU" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="rcbSites" />
                    <telerik:AjaxUpdatedControl ControlID="rgSiteDetails" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="hlCurrentSite" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap">
    </telerik:RadAjaxLoadingPanel>

    <uc:PageHeading id="ph1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <div class="mainContent">
    <asp:Panel ID="pnlMANU" runat="server">
        <div style="padding-bottom:10px;">
        <telerik:RadPanelBar ID="rpbInstructions" runat="server" Width="100%" Skin="Bootstrap">
            <Items>
                <telerik:RadPanelItem Text="Steps for approving a manuscript (click here to collapse/expand)" Expanded="false">
                    <ContentTemplate>
                        <div style="padding:5px;font-size:10pt;">
                            <ul class="fancyList" style="line-height:15px;">
                                <li>At the bottom of this page, there will be an Approve Manuscript button visible if the manuscript requires approval.</li>
                                <li>Review the manuscript information. Ensure the text contains no 
                                    <a href="http://nwis.usgs.gov/communications/2014news/140313sensitive_data_guidelines.html" target="_blank">Sensitive</a> or
                                    <a href="http://nwis.usgs.gov/communications/2014news/140313sensitive_data_guidelines.html" target="_blank">Personally Identifiable Information</a> and 
                                    the text follows guidelines set forth in WRD Data Reports Preparation Guide (<a href="http://pubs.usgs.gov/of/1985/0480/report.pdf" target="_blank">PDF 
                                    format</a>/<a href="http://water.usgs.gov/usgs/adr/library/wdr/novak1985html/" target="_blank">HTML format excluding appendixes</a>) 
                                    and the <a href="https://collaboration.usgs.gov/wg/owi/specialprojects/SIMS/Shared%20Documents/PADRE/FAQ.html" target="_blank">FAQ</a>.</li>
                                <li>Download and watch the 
                                    <a href="https://collaboration.usgs.gov/wg/owi/specialprojects/SIMS/Shared%20Documents/PADRE/Manuscript_Approval_Interface.wmv" target="_blank">MAI Training Video</a>.</li>
                                <li>If text meets standards, approve the manuscript by clicking on the Approve Manuscript button.</li>
                                <li>If manuscript requires editing, click on Edit Manuscript.
                                    <ul class="plainList">
                                        <li>The SIMS Site Information Page for the station will pop up.</li>
                                        <li>Login to the site by clicking on Logon at the top of the page.</li>
                                        <li>Select Edit under Station Documents. Select and edit the element in question.</li>
                                        <li>Return to the Manuscript Approval System and select the refresh button on the right hand side of the Manuscript block to refresh the data.</li>
                                        <li>Review your changes.</li>
                                        <li>Approve the manuscript by clicking on the Approve Manuscript button.</li>
                                    </ul>
                                </li>
                                <li>The approved manuscript will be available on NWISWeb soon.</li>
                            </ul>
                        </div>
                    </ContentTemplate>
                </telerik:RadPanelItem>
            </Items>
            <ExpandAnimation Type="InQuart" />
            <CollapseAnimation Type="InQuart" />
        </telerik:RadPanelBar>
        </div>
        <div class="heading">
            <h3 style="margin:0">Navigation</h3>
        </div>
        <div style="width:100%">
            <div class="linkBox">
                <h4>Original Site <asp:Image ID="imgOriginalSiteHelp" runat="server" ImageUrl="~/images/tooltip.png" /></h4>
                <asp:HyperLink ID="hlOriginalSite" runat="server" />
                <telerik:RadToolTip ID="rttOriginalSiteHelp" runat="server" TargetControlID="imgOriginalSiteHelp"
                    RelativeTo="Element" Width="300px" AutoCloseDelay="10000" Skin="Bootstrap"
                    Height="70px" IsClientID="false" Animation="Fade" Position="TopRight">
                    You arrived on this interface from this site's Station Information page. To return, click the site number/name link.
                </telerik:RadToolTip>
            </div>
            <div class="linkBox">
                <h4>Current Site <asp:Image ID="imgCurrentSiteHelp" runat="server" ImageUrl="~/images/tooltip.png" /></h4>
                <asp:HyperLink ID="hlCurrentSite" runat="server" />
                <telerik:RadToolTip ID="rttCurrentSiteHelp" runat="server" TargetControlID="imgCurrentSiteHelp"
                    RelativeTo="Element" Width="300px" AutoCloseDelay="10000" Skin="Bootstrap"
                    Height="90px" IsClientID="false" Animation="Fade" Position="TopRight">
                    This is the site for which you're viewing the information on this page.  Click to the site number/name link to go to its Station Information page.
                </telerik:RadToolTip>
            </div>
            <div class="linkBox">
                <h4>Manuscript Approval Interface <asp:Image ID="imgMANUApprovalSystem" runat="server" ImageUrl="~/images/tooltip.png" /></h4>
                <asp:HyperLink ID="hlFullReport" runat="server" Text="Click here to go to the full report" Target="_blank" />
                <telerik:RadToolTip ID="rttMANUApprovalSystem" runat="server" TargetControlID="imgMANUApprovalSystem"
                    RelativeTo="Element" Width="300px" AutoCloseDelay="10000" Skin="Bootstrap"
                    Height="70px" IsClientID="false" Animation="Fade" Position="TopRight">
                    Find all active and inactive sites' manuscripts for your WSC in tabular format here.  Approve manuscripts in this interface, too.
                </telerik:RadToolTip>
            </div>
        </div>
        <br style="clear:left;"/>
        <div class="heading">
            <h3 style="margin:0;">Select a Site</h3>
        </div>
        <div class="siteSelection">
            <p>Scroll through the drop-down list below to find your site number. Once found, click the 'Choose this site!' button to confirm and view its manuscript.</p>
            <div style="float:left;">
                <asp:DropDownList ID="rcbSites" runat="server" Width="500px" />
            </div>
            <telerik:RadButton ID="btnSubmitSite" runat="server" Text="Choose this site!" OnCommand="btnSubmitSite_Command" CommandArgument="choosesite" Skin="Bootstrap" />
            <p><b>-OR-</b></p>
            <table>
                <tr>
                    <td>Enter a site number and agency code:</td>
                    <td><asp:TextBox ID="tbSiteNo" runat="server" Width="150px" /></td>
                    <td><asp:TextBox ID="tbAgencyCd" runat="server" Text="USGS" Width="80px" /></td>
                    <td><telerik:RadButton ID="btnSubmitTBSite" runat="server" Text="Go!" OnCommand="btnSubmitSite_Command" CommandArgument="entersite" Skin="Bootstrap" /></td>
                </tr>
                <tr>
                    <td colspan="4"><asp:Label ID="lblAlert" runat="server" Text="Site must be added to SIMS first!" ForeColor="Red" Font-Bold="true" Visible="false" /></td>
                </tr>
            </table>
        </div>
        <div class="heading">
            <h3 style="margin:0;">Site Details</h3>
        </div>
        <div style="padding-bottom:20px;">
            <telerik:RadGrid ID="rgSiteDetails" runat="server" AutoGenerateColumns="false" EnableLinqExpressions="false"
                Skin="Bootstrap" GridLines="None" ShowStatusBar="true" OnNeedDataSource="rgSiteDetails_NeedDataSource"
                AllowSorting="false" AllowFilteringByColumn="false" AllowPaging="false" OnItemDataBound="rgSiteDetails_ItemDataBound">
                <SortingSettings SortedDescToolTip="" />
                <MasterTableView DataKeyNames="site_id" Width="100%" CommandItemDisplay="None">
                    <Columns>
                        <telerik:GridBoundColumn DataField="office_cd" UniqueName="office_cd" HeaderText="Office Code" FilterControlWidth="25px" HeaderStyle-Width="35px" />
                        <telerik:GridTemplateColumn DataField="site_no" SortExpression="site_no" UniqueName="site_no" HeaderText="Site No." FilterControlWidth="50px" HeaderStyle-Width="55px">
                            <ItemTemplate>
                                <asp:HyperLink ID="hlSiteNo" runat="server" Text='<%# Bind("site_no")%>' Target="_blank" />&nbsp;
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="station_nm" SortExpression="station_nm" UniqueName="station_nm" HeaderText="Station Name" FilterControlWidth="150px" HeaderStyle-Width="260px" />
                        <telerik:GridBoundColumn DataField="revised_dt" UniqueName="revised_dt" HeaderText="Last Revised" DataFormatString="{0:MM/dd/yyyy}" AllowFiltering="false" HeaderStyle-Width="80px" />
                        <telerik:GridBoundColumn DataField="sitefile_md" UniqueName="sitefile_md" HeaderText="SITEFILE Last Modified" DataFormatString="{0:MM/dd/yyyy}" AllowFiltering="false" HeaderStyle-Width="80px" />
                        <telerik:GridBoundColumn DataField="approved_dt" UniqueName="approved_dt" HeaderText="Approved Date" DataFormatString="{0:MM/dd/yyyy}" AllowFiltering="false" HeaderStyle-Width="80px" />
                        <telerik:GridBoundColumn DataField="approved_by" UniqueName="approved_by" HeaderText="Approved By" FilterControlWidth="40px" HeaderStyle-Width="60px" />
                        <telerik:GridBoundColumn DataField="approver_comments" UniqueName="approver_comments" HeaderText="Approver Comments" HeaderStyle-Width="120px" FilterControlWidth="100px" />
                        <telerik:GridBoundColumn DataField="needs_approval" UniqueName="needs_approval" HeaderText="Needs Approval" HeaderStyle-Width="30px" FilterControlWidth="25px" />
                        <telerik:GridBoundColumn DataField="days_since_last_approved" UniqueName="days_since" HeaderText="Days Since Last Approved" AllowFiltering="false" HeaderStyle-Width="30px" />
                        <telerik:GridBoundColumn DataField="publish_complete" UniqueName="publish_complete" Display="false" />
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </div>
        <asp:Panel ID="pnlucMANU" runat="server" CssClass="MANU">
            <ucMANU:ApproveMANU ID="ucApproveMANU" runat="server" OnSubmitEvent="ucApproveMANU_SubmitEvent" />
        </asp:Panel>
    </asp:Panel>
    </div>
</asp:Content>
