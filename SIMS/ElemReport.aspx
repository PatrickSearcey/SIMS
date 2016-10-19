<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SIMSSafety.Master" CodeBehind="ElemReport.aspx.vb" Inherits="SIMS.ElemReport" %>
<%@ MasterType  virtualPath="~/SIMSSafety.master" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<%@ Register TagPrefix="ucMANU" TagName="MANU" Src="~/Controls/ApproveMANU.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        * + html .riSingle .riTextBox
        {
            width: 10px !important;
            height: 14px !important;
        }
        .multiPage
        {
            display: -moz-inline-box;
            display: inline-block;
            zoom: 1;
            *display: inline;
            position: relative;
            margin-bottom: -3px;
            border: 1px solid #edf5c6;
            background-color: #fdfff6;
        }
        .selectedTab
        {
            font-weight: bold !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rgApprove">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgApprove" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rts1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rts1" />
                    <telerik:AjaxUpdatedControl ControlID="rmp1" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rmp1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rmp1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbActiveSiteToggle">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lbActiveSiteToggle" />
                    <telerik:AjaxUpdatedControl ControlID="ltlActiveSiteToggle" />
                    <telerik:AjaxUpdatedControl ControlID="rgAllSites" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Web20">
    </telerik:RadAjaxLoadingPanel>

    <asp:Panel ID="pnlSystem" runat="server">
        <telerik:RadTabStrip runat="server" ID="rts1" Orientation="HorizontalTop" 
            SelectedIndex="0" MultiPageID="rmp1" Skin="Web20" OnTabClick="rts1_TabClick">
            <Tabs>
                <telerik:RadTab Text="Manuscripts Ready for Approval for Active Sites" SelectedCssClass="selectedTab" SelectedIndex="0" />
                <telerik:RadTab Text="Lookup Manuscripts for All Sites" SelectedCssClass="selectedTab" />
            </Tabs>
        </telerik:RadTabStrip><telerik:RadMultiPage ID="rmp1" runat="server" SelectedIndex="0" Width="100%" CssClass="multiPage">
            <telerik:RadPageView runat="server" ID="rpv0">
                <div style="padding:10px;">
                    <h3>Manuscripts Ready for Approval for Active Sites</h3>
                    <telerik:RadPanelBar ID="rpbInstructions1" runat="server" Width="100%" Skin="Web20">
                        <Items>
                            <telerik:RadPanelItem Text="Steps for approving a manuscript (click here to collapse/expand)" Expanded="true">
                                <ContentTemplate>
                                    <div style="padding: 0px 5px 10px 5px;font-size:10pt;line-height:15px;">
                                        <p>Sites listed in this table require manuscript approval before manuscript element changes are published to NWISWeb.  A Manuscript requires approval when a 
                                            manuscript element has been changed since the last approval date and can only be approved by someone with SIMS WSC level access.  Click on the icon in the 
                                            first column to review and approve the manuscript. Clicking the site number link will open up the Station Information page in a new browser window.
                                        </p>
                                        <p style="font-weight:bold;">
                                            <a href="https://collaboration.usgs.gov/wg/owi/specialprojects/SIMS/Shared%20Documents/PADRE/FAQ.html" target="_blank">Visit the FAQ for additional help</a>
                                            &nbsp;&nbsp;|&nbsp;&nbsp;
                                            <a href="https://collaboration.usgs.gov/wg/owi/specialprojects/SIMS/Shared%20Documents/PADRE/Manuscript_Approval_Interface.wmv" target="_blank">MAI Training Video</a>
                                        </p>
                                        <ul class="fancyList" style="line-height:15px;">
                                            <li>Select a site manuscript to approve by clicking on the icon in the first column</li>
                                            <li>Review the manuscript information. Ensure the text contains no 
                                                <a href="http://nwis.usgs.gov/communications/2014news/140313sensitive_data_guidelines.html" target="_blank">Sensitive</a> or  
                                                <a href="http://nwis.usgs.gov/communications/2014news/140313sensitive_data_guidelines.html" target="_blank">Personally Identifiable Information</a> and 
                                                the text follows guidelines set forth in WRD Data Reports Preparation Guide (<a href="http://pubs.usgs.gov/of/1985/0480/report.pdf" target="_blank">PDF 
                                                format</a>/<a href="http://water.usgs.gov/usgs/adr/library/wdr/novak1985html/" target="_blank">HTML format excluding appendixes</a>) 
                                                and the <a href="https://collaboration.usgs.gov/wg/owi/specialprojects/SIMS/Shared%20Documents/PADRE/FAQ.html" target="_blank">FAQ</a>.</li>
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
                    <br /><br />
                    <telerik:RadGrid ID="rgApprove" runat="server" AutoGenerateColumns="false" EnableLinqExpressions="false" 
                        Skin="Web20" GridLines="None" ShowStatusBar="true" PageSize="50" OnNeedDataSource="rgApprove_NeedDataSource"
                        AllowSorting="true" 
                        AllowMultiRowSelection="false" 
                        AllowFiltering="true"
                        AllowPaging="true"
                        AllowAutomaticDeletes="true" 
                        OnItemDataBound="rgApprove_ItemDataBound"
                        OnPreRender="rgApprove_PreRender">
                        <PagerStyle Mode="NextPrevNumericAndAdvanced" AlwaysVisible="true" />
                        <SortingSettings SortToolTip="" />
                        <MasterTableView DataKeyNames="site_id" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" AllowFilteringByColumn="true">
                            <Columns>
                                <telerik:GridEditCommandColumn UniqueName="EditCommandColumn" HeaderStyle-Width="25px" ButtonType="ImageButton" EditImageUrl="~/Images/approve.png" />
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
                                <telerik:GridBoundColumn DataField="approver_comments" UniqueName="approver_comments" HeaderText="Approver Comments" AllowFiltering="false" HeaderStyle-Width="150px" />
                                <telerik:GridBoundColumn DataField="days_since_last_approved" UniqueName="days_since" HeaderText="Days Since Last Approved" AllowFiltering="false" HeaderStyle-Width="40px" />
                                <telerik:GridBoundColumn DataField="publish_complete" UniqueName="publish_complete" Display="false" />
                            </Columns>
                            <EditFormSettings UserControlName="~/Controls/ApproveMANU.ascx" EditFormType="WebUserControl">
                                <EditColumn UniqueName="EditCommandColumn" />
                            </EditFormSettings>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </telerik:RadPageView>
            <telerik:RadPageView runat="server" ID="rpv1">
                <div style="padding:10px;">
                    <h3>Lookup Manuscripts for All Sites</h3>
                    <telerik:RadPanelBar ID="rpbInstructions2" runat="server" Width="100%" Skin="Web20">
                        <Items>
                            <telerik:RadPanelItem Text="Steps for approving a manuscript (click here to collapse/expand)" Expanded="true">
                                <ContentTemplate>
                                    <div style="padding: 0px 5px 10px 5px;font-size:10pt;line-height:15px;">
                                        <p>All sites appear in this table, regardless of the manuscript approval status.  Only users with WSC level admin access
                                            have the ability to approve manuscripts. Click on the icon in the first column to review and, if ready for approval,
                                            approve the manuscript. Clicking the site number link will open up the Station Information page in a new browser window. 
                                        </p>
                                        <p style="font-weight:bold;">
                                            <a href="https://collaboration.usgs.gov/wg/owi/specialprojects/SIMS/Shared%20Documents/PADRE/FAQ.html" target="_blank">Visit the FAQ for additional help</a>
                                            &nbsp;&nbsp;|&nbsp;&nbsp;
                                            <a href="https://collaboration.usgs.gov/wg/owi/specialprojects/SIMS/Shared%20Documents/PADRE/Manuscript_Approval_Interface.wmv" target="_blank">MAI Training Video</a>
                                        </p>
                                        <ul class="fancyList" style="line-height:15px;">
                                            <li>Select a site manuscript to approve by clicking on the icon in the first column</li>
                                            <li>Review the manuscript information. Ensure the text contains no 
                                                <a href="http://nwis.usgs.gov/communications/2014news/140313sensitive_data_guidelines.html" target="_blank">Sensitive</a> or
                                                <a href="http://nwis.usgs.gov/communications/2014news/140313sensitive_data_guidelines.html" target="_blank">Personally Identifiable Information</a> and 
                                                the text follows guidelines set forth in WRD Data Reports Preparation Guide (<a href="http://pubs.usgs.gov/of/1985/0480/report.pdf" target="_blank">PDF 
                                                format</a>/<a href="http://water.usgs.gov/usgs/adr/library/wdr/novak1985html/" target="_blank">HTML format excluding appendixes</a>) 
                                                and the <a href="https://collaboration.usgs.gov/wg/owi/specialprojects/SIMS/Shared%20Documents/PADRE/FAQ.html" target="_blank">FAQ</a>.</li>
                                            <li>If text meets standards, approve the manuscript by clicking on the Approve Manuscript button.</li>
                                            <li>If manuscript requires editing, click on Edit Manuscript.
                                                <ul class="plainList">
                                                    <li>The SIMS Site Information Page for the station will pop up.</li>
                                                    <li>Login to the site by clicking on Logon at the top of the page.</li>
                                                    <li>Select Edit under Station Documents. Select and edit the element in question.</li>
                                                    <li>Return to the Manuscript Approval System and select the refresh button on the right hand side of the Manuscript block to refresh the data.</li>
                                                    <li>Review your changes.</li>
                                                    <li>Approve the manuscript by clicking on the Approve Manuscript button.</li>
                                                    <li>The <b>Go!</b> button allows a manual push of an already approved manuscript to NWISWeb. A user may want to use this option if an approved
                                                        manuscript is not shown in NWISWeb or if autogenerated manuscript fields from NWIS have been changed and need to be sent to NWISWeb. 
                                                    </li>
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
                    <table cellpadding="5">
                        <tr>
                            <td>
                                <b><asp:Literal ID="ltlActiveSiteToggle" runat="server" Text="Viewing active sites only" /></b>
                            </td>
                            <td>
                                &#10146;
                            </td>
                            <td>
                                <asp:LinkButton ID="lbActiveSiteToggle" runat="server" OnCommand="lbActiveSiteToggle_Command" CommandArgument="viewInactive" Text="Click to view inactive sites" />
                            </td>
                        </tr>
                    </table>
                    <telerik:RadGrid ID="rgAllSites" runat="server" AutoGenerateColumns="false" EnableLinqExpressions="false" 
                        Skin="Web20" GridLines="None" ShowStatusBar="true" PageSize="50" OnNeedDataSource="rgAllSites_NeedDataSource"
                        AllowSorting="true" 
                        AllowMultiRowSelection="false" 
                        AllowFiltering="true"
                        AllowPaging="true"
                        AllowAutomaticDeletes="true" 
                        OnItemDataBound="rgAllSites_ItemDataBound"
                        OnPreRender="rgAllSites_PreRender">
                        <PagerStyle Mode="NextPrevNumericAndAdvanced" AlwaysVisible="true" />
                        <SortingSettings SortToolTip="" />
                        <MasterTableView DataKeyNames="site_id" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" AllowFilteringByColumn="true">
                            <Columns>
                                <telerik:GridEditCommandColumn UniqueName="EditCommandColumn" HeaderStyle-Width="25px" ButtonType="ImageButton" EditImageUrl="~/Images/review.png" />
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
                                <telerik:GridTemplateColumn UniqueName="SendToNWISWeb" HeaderText="Manual NWIS Web Send" HeaderStyle-Width="25px" AllowFiltering="false">
                                    <ItemTemplate>
                                        <asp:Button ID="btnNWISWebSend" runat="server" Text="Go!" OnCommand="btnNWISWebSend_Command" CommandArgument='<%# Bind("site_id") %>' Visible='<%# GetVisibleValue(Eval("publish_complete"))%>' />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="publish_complete" UniqueName="publish_complete" Display="false" />
                            </Columns>
                            <EditFormSettings UserControlName="~/Controls/ApproveMANU.ascx" EditFormType="WebUserControl">
                                <EditColumn UniqueName="EditCommandColumn" />
                            </EditFormSettings>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </telerik:RadPageView>
        </telerik:RadMultiPage>
    </asp:Panel>
</asp:Content>
